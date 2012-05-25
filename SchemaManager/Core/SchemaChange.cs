using System.IO;
using SchemaManager.Databases;
using Utilities.Data;

namespace SchemaManager.Core
{
	public class SchemaChange : ScriptBase, ISchemaChange
	{
		private const string BackFile = "Back.sql";
		private const string ForwardFile = "Forward.sql";

		public string PathToSchemaChangeFolder { get; private set; }
		public DatabaseVersion Version { get; private set; }
		public DatabaseVersion PreviousVersion { get; private set; }

		public SchemaChange(string pathToSchemaChangeFolder, DatabaseVersion version, DatabaseVersion previousVersion)
		{
			PathToSchemaChangeFolder = pathToSchemaChangeFolder;
			Version = version;
			PreviousVersion = previousVersion;
		}

		public void Execute(IDbContext context)
		{
			RunAllBatchesFromText(context, File.ReadAllText(Path.Combine(PathToSchemaChangeFolder, ForwardFile)));
		}

		public void Rollback(IDbContext context)
		{
			RunAllBatchesFromText(context, File.ReadAllText(Path.Combine(PathToSchemaChangeFolder, BackFile)));
		}

		public bool NeedsToBeAppliedTo(IDatabase database)
		{
			return Version > database.Revision;
		}

		public bool NeedsToBeRolledBackFrom(IDatabase database)
		{
			return database.Revision >= Version;
		}
	}
}