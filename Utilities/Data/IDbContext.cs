namespace Utilities.Data
{
	public interface IDbContext
	{
		ICommand CreateCommand();
	}
}