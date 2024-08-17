using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint;
using System;
using System.Globalization;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.OM
{
    internal class Context : IDisposable
    {
        private bool m_bDispose;

        private bool m_bSuppressing;

        private SPSite m_currentSite;

        private SPWeb m_currentWeb;

        private CultureInfo m_originalThreadCulture;

        public SPSite Site
        {
            get { return this.m_currentSite; }
        }

        public SPWeb Web
        {
            get
            {
                if (this.m_currentWeb == null)
                {
                    this.m_currentWeb = this.Site.OpenWeb();
                }

                this.m_currentWeb.AllowUnsafeUpdates = (true);
                return this.m_currentWeb;
            }
        }

        public Context(SPSite currentSite, SharePointAdapter adapter, bool dispose)
        {
            this.m_currentSite = currentSite;
            this.m_bDispose = dispose;
            if (adapter.ServerAdapterConfiguration.SuppressEvents)
            {
                this.m_bSuppressing = true;
                EventSuppressionReceiver.SuppressEvents();
            }
        }

        public Context(SPSite currentSite, SPWeb currentWeb, SharePointAdapter adapter, bool dispose)
        {
            this.m_currentSite = currentSite;
            this.m_currentWeb = currentWeb;
            this.m_bDispose = dispose;
            if (adapter.ServerAdapterConfiguration.SuppressEvents)
            {
                this.m_bSuppressing = true;
                EventSuppressionReceiver.SuppressEvents();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDispose)
        {
            if (this.m_bDispose && bDispose)
            {
                if (this.m_currentWeb != null)
                {
                    this.m_currentWeb.Dispose();
                    this.m_currentWeb = null;
                }

                if (this.m_currentSite != null)
                {
                    this.m_currentSite.Dispose();
                    this.m_currentSite = null;
                }
            }

            if (this.m_bSuppressing)
            {
                EventSuppressionReceiver.AllowEvents();
            }

            if (this.m_originalThreadCulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = this.m_originalThreadCulture;
            }
        }
    }
}