using System;
using System.Diagnostics;

namespace Metalogix.Licensing.Logging
{
    public static class Logger
    {
        private static ILogMethods _error;

        private static ILogMethods _warning;

        private static ILogMethods _debug;

        private static bool _initialized;

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
            Logger._error = new TraceLog(LogTypes.Error);
            Logger._warning = new TraceLog(LogTypes.Warning);
            Logger._debug = new TraceLog(LogTypes.Debug);
        }

        private static void Init()
        {
            if (Logger._initialized)
            {
                return;
            }

            lock (typeof(Logger))
            {
                if (!Logger._initialized)
                {
                    Logger._error = new TraceLog(LogTypes.Error);
                    Logger._warning = new TraceLog(LogTypes.Warning);
                    Logger._debug = new TraceLog(LogTypes.Debug);
                    Logger._initialized = true;
                }
            }
        }

        public static void SetStrategy(ILogMethods error, ILogMethods warning, ILogMethods debug)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            if (warning == null)
            {
                throw new ArgumentNullException("warning");
            }

            if (debug == null)
            {
                throw new ArgumentNullException("debug");
            }

            lock (typeof(Logger))
            {
                Logger._error = error;
                Logger._warning = warning;
                Logger._debug = debug;
                Logger._initialized = true;
            }

            Trace.WriteLine(string.Format(
                "Logger >> SetStrategy: new strategies were set Debug={0}, Warning={1}; Error={2}", debug.GetType(),
                warning.GetType(), error.GetType()));
        }
    }
}