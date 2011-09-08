using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.General
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> PageAndSort<T>(this IEnumerable<T> list, int pageSize, int page, string sortField)
        {
            return list.OrderBy(sortField).Skip(pageSize*(page - 1)).Take(pageSize);
        }

        public static IEnumerable<T> PageAndSortDescending<T>(this IEnumerable<T> list, int pageSize, int page, string sortField)
        {
            return list.OrderByDescending(sortField).Skip(pageSize * (page - 1)).Take(pageSize);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortField)
        {
            if(string.IsNullOrEmpty(sortField))
            {
                return list;
            }

            var prop = typeof(T).GetProperty(sortField);

            if (prop != null)
            {
                return list.OrderBy(x => prop.GetValue(x, null));
            }

            var field = typeof (T).GetField(sortField);

            if(field == null)
            {
                throw new Exception("No property or field '" + sortField + "' in + " + typeof(T).Name + "'");
            }

            return list.OrderBy(x => field.GetValue(x));

        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> list, string sortField)
        {
            if (string.IsNullOrEmpty(sortField))
            {
                return list;
            }

            var prop = typeof(T).GetProperty(sortField);

            if (prop != null)
            {
                return list.OrderByDescending(x => prop.GetValue(x, null));
            }

            var field = typeof(T).GetField(sortField);

            if (field == null)
            {
                throw new Exception("No property or field '" + sortField + "' in + " + typeof(T).Name + "'");
            }

            return list.OrderByDescending(x => field.GetValue(x));

        }

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