using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Core.OperationLog
{
    public class OperationReportingResultObject<T> : OperationReportingResult
    {
        public T ResultObject { get; set; }

        public OperationReportingResultObject(string adapterResultXml) : base(adapterResultXml)
        {
        }
    }
}