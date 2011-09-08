using Microsoft.Build.Framework;
using Ninject;
using SchemaManager.Infrastructure;

namespace SchemaManager.Rollback
{
	public class RollbackDatabase : SchemaChangeTaskBase
	{
		[Required]
		public override string TargetRevision
		{
			get
			{
				return base.TargetRevision;
			}
			set
			{
				base.TargetRevision = value;
			}
		}

		protected override void RunSchemaChanges(StandardKernel kernel)
		{
			var reverter = kernel.Get<IRollbackDatabase>();
			reverter.ApplyRollbacks();
		}
	}
}