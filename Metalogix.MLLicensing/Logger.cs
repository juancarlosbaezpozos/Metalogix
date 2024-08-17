using System;

namespace Metalogix
{
    public static class Logger
    {
        private static ILogMethods _error;

        private static ILogMethods _warning;

        private static ILogMethods _debug;

        public static ILogMethods Debug
        {
            get
            {
                Logger.Init();
                return Logger._debug;
            }
        }

        public static ILogMethods Error
        {
            get
            {
                Logger.Init();
                return Logger._error;
            }
        }

        public static ILogMethods Warning
        {
            get
            {
                Logger.Init();
                return Logger._warning;
            }
        }

        static Logger()
        {
            Logger._error = new TraceLog(BaseLog.LogTypes.Error);
            Logger._warning = new TraceLog(BaseLog.LogTypes.Warning);
            Logger._debug = new TraceLog(BaseLog.LogTypes.Debug);
        }

        public static void Init()
        {
            if (Logger._error != null)
            {
                return;
            }

            Logger._error = new TraceLog(BaseLog.LogTypes.Error);
            Logger._warning = new TraceLog(BaseLog.LogTypes.Warning);
            Logger._debug = new TraceLog(BaseLog.LogTypes.Debug);
        }
    }
}