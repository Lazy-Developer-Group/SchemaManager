using System.Collections.Generic;
using SchemaManager.Core;

namespace SchemaManager.AlwaysRun
{
	public interface IProvideAlwaysRunScripts
	{
		IEnumerable<ISimpleScript> GetScripts();
	}
}