using System;

namespace Metalogix.Licensing.Logging
{
    public class EmptyLog : BaseLog
    {
        public EmptyLog() : base(LogTypes.Debug)
        {
        }

        public override void Write(string message)
        {
        }

        public override void Write(string message, Exception ex)
        {
        }
    }
}