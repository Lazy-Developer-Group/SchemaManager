using Ninject;
using SchemaManager.ChangeProviders;
using SchemaManager.Infrastructure;

namespace SchemaManager.Update
{
	public class UpdateDatabase : SchemaChangeTaskBase
	{
		public bool RebuildLatestVersion { get; set; }

		protected override void RunSchemaChanges(StandardKernel kernel)
		{
			var updater = kernel.Get<IUpdateDatabase>();
			updater.ApplyUpdates();
		}

		protected override StandardKernel BuildKernel()
		{
			var kernel = base.BuildKernel();

			if (RebuildLatestVersion)
			{
				var realService = kernel.Get<IProvideSchemaChanges>();
				kernel.Unbind<IProvideSchemaChanges>();
				//Decorate the real change provider. 
				kernel.Bind<IProvideSchemaChanges>().To<RebuildLatestVersionDecorator>().WithConstructorArgument("changeProvider", realService);
			}

			return kernel;
		}
	}
}