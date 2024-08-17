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
    public class Message
    {
        private string shortMessageField;

        private string detailedMessageField;

        private Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.SourceLocation sourceLocationField;

        private int idField;

        private MessageType typeField;

        private Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.Feature featureField;

        private Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.Category categoryField;

        [XmlAttribute]
        public Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.Category Category
        {
            get { return this.categoryField; }
            set { this.categoryField = value; }
        }

        public string DetailedMessage
        {
            get { return this.detailedMessageField; }
            set { this.detailedMessageField = value; }
        }

        [XmlAttribute]
        public Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.Feature Feature
        {
            get { return this.featureField; }
            set { this.featureField = value; }
        }

        [XmlAttribute]
        public int Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        public string ShortMessage
        {
            get { return this.shortMessageField; }
            set { this.shortMessageField = value; }
        }

        public Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices.SourceLocation SourceLocation
        {
            get { return this.sourceLocationField; }
            set { this.sourceLocationField = value; }
        }

        [XmlAttribute]
        public MessageType Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        public Message()
        {
        }
    }
}