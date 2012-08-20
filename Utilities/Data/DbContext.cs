using System;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.Data
{
	public class DbContext : IDbContext, IDisposable
	{
		private readonly SqlConnection _connection;

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

			return new CommandWrapper(_connection);
		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}