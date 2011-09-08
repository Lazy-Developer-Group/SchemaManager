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

			return Regex.IsMatch(directoryName, @"^\d+\.\d+");
		}

		private double GetMajorVersion(string majorVersionFolder)
		{
			double majorVersion;
			if (!double.TryParse(majorVersionFolder.Split('\\').Last(), out majorVersion))
			{
				throw new InvalidOperationException("Unable to exract the major version from folder: " + majorVersionFolder);
			}

			return majorVersion;
		}

		private double GetMinorVersion(string schemaChangeFolder)
		{
			double minorVersion;

			//'schemaChange' will look like ParentDir\01.Blah, this parses out the '01' part. 
			if (!double.TryParse(schemaChangeFolder.Split('\\').Last().Split('.').First(), out minorVersion))
			{
				throw new InvalidOperationException("Unable to extract the minor version from folder: " + minorVersion);
			}

			return minorVersion;
		}

		public IEnumerable<ISchemaChange> GetAllChanges()
		{
			var previousVersion = new DatabaseVersion(1, 0);

			return (from majorVersionFolder in Directory.GetDirectories(_pathToSchemaScripts).Where(d => IsVersionFolder(d))
			        let majorVersion = GetMajorVersion(majorVersionFolder)
			        from schemaChangeFolder in Directory.GetDirectories(majorVersionFolder).Where(d => IsSchemaChangeFolder(d))
			        let minorVersion = GetMinorVersion(schemaChangeFolder)
			        let currentVersion = new DatabaseVersion(majorVersion, minorVersion)
					select new SchemaChange(Path.GetFullPath(schemaChangeFolder), currentVersion, previousVersion))
					.Do(s => previousVersion = s.Version)
					.OrderBy(s => s.Version)
					.Cast<ISchemaChange>();
		}
	}
}