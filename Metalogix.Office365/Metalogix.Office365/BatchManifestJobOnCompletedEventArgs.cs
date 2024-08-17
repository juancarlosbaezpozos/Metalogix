using System;

namespace Metalogix.Office365
{
    public class BatchManifestJobOnCompletedEventArgs : EventArgs
    {
        private int _batchNo;

        private bool _operationSuccessful;

        private string _jobConfiguration = string.Empty;

        private Exception _workLoadException;

        public int BatchNo
        {
            get { return this._batchNo; }
        }

        public string JobConfiguration
        {
            get { return this._jobConfiguration; }
        }

        public bool OperationSuccessful
        {
            get { return this._operationSuccessful; }
        }

        public Exception WorkloadException
        {
            get { return this._workLoadException; }
        }

        public BatchManifestJobOnCompletedEventArgs(int batchNo, bool operationSuccessful, Exception workLoadException,
            string jobConfiguration)
        {
            this._batchNo = batchNo;
            this._operationSuccessful = operationSuccessful;
            this._workLoadException = workLoadException;
            this._jobConfiguration = jobConfiguration;
        }
    }
}