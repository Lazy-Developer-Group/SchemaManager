using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using Should;
using Utilities.Testing;
using System.Linq;
using Utilities.General;

namespace SchemaManager.Tests.ChangeProviders
{
	public class FileSystemSchemaChangeProviderSpecs
	{
		[TestFixture]
		public class when_getting_changes : given.the_default_state
		{
			private IEnumerable<ISchemaChange> _results;

			protected override void When()
			{
				_results = SUT.GetAllChanges();
			}

			[Test]
			public void then_it_returns_the_changes()
			{
				_results.ShouldNotBeNull();
				_results.ShouldNotBeEmpty();
			}

			[Test]
			public void then_the_changes_are_ordered_by_revision()
			{
				var results = _results.ToArray();

				for (int i = 0; i < results.Length -1; i++)
				{
					results[i].Version.ShouldBeLessThan(results[i+1].Version);
				}
			}

			[Test]
			public void then_it_returns_a_change_for_each_update_folder()
			{
				//NOTE: There are 8 schema folders that SchemaChanges should be created for.
				_results.Count().ShouldEqual(8);
			}

			[Test]
			public void then_it_sets_the_path_to_the_schema_change_folder()
			{
				var path = _results.Cast<SchemaChange>().First().PathToSchemaChangeFolder;

				File.Exists(Path.Combine(path, "Forward.sql")).ShouldBeTrue("Path not set correctly: " + path);
				File.Exists(Path.Combine(path, "Back.sql")).ShouldBeTrue("Path not set correctly: " + path);
			}

			[Test]
			public void then_it_sets_the_schema_changes_path_to_the_full_path_to_the_change_script()
			{
				Path.GetFullPath(_results.Cast<SchemaChange>().First().PathToSchemaChangeFolder)
					.ShouldEqual(_results.Cast<SchemaChange>().First().PathToSchemaChangeFolder);
			}

			[Test]
			public void then_it_sets_the_previous_version_for_each_change()
			{
				_results.ForEach(c => c.PreviousVersion.ShouldNotBeNull());
			}

			[Test]
			public void then_each_changes_previous_version_matches_the_previous_change()
			{
				var changes = _results.ToArray();

				for (int i=1; i < changes.Length; i++)
				{
					changes[i].PreviousVersion.ShouldEqual(changes[i - 1].Version);
				}
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<FileSystemSchemaChangeProvider>
			{
				protected override void ConfigureKernel(Ninject.IKernel kernel)
				{
					base.ConfigureKernel(kernel);

					var testScriptPath = "ChangeScripts";

					//MS Test handles test files very differently than TDD, Resharper, or any real testing framework. 
					if (!Directory.Exists(testScriptPath))
					{
						testScriptPath = @"TestChangeScripts\ChangeScripts";
					}

					kernel.Bind<FileSystemSchemaChangeProvider>().ToMethod(ctx => new FileSystemSchemaChangeProvider(testScriptPath));
				}
			}
		}
	}
}