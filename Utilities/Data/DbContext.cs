using System;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.Data
{
	public class DbContext : IDbContext
	{
		void IDisposable.Dispose()
		{
			Close();
		}

		public IDbConnection Connection { get; set; }

		public DbContext(string connectionString)
		{
			Connection = new SqlConnection(connectionString);
		}

		public IDbCommand CreateCommand()
		{
			PrepareConnection();
			var command = Connection.CreateCommand();
			command.CommandTimeout = 60;
			return command;
		}

		private void PrepareConnection()
		{
			if (Connection.State != ConnectionState.Open)
			{
				Connection.Open();
			}
		}

		public void Close()
		{
			if (Connection.State == ConnectionState.Open)
			{
				Connection.Close();
			}
		}
	}
}