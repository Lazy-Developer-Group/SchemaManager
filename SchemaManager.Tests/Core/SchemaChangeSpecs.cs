using System.Data;
using System.IO;
using NUnit.Framework;
using SchemaManager.Core;
using Moq;
using StructureMap;
using Utilities.General;
using Utilities.Data;
using SpecsFor;

namespace SchemaManager.Tests.Core
{
	public class SchemaChangeSpecs
	{
		[TestFixture]
		public class when_running_an_update : given.the_update_contains_a_single_statement
		{
			protected override void When()
			{
				SUT.Execute(GetMockFor<IDbContext>().Object);
			}

			[Test]
			public void then_it_creates_a_command()
			{
				GetMockFor<IDbContext>()
					.Verify(c => c.CreateCommand());
			}

			[Test]
			public void then_it_sets_the_text_of_the_command()
			{
				GetMockFor<IDbCommand>()
					.VerifySet(c => c.CommandText = File.ReadAllText(ForwardScript));
			}

			[Test]
			public void then_it_executes_the_command()
			{
				GetMockFor<IDbCommand>()
					.Verify(c => c.ExecuteNonQuery());
			}
		}

		[TestFixture]
		public class when_running_a_complex_update : given.the_update_contains_multiple_statements
		{
			protected override void When()
			{
				SUT.Execute(GetMockFor<IDbContext>().Object);
			}

			[Test]
			public void then_it_creates_a_command_for_each_batch()
			{
				GetMockFor<IDbContext>()
					.Verify(c => c.CreateCommand(), Times.Exactly(3));
			}
		}

		[TestFixture]
		public class when_running_a_complex_update_with_lower_case_go : given.the_update_contains_multiple_statements_separated_with_lower_case_go
		{
			protected override void When()
			{
				SUT.Execute(GetMockFor<IDbContext>().Object);
			}

			[Test]
			public void then_it_creates_a_command_for_each_batch()
			{
				GetMockFor<IDbContext>()
					.Verify(c => c.CreateCommand(), Times.Exactly(3));
			}
		}

		[TestFixture]
		public class when_rolling_back_a_complex_change : given.the_update_contains_multiple_statements
		{
			protected override void When()
			{
				SUT.Rollback(GetMockFor<IDbContext>().Object);
			}

			[Test]
			public void then_it_creates_a_command_for_each_batch()
			{
				GetMockFor<IDbContext>()
					.Verify(c => c.CreateCommand(), Times.Exactly(3));
			}

			[Test]
			public void then_it_executes_a_command_for_each_batch()
			{
				GetMockFor<IDbCommand>()
					.Verify(c => c.ExecuteNonQuery(), Times.Exactly(3));
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<SchemaChange>
			{
				protected string TestScript = "SingleBatch.sql";
				protected string ForwardScript = "Forward.sql";
				protected string BackScript = "Back.sql";

				public override void SetupEachSpec()
				{
					File.WriteAllText(ForwardScript, ResourceHelper.GetResourceAsString(typeof(given), typeof(given).Namespace, TestScript));
					File.WriteAllText(BackScript, ResourceHelper.GetResourceAsString(typeof(given), typeof(given).Namespace, TestScript));
					
					base.SetupEachSpec();
				}

				protected override void AfterEachSpec()
				{
					base.AfterEachSpec();

					if (File.Exists(ForwardScript))
					{
						File.Delete(ForwardScript);
					}

					if (File.Exists(BackScript))
					{
						File.Delete(BackScript);
					}
				}

				protected override void ConfigureContainer(IContainer container)
				{
					base.ConfigureContainer(container);

					container.Configure(cfg => cfg.For<SchemaChange>().Use(new SchemaChange(Directory.GetCurrentDirectory(), new DatabaseVersion(), new DatabaseVersion())));
				}

				protected override void Given()
				{
					base.Given();

					GetMockFor<IDbContext>()
						.Setup(c => c.CreateCommand())
						.Returns(GetMockFor<IDbCommand>().Object);
				}
			}

			public abstract class the_update_contains_a_single_statement : the_default_state
			{
			}

			public abstract class the_update_contains_multiple_statements : the_default_state 
			{
				public override void SetupEachSpec()
				{
					TestScript = "MultipleBatches.sql";
					base.SetupEachSpec();
				}
			}

			public class the_update_contains_multiple_statements_separated_with_lower_case_go : the_default_state
			{
				public override void SetupEachSpec()
				{
					TestScript = "MultipleBatchesWithLowerCaseGo.sql";
					base.SetupEachSpec();
				}
			}
		}
	}
}