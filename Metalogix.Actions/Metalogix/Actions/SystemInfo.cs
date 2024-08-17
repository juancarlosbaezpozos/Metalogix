using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    internal class SystemInfo
    {
        public long MemoryCapacity { get; private set; }

        public int MemorySpeed { get; private set; }

        public string MemoryType { get; private set; }

        public int NumberOfCores { get; private set; }

        public int NumberOfLogicalProcessors { get; private set; }

        public string OSVersion { get; private set; }

        public string ProcessorName { get; private set; }

        public int ProcessorSpeed { get; private set; }

        public SystemInfo()
        {
        }

        public void AutoDetect()
        {
            this.SetEnvironment();
            this.SetProcessor();
            this.SetMemory();
        }

        private void SetEnvironment()
        {
            this.OSVersion = Environment.OSVersion.ToString();
        }

        private void SetMemory()
        {
            using (ManagementClass managementClass = new ManagementClass("Win32_PhysicalMemory"))
            {
                foreach (ManagementBaseObject instance in managementClass.GetInstances())
                {
                    using (instance)
                    {
                        Dictionary<string, string> dictionary = instance.Properties.Cast<PropertyData>()
                            .ToDictionary<PropertyData, string, string>((PropertyData data) => data.Name,
                                (PropertyData data) =>
                                {
                                    if (data.Value == null)
                                    {
                                        return string.Empty;
                                    }

                                    return data.Value.ToString();
                                });
                        if (string.IsNullOrEmpty(this.MemoryType))
                        {
                            this.MemoryType = (dictionary.ContainsKey("MemoryType")
                                ? dictionary["MemoryType"]
                                : "Unknown");
                        }

                        if (dictionary.ContainsKey("Speed") && !string.IsNullOrEmpty(dictionary["Speed"]))
                        {
                            int num = int.Parse(dictionary["Speed"]);
                            if (this.MemorySpeed == 0)
                            {
                                this.MemorySpeed = num;
                            }
                            else if (this.MemorySpeed > num)
                            {
                                this.MemorySpeed = num;
                            }
                        }

                        this.MemoryCapacity =
                            (!dictionary.ContainsKey("Capacity") || string.IsNullOrEmpty(dictionary["Capacity"])
                                ? 0L
                                : long.Parse(dictionary["Capacity"]));
                    }
                }
            }
        }

        private void SetProcessor()
        {
            using (ManagementClass managementClass = new ManagementClass("win32_processor"))
            {
                foreach (ManagementBaseObject instance in managementClass.GetInstances())
                {
                    using (instance)
                    {
                        Dictionary<string, string> dictionary = instance.Properties.Cast<PropertyData>()
                            .ToDictionary<PropertyData, string, string>((PropertyData data) => data.Name,
                                (PropertyData data) =>
                                {
                                    if (data.Value == null)
                                    {
                                        return string.Empty;
                                    }

                                    return data.Value.ToString();
                                });
                        if (string.IsNullOrEmpty(this.ProcessorName))
                        {
                            this.ProcessorName = (dictionary.ContainsKey("Name") ? dictionary["Name"] : "Unknown");
                        }

                        this.NumberOfCores = (dictionary.ContainsKey("NumberOfCores")
                            ? int.Parse(dictionary["NumberOfCores"])
                            : 0);
                        this.NumberOfLogicalProcessors = (dictionary.ContainsKey("NumberOfLogicalProcessors")
                            ? int.Parse(dictionary["NumberOfLogicalProcessors"])
                            : 0);
                        if (dictionary.ContainsKey("MaxClockSpeed") &&
                            !string.IsNullOrEmpty(dictionary["MaxClockSpeed"]))
                        {
                            int num = int.Parse(dictionary["MaxClockSpeed"]);
                            if (this.ProcessorSpeed == 0)
                            {
                                this.ProcessorSpeed = num;
                            }
                            else if (this.ProcessorSpeed > num)
                            {
                                this.ProcessorSpeed = num;
                            }
                        }
                    }
                }
            }
        }
    }
}