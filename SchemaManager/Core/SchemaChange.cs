using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SchemaManager.Databases;
using Utilities.Data;

namespace SchemaManager.Core
{
	public class SchemaChange : ISchemaChange
	{
		private const string BackFile = "Back.sql";
		private const string ForwardFile = "Forward.sql";

		private static readonly Regex _batchSplitter = new Regex(@"^GO\W*$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public string PathToSchemaChangeFolder { get; private set; }
		public DatabaseVersion Version { get; private set; }
		public DatabaseVersion PreviousVersion { get; private set; }

		public SchemaChange(string pathToSchemaChangeFolder, DatabaseVersion version, DatabaseVersion previousVersion)
		{
			PathToSchemaChangeFolder = pathToSchemaChangeFolder;
			Version = version;
			PreviousVersion = previousVersion;
		}

		private IEnumerable<string> GetBatchesFrom(string sqlScriptFile)
		{
			var sql = File.ReadAllText(sqlScriptFile);

			return from batch in _batchSplitter.Split(sql)
			       let trimmedBatch = batch.Trim()
			       where !string.IsNullOrEmpty(trimmedBatch)
			       select trimmedBatch;
		}

		private void RunAllBatchesFromFile(IDbContext context, string sqlScriptFile)
		{
			foreach (var sqlBatch in GetBatchesFrom(sqlScriptFile))
			{
				var command = context.CreateCommand();
				command.CommandTimeout = (int)TimeSpan.FromMinutes(5).TotalSeconds;

				command.CommandText = sqlBatch;

				command.ExecuteNonQuery();
			}
		}

		public void Execute(IDbContext context)
		{
			RunAllBatchesFromFile(context, Path.Combine(PathToSchemaChangeFolder, ForwardFile));
		}

		public void Rollback(IDbContext context)
		{
			RunAllBatchesFromFile(context, Path.Combine(PathToSchemaChangeFolder, BackFile));
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