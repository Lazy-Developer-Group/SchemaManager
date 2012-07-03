using System;
using System.Data;

namespace Utilities.Data
{
	public interface IDbContext
	{
		ICommand CreateCommand();
	}

	public interface ICommand : IDisposable
	{
		string CommandText { get; set; }
		int CommandTimeout { get; set; }
		void ExecuteNonQuery();
		object ExecuteScalar();
	}
}