using System;
using NUnit.Framework;

namespace SchemaManager.Tests.Helpers
{
	public static class ShouldExtensions
	{
		public static void ShouldBeLessThan<T>(this T actual, T expected) where T : IComparable
		{
			Assert.IsTrue(actual.CompareTo(expected) < 0, "{0} is not less than {1}", actual, expected);
		}

		public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable
		{
			Assert.IsTrue(actual.CompareTo(expected) > 0, "{0} is not greater than {1}", actual, expected);
		}
	}
}