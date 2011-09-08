using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Utilities.Web
{
	public static class GridViewExtensions
	{
		public static DataControlFieldCell GetCellFor(this GridViewRow row, string columnName)
		{
			var cell = row.Cells.OfType<DataControlFieldCell>()
				.Where(c => c.ContainingField is BoundField)
				.Where(c => ((BoundField)c.ContainingField).DataField == columnName)
				.FirstOrDefault();

			if (cell == null)
			{
				throw new InvalidOperationException("No cell found for column named " + columnName);
			}

			return cell;
		}
	}
}