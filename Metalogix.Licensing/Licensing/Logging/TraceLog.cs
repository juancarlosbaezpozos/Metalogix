using System;
using System.Diagnostics;

namespace Metalogix.Licensing.Logging
{
    public class TraceLog : BaseLog
    {
        public TraceLog(LogTypes type) : base(type)
        {
        }

        public override void Write(string message)
        {
            this.Write(message, null);
        }

        public override void Write(string message, Exception ex)
        {
            Trace.WriteLine(string.Concat(message, (ex != null ? string.Concat(": ", ex) : "")));
        }
    }
}