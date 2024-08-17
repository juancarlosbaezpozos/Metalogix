using Metalogix.Actions;
using Metalogix.Azure;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Office365
{
    public class BatchManifestJob : IOperationLogging
    {
        private readonly AzureMigrationHelper _azureMigrationHelper;

        private readonly Queue<BaseManifestItem> _manifestItems;

        private IOperationLoggingManagement _operationLoggingManagement;

        private Thread _processThread;

        public int BatchNo { get; set; }

        public Queue<BaseManifestItem> ManifestItems
        {
            get { return this._manifestItems; }
        }

        public string WebId { get; set; }

        public BatchManifestJob(IOperationLoggingManagement operationLoggingManagement, int batchNo,
            AzureMigrationHelper azureMigrationHelper, Guid webId)
        {
            this.BatchNo = batchNo;
            this.WebId = webId.ToString("D");
            this._manifestItems = new Queue<BaseManifestItem>();
            this._azureMigrationHelper = azureMigrationHelper;
            this._operationLoggingManagement = operationLoggingManagement;
        }

        private void FireOperationFinished(LogItem operation)
        {
            if (this.OperationFinished != null)
            {
                this.OperationFinished(operation);
            }
        }

        private void FireOperationStarted(LogItem operation)
        {
            if (this.OperationStarted != null)
            {
                this.OperationStarted(operation);
            }
        }

        private void FireOperationUpdated(LogItem operation)
        {
            if (this.OperationUpdated != null)
            {
                this.OperationUpdated(operation);
            }
        }

        private void ProcessManifestItems()
        {
            this._operationLoggingManagement.ConnectOperationLogging(this);
            Exception exception = null;
            string empty = string.Empty;
            bool success = false;
            LogItem logItem = new LogItem("Uploading to Azure", string.Format("Batch {0}", this.BatchNo), "", "",
                ActionOperationStatus.Running);
            this.FireOperationStarted(logItem);
            try
            {
                try
                {
                    this._azureMigrationHelper.ProcessQueueIntoManifest(this._manifestItems);
                    Response response = this._azureMigrationHelper.Upload();
                    empty = this._azureMigrationHelper.GetRequestMigrationJobConfiguration(this.WebId);
                    logItem.Information = "Please see details...";
                    logItem.Details = response.Details;
                    logItem.Status =
                        (response.Success ? ActionOperationStatus.Completed : ActionOperationStatus.Failed);
                    success = response.Success;
                }
                catch (Exception exception2)
                {
                    Exception exception1 = exception2;
                    logItem.Exception = exception1;
                    exception = exception1;
                }
            }
            finally
            {
                this.FireOperationFinished(logItem);
                if (this.OnCompleted != null)
                {
                    this.OnCompleted(this,
                        new BatchManifestJobOnCompletedEventArgs(this.BatchNo, success, exception, empty));
                }

                this._operationLoggingManagement.DisconnectOperationLogging(this);
            }
        }

        public void Start()
        {
            if (this._azureMigrationHelper.IsOperationCancelled)
            {
                return;
            }

            this._processThread = new Thread(new ThreadStart(this.ProcessManifestItems));
            this._processThread.Start();
        }

        public event EventHandler<BatchManifestJobOnCompletedEventArgs> OnCompleted;

        public event ActionEventHandler OperationFinished;

        public event ActionEventHandler OperationStarted;

        public event ActionEventHandler OperationUpdated;
    }
}