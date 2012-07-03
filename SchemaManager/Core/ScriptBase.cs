using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.Data;

namespace SchemaManager.Core
{
	public abstract class ScriptBase
	{
		private static readonly Regex BatchSplitter = new Regex(@"^GO\s*$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private IEnumerable<string> GetBatchesFrom(string sql)
		{
			return from batch in BatchSplitter.Split(sql)
			       let trimmedBatch = batch.Trim()
			       where !string.IsNullOrEmpty(trimmedBatch)
			       select trimmedBatch;
		}

		protected void RunAllBatchesFromText(IDbContext context, string script)
		{
			foreach (var sqlBatch in GetBatchesFrom(script))
			{
				using (var command = context.CreateCommand())
				{
					//TODO: Is this safe?  30 minutes is a heck of a timeout...
					command.CommandTimeout = (int) TimeSpan.FromMinutes(30).TotalSeconds;

					command.CommandText = sqlBatch;

					command.ExecuteNonQuery();
				}
			}
		}
	}
}