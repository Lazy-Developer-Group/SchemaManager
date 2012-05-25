using Microsoft.Build.Utilities;
using Ninject.Modules;
using SchemaManager.AlwaysRun;
using SchemaManager.ChangeProviders;
using SchemaManager.Core;
using SchemaManager.Databases;
using SchemaManager.Rollback;
using SchemaManager.Update;
using Utilities.Data;

namespace SchemaManager.Infrastructure
{
	public class SchemaManagerModule : NinjectModule
	{
		private readonly Task _owner;
		private readonly string _pathToSchemaScripts;
		private readonly string _connectionString;
		private readonly string _pathToAlwaysRunScripts;
		private readonly DatabaseVersion _targetVersion;
		private readonly bool _whatIf;

		public SchemaManagerModule(Task owner, string pathToSchemaScripts, string pathToAlwaysRunScripts, string connectionString, DatabaseVersion targetVersion, bool whatIf)
		{
			_owner = owner;
			_pathToSchemaScripts = pathToSchemaScripts;
			_connectionString = connectionString;
			_pathToAlwaysRunScripts = pathToAlwaysRunScripts;
			_targetVersion = targetVersion;
			_whatIf = whatIf;
		}

		public override void Load()
		{
			Bind<Task>().ToConstant(_owner);

			Bind<IUpdateDatabase>().To<DatabaseUpdater>()
				.WithConstructorArgument("targetVersion", _targetVersion);

			Bind<IRollbackDatabase>().To<DatabaseReverter>()
				.WithConstructorArgument("targetVersion", _targetVersion);

			Bind<ILogger>().To<MSBuildLoggerAdapter>();

			Bind<IDatabase>().To<SqlServerDatabase>();

			Bind<IDbContext>().To<DbContext>()
				.InSingletonScope()
				.WithConstructorArgument("connectionString", _connectionString);

			Bind<IProvideSchemaChanges>().To<FileSystemSchemaChangeProvider>()
				.WithConstructorArgument("pathToSchemaScripts", _pathToSchemaScripts);

			if (!string.IsNullOrEmpty(_pathToAlwaysRunScripts))
			{
				Bind<IProvideAlwaysRunScripts>().To<FileSystemAlwaysRunScriptsProvider>()
					.WithConstructorArgument("pathToScripts", _pathToAlwaysRunScripts);
			}
			else
			{
				Bind<IProvideAlwaysRunScripts>().To<NullAlwaysRunScriptsProvider>();
			}

			if (_whatIf)
			{
				Unbind<IDatabase>();
				Bind<IDatabase>().To<SqlServerDatabase>().WhenInjectedInto<NullDatabase>();
				Bind<IDatabase>().To<NullDatabase>();
			}
		}
	}
}