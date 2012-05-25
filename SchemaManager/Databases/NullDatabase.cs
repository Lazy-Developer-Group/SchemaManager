using SchemaManager.Core;

namespace SchemaManager.Databases
{
	public class NullDatabase : IDatabase
	{
		private readonly IDatabase _underlyingDatabase;

		public NullDatabase(IDatabase underlyingDatabase)
		{
			_underlyingDatabase = underlyingDatabase;
		}

		public void ExecuteUpdate(ISchemaChange schemaChange)
		{
			
		}

		public void ExecuteRollback(ISchemaChange schemaChange)
		{
			
		}

		public void ExecuteScript(ISimpleScript script)
		{
			
		}

		public DatabaseVersion Revision
		{
			get { return _underlyingDatabase.Revision; }
		}
	}
}