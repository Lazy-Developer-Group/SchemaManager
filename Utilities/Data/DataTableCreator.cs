using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Utilities.General;

namespace Utilities.Data
{
	public class DataTableCreator
	{
		public static DataTable FromObject(object[] obj)
		{
			if(obj.Length == 0)
			{
				return new DataTable();
			}

			var dataTable = new DataTable();

			dataTable.BeginLoadData();

			var properties = obj[0].GetType().GetProperties();

			properties.ForEach(p => dataTable.Columns.Add(p.Name, p.PropertyType));

			obj.ForEach(o => dataTable.Rows.Add(GetPropertiesArray(properties, o)));
			dataTable.EndLoadData();

			return dataTable;
		}

		private static object[] GetPropertiesArray(IEnumerable<PropertyInfo> properties, object obj)
		{
			return (from p in properties
			        select p.GetValue(obj, null)).ToArray();
		}
	}
}
