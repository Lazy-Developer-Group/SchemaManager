using System.Configuration;
using NUnit.Framework;
using SchemaManager.Core;
using SchemaManager.Databases;
using Should;
using Moq;
using Utilities.Data;
using Utilities.Testing;

namespace SchemaManager.Tests.Databases
{
	public class SqlServerDatabaseSpecs
	{
		[TestFixture]
		public class when_checking_the_database_version : given.the_database_version_is_not_set
		{
			private DatabaseVersion _revision;

			protected override void When()
			{
				_revision = SUT.Revision;
			}

			[Test]
			public void then_it_adds_version_information_to_the_database()
			{
				ExecuteScalar<int>("select count(*) from sys.extended_properties WHERE name = 'DatabaseVersion'").ShouldEqual(1);
			}

			[Test]
			public void then_it_returns_version_1()
			{
				_revision.MajorVersion.ShouldEqual(1);
				_revision.MinorVersion.ShouldEqual(0);
			}
		}

		[TestFixture]
		public class when_checking_the_database_version_and_it_is_set : given.the_database_version_is_set
		{
			private DatabaseVersion _revision;

			protected override void When()
			{
				_revision = SUT.Revision;
			}

			[Test]
			public void then_it_returns_the_correct_version()
			{
				_revision.MajorVersion.ShouldEqual(1.15);
				_revision.MinorVersion.ShouldEqual(1);
			}
		}

		[TestFixture]
		public class when_applying_a_schema_change : given.the_database_version_is_set
		{
			protected override void When()
			{
				var schemaChange = GetMockFor<ISchemaChange>();
				schemaChange.Setup(s => s.Version).Returns(new DatabaseVersion(2, 0));

				SUT.ExecuteUpdate(schemaChange.Object);
			}

			[Test]
			public void then_it_updates_the_database_version()
			{
				SUT.Revision.MajorVersion.ShouldEqual(2);
				SUT.Revision.MinorVersion.ShouldEqual(0);
			}

			[Test]
			public void then_it_applies_the_update()
			{
				GetMockFor<ISchemaChange>()
					.Verify(s => s.Execute(It.IsAny<IDbContext>()));
			}
		}

		[TestFixture]
		public class when_rolling_back_a_schema_change : given.the_database_version_is_set
		{
			protected override void When()
			{
				var schemaChange = GetMockFor<ISchemaChange>();
				schemaChange.Setup(s => s.Version).Returns(new DatabaseVersion(1, 1));
				schemaChange.Setup(s => s.PreviousVersion).Returns(new DatabaseVersion(1, 0));

				SUT.ExecuteRollback(schemaChange.Object);
			}

			[Test]
			public void then_it_rolls_the_update_back()
			{
				GetMockFor<ISchemaChange>()
					.Verify(s => s.Rollback(It.IsAny<IDbContext>()));
			}

			[Test]
			public void then_it_updates_the_database_version()
			{
				SUT.Revision.MajorVersion.ShouldEqual(1);
				SUT.Revision.MinorVersion.ShouldEqual(0);
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<SqlServerDatabase>.WithDatabaseSupport
			{
				protected override string GetConnectionString()
				{
					return ConfigurationManager.ConnectionStrings["SchemaManagerIntegrationTests"].ConnectionString;
				}
			}

			public abstract class the_database_version_is_not_set : the_default_state
			{
				protected override void Given()
				{
					base.Given();

					ExecuteNonQuery(@"IF EXISTS (select * from sys.extended_properties WHERE name = 'DatabaseVersion')
											exec sp_dropextendedproperty @name='DatabaseVersion'");
				}
			}

			public abstract class the_database_version_is_set : the_default_state
			{
				protected override void Given()
				{
					base.Given();

					//Clear it if it's there, then insert a known state.
					ExecuteNonQuery(@"IF EXISTS (select * from sys.extended_properties WHERE name = 'DatabaseVersion')
											exec sp_dropextendedproperty @name='DatabaseVersion'");

					ExecuteNonQuery("exec sp_addextendedproperty @name='DatabaseVersion', @value='1.15.1'");
				}
			}
		}
	}
}