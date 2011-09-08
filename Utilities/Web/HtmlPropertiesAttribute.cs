using System;
using System.Collections.Generic;

namespace Utilities.Web
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class HtmlPropertiesAttribute : Attribute
	{
		public string CssClass { get; set; }
		public int Size { get; set; }
		public int MaxLength { get; set; }
		public int MinLength { get; set; }
		public int Cols { get; set; }
		public int Rows { get; set; }
		public string Width { get; set; }

		public IDictionary<string, object> HtmlAttributes()
		{
			IDictionary<string, object> htmlatts = new Dictionary<string, object>();
			if (MaxLength != 0)
			{
				htmlatts.Add("maxlength", MaxLength);
			}
			if (MinLength != 0)
			{
				htmlatts.Add("minlength", MinLength);
			}
			if (Size != 0)
			{
				htmlatts.Add("size", Size);
			}
			if (CssClass != "")
			{
				htmlatts.Add("class", CssClass);
			}
			if (Cols != 0)
			{
				htmlatts.Add("cols", Cols);
			}
			if (Rows != 0)
			{
				htmlatts.Add("rows", Rows);
			}
			if (Width != "")
			{
				htmlatts.Add("width", Width);
			}

			return htmlatts;
		}
	}
}