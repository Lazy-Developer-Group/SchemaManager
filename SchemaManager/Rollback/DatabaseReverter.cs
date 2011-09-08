using System.Linq;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;

namespace SchemaManager.Rollback
{
	public class DatabaseReverter : IRollbackDatabase
	{
		private readonly IProvideSchemaChanges _schemaChangeProvider;
		private readonly ILogger _logger;
		private readonly IDatabase _database;
		private readonly DatabaseVersion _targetVersion;

		public DatabaseReverter(IProvideSchemaChanges schemaChangeProvider, ILogger logger, IDatabase database, DatabaseVersion targetVersion)
		{
			_schemaChangeProvider = schemaChangeProvider;
			_logger = logger;
			_database = database;
			_targetVersion = targetVersion;
		}

		public void ApplyRollbacks()
		{
			_logger.Info("Reverting database to revision {0}...", _targetVersion);

			foreach (var change in _schemaChangeProvider.GetAllChanges().Reverse().Where(u => u.Version > _targetVersion))
			{
				if (change.NeedsToBeRolledBackFrom(_database))
				{
					_logger.Info("Applying rollback for database version {0}...", change.Version);
					_database.ExecuteRollback(change);
					_logger.Info("Finished.");
				}
			}

			_logger.Info("Database is at revision {0}", _database.Revision);

			_logger.Info("Done!");
		}
	}
}