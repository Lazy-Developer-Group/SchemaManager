using NUnit.Framework;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using Moq;
using SchemaManager.Databases;
using SchemaManager.Update;
using SpecsFor;

namespace SchemaManager.Tests.Update
{
	public class DatabaseUpdaterSpecs
	{
		[TestFixture]
		public class when_updating_a_database : given.there_are_no_updates
		{
			protected override void When()
			{
				SUT.ApplyUpdates();
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
					.Verify(l => l.Info(It.IsAny<string>()));
			}

			[Test]
			public void then_it_checks_the_target_to_see_what_revision_it_is_at()
			{
				GetMockFor<IDatabase>()
					.VerifyGet(d => d.Revision);
			}

			[Test]
			public void then_it_does_not_apply_any_updates()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteUpdate(It.IsAny<ISchemaChange>()), Times.Never());
			}
		}

		[TestFixture]
		public class when_updating_an_out_of_date_database : given.there_are_updates_available_for_out_of_date_database
		{
			protected override void When()
			{
				SUT.ApplyUpdates();
			}

			[Test]
			public void then_it_applies_updates_to_the_target_database()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteUpdate(GetMockFor<ISchemaChange>().Object));
			}

			[Test]
			public void then_it_logs_that_it_is_applying_the_update()
			{
				GetMockFor<ILogger>()
					.Verify(l => l.Info("Applying update for database version {0}...", GetMockFor<ISchemaChange>().Object.Version));
			}
		}

		[TestFixture]
		public class when_updating_a_current_database : given.there_are_updates_available_for_a_current_database
		{
			protected override void When()
			{
				SUT.ApplyUpdates();
			}

			[Test]
			public void then_it_applies_updates_to_the_target_database()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteUpdate(GetMockFor<ISchemaChange>().Object), Times.Never());
			}
		}

		[TestFixture]
		public class when_updating_an_out_of_date_database_to_a_target_revision : given.the_target_revision_is_the_current_revision
		{
			protected override void When()
			{
				SUT.ApplyUpdates();
			}

			[Test]
			public void then_it_will_not_apply_any_updates()
			{
				GetMockFor<IDatabase>()
					.Verify(d => d.ExecuteUpdate(It.IsAny<ISchemaChange>()), Times.Never());
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<DatabaseUpdater>
			{
				protected override void ConfigureContainer(StructureMap.IContainer container)
				{
					base.ConfigureContainer(container);

					container.Configure(cfg => cfg.For<DatabaseVersion>().Use(DatabaseVersion.Max));
				}

				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(0,0, 0, 0));
				}
			}

			public abstract class there_are_no_updates : the_default_state
			{
			}

			public abstract class there_are_updates_available : the_default_state
			{
				protected override void Given()
				{
					base.Given();

					var schemaChange = GetMockFor<ISchemaChange>();
					var version = new DatabaseVersion(1, 0, 0, 0);
					schemaChange.SetupGet(s => s.Version).Returns(version);
					schemaChange.Setup(s => s.NeedsToBeAppliedTo(It.IsAny<IDatabase>()))
						.Returns((IDatabase d) => version > d.Revision);

					GetMockFor<IProvideSchemaChanges>()
						.Setup(p => p.GetAllChanges())
						.Returns(new[] {schemaChange.Object});
				}
			}

			public abstract class there_are_updates_available_for_out_of_date_database : there_are_updates_available
			{
				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(0, 0, 0, 0));
				}
			}

			public abstract class there_are_updates_available_for_a_current_database : there_are_updates_available
			{
				protected override void Given()
				{
					base.Given();

					GetMockFor<IDatabase>()
						.Setup(d => d.Revision)
						.Returns(new DatabaseVersion(1, 0, 0, 0));
				}
			}

			public abstract class the_target_revision_is_the_current_revision : there_are_updates_available_for_out_of_date_database
			{
				protected override void ConfigureContainer(StructureMap.IContainer container)
				{
					base.ConfigureContainer(container);

					container.Model.EjectAndRemove(typeof (DatabaseVersion));
					container.Configure(cfg => cfg.For<DatabaseVersion>().Use(new DatabaseVersion(0, 0, 0, 0)));
				}
			}
		}
	}
}