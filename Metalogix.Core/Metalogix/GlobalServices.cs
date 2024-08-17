using Metalogix.Interfaces;
using System;

namespace Metalogix
{
    public static class GlobalServices
    {
        private static object _ERROR_FORMATTER_LOCK;

        private static volatile IErrorFormatter _ERROR_FORMATTER;

        private static object _ERROR_HANDLER_LOCK;

        private static volatile IErrorHandler _ERROR_HANDLER;

        public static IErrorFormatter ErrorFormatter
        {
            get
            {
                if (GlobalServices._ERROR_FORMATTER != null)
                {
                    return GlobalServices._ERROR_FORMATTER;
                }

                lock (GlobalServices._ERROR_FORMATTER_LOCK)
                {
                    if (GlobalServices._ERROR_FORMATTER == null)
                    {
                        GlobalServices._ERROR_FORMATTER = new Metalogix.ErrorFormatter();
                    }
                }

                return GlobalServices._ERROR_FORMATTER;
            }
            set { GlobalServices._ERROR_FORMATTER = value; }
        }

        public static IErrorHandler ErrorHandler
        {
            get
            {
                IErrorHandler _ERRORHANDLER;
                IErrorHandler errorHandler;
                if (GlobalServices._ERROR_HANDLER != null)
                {
                    return GlobalServices._ERROR_HANDLER;
                }

                lock (GlobalServices._ERROR_HANDLER_LOCK)
                {
                    if (ApplicationData.MainAssembly != null)
                    {
                        if (GlobalServices._ERROR_HANDLER == null)
                        {
                            Type[] typesByInterface =
                                Catalogs.GetTypesByInterface(typeof(IErrorHandler), AssemblyTiers.Referenced);
                            if ((int)typesByInterface.Length > 0)
                            {
                                Type type = typesByInterface[0];
                                if (type.GetConstructor(new Type[0]) != null)
                                {
                                    errorHandler = (IErrorHandler)Activator.CreateInstance(type);
                                }
                                else
                                {
                                    object[] errorFormatter = new object[] { GlobalServices.ErrorFormatter };
                                    errorHandler = (IErrorHandler)Activator.CreateInstance(type, errorFormatter);
                                }

                                GlobalServices._ERROR_HANDLER = errorHandler;
                            }
                        }

                        return GlobalServices._ERROR_HANDLER;
                    }
                    else
                    {
                        _ERRORHANDLER = GlobalServices._ERROR_HANDLER;
                    }
                }

                return _ERRORHANDLER;
            }
            set { GlobalServices._ERROR_HANDLER = value; }
        }

        static GlobalServices()
        {
            GlobalServices._ERROR_FORMATTER_LOCK = new object();
            GlobalServices._ERROR_HANDLER_LOCK = new object();
        }

        public static T GetErrorHandlerAs<T>()
            where T : IErrorHandler
        {
            T errorHandler;
            try
            {
                errorHandler = (T)GlobalServices.ErrorHandler;
            }
            catch
            {
                errorHandler = default(T);
            }

            return errorHandler;
        }
    }
}