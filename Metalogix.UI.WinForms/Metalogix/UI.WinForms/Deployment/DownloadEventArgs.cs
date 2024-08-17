using System;

namespace Metalogix.UI.WinForms.Deployment
{
	public class DownloadEventArgs : EventArgs
	{
		private int percentDone;

		private string downloadState;

		private long totalFileSize;

		private long currentFileSize;

		public long CurrentFileSize
		{
			get
			{
				return this.currentFileSize;
			}
			set
			{
				this.currentFileSize = value;
			}
		}

		public string DownloadState
		{
			get
			{
				return this.downloadState;
			}
		}

		public int PercentDone
		{
			get
			{
				return this.percentDone;
			}
		}

		public long TotalFileSize
		{
			get
			{
				return this.totalFileSize;
			}
			set
			{
				this.totalFileSize = value;
			}
		}

		public DownloadEventArgs(long totalFileSize, long currentFileSize)
		{
			this.totalFileSize = totalFileSize;
			this.currentFileSize = currentFileSize;
			this.percentDone = (int)((double)currentFileSize / (double)totalFileSize * 100);
		}

		public DownloadEventArgs(string state)
		{
			this.downloadState = state;
		}

		public DownloadEventArgs(int percentDone, string state)
		{
			this.percentDone = percentDone;
			this.downloadState = state;
		}
	}
}