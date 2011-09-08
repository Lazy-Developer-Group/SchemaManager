
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;

namespace Utilities.Testing
{
	public static class ShouldExtensions
	{
		public static void ShouldStartWith(this string actual, string expected)
		{
			Assert.IsTrue(actual.StartsWith(expected), "{0} does not start with {1}", actual, expected);
		}

		public static void ShouldEndWith(this string actual, string expected)
		{
			Assert.IsTrue(actual.EndsWith(expected), "{0} does not end with {1}", actual, expected);
		}

		public static void ShouldBeLessThan<T>(this T actual, T expected) where T : IComparable
		{
			Assert.IsTrue(actual.CompareTo(expected) < 0, "{0} is not less than {1}", actual, expected);
		}

		public static void ShouldBeGreaterThan<T>(this T actual, T expected) where T : IComparable
		{
			Assert.IsTrue(actual.CompareTo(expected) > 0, "{0} is not greater than {1}", actual, expected);
		}

		//TODO: If/when we pull in Matt's 'looks-like' helper library, compress these extension
		//		methods down. 
		public static void ShouldLookLike<T>(this T[] actual, T[] expected)
		{
			AssertExtensions.LookLikeEachOtherArray(expected, actual);
		}

		public static void ShouldLookLike<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
		{
			AssertExtensions.LookLikeEachOtherArray(expected, actual);
		}

		public static void ShouldLookLike<T>(this T actual, T expected)
		{
			AssertExtensions.LookLikeEachOther(expected, actual);
		}

		public static void ShouldBeTrueForAll<T>(this IEnumerable<T> enumerable, Func<T, bool> assertion)
		{
			foreach (var item in enumerable)
			{
				assertion(item).ShouldBeTrue("Assertion failed for " + item);
			}
		}
	}
}