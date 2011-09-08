using System;

namespace Utilities.Web
{
	public class CheckedValueAttribute : Attribute
	{
		public object Value { get; set; }

		public CheckedValueAttribute(object value)
		{
			Value = value;
		}
	}
}