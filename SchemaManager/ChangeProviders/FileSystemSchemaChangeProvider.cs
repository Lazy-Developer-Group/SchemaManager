using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SchemaManager.Core;
using System.Linq;
using Utilities.General;

namespace SchemaManager.ChangeProviders
{
	public class FileSystemSchemaChangeProvider : IProvideSchemaChanges
	{
		private readonly string _pathToSchemaScripts;

		public FileSystemSchemaChangeProvider(string pathToSchemaScripts)
		{
			_pathToSchemaScripts = pathToSchemaScripts;
		}

		private static bool IsSchemaChangeFolder(string directory)
		{
			var directoryName = directory.Split('\\').Last();

			return Regex.IsMatch(directoryName, @"^\d+\.");
		}

		private bool IsVersionFolder(string directory)
		{
			var directoryName = directory.Split('\\').Last();

			return Regex.IsMatch(directoryName, @"^\d+\.\d+\.\d+");
		}

		private string GetMajorVersion(string majorVersionFolder)
		{
			return majorVersionFolder.Split('\\').Last();
		}

		private string GetMinorVersion(string schemaChangeFolder)
		{
			//'schemaChange' will look like ParentDir\01.Blah, this parses out the '01' part. 
			return schemaChangeFolder.Split('\\').Last().Split('.').First();
		}

		public IEnumerable<ISchemaChange> GetAllChanges()
		{
			var previousVersion = new DatabaseVersion(1, 0, 0, 0);

			return (from majorVersionFolder in Directory.GetDirectories(_pathToSchemaScripts).Where(d => IsVersionFolder(d))
			        let majorVersion = GetMajorVersion(majorVersionFolder)
			        from schemaChangeFolder in Directory.GetDirectories(majorVersionFolder).Where(d => IsSchemaChangeFolder(d))
			        let minorVersion = GetMinorVersion(schemaChangeFolder)
			        let currentVersion = DatabaseVersion.FromString(majorVersion + "." + minorVersion)
					select new SchemaChange(Path.GetFullPath(schemaChangeFolder), currentVersion, previousVersion))
					.Do(s => previousVersion = s.Version)
					.OrderBy(s => s.Version)
					.Cast<ISchemaChange>();
		}
	}
}