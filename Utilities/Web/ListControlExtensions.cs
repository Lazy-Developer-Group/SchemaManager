using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Utilities.Web
{ 
    public static class ListControlExtensions
    {
        public static void PopulateListFromEnum<TEnum>(this ListControl list, string blankText, string blankValue, Func<TEnum, string> setDescription)
        {
            list.Items.Add(new ListItem(blankText, blankValue));

            list.PopulateListFromEnum(setDescription);
        }

        public static void PopulateListFromEnum<TEnum>(this ListControl list, Func<TEnum, string> setDescription)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            foreach (var value in values.OrderBy(t => t.ToString()))
            {
                var enumValue = (TEnum)value;
                list.Items.Add(new ListItem(setDescription(enumValue), value.ToString()));
            }
        }

        public static void PopulateList<T>(this ListControl list, string blankText, IEnumerable<T> items,
                                           Func<T, string> setDescription, Func<T, int> setID)
        {
            list.PopulateList(blankText, "", items, setDescription, setID);
        }

        public static void PopulateList<T>(this ListControl list, string blankText, string blankValue, IEnumerable<T> items,
                                           Func<T, string> setDescription, Func<T, int> setID)
        {
            list.Items.Add(new ListItem(blankText, blankValue));

            foreach (var item in items)
            {
                list.Items.Add(new ListItem(setDescription(item), setID(item).ToString()));
            }
        }

        public static void PopulateList<T>(this ListControl list, IEnumerable<T> items,
                                           Func<T, string> setDescription, Func<T, int> setID)
        {
            foreach (var item in items)
            {
                list.Items.Add(new ListItem(setDescription(item), setID(item).ToString()));
            }
        }

        public static void PopulateList<T>(this ListControl list, IEnumerable<T> items,
                                           Func<T, string> setDescription, Func<T, string> setID)
        {
            foreach (var item in items)
            {
                list.Items.Add(new ListItem(setDescription(item), setID(item)));
            }
        }

        public static void PopulateList<T>(this ListControl list, string blankText, IEnumerable<T> items,
                                           Func<T, string> setDescription, Func<T, string> setID)
        {
            list.Items.Add(new ListItem(blankText, ""));

            list.PopulateList(items, setDescription, setID);
        }
    }
}