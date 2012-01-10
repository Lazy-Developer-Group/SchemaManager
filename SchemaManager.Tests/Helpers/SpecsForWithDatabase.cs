using System.Data;
using System.Transactions;
using Utilities.Data;
using SpecsFor;

namespace SchemaManager.Tests.Helpers
{
	public abstract class SpecsForWithDatabase<T> : SpecsFor<T> where T : class
	{
		protected DbContext Context;
		protected TransactionScope Transaction;

		protected abstract string GetConnectionString();

		protected override void ConfigureContainer(StructureMap.IContainer container)
		{
			base.ConfigureContainer(container);

			Transaction = new TransactionScope();

			Context = new DbContext(GetConnectionString());

			container.Configure(cfg => cfg.For<IDbContext>().Use(Context));
		}

		protected override void AfterEachSpec()
		{
			base.AfterEachSpec();

			//Since the transaction hasn't been completed, it will be rolled back when it is disposed. 
			Transaction.Dispose();

			Context.Close();
		}

		protected IDbCommand GetCommand(string commandText)
		{
			var command = Context.CreateCommand();
			command.CommandText = commandText;
			return command;
		}

		protected void ExecuteNonQuery(string commandText)
		{
			var command = GetCommand(commandText);

			command.ExecuteNonQuery();
		}

		protected TScalar ExecuteScalar<TScalar>(string query)
		{
			var command = GetCommand(query);
			return (TScalar)command.ExecuteScalar();
		}
	}
}