using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices")]
    public class MessagesResponse
    {
        private Message[] messagesField;

        public Message[] Messages
        {
            get { return this.messagesField; }
            set { this.messagesField = value; }
        }

        public MessagesResponse()
        {
        }
    }
}