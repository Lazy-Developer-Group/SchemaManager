namespace Utilities.Data
{
	public class DbContext : IDbContext
	{
		private readonly string _connectionString;

		public DbContext(string connectionString)
		{
			_connectionString = connectionString;
		}

		public ICommand CreateCommand()
		{
			return new CommandWrapper(_connectionString);
		}
	}
}