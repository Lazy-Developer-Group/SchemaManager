using System;
using System.Transactions;
using SchemaManager.Core;
using Utilities.Data;

namespace SchemaManager.Databases
{
	public class SqlServerDatabase : IDatabase
	{
		private readonly IDbContext _context;

		private DatabaseVersion _revision;

		public SqlServerDatabase(IDbContext context)
		{
			_context = context;
		}

		public DatabaseVersion Revision
		{
			get { return _revision ?? (_revision = LoadRevision()); }
		}

		private DatabaseVersion LoadRevision()
		{
			var command = _context.CreateCommand();

			command.CommandText = "SELECT COUNT(*) FROM sys.extended_properties WHERE name = 'DatabaseVersion'";
			command.CommandTimeout = 60;
			var count = (int)command.ExecuteScalar();

			if (count == 0)
			{
				using (var scope = new TransactionScope())
				{
					command.CommandText = "exec sp_addextendedproperty @name='DatabaseVersion', @value='1.0.0.0'";
					command.ExecuteNonQuery();

					scope.Complete();
				}
			}

			command.CommandText = "SELECT value FROM sys.extended_properties WHERE name = 'DatabaseVersion'";
			var value = (string)command.ExecuteScalar();

			return DatabaseVersion.FromString(value);
		}

		private void SetDatabaseRevisionTo(DatabaseVersion version)
		{
			var command = _context.CreateCommand();
			command.CommandText = string.Format("exec sp_updateextendedproperty @name='DatabaseVersion', @value='{0:F2}.{1:F0}'",
			                                    version.MajorVersion, version.MinorVersion);

			command.ExecuteNonQuery();

			_revision = new DatabaseVersion(version.MajorVersion, version.MinorVersion, version.PatchVersion, version.ScriptVersion);
		}

		public void ExecuteUpdate(ISchemaChange schemaChange)
		{
			using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
			{
				schemaChange.Execute(_context);

				SetDatabaseRevisionTo(schemaChange.Version);

				transaction.Complete();
			}
		}

		public void ExecuteRollback(ISchemaChange schemaChange)
		{
			using (var transaction = new TransactionScope())
			{
				schemaChange.Rollback(_context);

				SetDatabaseRevisionTo(schemaChange.PreviousVersion);

				transaction.Complete();
			}
		}
	}
}