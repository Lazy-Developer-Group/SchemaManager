using System.Collections.Generic;
using SchemaManager.Core;
using System.Linq;
using SchemaManager.Databases;
using Utilities.Data;

namespace SchemaManager.ChangeProviders
{
	public class RebuildLatestVersionDecorator : IProvideSchemaChanges
	{
		private readonly IProvideSchemaChanges _provider;

		public RebuildLatestVersionDecorator(IProvideSchemaChanges changeProvider)
		{
			_provider = changeProvider;
		}

		public IEnumerable<ISchemaChange> GetAllChanges()
		{
			var changes = _provider.GetAllChanges();

			var maxVersion = changes.Max(c => c.Version.MajorVersion);

			return from c in changes
			       select c.Version.MajorVersion == maxVersion
			              	? new ChangeDecorator(c)
			              	: c;
		}

		//This decoator is used to force all of the most recent major version's
		//changes to be re-applied. 
		private class ChangeDecorator : ISchemaChange
		{
			private readonly ISchemaChange _schemaChange;

			public ChangeDecorator(ISchemaChange schemaChange)
			{
				_schemaChange = schemaChange;
			}

			public void Rollback(IDbContext context)
			{
				_schemaChange.Rollback(context);
			}

			public bool NeedsToBeAppliedTo(IDatabase database)
			{
				return true;
			}

			public bool NeedsToBeRolledBackFrom(IDatabase database)
			{
				return false;
			}

			public DatabaseVersion Version
			{
				get { return _schemaChange.Version; }
			}

			public DatabaseVersion PreviousVersion
			{
				get { return _schemaChange.PreviousVersion; }
			}

			public void Execute(IDbContext context)
			{
				_schemaChange.Execute(context);
			}
		}
	}
}