using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class LocalSharePointServerAdapterConfig : SharePointServerAdapterConfig
    {
        public LocalSharePointServerAdapterConfig(SharePointAdapter adapter) : base(adapter)
        {
        }

        protected override void FetchConfig()
        {
            string str;
            try
            {
                try
                {
                    str = (HttpContext.Current == null
                        ? string.Concat(Utils.GetSharePointIsapiFolderFromRegistry(), "ML\\ServerAdapterConfig.xml")
                        : HttpContext.Current.Server.MapPath("/_vti_bin/ML/ServerAdapterConfig.xml"));
                    if (File.Exists(str))
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(str);
                        base.FromXml(xmlDocument.DocumentElement);
                    }
                }
                catch (XmlException xmlException1)
                {
                    XmlException xmlException = xmlException1;
                    Utils.LogExceptionDetails(xmlException, MethodBase.GetCurrentMethod().Name,
                        MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    Utils.LogExceptionDetails(exception, MethodBase.GetCurrentMethod().Name,
                        MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                }
            }
            finally
            {
                if (this.m_hashDisabledActions == null || this.m_hashExemptUsers == null ||
                    !this.m_bSuppressEvents.HasValue)
                {
                    this.m_hashDisabledActions = new Dictionary<string, bool>(0);
                    this.m_hashExemptUsers = new Dictionary<string, bool>(0);
                    this.m_bSuppressEvents = new bool?(true);
                }
            }
        }
    }
}