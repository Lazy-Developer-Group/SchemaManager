using System;
using System.Data;

namespace Utilities.Data
{
	public interface IDbContext : IDisposable
	{
		IDbCommand CreateCommand();
	}
}