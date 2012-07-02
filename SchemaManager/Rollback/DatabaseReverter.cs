using System.Linq;
using System.Transactions;
using SchemaManager.AlwaysRun;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;

namespace SchemaManager.Rollback
{
	public class DatabaseReverter : IRollbackDatabase
	{
		private readonly IProvideAlwaysRunScripts _alwaysRunScripts;
		private readonly IProvideSchemaChanges _schemaChangeProvider;
		private readonly ILogger _logger;
		private readonly IDatabase _database;
		private readonly DatabaseVersion _targetVersion;

		public DatabaseReverter(IProvideAlwaysRunScripts alwaysRunScripts, IProvideSchemaChanges schemaChangeProvider, ILogger logger, IDatabase database, DatabaseVersion targetVersion)
		{
			_alwaysRunScripts = alwaysRunScripts;
			_schemaChangeProvider = schemaChangeProvider;
			_logger = logger;
			_database = database;
			_targetVersion = targetVersion;
		}

		public void ApplyRollbacks()
		{
			using (var scope = new TransactionScope())
			{
				_logger.Info("Executing 'always run' scripts...");

				foreach (var script in _alwaysRunScripts.GetScripts())
				{
					_database.ExecuteScript(script);
				}

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

				scope.Complete();
			}

			_logger.Info("Database is at revision {0}", _database.Revision);

			_logger.Info("Done!");
		}
	}
}