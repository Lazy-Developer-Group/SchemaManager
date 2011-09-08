using System.Linq;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;

namespace SchemaManager.Update
{
	public class DatabaseUpdater : IUpdateDatabase
	{
		private readonly IProvideSchemaChanges _schemaChangeProvider;
		private readonly ILogger _logger;
		private readonly IDatabase _database;
		private readonly DatabaseVersion _targetVersion;

		public DatabaseUpdater(IProvideSchemaChanges schemaChangeProvider, ILogger logger, IDatabase database, DatabaseVersion targetVersion)
		{
			_schemaChangeProvider = schemaChangeProvider;
			_database = database;
			_targetVersion = targetVersion;
			_logger = logger;
		}

		public void ApplyUpdates()
		{
			if (_targetVersion == DatabaseVersion.Max)
			{
				_logger.Info("Applying all available updates to database...");
			}
			else
			{
				_logger.Info("Updating database to revision {0}...", _targetVersion);
			}

			foreach (var update in _schemaChangeProvider.GetAllChanges().Where(u => u.Version <= _targetVersion))
			{
				if (update.NeedsToBeAppliedTo(_database))
				{
					_logger.Info("Applying update for database version {0}...", update.Version);
					_database.ExecuteUpdate(update);
					_logger.Info("Finished.");
				}
			}

			_logger.Info("Database is at revision {0}", _database.Revision);

			_logger.Info("Done!");
		}
	}
}