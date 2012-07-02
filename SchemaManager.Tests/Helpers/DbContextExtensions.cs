using System.Data;
using Utilities.Data;

namespace SchemaManager.Tests.Helpers
{
	public static class DbContextExtensions
	{
		public static IDbCommand GetCommand(this DbContext context, string commandText)
		{
			var command = context.CreateCommand();
			command.CommandText = commandText;
			return command;
		}

		public static void ExecuteNonQuery(this DbContext context, string commandText)
		{
			var command = context.GetCommand(commandText);

			command.ExecuteNonQuery();
		}

		public static TScalar ExecuteScalar<TScalar>(this DbContext context, string query)
		{
			var command = context.GetCommand(query);
			return (TScalar)command.ExecuteScalar();
		}
	}
}