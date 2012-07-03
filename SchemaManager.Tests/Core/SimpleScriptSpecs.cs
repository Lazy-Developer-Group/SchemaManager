using System.Data;
using Moq;
using NUnit.Framework;
using SchemaManager.Core;
using SpecsFor;
using Utilities.Data;

namespace SchemaManager.Tests.Core
{
	public class SimpleScriptSpecs
	{
		public class when_running_a_simple_script : SpecsFor<SimpleScript>
		{
			public const string Script = @"Line 1
Go
Line 2
Go
Line 3";

			protected override void InitializeClassUnderTest()
			{
				SUT = new SimpleScript(Script);
			}

			protected override void Given()
			{
				GetMockFor<IDbContext>()
					.Setup(c => c.CreateCommand())
					.Returns(GetMockFor<ICommand>().Object);
			}

			protected override void When()
			{
				SUT.Execute(GetMockFor<IDbContext>().Object);
			}

			[Test]
			public void then_it_runs_all_the_batches_in_the_file()
			{
				GetMockFor<ICommand>()
					.Verify(c => c.ExecuteNonQuery(), Times.Exactly(3));
			}
		}
	}
}