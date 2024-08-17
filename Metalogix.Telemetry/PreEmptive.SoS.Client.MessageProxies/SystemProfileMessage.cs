using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.MessageProxies
{
    [DebuggerStepThrough]
    [Serializable]
    [XmlType(Namespace = "http://schemas.preemptive.com/services/messaging")]
    public class SystemProfileMessage : Message
    {
        private ProcessorInformation[] processorsField;

        private LogicalDiskInformation[] logicalDisksField;

        private MemoryInformation memoryField;

        private NetworkAdapterInformation[] networkAdaptersField;

        private DomainInformation domainField;

        private ManufacturerInformation manufacturerField;

        private TimeZoneInformation timeZoneField;

        private PageFileInformation[] pageFilesField;

        private DisplayControllerInformation displayControllerField;

        private VideoControllerInformation videoControllerField;

        private TerminalServicesInformation terminalServicesField;

        private SoundCardInformation soundCardField;

        private ModemInformation modemField;

        private string powerStateField;

        public DisplayControllerInformation DisplayController
        {
            get { return this.displayControllerField; }
            set { this.displayControllerField = value; }
        }

        public DomainInformation Domain
        {
            get { return this.domainField; }
            set { this.domainField = value; }
        }

        public LogicalDiskInformation[] LogicalDisks
        {
            get { return this.logicalDisksField; }
            set { this.logicalDisksField = value; }
        }

        public ManufacturerInformation Manufacturer
        {
            get { return this.manufacturerField; }
            set { this.manufacturerField = value; }
        }

        public MemoryInformation Memory
        {
            get { return this.memoryField; }
            set { this.memoryField = value; }
        }

        public ModemInformation Modem
        {
            get { return this.modemField; }
            set { this.modemField = value; }
        }

        public NetworkAdapterInformation[] NetworkAdapters
        {
            get { return this.networkAdaptersField; }
            set { this.networkAdaptersField = value; }
        }

        public PageFileInformation[] PageFiles
        {
            get { return this.pageFilesField; }
            set { this.pageFilesField = value; }
        }

        public string PowerState
        {
            get { return this.powerStateField; }
            set { this.powerStateField = value; }
        }

        public ProcessorInformation[] Processors
        {
            get { return this.processorsField; }
            set { this.processorsField = value; }
        }

        public SoundCardInformation SoundCard
        {
            get { return this.soundCardField; }
            set { this.soundCardField = value; }
        }

        public TerminalServicesInformation TerminalServices
        {
            get { return this.terminalServicesField; }
            set { this.terminalServicesField = value; }
        }

        public TimeZoneInformation TimeZone
        {
            get { return this.timeZoneField; }
            set { this.timeZoneField = value; }
        }

        public VideoControllerInformation VideoController
        {
            get { return this.videoControllerField; }
            set { this.videoControllerField = value; }
        }

        public SystemProfileMessage()
        {
        }
    }
}