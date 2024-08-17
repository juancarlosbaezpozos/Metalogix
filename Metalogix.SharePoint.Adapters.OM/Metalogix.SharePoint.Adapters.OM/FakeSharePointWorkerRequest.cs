using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class FakeSharePointWorkerRequest : SimpleWorkerRequest
    {
        private readonly string _ServerName;

        public FakeSharePointWorkerRequest(SPWeb web) : base(web.ServerRelativeUrl,
            web.Site.WebApplication.IisSettings[0].Path.FullName, string.Empty, string.Empty, null)
        {
            this._ServerName = web.Site.HostName;
        }

        public FakeSharePointWorkerRequest(SPWeb web, TextWriter resp) : base(string.Empty, string.Empty, resp)
        {
            this._ServerName = web.Site.HostName;
        }

        public static HttpContext GetFakeHttpContextForSharePoint(SPWeb web, bool replaceExisting)
        {
            if (replaceExisting)
            {
                if (string.IsNullOrEmpty(Thread.GetDomain().GetData(".appPath") as string))
                {
                    Thread.GetDomain().SetData(".appPath", "C:\\FakeWorkerRequest");
                }

                if (string.IsNullOrEmpty(Thread.GetDomain().GetData(".appVPath") as string))
                {
                    Thread.GetDomain().SetData(".appVPath", "/_FakeWorkerRequest");
                }
            }

            HttpContext httpContext = new HttpContext((replaceExisting
                ? new FakeSharePointWorkerRequest(web, new StringWriter())
                : new FakeSharePointWorkerRequest(web)));
            httpContext.Request.Browser = new HttpBrowserCapabilities();
            httpContext.Items["HttpHandlerSPWeb"] = web;
            httpContext.Items["HttpHandlerSPSite"] = web.Site;
            httpContext.Items["FormDigestValidated"] = true;
            return httpContext;
        }

        public static HttpContext GetFakeHttpContextForSharePoint(SPWeb web)
        {
            FakeSharePointWorkerRequest fakeSharePointWorkerRequest = null;
            try
            {
                fakeSharePointWorkerRequest = new FakeSharePointWorkerRequest(web);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Utils.LogExceptionDetails(exception, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                fakeSharePointWorkerRequest = new FakeSharePointWorkerRequest(web, null);
            }

            HttpContext httpContext = new HttpContext(fakeSharePointWorkerRequest);
            httpContext.Request.Browser = new HttpBrowserCapabilities();
            httpContext.Items["HttpHandlerSPWeb"] = web;
            httpContext.Items["HttpHandlerSPSite"] = web.Site;
            return httpContext;
        }

        public override string GetServerName()
        {
            return this._ServerName;
        }
    }
}