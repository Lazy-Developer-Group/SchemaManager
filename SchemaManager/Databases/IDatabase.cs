using SchemaManager.Core;

namespace SchemaManager.Databases
{
	public interface IDatabase
	{
		DatabaseVersion Revision { get; }
		void ExecuteUpdate(ISchemaChange schemaChange);
		void ExecuteRollback(ISchemaChange schemaChange);
		void ExecuteScript(ISimpleScript script);
	}
}