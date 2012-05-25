using System.Collections.Generic;
using SchemaManager.Core;

namespace SchemaManager.AlwaysRun
{
	public class NullAlwaysRunScriptsProvider : IProvideAlwaysRunScripts
	{
		public IEnumerable<ISimpleScript> GetScripts()
		{
			yield break;
		}
	}
}