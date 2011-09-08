using NUnit.Framework;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;
using SchemaManager.Rollback;
using Moq;
using Utilities.Testing;

namespace SchemaManager.Tests.Rollback
{
	public class DatabaseReverterSpecs
	{
		[TestFixture]
		public class when_rolling_back_a_database : given.there_are_no_rollbacks
		{
			protected override void When()
			{
				SUT.ApplyRollbacks();
			}

			[Test]
			public void then_it_gets_the_list_of_updates()
			{
				GetMockFor<IProvideSchemaChanges>()
					.Verify(p => p.GetAllChanges());
			}

			[Test]
			public void then_it_logs_what_it_is_going_to_do()
			{
				GetMockFor<ILogger>()
					.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<DatabaseVersion>()));
			}

			[Test]
			public void then_it_checks_the_target_to_see_what_revision_it_is_at()
			{
				GetMockFor<IDatabase>()
					.VerifyGet(d => d.Revision);
			}

			[Test]
			public void then_it_does_not_apply_any_rollbacks()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteUpdate(It.IsAny<ISchemaChange>()), Times.Never());
			}
		}

		[TestFixture]
		public class when_rolling_back_a_newer_database : given.there_are_rollbacks_available_for_newer_database
		{
			protected override void When()
			{
				SUT.ApplyRollbacks();
			}

			[Test]
			public void then_it_applies_rollbacks_to_the_target_database()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteRollback(GetMockFor<ISchemaChange>().Object));
			}

			[Test]
			public void then_it_logs_that_it_is_applying_the_rollback()
			{
				GetMockFor<ILogger>()
					.Verify(l => l.Info("Applying rollback for database version {0}...", GetMockFor<ISchemaChange>().Object.Version));
			}
		}

		[TestFixture]
		public class when_rolling_back_to_current_version : given.there_are_rollbacks_available_for_a_current_database
		{
			protected override void When()
			{
				SUT.ApplyRollbacks();
			}

			[Test]
			public void then_it_does_not_apply_rollbacks_to_the_target_database()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteRollback(GetMockFor<ISchemaChange>().Object), Times.Never());
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<DatabaseReverter>
			{
				protected override void ConfigureKernel(Ninject.IKernel kernel)
				{
					base.ConfigureKernel(kernel);

					kernel.Bind<DatabaseVersion>().ToConstant(new DatabaseVersion(0, 0));
				}

				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(double.MaxValue, double.MaxValue));
				}
			}

			public abstract class there_are_no_rollbacks : the_default_state
			{
			}

			public abstract class there_are_rollbacks_available : the_default_state
			{
				protected override void Given()
				{
					base.Given();

					var schemaChange = GetMockFor<ISchemaChange>();
					var version = new DatabaseVersion(1, 0);
					schemaChange.SetupGet(s => s.Version).Returns(version);
					schemaChange.Setup(s => s.NeedsToBeRolledBackFrom(It.IsAny<IDatabase>()))
						.Returns((IDatabase d) => d.Revision >= version);

					GetMockFor<IProvideSchemaChanges>()
						.Setup(p => p.GetAllChanges())
						.Returns(new[] { schemaChange.Object });
				}
			}

			public abstract class there_are_rollbacks_available_for_newer_database : there_are_rollbacks_available
			{
				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(2, 0));
				}
			}

			public abstract class there_are_rollbacks_available_for_a_current_database : there_are_rollbacks_available
			{
				protected override void ConfigureKernel(Ninject.IKernel kernel)
				{
					base.ConfigureKernel(kernel);

					kernel.Unbind<DatabaseVersion>();
					kernel.Bind<DatabaseVersion>().ToConstant(new DatabaseVersion(1, 0));
				}

				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(1, 0));
				}
			}
		}
	}
}