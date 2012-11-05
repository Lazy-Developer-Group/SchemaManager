using System;

namespace Utilities.Data
{
	public interface ICommand : IDisposable
	{
		string CommandText { get; set; }
		int CommandTimeout { get; set; }
		void ExecuteNonQuery();
		object ExecuteScalar();
	}
}