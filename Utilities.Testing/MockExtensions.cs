using System.Collections.Generic;
using Moq.Language;
using Moq.Language.Flow;

namespace Utilities.Testing
{
	public static class MockExtensions
	{
		public static IReturnsResult<TMock> ReturnsSequence<TMock, TResult>(this IReturns<TMock, TResult> returns, params TResult[] results) where TMock : class
		{
			var resultsQueue = new Queue<TResult>(results);

			return returns.Returns(() => resultsQueue.Dequeue());
		}
	}
}