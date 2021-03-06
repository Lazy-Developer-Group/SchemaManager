﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using SchemaManager.AlwaysRun;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;
using SchemaManager.Tests.Helpers;
using SchemaManager.Update;
using SpecsFor;
using Should;
using Utilities.Data;

namespace SchemaManager.Tests.IntegrationTests
{
	public class SchemaUpdate
	{
		public class when_a_script_fails_it_rolls_back_all_changes_the_script_made : SpecsFor<DatabaseUpdater>
		{
			private const string TestScriptPath = @"IntegrationTests\SchemaUpdateChanges\";
			private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SchemaManagerIntegrationTests"].ConnectionString;
			private DbContext _context;

			protected override void InitializeClassUnderTest()
			{
				_context = new DbContext(ConnectionString);
				SUT = new DatabaseUpdater(new NullAlwaysRunScriptsProvider(),
				                          new FileSystemSchemaChangeProvider(TestScriptPath), new NullLogger(), 
				                          new SqlServerDatabase(_context), SchemaManagerGlobalOptions.Defaults);
			}

			public override void TearDown()
			{
				_context.Dispose();
			}

			protected override void When()
			{
				try
				{
					SUT.ApplyUpdates();
				}
				catch (SqlException ex)
				{
					//The bad script attempts to create the Logs table again.
					if (ex.Message != "There is already an object named 'Logs' in the database.")
					{
						throw;
					}
				}
			}

			[Test]
			public void then_it_rolls_back_all_changes()
			{
				_context.ExecuteScalar<object>("SELECT OBJECT_ID('Logs')").ShouldEqual(DBNull.Value);
			}

			[Test]
			public void then_the_database_is_not_versioned()
			{
				_context.ExecuteScalar<int>("SELECT COUNT(*) FROM sys.extended_properties WHERE name = 'DatabaseVersion'").ShouldEqual(0);
			}
		}

		public abstract class given_incremental_commits_are_enabled : SpecsFor<DatabaseUpdater>
		{
			private const string TestScriptPath = @"IntegrationTests\SchemaUpdateChanges\";
			private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SchemaManagerIntegrationTests"].ConnectionString;
			private DbContext _context;

			protected override void InitializeClassUnderTest()
			{
				var options = SchemaManagerGlobalOptions.Defaults;
				options.UseIncrementalTransactions = true;
				_context = new DbContext(ConnectionString);
				SUT = new DatabaseUpdater(new NullAlwaysRunScriptsProvider(),
											new FileSystemSchemaChangeProvider(TestScriptPath), new NullLogger(), 
											new SqlServerDatabase(_context), options);
			}

			public override void TearDown()
			{
				//Remove the extended property
				_context.ExecuteNonQuery("IF EXISTS (SELECT * FROM sys.extended_properties WHERE name = 'DatabaseVersion') EXEC sp_dropextendedproperty @name='DatabaseVersion' ");
				//Drop the sproc
				_context.ExecuteNonQuery("IF EXISTS (SELECT	* FROM sys.procedures WHERE name = 'AddLogMsg') DROP PROCEDURE AddLogMsg");
				//Drop the table
				_context.ExecuteNonQuery("IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Logs') DROP TABLE Logs");

				_context.Dispose();
			}

			public class when_a_script_fails_it_rolls_back_all_changes_the_script_made : given_incremental_commits_are_enabled
			{
				protected override void When()
				{
					try
					{
						SUT.ApplyUpdates();
					}
					catch (SqlException ex)
					{
						//The bad script attempts to create the Logs table again.
						if (ex.Message != "There is already an object named 'Logs' in the database.")
						{
							throw;
						}
					}
				}

				[Test]
				public void then_logs_in_the_second_script_should_not_be_added()
				{
					_context.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Logs").ShouldEqual(0);
				}

				[Test]
				public void then_the_database_is_versioned()
				{
					_context.ExecuteScalar<int>("SELECT COUNT(*) FROM sys.extended_properties WHERE name = 'DatabaseVersion'").ShouldEqual(1);
				}
			}

		}
	}
}