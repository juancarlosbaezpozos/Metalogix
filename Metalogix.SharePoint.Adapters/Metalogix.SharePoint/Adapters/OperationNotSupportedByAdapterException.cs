using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint.Adapters
{
    public class OperationNotSupportedByAdapterException : Exception
    {
        private string m_sAdapterTypeName;

        public override string Message
        {
            get
            {
                return string.Format(Resources.NotSupportedByAdapter, Environment.NewLine, base.Message,
                    this.m_sAdapterTypeName);
            }
        }

        public OperationNotSupportedByAdapterException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }

        public OperationNotSupportedByAdapterException(string sOperation, string adapterTypeName) : base(sOperation)
        {
            this.m_sAdapterTypeName = adapterTypeName;
        }
    }
}