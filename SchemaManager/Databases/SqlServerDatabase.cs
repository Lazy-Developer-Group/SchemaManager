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
			using (var command = _context.CreateCommand())
			{
				command.CommandText = "SELECT COUNT(*) FROM sys.extended_properties WHERE name = 'DatabaseVersion'";
				var count = (int) command.ExecuteScalar();

				if (count == 0)
				{
					command.CommandText = "exec sp_addextendedproperty @name='DatabaseVersion', @value='1.0.0.0'";
					command.ExecuteNonQuery();
				}

				command.CommandText = "SELECT value FROM sys.extended_properties WHERE name = 'DatabaseVersion'";
				var value = (string) command.ExecuteScalar();

				return DatabaseVersion.FromString(value);
			}
		}

		private void SetDatabaseRevisionTo(DatabaseVersion version)
		{
			using (var command = _context.CreateCommand())
			{
				command.CommandText = string.Format("exec sp_updateextendedproperty @name='DatabaseVersion', @value='{0}.{1}.{2}.{3}'",
													version.MajorVersion, version.MinorVersion, version.PatchVersion, version.ScriptVersion);

				command.ExecuteNonQuery();

				_revision = new DatabaseVersion(version.MajorVersion, version.MinorVersion, version.PatchVersion, version.ScriptVersion);
			}
		}

		public void ExecuteUpdate(ISchemaChange schemaChange)
		{
			schemaChange.Execute(_context);

			SetDatabaseRevisionTo(schemaChange.Version);
		}

		public void ExecuteRollback(ISchemaChange schemaChange)
		{
			schemaChange.Rollback(_context);

			SetDatabaseRevisionTo(schemaChange.PreviousVersion);
		}

		public void ExecuteScript(ISimpleScript script)
		{
			script.Execute(_context);
		}
	}
}