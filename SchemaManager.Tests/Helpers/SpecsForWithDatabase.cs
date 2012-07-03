using System.Transactions;
using Utilities.Data;
using SpecsFor;

namespace SchemaManager.Tests.Helpers
{
	public abstract class SpecsForWithDatabase<T> : SpecsFor<T> where T : class
	{
		protected IDbContext Context;
		protected TransactionScope Transaction;

		protected abstract string GetConnectionString();

		protected override void ConfigureContainer(StructureMap.IContainer container)
		{
			base.ConfigureContainer(container);

			Transaction = new TransactionScope();

			Context = new TestDbContext(GetConnectionString());

			container.Configure(cfg => cfg.For<IDbContext>().Use(Context));
		}

		public override void TearDown()
		{
			try
			{
				base.TearDown();

			}
			finally
			{
				Transaction.Dispose();
			}
		}
	}
}