using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.Meetings
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/")]
    public class TimeZoneInf
    {
        private int biasField;

        private SysTime standardDateField;

        private int standardBiasField;

        private SysTime daylightDateField;

        private int daylightBiasField;

        public int bias
        {
            get { return this.biasField; }
            set { this.biasField = value; }
        }

        public int daylightBias
        {
            get { return this.daylightBiasField; }
            set { this.daylightBiasField = value; }
        }

        public SysTime daylightDate
        {
            get { return this.daylightDateField; }
            set { this.daylightDateField = value; }
        }

        public int standardBias
        {
            get { return this.standardBiasField; }
            set { this.standardBiasField = value; }
        }

        public SysTime standardDate
        {
            get { return this.standardDateField; }
            set { this.standardDateField = value; }
        }

        public TimeZoneInf()
        {
        }
    }
}