using System;
using System.Data.SqlClient;

namespace Utilities.Data
{
	public class DbContext : IDbContext, IDisposable
	{
		private readonly SqlConnection _connection;

		public DbContext(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
			_connection.Open();
		}

		public ICommand CreateCommand()
		{
			return new CommandWrapper(_connection);
		}

		public void Dispose()
		{
			_connection.Close();
		}
	}
}