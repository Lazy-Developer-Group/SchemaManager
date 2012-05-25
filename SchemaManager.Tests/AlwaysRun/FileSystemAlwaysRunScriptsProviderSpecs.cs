using NUnit.Framework;
using SchemaManager.AlwaysRun;
using SchemaManager.Core;
using SpecsFor;
using System.Linq;
using Should;

namespace SchemaManager.Tests.AlwaysRun
{
	public class FileSystemAlwaysRunScriptsProviderSpecs
	{
		public class when_retreiving_scripts : SpecsFor<FileSystemAlwaysRunScriptsProvider>
		{
			private ISimpleScript[] _results;

			protected override void InitializeClassUnderTest()
			{
				SUT = new FileSystemAlwaysRunScriptsProvider(@"TestScripts\AlwaysRunScripts\");
			}

			protected override void When()
			{
				_results = SUT.GetScripts().ToArray();
			}

			[Test]
			public void then_it_returns_the_expected_number_of_scripts()
			{
				_results.Length.ShouldEqual(2);
			}
		}
	}
}