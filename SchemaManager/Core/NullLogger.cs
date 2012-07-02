namespace SchemaManager.Core
{
	public class NullLogger : ILogger
	{
		public void Info(string message, params object[] messageArgs)
		{
		}
	}
}