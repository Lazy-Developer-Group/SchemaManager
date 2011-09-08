using System.Reflection;
using System.Linq;

namespace Utilities.General
{
	public static class ReflectionExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
		{
			return (attributeProvider.GetCustomAttributes(true)).OfType<TAttribute>().FirstOrDefault();
		}
	}
}