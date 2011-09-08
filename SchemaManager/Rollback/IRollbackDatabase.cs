namespace SchemaManager.Rollback
{
	public interface IRollbackDatabase
	{
		void ApplyRollbacks();
	}
}