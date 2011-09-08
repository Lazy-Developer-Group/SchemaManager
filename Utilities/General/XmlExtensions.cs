using System.IO;
using System.Xml.Serialization;

namespace Utilities.General
{
	public static class XmlExtensions
	{
		public static string ToXml(this object obj)
		{
			var serializer = new XmlSerializer(obj.GetType());

			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, obj);
				return writer.ToString();
			}
		}
	}
}