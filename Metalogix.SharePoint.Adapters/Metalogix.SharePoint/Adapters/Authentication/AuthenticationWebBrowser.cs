using Metalogix.DataStructures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class AuthenticationWebBrowser : Form
    {
        private string m_sSiteUrl;

        private WebProxy m_webProxy;

        private X509CertificateWrapperCollection m_includedCertificates;

        private ReadOnlyCollection<Cookie> m_cookies;

        private Exception m_lastAttemptException;

        private IContainer components;

        private StatusStrip statusStrip1;

        private ToolStripProgressBar w_tspbMarquee;

        private ToolStripStatusLabel w_tslbStatus;

        private WebBrowser w_wbBrowser;

        public ReadOnlyCollection<Cookie> Cookies
        {
            get { return this.m_cookies; }
        }

        public Exception LastAttemptException
        {
            get { return this.m_lastAttemptException; }
        }

        public AuthenticationWebBrowser(string sSiteUrl, WebProxy webProxy,
            X509CertificateWrapperCollection includedCertificates)
        {
            this.InitializeComponent();
            this.m_sSiteUrl = sSiteUrl;
            this.m_webProxy = webProxy;
            this.m_includedCertificates = includedCertificates;
            this.m_cookies = null;
            this.m_lastAttemptException = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager =
                new ComponentResourceManager(typeof(AuthenticationWebBrowser));
            this.statusStrip1 = new StatusStrip();
            this.w_tspbMarquee = new ToolStripProgressBar();
            this.w_tslbStatus = new ToolStripStatusLabel();
            this.w_wbBrowser = new WebBrowser();
            this.statusStrip1.SuspendLayout();
            base.SuspendLayout();
            ToolStripItemCollection items = this.statusStrip1.Items;
            ToolStripItem[] wTspbMarquee = new ToolStripItem[] { this.w_tspbMarquee, this.w_tslbStatus };
            items.AddRange(wTspbMarquee);
            componentResourceManager.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.w_tspbMarquee.Name = "w_tspbMarquee";
            componentResourceManager.ApplyResources(this.w_tspbMarquee, "w_tspbMarquee");
            this.w_tspbMarquee.Style = ProgressBarStyle.Marquee;
            this.w_tslbStatus.Name = "w_tslbStatus";
            componentResourceManager.ApplyResources(this.w_tslbStatus, "w_tslbStatus");
            this.w_wbBrowser.AllowWebBrowserDrop = false;
            componentResourceManager.ApplyResources(this.w_wbBrowser, "w_wbBrowser");
            this.w_wbBrowser.Name = "w_wbBrowser";
            this.w_wbBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.On_Navigated);
            this.w_wbBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.On_Navigating);
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.w_wbBrowser);
            base.Controls.Add(this.statusStrip1);
            base.MinimizeBox = false;
            base.Name = "AuthenticationWebBrowser";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.TopMost = true;
            base.FormClosed += new FormClosedEventHandler(this.On_FormClosed);
            base.Load += new EventHandler(this.On_FormLoad);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (base.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }

        private void On_FormLoad(object sender, EventArgs e)
        {
            base.TopMost = false;
            base.TopMost = true;
            this.w_wbBrowser.Navigate(this.m_sSiteUrl);
        }

        private void On_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.w_tslbStatus.Text = "Currently detected credentials are insufficient";
            this.w_tspbMarquee.Visible = false;
            this.UpdateCookies();
            if (WebBrowserLoginUtils.TestSharePointConnection(this.m_sSiteUrl, this.m_webProxy,
                    this.m_includedCertificates, this.Cookies, out this.m_lastAttemptException))
            {
                base.DialogResult = System.Windows.Forms.DialogResult.OK;
                base.Close();
            }
        }

        private void On_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.w_tslbStatus.Text = string.Concat("Loading: ", e.Url.ToString());
            this.w_tspbMarquee.Visible = true;
        }

        private void UpdateCookies()
        {
            Cookie[] cookieArray = WebBrowserLoginUtils.ReadBrowserCookies(this.m_sSiteUrl, this.w_wbBrowser.Version);
            this.m_cookies = new ReadOnlyCollection<Cookie>(cookieArray);
        }
    }
}