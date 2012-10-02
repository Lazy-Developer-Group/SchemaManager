using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Ninject;
using SchemaManager.Core;

namespace SchemaManager.Infrastructure
{
	public abstract class SchemaChangeTaskBase : Task
	{
		private DatabaseVersion _targetRevision = DatabaseVersion.Max;

		[Required]
		public string ConnectionString { get; set; }

		[Required]
		public string PathToChangeScripts { get; set; }

		public string PathToAlwaysRunScripts { get; set; }

		public bool WhatIf { get; set; }

		public int TimeoutMinutes { get; set; }

		protected SchemaChangeTaskBase()
		{
			TimeoutMinutes = 30;
		}

		public virtual string TargetRevision
		{
			get
			{
				return _targetRevision.ToString();
			}
			set
			{
				_targetRevision = DatabaseVersion.FromString(value);
			}
		}

		public override bool Execute()
		{
			using (var kernel = BuildKernel())
			{
				TransactionmanagerHelper.OverrideMaximumTimeout(TimeSpan.FromMinutes(TimeoutMinutes));
				try
				{
					RunSchemaChanges(kernel);
				}
				catch (Exception ex)
				{
					Log.LogError("An error has occurred:");
					Log.LogError(ex.ToString());
					//Log.LogErrorFromException(ex);
					return false;
				}
			}

			return true;
		}

		protected virtual StandardKernel BuildKernel()
		{
			var module = new SchemaManagerModule(this, 
				PathToChangeScripts, 
				PathToAlwaysRunScripts, 
				ConnectionString, 
				_targetRevision, 
				TimeSpan.FromMinutes(TimeoutMinutes),
				WhatIf);
			return new StandardKernel(module);
		}

		protected abstract void RunSchemaChanges(StandardKernel kernel);
	}
}