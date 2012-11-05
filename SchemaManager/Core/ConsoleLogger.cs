using System;

namespace SchemaManager.Core
{
	public class ConsoleLogger : ILogger
	{
		public void Info(string message, params object[] messageArgs)
		{
			Console.WriteLine(message, messageArgs);
		}
	}
}