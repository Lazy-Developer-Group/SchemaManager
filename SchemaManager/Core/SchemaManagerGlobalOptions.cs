using System;

namespace SchemaManager.Core
{
	public class SchemaManagerGlobalOptions
	{
		public static SchemaManagerGlobalOptions Defaults
		{
			get
			{
				return new SchemaManagerGlobalOptions
				{
					TargetRevision = DatabaseVersion.Max,
					Timeout = TimeSpan.FromMinutes(30),
					UseIncrementalTransactions = false,
					WhatIfEnabled = false
				};
			}
		}

		public DatabaseVersion TargetRevision { get; set; }

		public TimeSpan Timeout { get; set; }

		public bool UseIncrementalTransactions { get; set; }

		public bool WhatIfEnabled { get; set; }
	}
}