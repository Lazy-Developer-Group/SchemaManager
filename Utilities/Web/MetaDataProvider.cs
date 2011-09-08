using System.Linq;
using System.Web.Mvc;

namespace Utilities.Web
{
	public class MetaDataProvider : DataAnnotationsModelMetadataProvider
	{
		protected override ModelMetadata CreateMetadata(System.Collections.Generic.IEnumerable<System.Attribute> attributes, System.Type containerType, System.Func<object> modelAccessor, System.Type modelType, string propertyName)
		{
			var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

			var additionalValues = attributes.OfType<HtmlPropertiesAttribute>().FirstOrDefault();

			if (additionalValues != null)
			{

				metadata.AdditionalValues.Add("HtmlAttributes", additionalValues);

			}

			return metadata;
		}

	}
}