using System.Data;
using System.Transactions;
using Ninject;
using Utilities.Data;

namespace Utilities.Testing
{
	public abstract partial class SpecsFor<T> where T : class
	{
		public abstract class WithDatabaseSupport : SpecsFor<T>
		{
			protected DbContext Context;
			protected TransactionScope Transaction;

			protected abstract string GetConnectionString();

			protected override void ConfigureKernel(IKernel kernel)
			{
				base.ConfigureKernel(kernel);

				Transaction = new TransactionScope();

				Context = new DbContext(GetConnectionString());

				kernel.Bind<IDbContext>().ToConstant(Context);
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
}