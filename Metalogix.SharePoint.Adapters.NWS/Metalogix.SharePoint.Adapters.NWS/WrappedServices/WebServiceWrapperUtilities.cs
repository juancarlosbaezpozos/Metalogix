using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Net;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    internal static class WebServiceWrapperUtilities
    {
        internal static int WebServiceTimeoutTime
        {
            get { return AdapterConfigurationVariables.WebServiceTimeoutTime; }
        }

        internal static object ExecuteMethod(IWebServiceWrapper service, string sMethodName, object[] parameters)
        {
            object obj;
            MethodInfo method = service.WrappedService.GetType().GetMethod(sMethodName);
            if (method == null)
            {
                throw new Exception(string.Concat("Cannot find method \"", sMethodName, "\" in web service wrapper."));
            }

            if (service.ParentAdapter.HasActiveCookieManager && service.ParentAdapter.CookieManager.LockCookie)
            {
                service.ParentAdapter.CookieManager.AquireCookieLock();
            }

            try
            {
                try
                {
                    obj = method.Invoke(service.WrappedService, parameters);
                }
                catch (Exception exception2)
                {
                    Exception exception = exception2;
                    if (!service.ParentAdapter.HasActiveCookieManager)
                    {
                        throw WebServiceWrapperUtilities.GetExecutedMethodException(exception);
                    }

                    WebServiceWrapperUtilities.UpdateCookie(service);
                    try
                    {
                        obj = method.Invoke(service.WrappedService, parameters);
                    }
                    catch (Exception exception1)
                    {
                        throw WebServiceWrapperUtilities.GetExecutedMethodException(exception1);
                    }
                }
            }
            finally
            {
                if (service.ParentAdapter.HasActiveCookieManager && service.ParentAdapter.CookieManager.LockCookie)
                {
                    service.ParentAdapter.CookieManager.ReleaseCookieLock();
                }
            }

            return obj;
        }

        private static Exception GetExecutedMethodException(Exception ex)
        {
            if (!(ex is TargetInvocationException) || ex.InnerException == null)
            {
                return ex;
            }

            return ex.InnerException;
        }

        internal static void UpdateCookie(IWebServiceWrapper service)
        {
            if (!service.ParentAdapter.HasActiveCookieManager)
            {
                return;
            }

            service.ParentAdapter.CookieManager.UpdateCookie();
            service.CookieContainer = new CookieContainer();
            service.ParentAdapter.CookieManager.AddCookiesTo(service.CookieContainer);
        }
    }
}