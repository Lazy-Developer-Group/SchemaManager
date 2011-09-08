using System.Linq;
using System.Linq.Dynamic;

namespace Utilities.General
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> PageAndSort<T>(this IQueryable<T> list, int pageSize, int page, string sortField)
        {
            return list.OrderBy(sortField, null).Skip(pageSize * (page - 1)).Take(pageSize);
        }

        public static IQueryable<T> PageAndSortDescending<T>(this IQueryable<T> list, int pageSize, int page, string sortField)
        {
            return list.OrderBy(sortField + " DESC", null).Skip(pageSize * (page - 1)).Take(pageSize);
        }

        //public static IQueryable<T> OrderBy<T>(this IQueryable<T> list, string sortField)
        //{
        //    if (string.IsNullOrEmpty(sortField))
        //    {
        //        return list;
        //    }

        //    var prop = typeof(T).GetProperty(sortField);

        //    if (prop != null)
        //    {
        //        return list.OrderBy(x => prop.GetValue(x, null));
        //    }

        //    var field = typeof(T).GetField(sortField);

        //    if (field == null)
        //    {
        //        throw new Exception("No property or field '" + sortField + "' in + " + typeof(T).Name + "'");
        //    }

        //    return list.OrderBy(x => field.GetValue(x));

        //}

        //public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> list, string sortField)
        //{
        //    if (string.IsNullOrEmpty(sortField))
        //    {
        //        return list;
        //    }

        //    var prop = typeof(T).GetProperty(sortField);

        //    if (prop != null)
        //    {
        //        return list.OrderByDescending(x => prop.GetValue(x, null));
        //    }

        //    var field = typeof(T).GetField(sortField);

        //    if (field == null)
        //    {
        //        throw new Exception("No property or field '" + sortField + "' in + " + typeof(T).Name + "'");
        //    }

        //    return list.OrderByDescending(x => field.GetValue(x));

        //}
    }
}
