using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.ComponentModel;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public class SpoOperationMonitor
	{
		private readonly SpoOperation _operation;

		public SpoOperationMonitor(SpoOperation operation)
		{
			this._operation = operation;
		}

		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			this.WaitUntilOperationCompleted();
		}

		private void FireCompletion()
		{
			if (this.Completed != null)
			{
				this.Completed(this, new EventArgs());
			}
		}

		public void WaitUntilOperationCompleted()
		{
			while (!this._operation.IsComplete)
			{
				if (this._operation.PollingInterval != -1)
				{
					Thread.Sleep(this._operation.PollingInterval);
				}
				this._operation.RefreshLoad();
				this._operation.Context.ExecuteQuery();
			}
			this.FireCompletion();
		}

		public void WaitUntilOperationCompletedAsync()
		{
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(this.bw_DoWork);
			backgroundWorker.RunWorkerAsync();
		}

		public event EventHandler Completed;
	}
}