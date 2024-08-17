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
    public class SysTime
    {
        private ushort yearField;

        private ushort monthField;

        private ushort dayOfWeekField;

        private ushort dayField;

        private ushort hourField;

        private ushort minuteField;

        private ushort secondField;

        private ushort millisecondsField;

        public ushort day
        {
            get { return this.dayField; }
            set { this.dayField = value; }
        }

        public ushort dayOfWeek
        {
            get { return this.dayOfWeekField; }
            set { this.dayOfWeekField = value; }
        }

        public ushort hour
        {
            get { return this.hourField; }
            set { this.hourField = value; }
        }

        public ushort milliseconds
        {
            get { return this.millisecondsField; }
            set { this.millisecondsField = value; }
        }

        public ushort minute
        {
            get { return this.minuteField; }
            set { this.minuteField = value; }
        }

        public ushort month
        {
            get { return this.monthField; }
            set { this.monthField = value; }
        }

        public ushort second
        {
            get { return this.secondField; }
            set { this.secondField = value; }
        }

        public ushort year
        {
            get { return this.yearField; }
            set { this.yearField = value; }
        }

        public SysTime()
        {
        }
    }
}