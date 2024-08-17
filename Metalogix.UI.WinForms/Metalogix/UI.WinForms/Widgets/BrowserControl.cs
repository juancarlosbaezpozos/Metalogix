using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Widgets
{
    public class BrowserControl : UserControl
    {
        private IContainer components;

        private ToolStrip toolStrip1;

        private ToolStripButton w_tsBack;

        private ToolStripButton w_tsForward;

        private ToolStripButton w_tsStop;

        private ToolStripButton w_tsRefresh;

        private WebBrowser w_browser;

        public WebBrowser Browser => w_browser;

        public BrowserControl()
	{
		InitializeComponent();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		this.toolStrip1 = new System.Windows.Forms.ToolStrip();
		this.w_tsBack = new System.Windows.Forms.ToolStripButton();
		this.w_tsForward = new System.Windows.Forms.ToolStripButton();
		this.w_tsStop = new System.Windows.Forms.ToolStripButton();
		this.w_tsRefresh = new System.Windows.Forms.ToolStripButton();
		this.w_browser = new System.Windows.Forms.WebBrowser();
		this.toolStrip1.SuspendLayout();
		base.SuspendLayout();
		this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(230, 230, 230);
		this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.toolStrip1.Items;
		System.Windows.Forms.ToolStripItem[] wTsBack = new System.Windows.Forms.ToolStripItem[4] { this.w_tsBack, this.w_tsForward, this.w_tsStop, this.w_tsRefresh };
		items.AddRange(wTsBack);
		this.toolStrip1.Location = new System.Drawing.Point(0, 0);
		this.toolStrip1.Name = "toolStrip1";
		this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.toolStrip1.Size = new System.Drawing.Size(337, 25);
		this.toolStrip1.TabIndex = 2;
		this.toolStrip1.Text = "toolStrip1";
		this.w_tsBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_tsBack.Image = Metalogix.UI.WinForms.Properties.Resources.NavigateLeft16;
		this.w_tsBack.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.w_tsBack.Name = "w_tsBack";
		this.w_tsBack.Size = new System.Drawing.Size(23, 22);
		this.w_tsBack.Text = "Back";
		this.w_tsBack.Click += new System.EventHandler(w_tsBack_Click);
		this.w_tsForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_tsForward.Image = Metalogix.UI.WinForms.Properties.Resources.Navigate16;
		this.w_tsForward.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.w_tsForward.Name = "w_tsForward";
		this.w_tsForward.Size = new System.Drawing.Size(23, 22);
		this.w_tsForward.Text = "Forward";
		this.w_tsForward.Click += new System.EventHandler(w_tsForward_Click);
		this.w_tsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_tsStop.Image = Metalogix.UI.WinForms.Properties.Resources.Delete16;
		this.w_tsStop.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.w_tsStop.Name = "w_tsStop";
		this.w_tsStop.Size = new System.Drawing.Size(23, 22);
		this.w_tsStop.Text = "Stop";
		this.w_tsStop.Click += new System.EventHandler(w_tsStop_Click);
		this.w_tsRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.w_tsRefresh.Image = Metalogix.UI.WinForms.Properties.Resources.RefreshButton1;
		this.w_tsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.w_tsRefresh.Name = "w_tsRefresh";
		this.w_tsRefresh.Size = new System.Drawing.Size(23, 22);
		this.w_tsRefresh.Text = "Refresh";
		this.w_tsRefresh.Click += new System.EventHandler(w_tsRefresh_Click);
		this.w_browser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.w_browser.Location = new System.Drawing.Point(0, 25);
		this.w_browser.MinimumSize = new System.Drawing.Size(20, 20);
		this.w_browser.Name = "w_browser";
		this.w_browser.Size = new System.Drawing.Size(337, 267);
		this.w_browser.TabIndex = 3;
		this.w_browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(On_Navigated);
		this.w_browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(On_Navigating);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_browser);
		base.Controls.Add(this.toolStrip1);
		base.Name = "BrowserControl";
		base.Size = new System.Drawing.Size(337, 292);
		this.toolStrip1.ResumeLayout(false);
		this.toolStrip1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        public static bool IsFileTypeDisplayable(string fileExtension)
	{
		if (fileExtension == ".wbs")
		{
			return false;
		}
		return true;
	}

        private void On_Navigated(object sender, WebBrowserNavigatedEventArgs e)
	{
		SuspendLayout();
		w_tsStop.Enabled = false;
		w_tsBack.Enabled = w_browser.CanGoBack;
		w_tsForward.Enabled = w_browser.CanGoForward;
		ResumeLayout();
	}

        private void On_Navigating(object sender, WebBrowserNavigatingEventArgs e)
	{
		SuspendLayout();
		w_tsStop.Enabled = true;
		w_tsBack.Enabled = w_browser.CanGoBack;
		w_tsForward.Enabled = w_browser.CanGoForward;
		ResumeLayout();
	}

        private void w_tsBack_Click(object sender, EventArgs e)
	{
		w_browser.GoBack();
	}

        private void w_tsForward_Click(object sender, EventArgs e)
	{
		w_browser.GoForward();
	}

        private void w_tsRefresh_Click(object sender, EventArgs e)
	{
		w_browser.Refresh();
	}

        private void w_tsStop_Click(object sender, EventArgs e)
	{
		w_browser.Stop();
	}
    }
}
