using System;
using NUnit.Framework;
using SchemaManager.Core;
using SchemaManager.Tests.Helpers;
using StructureMap;
using Should;
using SpecsFor;
using SpecsFor.ShouldExtensions;

namespace SchemaManager.Tests.Core
{
	public class DatabaseVersionSpecs
	{
		[TestFixture]
		public class when_comparing_to_a_newer_revision : given.the_default_state
		{
			private int _result;

			protected override void When()
			{
				_result = SUT.CompareTo(new DatabaseVersion(2, 0, 0, 0));
			}

			[Test]
			public void then_it_is_less_than_the_other_revision()
			{
				_result.ShouldBeLessThan(0);
			}
		}

		[TestFixture]
		public class when_comparing_to_an_older_revision : given.the_default_state
		{
			private int _result;

			protected override void When()
			{
				_result = SUT.CompareTo(new DatabaseVersion(0, 0, 0, 0));
			}

			[Test]
			public void then_it_is_greater_than_the_other_revision()
			{
				_result.ShouldBeGreaterThan(0);
			}
		}

		[TestFixture]
		public class when_comparing_to_a_revision_thats_newer_by_minor_version_only : given.the_default_state
		{
			private int _result;

			protected override void When()
			{
				_result = SUT.CompareTo(new DatabaseVersion(1, 0, 0, 1));
			}

			[Test]
			public void then_it_is_less_than_the_newer_version()
			{
				_result.ShouldBeLessThan(0);
			}
		}

		[TestFixture]
		public class when_to_stringing_the_version : given.the_default_state
		{
			private string _result;

			protected override void When()
			{
				_result = new DatabaseVersion(1, 2, 5, 13).ToString();
			}

			[Test]
			public void then_it_formats_the_string_correctly()
			{
				_result.ShouldEqual("1.2.5.13");
			}
		}

		[TestFixture]
		public class when_to_stringing_the_max_version : given.the_default_state
		{
			private string _result;

			protected override void When()
			{
				_result = DatabaseVersion.Max.ToString();
			}

			[Test]
			public void then_it_formats_the_string_correctly()
			{
				_result.ShouldEqual("*.*.*.*");
			}
		}

		[TestFixture]
		public class when_to_stringing_with_the_minor_version_set_to_max : given.the_default_state
		{
			private string _result;

			protected override void When()
			{
				_result = (new DatabaseVersion(1, 2, 5, int.MaxValue)).ToString();
			}

			[Test]
			public void then_the_minor_version_is_represented_by_an_asterisk()
			{
				_result.ShouldEqual("1.2.5.*");
			}
		}

		[TestFixture]
		public class when_creating_a_version_without_a_database_version : given.the_default_state
		{
			private DatabaseVersion _result;

			protected override void When()
			{
				_result = DatabaseVersion.FromString("1.2.5");
			}

			[Test]
			public void then_it_parses_the_major_version()
			{
				_result.MajorVersion.ShouldEqual(1);
			}

			[Test]
			public void then_it_parses_the_minor_version()
			{
				_result.MinorVersion.ShouldEqual(2);
			}
			
			[Test]
			public void then_it_parses_the_patch_version()
			{
				_result.PatchVersion.ShouldEqual(5);
			}
			
			[Test]
			public void then_the_database_version_is_set_to_max_value()
			{
				_result.ScriptVersion.ShouldEqual(int.MaxValue);
			}
		}

		[TestFixture]
		public class when_creating_a_version_from_an_improperly_formatted_string : given.the_default_state
		{
			private ArgumentException _result;

			protected override void When()
			{
				_result = Assert.Throws<ArgumentException>(() => DatabaseVersion.FromString("1"));
			}

			[Test]
			public void then_it_throws_an_exception()
			{
				_result.ShouldNotBeNull();
			}

			[Test]
			public void then_the_message_includes_the_expected_format()
			{
				_result.Message.ShouldContain("##.##.##.##");
			}
		}

		public class when_parsing_a_fykk_version_number : given.the_default_state
		{
			private DatabaseVersion _result;

			protected override void When()
			{
				_result = DatabaseVersion.FromString("1.2.6.13");
			}

			[Test]
			public void then_it_populates_the_version_parts_correctly()
			{
				_result.ShouldLookLikePartial(new {MajorVersion = 1, MinorVersion = 2, PatchVersion = 6, ScriptVersion = 13});
			}
		}

		public static class given
		{
			public abstract class the_default_state : SpecsFor<DatabaseVersion>
			{
				protected override void ConfigureContainer(IContainer container)
				{
					container.Configure(cfg => cfg.For<DatabaseVersion>().Use(new DatabaseVersion(1, 0, 0, 0)));

					base.ConfigureContainer(container);
				}
			}
		}
	}
}