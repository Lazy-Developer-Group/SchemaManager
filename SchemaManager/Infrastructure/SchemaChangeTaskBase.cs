using System;
using System.Reflection;
using System.Transactions;
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
				TransactionmanagerHelper.OverrideMaximumTimeout(TimeSpan.FromMinutes(30));
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
			return new StandardKernel(new SchemaManagerModule(this, PathToChangeScripts, PathToAlwaysRunScripts, ConnectionString, _targetRevision, WhatIf));
		}

		protected abstract void RunSchemaChanges(StandardKernel kernel);
	}

	public static class TransactionmanagerHelper
	{
		public static void OverrideMaximumTimeout(TimeSpan timeout)
		{
			//TransactionScope inherits a *maximum* timeout from Machine.config.  There's no way to override it from
			//code unless you use reflection.  Hence this code!
			//TransactionManager._cachedMaxTimeout
			var type = typeof(TransactionManager);
			var cachedMaxTimeout = type.GetField("_cachedMaxTimeout", BindingFlags.NonPublic | BindingFlags.Static);
			cachedMaxTimeout.SetValue(null, true);

			//TransactionManager._maximumTimeout
			var maximumTimeout = type.GetField("_maximumTimeout", BindingFlags.NonPublic | BindingFlags.Static);
			maximumTimeout.SetValue(null, TimeSpan.FromMinutes(30));
		}
	}
}