using System.Collections.Generic;
using System.IO;
using System.Linq;
using SchemaManager.Core;

namespace SchemaManager.AlwaysRun
{
	public class FileSystemAlwaysRunScriptsProvider : IProvideAlwaysRunScripts
	{
		private readonly string _pathToScripts;

		public FileSystemAlwaysRunScriptsProvider(string pathToScripts)
		{
			_pathToScripts = pathToScripts;
		}

		public IEnumerable<ISimpleScript> GetScripts()
		{
			return Directory.EnumerateFiles(_pathToScripts).Select(script => new SimpleScript(File.ReadAllText(script)));
		}
	}
}