namespace SchemaManager.Core
{
	public interface ILogger
	{
		void Info(string message, params object[] messageArgs);
	}
}