using Metalogix.DataStructures;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointServerAdapterConfig
    {
        protected SharePointAdapter m_adapter;

        protected Dictionary<string, bool> m_hashDisabledActions;

        protected Dictionary<string, bool> m_hashExemptUsers;

        private static object s_oLock;

        protected bool? m_bSuppressEvents = null;

        private Dictionary<string, bool> DisabledActions
        {
            get
            {
                lock (SharePointServerAdapterConfig.s_oLock)
                {
                    if (this.m_hashDisabledActions == null)
                    {
                        this.FetchConfig();
                    }
                }

                return this.m_hashDisabledActions;
            }
        }

        private Dictionary<string, bool> ExemptUsers
        {
            get
            {
                lock (SharePointServerAdapterConfig.s_oLock)
                {
                    if (this.m_hashExemptUsers == null)
                    {
                        this.FetchConfig();
                    }
                }

                return this.m_hashExemptUsers;
            }
        }

        public bool SuppressEvents
        {
            get
            {
                if (!this.m_bSuppressEvents.HasValue)
                {
                    this.FetchConfig();
                }

                return this.m_bSuppressEvents.Value;
            }
        }

        static SharePointServerAdapterConfig()
        {
            SharePointServerAdapterConfig.s_oLock = new object();
        }

        public SharePointServerAdapterConfig(SharePointAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("Adapater cannot be null");
            }

            this.m_adapter = adapter;
        }

        public void Clear()
        {
            this.m_hashDisabledActions = null;
            this.m_hashExemptUsers = null;
        }

        protected virtual void FetchConfig()
        {
            try
            {
                string str = this.m_adapter.Server.Trim();
                char[] chrArray = new char[] { '/' };
                string str1 = string.Format("{0}{1}", str.Trim(chrArray), "/_vti_bin/ML/ServerAdapterConfig.xml");
                CookieAwareWebClient cookieAwareWebClient = new CookieAwareWebClient()
                {
                    Credentials = this.m_adapter.Credentials.NetworkCredentials
                };
                if (this.m_adapter.AdapterProxy == null)
                {
                    cookieAwareWebClient.Proxy = null;
                }
                else
                {
                    cookieAwareWebClient.Proxy = this.m_adapter.AdapterProxy;
                }

                this.m_adapter.IncludedCertificates.CopyCertificatesToCollection(
                    cookieAwareWebClient.ClientCertificates);
                if (this.m_adapter.HasActiveCookieManager)
                {
                    CookieContainer cookieContainer = new CookieContainer();
                    this.m_adapter.CookieManager.AddCookiesTo(cookieContainer);
                    cookieAwareWebClient.UseDefaultCredentials = true;
                    cookieAwareWebClient.CookieContainer = cookieContainer;
                }

                this.FromXml(cookieAwareWebClient.DownloadString(str1));
            }
            catch (Exception exception)
            {
                this.m_hashDisabledActions = new Dictionary<string, bool>(0);
                this.m_hashExemptUsers = new Dictionary<string, bool>(0);
                this.m_bSuppressEvents = new bool?(true);
            }
        }

        protected void FromXml(string sXml)
        {
            if (sXml == null)
            {
                throw new ArgumentNullException("SharePointServerAdapterConfig.FromXml(): XML cannot be null");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXml(xmlDocument.DocumentElement);
        }

        protected void FromXml(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("SharePointServerAdapterConfig.FromXml(): XML cannot be null");
            }

            this.m_bSuppressEvents = new bool?(true);
            XmlNode xmlNodes = node.SelectSingleNode(string.Format("//{0}", "SuppressEvents"));
            if (xmlNodes != null)
            {
                bool flag = true;
                if (bool.TryParse(xmlNodes.InnerText, out flag))
                {
                    this.m_bSuppressEvents = new bool?(flag);
                }
            }

            this.m_hashExemptUsers = new Dictionary<string, bool>(4);
            XmlNodeList xmlNodeLists = node.SelectNodes(string.Format("//{0}/{1}", "ExemptUsers", "User"));
            if (xmlNodeLists != null)
            {
                foreach (XmlNode xmlNodes1 in xmlNodeLists)
                {
                    string str = xmlNodes1.InnerText.Trim();
                    if (this.m_hashExemptUsers.ContainsKey(str))
                    {
                        continue;
                    }

                    this.m_hashExemptUsers.Add(str.ToLower(), true);
                }
            }

            this.m_hashDisabledActions = new Dictionary<string, bool>(4);
            XmlNodeList xmlNodeLists1 = node.SelectNodes(string.Format("//{0}/{1}", "DisabledActions", "Action"));
            if (xmlNodeLists1 != null)
            {
                foreach (XmlNode xmlNodes2 in xmlNodeLists1)
                {
                    string str1 = xmlNodes2.InnerText.Trim();
                    if (this.m_hashDisabledActions.ContainsKey(str1))
                    {
                        continue;
                    }

                    this.m_hashDisabledActions.Add(str1.ToLower(), true);
                }
            }
        }

        public bool IsEnabled(string sUserName, string sActionName)
        {
            if (this.ExemptUsers.ContainsKey(sUserName.ToLower()))
            {
                return true;
            }

            return this.IsEnabled(sActionName);
        }

        public bool IsEnabled(string sActionName)
        {
            return !this.DisabledActions.ContainsKey(sActionName.ToLower());
        }

        public void Load()
        {
            if (this.m_hashDisabledActions == null || this.m_hashExemptUsers == null)
            {
                this.FetchConfig();
            }
        }

        private class XmlNames
        {
            public const string ServerAdapterConfig = "ServerAdapterConfig";

            public const string ExemptUsers = "ExemptUsers";

            public const string User = "User";

            public const string DisabledActions = "DisabledActions";

            public const string Action = "Action";

            public const string SuppressEvents = "SuppressEvents";

            public XmlNames()
            {
            }
        }
    }
}