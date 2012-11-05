using Utilities.Data;

namespace SchemaManager.Tests.Helpers
{
	public static class DbContextExtensions
	{
		public static ICommand GetCommand(this IDbContext context, string commandText)
		{
			var command = context.CreateCommand();
			command.CommandText = commandText;
			return command;
		}

		public static void ExecuteNonQuery(this IDbContext context, string commandText)
		{
			using (var command = context.GetCommand(commandText))
			{
				command.ExecuteNonQuery();	
			}
		}

		public static TScalar ExecuteScalar<TScalar>(this IDbContext context, string query)
		{
			using (var command = context.GetCommand(query))
			{
				return (TScalar)command.ExecuteScalar();	
			}
		}
	}
}