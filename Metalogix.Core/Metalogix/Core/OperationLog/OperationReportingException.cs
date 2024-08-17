using System;

namespace Metalogix.Core.OperationLog
{
    public class OperationReportingException : Exception
    {
        private string _operationResultErrors;

        public override string StackTrace
        {
            get { return string.Concat(base.StackTrace, Environment.NewLine, this._operationResultErrors); }
        }

        public OperationReportingException(string message, string operationResultErrors) : base(message)
        {
            this._operationResultErrors = operationResultErrors;
        }
    }
}