using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Collections;
using System.Diagnostics;
using System.Management;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class SystemProfileMessage : PreEmptive.SoS.Client.Messages.Message
    {
        private ProcessorInformation[] processors;

        private LogicalDiskInformation[] logicalDisks;

        private MemoryInformation memory;

        private NetworkAdapterInformation[] networkAdapters;

        private DomainInformation domain;

        private ManufacturerInformation manufacturer;

        private TimeZoneInformation timeZone;

        private PageFileInformation[] pageFiles;

        private DisplayControllerInformation displayController;

        private VideoControllerInformation videoController;

        private TerminalServicesInformation terminalServices;

        private SoundCardInformation soundCard;

        private ModemInformation modem;

        private static TraceSwitch traceSwitch;

        public DisplayControllerInformation DisplayController
        {
            get
            {
                if (this.displayController == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_DisplayControllerConfiguration")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.displayController = new DisplayControllerInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.displayController.HorizontalResolution =
                                    Convert.ToInt32(managementObject["HorizontalResolution"]);
                                this.displayController.VerticalResolution =
                                    Convert.ToInt32(managementObject["VerticalResolution"]);
                                this.displayController.Name = (string)managementObject["Name"];
                                this.displayController.RefreshRate = Convert.ToInt32(managementObject["RefreshRate"]);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.displayController;
            }
        }

        public DomainInformation Domain
        {
            get
            {
                if (this.domain == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.domain = new DomainInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.domain.Domain = (string)managementObject["Domain"];
                                this.domain.DomainRole = Convert.ToString(managementObject["DomainRole"]);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.domain;
            }
        }

        public LogicalDiskInformation[] LogicalDisks
        {
            get
            {
                if (this.logicalDisks == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            int num = 0;
                            this.logicalDisks = new LogicalDiskInformation[managementObjectCollections.Count];
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                LogicalDiskInformation logicalDiskInformation = new LogicalDiskInformation()
                                {
                                    FileSystem = (string)managementObject["FileSystem"],
                                    FreeSpaceMb = Convert.ToInt64(managementObject["FreeSpace"]) /
                                                  (long)((int)Math.Pow(1024, 2)),
                                    SizeMb = Convert.ToInt64(managementObject["Size"]) / (long)((int)Math.Pow(1024, 2)),
                                    VolumeSerialNumber = (string)managementObject["VolumeSerialNumber"],
                                    VolumeName = (string)managementObject["Name"]
                                };
                                int num1 = num;
                                num = num1 + 1;
                                this.logicalDisks[num1] = logicalDiskInformation;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.logicalDisks;
            }
        }

        public ManufacturerInformation Manufacturer
        {
            get
            {
                if (this.manufacturer == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.manufacturer = new ManufacturerInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.manufacturer.Manufacturer = (string)managementObject["Manufacturer"];
                                this.manufacturer.Model = (string)managementObject["Model"];
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.manufacturer;
            }
        }

        public MemoryInformation Memory
        {
            get
            {
                if (this.memory == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.memory = new MemoryInformation()
                            {
                                Capacity = 0
                            };
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                MemoryInformation capacity = this.memory;
                                capacity.Capacity = capacity.Capacity +
                                                    (int)(Convert.ToInt64(managementObject["Capacity"]) /
                                                          (long)((int)Math.Pow(1024, 2)));
                                this.memory.Speed = Math.Max(this.memory.Speed,
                                    Convert.ToInt32(managementObject["Speed"]));
                                foreach (ManagementObject managementObject1 in (new ManagementObjectSearcher(
                                             "SELECT * FROM Win32_ComputerSystem")).Get())
                                {
                                    this.memory.TotalPhysicalMemory =
                                        (int)(Convert.ToInt64(managementObject1["TotalPhysicalMemory"]) /
                                              (long)((int)Math.Pow(1024, 2)));
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.memory;
            }
        }

        public ModemInformation Modem
        {
            get
            {
                if (this.modem == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_POTSModem")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.modem = new ModemInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.modem.DeviceType = (string)managementObject["DeviceType"];
                                this.modem.Model = (string)managementObject["Model"];
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.modem;
            }
        }

        public NetworkAdapterInformation[] NetworkAdapters
        {
            get
            {
                if (this.networkAdapters == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            ArrayList arrayLists = new ArrayList();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                if (managementObject["IPAddress"] == null)
                                {
                                    continue;
                                }

                                NetworkAdapterInformation networkAdapterInformation = new NetworkAdapterInformation()
                                {
                                    NetworkInterfaceIPAddress = ((string[])managementObject["IPAddress"])[0],
                                    MACAddress = (string)managementObject["MACAddress"],
                                    DHCPEnabled = (bool)managementObject["DHCPEnabled"]
                                };
                                ManagementObjectSearcher managementObjectSearcher =
                                    new ManagementObjectSearcher(string.Concat(
                                        "SELECT * FROM Win32_NetworkAdapter Where MACAddress = '",
                                        networkAdapterInformation.MACAddress, "'"));
                                ManagementObjectCollection managementObjectCollections1 =
                                    managementObjectSearcher.Get();
                                if (managementObjectCollections1.Count <= 0)
                                {
                                    continue;
                                }

                                foreach (ManagementObject managementObject1 in managementObjectCollections1)
                                {
                                    networkAdapterInformation.Speed = Convert.ToInt64(managementObject1["Speed"]) /
                                                                      (long)((int)Math.Pow(1024, 2));
                                    networkAdapterInformation.MaxSpeed =
                                        Convert.ToInt64(managementObject1["MaxSpeed"]) / (long)((int)Math.Pow(1024, 2));
                                    networkAdapterInformation.NetConnectionID =
                                        (string)managementObject1["NetConnectionID"];
                                }

                                if (networkAdapterInformation.Speed == 0L)
                                {
                                    try
                                    {
                                        string[] instanceNames = (new PerformanceCounterCategory("Network Interface"))
                                            .GetInstanceNames();
                                        for (int i = 0; i < (int)instanceNames.Length; i++)
                                        {
                                            string str = instanceNames[i];
                                            if (!(str == "MS TCP Loopback interface") &&
                                                ((string)managementObject["Caption"]).EndsWith(str))
                                            {
                                                PerformanceCounter performanceCounter =
                                                    new PerformanceCounter("Network Interface", "Current Bandwidth",
                                                        str);
                                                networkAdapterInformation.Speed =
                                                    Convert.ToInt64((double)performanceCounter.NextValue() /
                                                                    Math.Pow(1024, 2));
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                    }
                                }

                                arrayLists.Add(networkAdapterInformation);
                            }

                            this.networkAdapters =
                                (NetworkAdapterInformation[])arrayLists.ToArray(typeof(NetworkAdapterInformation));
                        }
                    }
                    catch (Exception exception1)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception1);
                    }
                }

                return this.networkAdapters;
            }
        }

        public PageFileInformation[] PageFiles
        {
            get
            {
                if (this.pageFiles == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_PageFile")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            int num = 0;
                            this.pageFiles = new PageFileInformation[managementObjectCollections.Count];
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                PageFileInformation pageFileInformation = new PageFileInformation()
                                {
                                    InitialSize = Convert.ToInt32(managementObject["InitialSize"]),
                                    MaxSize = Convert.ToInt32(managementObject["MaximumSize"]),
                                    Name = (string)managementObject["Name"]
                                };
                                int num1 = num;
                                num = num1 + 1;
                                this.pageFiles[num1] = pageFileInformation;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.pageFiles;
            }
        }

        public ProcessorInformation[] Processors
        {
            get
            {
                if (this.processors == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_Processor")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            int num = 0;
                            this.processors = new ProcessorInformation[managementObjectCollections.Count];
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                ProcessorInformation processorInformation = new ProcessorInformation()
                                {
                                    Id = (string)managementObject["ProcessorId"],
                                    Manufacturer = (string)managementObject["Manufacturer"],
                                    AddressWidth = Convert.ToInt16(managementObject["AddressWidth"]),
                                    Name = (string)managementObject["Name"],
                                    MaxClockSpeedMhz = Convert.ToInt32(managementObject["MaxClockSpeed"]),
                                    CurrentClockSpeedMhz = Convert.ToInt32(managementObject["CurrentClockSpeed"])
                                };
                                int num1 = num;
                                num = num1 + 1;
                                this.processors[num1] = processorInformation;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.processors;
            }
        }

        public SoundCardInformation SoundCard
        {
            get
            {
                if (this.soundCard == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.soundCard = new SoundCardInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.soundCard.Manufacturer = (string)managementObject["Manufacturer"];
                                this.soundCard.ProductName = (string)managementObject["ProductName"];
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.soundCard;
            }
        }

        public TerminalServicesInformation TerminalServices
        {
            get
            {
                if (this.terminalServices == null && PreEmptive.SoS.Client.Messages.UserInfo.IsAdministator)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_TerminalServiceSetting")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.terminalServices = new TerminalServicesInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.terminalServices.AllowTSConnections =
                                    Convert.ToBoolean(managementObject["AllowTSConnections"]);
                                this.terminalServices.LicensingName = (string)managementObject["LicensingName"];
                                this.terminalServices.TerminalServerMode =
                                    Convert.ToString(managementObject["TerminalServerMode"]);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.terminalServices;
            }
        }

        public TimeZoneInformation TimeZone
        {
            get
            {
                if (this.timeZone == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.timeZone = new TimeZoneInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                this.timeZone.CurrentTimeZone = Convert.ToInt16(managementObject["CurrentTimeZone"]);
                                this.timeZone.DaylightSavingsTimeInEffect = (bool)managementObject["DaylightInEffect"];
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.timeZone;
            }
        }

        public VideoControllerInformation VideoController
        {
            get
            {
                if (this.videoController == null)
                {
                    try
                    {
                        ManagementObjectCollection managementObjectCollections =
                            (new ManagementObjectSearcher("SELECT * FROM Win32_VideoController")).Get();
                        if (managementObjectCollections.Count > 0)
                        {
                            this.videoController = new VideoControllerInformation();
                            foreach (ManagementObject managementObject in managementObjectCollections)
                            {
                                if (managementObject["CurrentNumberOfColors"] == null)
                                {
                                    continue;
                                }

                                this.videoController.DriverVersion = (string)managementObject["DriverVersion"];
                                this.videoController.AdapterRAM =
                                    (int)(Convert.ToInt64(managementObject["AdapterRAM"]) /
                                          (long)((int)Math.Pow(1024, 2)));
                                this.videoController.Name = (string)managementObject["Name"];
                                this.videoController.CurrentNumberOfColors =
                                    Convert.ToInt64(managementObject["CurrentNumberOfColors"]);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
                    }
                }

                return this.videoController;
            }
        }

        public SystemProfileMessage()
        {
            base.Event = new PreEmptive.SoS.Client.Messages.EventInformation()
            {
                Code = "System.Profile"
            };
            PreEmptive.SoS.Client.Messages.SystemProfileMessage.traceSwitch = new TraceSwitch("traceSwitch",
                "This is a trace switch defined in App.config file.");
        }

        protected override PreEmptive.SoS.Client.MessageProxies.Message CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage();
        }

        private static void DisplayError(Exception exception_0)
        {
            Trace.WriteLineIf(PreEmptive.SoS.Client.Messages.SystemProfileMessage.traceSwitch.TraceInfo,
                string.Format(exception_0.ToString(), new object[0]));
        }

        internal override void FillInProxy()
        {
            try
            {
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).Processors = this.Processors;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).LogicalDisks =
                    this.LogicalDisks;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).Memory = this.Memory;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).NetworkAdapters =
                    this.NetworkAdapters;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).Domain = this.Domain;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).Manufacturer =
                    this.Manufacturer;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).TimeZone = this.TimeZone;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).PageFiles = this.PageFiles;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).DisplayController =
                    this.DisplayController;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).VideoController =
                    this.VideoController;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).TerminalServices =
                    this.TerminalServices;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).SoundCard = this.SoundCard;
                ((PreEmptive.SoS.Client.MessageProxies.SystemProfileMessage)base.Proxy).Modem = this.Modem;
            }
            catch (Exception exception)
            {
                PreEmptive.SoS.Client.Messages.SystemProfileMessage.DisplayError(exception);
            }

            base.FillInProxy();
        }
    }
}