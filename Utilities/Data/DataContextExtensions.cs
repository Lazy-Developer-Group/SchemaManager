using System;
using System.Data.Linq;
using Utilities.Data;

namespace Utilities.Data
{
	public static class DataContextExtensions
	{
		//TODO:
		// Look at a way to combine these extensions into a fluent API. Something like:
		//		dataContext.RunCommand(c => c.Whatever())
		//					.WithTimeout(60)
		//					.WithReadUncommitted()? 
		public static void RunCommandWithTimeout<TContext>(this TContext context, Action<TContext> command, TimeSpan timeout) where TContext : DataContext
		{
			var oldTimeout = context.CommandTimeout;
			context.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;

			try
			{
				command(context);
			}
			finally
			{
				context.CommandTimeout = oldTimeout;
			}
		}

		public static void RunCommandWithReadUncommitted<TContext>(this TContext context, Action<TContext> command) where TContext : DataContext
		{
			//Wraps Action<TContext> in an a Func<TContext,TResult> so that we can call the real extension method. 
			context.RunCommandWithReadUncommitted<TContext,object>(c =>
			                                      	{
			                                      		command(c);
			                                      		return null;
			                                      	});
		}

		public static void SetReadUncommitted<TContext>(this TContext context) where TContext : DataContext
		{
			context.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
		}

		public static void SetReadCommitted<TContext>(this TContext context) where TContext : DataContext
		{
			context.ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
		}

		public static TResult RunCommandWithReadUncommitted<TContext, TResult>(this TContext context, Func<TContext, TResult> command) where TContext : DataContext
		{
			context.SetReadUncommitted();

			TResult results;

			try
			{
				results = command(context);
			}
			finally
			{
				context.SetReadCommitted();
			}

			return results;
		}
	}
}