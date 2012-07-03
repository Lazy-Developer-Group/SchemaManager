using System;
using System.Data;
using System.Data.SqlClient;
using Utilities.Data;

namespace SchemaManager.Tests.Helpers
{
	public class TestDbContext : IDbContext, IDisposable
	{
		private readonly SqlConnection _connection;

		public TestDbContext(string connectionString)
		{
			_connection = new SqlConnection(connectionString);
		}

		public ICommand CreateCommand()
		{
			if (_connection.State != ConnectionState.Open)
			{
				_connection.Open();
			}
			
			return new CommandWrapper(_connection);
		}

		public void Dispose()
		{
			_connection.Dispose();
		}
	}
}