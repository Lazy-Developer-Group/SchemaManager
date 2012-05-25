using Utilities.Data;

namespace SchemaManager.Core
{
	public interface ISimpleScript
	{
		void Execute(IDbContext context);
	}
}