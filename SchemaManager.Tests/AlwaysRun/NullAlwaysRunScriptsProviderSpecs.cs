using System.Collections.Generic;
using NUnit.Framework;
using SchemaManager.AlwaysRun;
using SchemaManager.Core;
using SpecsFor;
using Should;

namespace SchemaManager.Tests.AlwaysRun
{
	public class NullAlwaysRunScriptsProviderSpecs
	{
		public class when_getting_scripts : SpecsFor<NullAlwaysRunScriptsProvider>
		{
			private IEnumerable<ISimpleScript> _results;

			protected override void When()
			{
				_results = SUT.GetScripts();
			}

			[Test]
			public void then_it_returns_no_scripts()
			{
				_results.ShouldBeEmpty();
			}
		}
	}
}