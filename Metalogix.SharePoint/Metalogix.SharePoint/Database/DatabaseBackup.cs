using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Database
{
	public class DatabaseBackup
	{
		public string Name;

		public string Description;

		public string SourceFile;

		public string SourceDatabaseName;

		public string SourceServerName;

		public DateTime StartDate;

		public DateTime FinishDate;

		public int Position;

		public DatabaseBackup.BackupType Type;

		public DatabaseBackup()
		{
		}

		public static int CompareChronologically(DatabaseBackup x, DatabaseBackup y)
		{
			return DateTime.Compare(x.StartDate, y.StartDate);
		}

		public bool IsAfter(DatabaseBackup other)
		{
			return DatabaseBackup.CompareChronologically(this, other) > 0;
		}

		public bool IsBefore(DatabaseBackup other)
		{
			return DatabaseBackup.CompareChronologically(this, other) < 0;
		}

		public static void SortBackups(List<DatabaseBackup> backups, DatabaseBackup.BackupSortingOrder sortOrder)
		{
			backups.Sort(new DatabaseBackup.BackupComparer());
			if (sortOrder == DatabaseBackup.BackupSortingOrder.MostRecentToEarliest)
			{
				backups.Reverse();
			}
		}

		public class BackupComparer : IComparer<DatabaseBackup>
		{
			public BackupComparer()
			{
			}

			public int Compare(DatabaseBackup x, DatabaseBackup y)
			{
				return DatabaseBackup.CompareChronologically(x, y);
			}
		}

		public enum BackupSortingOrder
		{
			EarliestToMostRecent,
			MostRecentToEarliest
		}

		public enum BackupType
		{
			Full,
			Differential,
			TransactionLog,
			Unknown
		}
	}
}