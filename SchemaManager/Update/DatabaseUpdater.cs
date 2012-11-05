using System;
using System.Linq;
using System.Transactions;
using SchemaManager.AlwaysRun;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;

namespace SchemaManager.Update
{
	public class DatabaseUpdater : IUpdateDatabase
	{
		private readonly IProvideAlwaysRunScripts _alwaysRunScripts;
		private readonly IProvideSchemaChanges _schemaChangeProvider;
		private readonly ILogger _logger;
		private readonly IDatabase _database;
		private readonly SchemaManagerGlobalOptions _globalOptions;

		private TransactionScope BuildTransactionScope()
		{
			return new TransactionScope(TransactionScopeOption.Required, _globalOptions.Timeout);
		}

		public DatabaseUpdater(IProvideAlwaysRunScripts alwaysRunScripts, 
			IProvideSchemaChanges schemaChangeProvider, 
			ILogger logger, 
			IDatabase database, 
			SchemaManagerGlobalOptions globalOptions)
		{
			_alwaysRunScripts = alwaysRunScripts;
			_schemaChangeProvider = schemaChangeProvider;
			_database = database;
			_globalOptions = globalOptions;
			_logger = logger;
		}

		public void ApplyUpdates()
		{
			TransactionScope scope = null;
			//This is one case where using != try/finally: when the variable in question
			//may be re-assigned. 
			try
			{
				scope = BuildTransactionScope();
				_logger.Info("Executing 'always run' scripts...");

				foreach (var script in _alwaysRunScripts.GetScripts())
				{
					_database.ExecuteScript(script);
				}

				_logger.Info("Database is currently at version {0}.", _database.Revision);

				if (_globalOptions.TargetRevision == DatabaseVersion.Max)
				{
					_logger.Info("Applying all available updates to database, timeout set to {0} minutes...", _globalOptions.Timeout.TotalMinutes);
				}
				else
				{
					_logger.Info("Updating database to revision {0}, timeout set to {1} minutes...", _globalOptions.TargetRevision, _globalOptions.Timeout.TotalMinutes);
				}

				foreach (var update in _schemaChangeProvider.GetAllChanges().Where(u => u.Version <= _globalOptions.TargetRevision))
				{
					if (update.NeedsToBeAppliedTo(_database))
					{
						_logger.Info("Applying update for database version {0}...", update.Version);
						_database.ExecuteUpdate(update);
						_logger.Info("Finished.");

						if (_globalOptions.UseIncrementalTransactions)
						{
							_logger.Info("Committing transaction...");
							scope.Complete();
							scope.Dispose();
							scope = BuildTransactionScope();
							_logger.Info("Done.");
						}
					}
				}

				scope.Complete();
			}
			finally
			{
				if (scope != null)
				{
					scope.Dispose();
				}
			}

			_logger.Info("Database is at revision {0}", _database.Revision);

			_logger.Info("Done!");
		}
	}
}