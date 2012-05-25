using Utilities.Data;

namespace SchemaManager.Core
{
	public class SimpleScript : ScriptBase, ISimpleScript
	{
		private readonly string _script;

		public SimpleScript(string script)
		{
			_script = script;
		}

		public void Execute(IDbContext context)
		{
			RunAllBatchesFromText(context, _script);
		}
	}
}