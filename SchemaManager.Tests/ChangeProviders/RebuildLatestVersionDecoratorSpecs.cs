using System.Linq;
using NUnit.Framework;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;
using Should;
using Moq;
using SpecsFor;

namespace SchemaManager.Tests.ChangeProviders
{
	public class RebuildLatestVersionDecoratorSpecs
	{
		[TestFixture]
		public class when_retrieving_changes_from_a_decorated_provider : given.the_default_state
		{
			private ISchemaChange[] _changes;

			protected override void When()
			{
				_changes = SUT.GetAllChanges().ToArray();
			}

			[Test]
			public void then_it_returns_the_updates_from_the_underlying_provider()
			{
				_changes.Length.ShouldEqual(Changes.Length);
			}

			[Test]
			public void then_all_the_updates_for_the_most_recent_version_should_return_true_regardless_of_database_version()
			{
				_changes.Where(c => c.Version.MajorVersion == LatestMajorVersion)
					.All(c => c.NeedsToBeAppliedTo(Database)).ShouldBeTrue("Not all of the latest version's updates returned true!");
			}

			[Test]
			public void then_versions_for_the_previous_major_vesion_should_not_return_true()
			{
				_changes.Where(c => c.Version.MajorVersion < LatestMajorVersion)
					.All(c => c.NeedsToBeAppliedTo(Database)).ShouldBeFalse("Older updates returned true!");				
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<RebuildLatestVersionDecorator>
			{
				protected const double LatestMajorVersion = 2.5;
				protected IDatabase Database;
				protected ISchemaChange[] Changes;

				protected override void InitializeClassUnderTest()
				{
					SUT = new RebuildLatestVersionDecorator(GetMockFor<IProvideSchemaChanges>().Object);

					var database = GetMockFor<IDatabase>();
					database.SetupGet(d => d.Revision).Returns(new DatabaseVersion(LatestMajorVersion, 4));

					Changes = Enumerable.Range(1, 10).Select(i => GetMockChangeForRevision(2.4, i)).Concat(
							Enumerable.Range(1, 5).Select(i => GetMockChangeForRevision(LatestMajorVersion, i))
						).ToArray();

					GetMockFor<IProvideSchemaChanges>()
						.Setup(p => p.GetAllChanges())
						.Returns(Changes);
				}

				private ISchemaChange GetMockChangeForRevision(double majorVersion, int minorVersion)
				{
					var change = new Mock<ISchemaChange>();
					change.SetupGet(c => c.Version).Returns(new DatabaseVersion(majorVersion, minorVersion));
					return change.Object;
				}
			}
		}
	}
}