using System;
using System.Text;

namespace Metalogix.Core.OperationLog
{
    public class ReportingElement
    {
        private readonly OperationReportingElements _entryType;

        private readonly string _message;

        private readonly string _detail;

        private readonly string _stack;

        private readonly int _errorCode;

        private readonly int _hResult;

        public string Detail
        {
            get { return this._detail; }
        }

        public OperationReportingElements EntryType
        {
            get { return this._entryType; }
        }

        public int ErrorCode
        {
            get { return this._errorCode; }
        }

        public int HResult
        {
            get { return this._hResult; }
        }

        public string Message
        {
            get { return this._message; }
        }

        public string Stack
        {
            get { return this._stack; }
        }

        internal ReportingElement(OperationReportingElements entryType, string message, string detail,
            string stack = null, int errorCode = 0, int hResult = 0)
        {
            this._entryType = entryType;
            this._message = message ?? string.Empty;
            this._detail = detail ?? string.Empty;
            this._stack = stack ?? string.Empty;
            this._errorCode = errorCode;
            this._hResult = hResult;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            stringBuilder.AppendLine(this.Message.Trim());
            if (!string.IsNullOrEmpty(this.Detail))
            {
                stringBuilder.AppendLine(this.Detail.Trim());
            }

            if (!string.IsNullOrEmpty(this.Stack))
            {
                stringBuilder.AppendLine(this.Stack.Trim());
            }

            return stringBuilder.ToString().Trim();
        }
    }
}