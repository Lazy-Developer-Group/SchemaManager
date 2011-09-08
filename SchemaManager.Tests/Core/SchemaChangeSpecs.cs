using System.Data;
using System.IO;
using NUnit.Framework;
using SchemaManager.Core;
using Moq;
using Utilities.Data;
using Utilities.General;
using Utilities.Testing;

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

				protected override void BeforeEachSpec()
				{
					base.BeforeEachSpec();

					File.WriteAllText(ForwardScript, ResourceHelper.GetResourceAsString(typeof(given), typeof(given).Namespace, TestScript));
					File.WriteAllText(BackScript, ResourceHelper.GetResourceAsString(typeof(given), typeof(given).Namespace, TestScript));
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

				protected override void ConfigureKernel(Ninject.IKernel kernel)
				{
					base.ConfigureKernel(kernel);

					kernel.Bind<SchemaChange>().ToConstant(new SchemaChange(Directory.GetCurrentDirectory(), new DatabaseVersion(), new DatabaseVersion()));
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
				protected override void ConfigureKernel(Ninject.IKernel kernel)
				{
					TestScript = "MultipleBatches.sql";
					base.ConfigureKernel(kernel);
				}
			}
		}
	}
}