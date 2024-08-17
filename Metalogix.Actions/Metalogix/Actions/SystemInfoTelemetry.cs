using Metalogix.Telemetry.Accumulators;
using System;
using System.Diagnostics;
using System.Text;

namespace Metalogix.Actions
{
    public static class SystemInfoTelemetry
    {
        private static string FormatSystemInfo(SystemInfo sysInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("OSVersion: {0}", sysInfo.OSVersion).Append(",");
            stringBuilder.AppendFormat("ProcessorName: {0}", sysInfo.ProcessorName).Append(",");
            stringBuilder.AppendFormat("ProcessorSpeed: {0}", sysInfo.ProcessorSpeed).Append(",");
            stringBuilder.AppendFormat("NumberOfCores: {0}", sysInfo.NumberOfCores).Append(",");
            stringBuilder.AppendFormat("NumberOfLogicalProcessors: {0}", sysInfo.NumberOfLogicalProcessors).Append(",");
            stringBuilder.AppendFormat("MemoryCapacity: {0}", sysInfo.MemoryCapacity).Append(",");
            stringBuilder.AppendFormat("MemorySpeed: {0}", sysInfo.MemorySpeed).Append(",");
            stringBuilder.AppendFormat("MemoryType: {0}", sysInfo.MemoryType);
            return stringBuilder.ToString();
        }

        public static void SendTelemetry()
        {
            try
            {
                SystemInfo systemInfo = new SystemInfo();
                systemInfo.AutoDetect();
                StringAccumulator.Message.Send("System Information", SystemInfoTelemetry.FormatSystemInfo(systemInfo),
                    false, null);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("Failed to send system info telemetry report data. Error : {0}",
                    exception));
            }
        }
    }
}