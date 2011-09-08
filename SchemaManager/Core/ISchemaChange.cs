using SchemaManager.Databases;
using Utilities.Data;

namespace SchemaManager.Core
{
	public interface ISchemaChange
	{
		DatabaseVersion Version { get; }
		DatabaseVersion PreviousVersion { get; }

		void Execute(IDbContext context);
		void Rollback(IDbContext context);
		bool NeedsToBeAppliedTo(IDatabase database);
		bool NeedsToBeRolledBackFrom(IDatabase database);
	}
}