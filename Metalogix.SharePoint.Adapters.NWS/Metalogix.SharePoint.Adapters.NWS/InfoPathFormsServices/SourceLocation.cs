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
    public class SourceLocation
    {
        private string controlIdField;

        private string fileNameField;

        private int lineNumberField;

        private bool lineNumberFieldSpecified;

        private int linePositionField;

        private bool linePositionFieldSpecified;

        [XmlAttribute]
        public string ControlId
        {
            get { return this.controlIdField; }
            set { this.controlIdField = value; }
        }

        [XmlAttribute]
        public string FileName
        {
            get { return this.fileNameField; }
            set { this.fileNameField = value; }
        }

        [XmlAttribute]
        public int LineNumber
        {
            get { return this.lineNumberField; }
            set { this.lineNumberField = value; }
        }

        [XmlIgnore]
        public bool LineNumberSpecified
        {
            get { return this.lineNumberFieldSpecified; }
            set { this.lineNumberFieldSpecified = value; }
        }

        [XmlAttribute]
        public int LinePosition
        {
            get { return this.linePositionField; }
            set { this.linePositionField = value; }
        }

        [XmlIgnore]
        public bool LinePositionSpecified
        {
            get { return this.linePositionFieldSpecified; }
            set { this.linePositionFieldSpecified = value; }
        }

        public SourceLocation()
        {
        }
    }
}