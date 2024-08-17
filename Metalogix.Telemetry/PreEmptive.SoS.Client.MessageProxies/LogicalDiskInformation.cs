using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class LogicalDiskInformation
    {
        private string volumeSerialNumberField;

        private string volumeNameField;

        private long sizeMbField;

        private long freeSpaceMbField;

        private string fileSystemField;

        public string FileSystem
        {
            get { return this.fileSystemField; }
            set { this.fileSystemField = value; }
        }

        public long FreeSpaceMb
        {
            get { return this.freeSpaceMbField; }
            set { this.freeSpaceMbField = value; }
        }

        public long SizeMb
        {
            get { return this.sizeMbField; }
            set { this.sizeMbField = value; }
        }

        public string VolumeName
        {
            get { return this.volumeNameField; }
            set { this.volumeNameField = value; }
        }

        public string VolumeSerialNumber
        {
            get { return this.volumeSerialNumberField; }
            set { this.volumeSerialNumberField = value; }
        }

        public LogicalDiskInformation()
        {
        }
    }
}