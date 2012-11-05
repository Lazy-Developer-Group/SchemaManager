using System;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace Utilities.Data
{
	public class DbContext : IDbContext, IDisposable
	{
		private readonly SqlConnection _connection;
		private Transaction _currentTransaction;

		public DbContext(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
		}

		public ICommand CreateCommand()
		{
			//Create the connection at the last possible second.  This 
			//will ensure it can enlist in the TransactionScope while still
			//reusing one connection across all scripts.
			if (_connection.State != ConnectionState.Open)
			{
				_connection.Open();
			}

			//If the connection is opened after a TransactionScope is created,
			//it will auto-enlist in the transaction.  But if the transaction
			//is created (or a new one is created) after the connection is open,
			//you have to manually enlist in it. 
			if (Transaction.Current != null && _currentTransaction != Transaction.Current)
			{
				_currentTransaction = Transaction.Current;
				_connection.EnlistTransaction(Transaction.Current);				
			}

			return new CommandWrapper(_connection);
		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}