using System.Web.Mvc;

namespace Utilities.ActionResults
{
	public class JsonResult<TModel> : JsonResult
	{
		public new TModel Data
		{
			get { return (TModel) base.Data; }
			set { base.Data = value; }
		}
	}
}