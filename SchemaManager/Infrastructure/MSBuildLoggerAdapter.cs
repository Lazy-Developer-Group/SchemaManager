using Microsoft.Build.Utilities;
using SchemaManager.Core;

namespace SchemaManager.Infrastructure
{
	public class MSBuildLoggerAdapter : ILogger
	{
		private readonly Task _task;

		public MSBuildLoggerAdapter(Task task)
		{
			_task = task;
		}

		public void Info(string message, params object[] messageArgs)
		{
			_task.Log.LogMessage(message, messageArgs);
		}
	}
}