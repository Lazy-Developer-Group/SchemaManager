using System.Data.SqlClient;

namespace Utilities.Data
{
	public class CommandWrapper : ICommand
	{
		private readonly SqlConnection _connection;
		private readonly SqlCommand _command;
		private readonly bool _disposeConnection;

		public CommandWrapper(string connectionString) : this(new SqlConnection(connectionString))
		{
			_connection.Open();
			_disposeConnection = true;
		}

		public CommandWrapper(SqlConnection connection)
		{
			_connection = connection;
			_command = _connection.CreateCommand();
			_command.CommandTimeout = 60;
		}

		public void Dispose()
		{
			_command.Dispose();

			if (_disposeConnection)
			{
				_connection.Dispose();
			}
		}

		public string CommandText
		{
			get { return _command.CommandText; }
			set { _command.CommandText = value;  }
		}

		public int CommandTimeout
		{
			get { return _command.CommandTimeout; }
			set { _command.CommandTimeout = value; }
		}

		public void ExecuteNonQuery()
		{
			_command.ExecuteNonQuery();
		}

		public object ExecuteScalar()
		{
			return _command.ExecuteScalar();
		}
	}
}