using System.Collections.Generic;
using SchemaManager.Core;

namespace SchemaManager.ChangeProviders
{
	public interface IProvideSchemaChanges
	{
		IEnumerable<ISchemaChange> GetAllChanges();
	}
}