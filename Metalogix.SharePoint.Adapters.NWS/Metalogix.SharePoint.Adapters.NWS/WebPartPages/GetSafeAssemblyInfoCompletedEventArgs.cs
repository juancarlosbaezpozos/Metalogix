using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WebPartPages
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    public class GetSafeAssemblyInfoCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public XmlNode Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return (XmlNode)this.results[0];
            }
        }

        internal GetSafeAssemblyInfoCompletedEventArgs(object[] results, Exception exception, bool cancelled,
            object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}