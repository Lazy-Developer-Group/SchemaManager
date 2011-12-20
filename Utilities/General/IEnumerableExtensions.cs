using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.General
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Do<T>(this IEnumerable<T> list, Action<T> action)
		{
			foreach (var element in list)
			{
				action(element);

				yield return element;
			}
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
		{
			foreach (var element in list)
			{
				action(element);
			}
		}

		public static bool HasAll<T>(this IEnumerable<T> list, Func<T, bool> predicate)
		{
			return list.Any(predicate) && list.All(predicate);
		}
	}
}