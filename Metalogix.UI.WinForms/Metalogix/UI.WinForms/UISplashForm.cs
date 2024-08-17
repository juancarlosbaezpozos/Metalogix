using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Licensing;

namespace Metalogix.UI.WinForms
{
    public class UISplashForm : XtraForm
    {
        private delegate void Action();

        private delegate void FadeDelegate();

        private const int MF_BYPOSITION = 1024;

        private UIApplication m_application;

        private IContainer components;

        private PictureBox w_pbSplashLogo;

        private Panel w_plBackground;

        private MarqueeProgressBarControl marqueeProgressBarControl1;

        private PictureBox w_waitMessage;

        public UIApplication Application
        {
            get
		{
			return m_application;
		}
            set
		{
			m_application = value;
			if (Application != null)
			{
				Text = Application.ProductName;
				base.Icon = Application.AppIcon;
				bool flag = LicensingUtils.IsLicenseFileExpressEdition();
				w_pbSplashLogo.InitialImage = (flag ? Application.ExpressSplashScreenImage : Application.SplashScreenImage);
				w_pbSplashLogo.Image = w_pbSplashLogo.InitialImage;
				w_pbSplashLogo.SizeMode = PictureBoxSizeMode.CenterImage;
				string str = "Metalogix.UI.WinForms.Icons.WaitMessage.png";
				w_waitMessage.InitialImage = ImageCache.GetImage(str, GetType().Assembly);
				w_waitMessage.Image = w_waitMessage.InitialImage;
				w_waitMessage.SizeMode = PictureBoxSizeMode.CenterImage;
			}
		}
        }

        internal UISplashForm()
	{
		base.Opacity = 0.99;
		BringToFront();
		InitializeComponent();
	}

        public void BringToFrontFromOffThread()
	{
		if (!base.InvokeRequired)
		{
			BringToFront();
			Refresh();
		}
		else
		{
			Invoke(new FadeDelegate(BringToFrontFromOffThread));
		}
	}

        protected override void Dispose(bool disposing)
	{
		if (!base.InvokeRequired)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		else
		{
			Action<bool> action = Dispose;
			object[] objArray = new object[1] { disposing };
			Invoke(action, objArray);
		}
	}

        private void FadeOut()
	{
		if (base.InvokeRequired)
		{
			Invoke(new FadeDelegate(FadeOut));
			return;
		}
		int num = (UISettings.IsRemoteSession ? 1 : 20);
		float single = 0.99f / (float)num;
		while (num > 0)
		{
			num--;
			Opacity -= single;
			Thread.Sleep(25);
		}
		SendToBack();
		Close();
	}

        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);

        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.UISplashForm));
		this.w_pbSplashLogo = new System.Windows.Forms.PictureBox();
		this.w_plBackground = new System.Windows.Forms.Panel();
		this.w_waitMessage = new System.Windows.Forms.PictureBox();
		this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		((System.ComponentModel.ISupportInitialize)this.w_pbSplashLogo).BeginInit();
		this.w_plBackground.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_waitMessage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl1.Properties).BeginInit();
		base.SuspendLayout();
		this.w_pbSplashLogo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_pbSplashLogo.BackColor = System.Drawing.Color.White;
		this.w_pbSplashLogo.Location = new System.Drawing.Point(0, 0);
		this.w_pbSplashLogo.Margin = new System.Windows.Forms.Padding(0);
		this.w_pbSplashLogo.Name = "w_pbSplashLogo";
		this.w_pbSplashLogo.Size = new System.Drawing.Size(392, 177);
		this.w_pbSplashLogo.TabIndex = 2;
		this.w_pbSplashLogo.TabStop = false;
		this.w_pbSplashLogo.UseWaitCursor = true;
		this.w_plBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.w_plBackground.Controls.Add(this.w_waitMessage);
		this.w_plBackground.Controls.Add(this.marqueeProgressBarControl1);
		this.w_plBackground.Controls.Add(this.w_pbSplashLogo);
		this.w_plBackground.Dock = System.Windows.Forms.DockStyle.Fill;
		this.w_plBackground.Location = new System.Drawing.Point(0, 0);
		this.w_plBackground.Name = "w_plBackground";
		this.w_plBackground.Size = new System.Drawing.Size(392, 177);
		this.w_plBackground.TabIndex = 4;
		this.w_plBackground.UseWaitCursor = true;
		this.w_waitMessage.Location = new System.Drawing.Point(22, 132);
		this.w_waitMessage.Name = "w_waitMessage";
		this.w_waitMessage.Size = new System.Drawing.Size(351, 16);
		this.w_waitMessage.TabIndex = 4;
		this.w_waitMessage.TabStop = false;
		this.w_waitMessage.UseWaitCursor = true;
		this.marqueeProgressBarControl1.EditValue = 0;
		this.marqueeProgressBarControl1.Location = new System.Drawing.Point(2, 168);
		this.marqueeProgressBarControl1.Margin = new System.Windows.Forms.Padding(0);
		this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
		this.marqueeProgressBarControl1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.marqueeProgressBarControl1.Properties.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.marqueeProgressBarControl1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.marqueeProgressBarControl1.Properties.EndColor = System.Drawing.Color.FromArgb(90, 135, 198);
		this.marqueeProgressBarControl1.Properties.LookAndFeel.SkinName = "Office 2013";
		this.marqueeProgressBarControl1.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.marqueeProgressBarControl1.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.marqueeProgressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.marqueeProgressBarControl1.Properties.StartColor = System.Drawing.Color.FromArgb(90, 135, 198);
		this.marqueeProgressBarControl1.Size = new System.Drawing.Size(385, 5);
		this.marqueeProgressBarControl1.TabIndex = 3;
		this.marqueeProgressBarControl1.UseWaitCursor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(392, 177);
		base.ControlBox = false;
		base.Controls.Add(this.w_plBackground);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.LookAndFeel.SkinName = "Office 2013";
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "UISplashForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Initializing Application";
		base.UseWaitCursor = true;
		base.Load += new System.EventHandler(UISplashScreenDialog_Load);
		base.Shown += new System.EventHandler(UISplashScreenDialog_Shown);
		((System.ComponentModel.ISupportInitialize)this.w_pbSplashLogo).EndInit();
		this.w_plBackground.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.w_waitMessage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl1.Properties).EndInit();
		base.ResumeLayout(false);
	}

        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        public void RequestClose()
	{
		ThreadStart threadStart = new FadeDelegate(FadeOut).Invoke;
		new Thread(threadStart).Start();
	}

        private void UISplashScreenDialog_Load(object sender, EventArgs e)
	{
		IntPtr systemMenu = GetSystemMenu(base.Handle, bRevert: false);
		int menuItemCount = GetMenuItemCount(systemMenu);
		RemoveMenu(systemMenu, menuItemCount - 1, 1024);
	}

        private void UISplashScreenDialog_Shown(object sender, EventArgs e)
	{
		BringToFront();
		Refresh();
	}
    }
}
