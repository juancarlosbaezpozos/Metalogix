using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.UI.WinForms.Properties;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class ConnectExtensionErrorDialog : XtraForm
    {
        private IContainer components;

        private LabelControl BodyText;

        private SimpleButton btn_OK;

        private PictureEdit _warningIcon;

        private HyperLinkEdit _extensionsInstallLink;

        public ConnectExtensionErrorDialog()
        {
            InitializeComponent();
        }

        private void _extensionsInstallLink_OpenLink(object sender, OpenLinkEventArgs e)
        {
            try
            {
                Process.Start("http://www.metalogix.com/document/content-matrix-advanced-installation-guide");
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                GlobalServices.ErrorHandler.HandleException("Help Error", $"Error opening Help file : {ex2.Message}", ex2, ErrorIcon.Error);
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.ConnectExtensionErrorDialog));
            this.BodyText = new DevExpress.XtraEditors.LabelControl();
            this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
            this._warningIcon = new DevExpress.XtraEditors.PictureEdit();
            this._extensionsInstallLink = new DevExpress.XtraEditors.HyperLinkEdit();
            ((System.ComponentModel.ISupportInitialize)this._warningIcon.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._extensionsInstallLink.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.BodyText, "BodyText");
            this.BodyText.Name = "BodyText";
            resources.ApplyResources(this.btn_OK, "btn_OK");
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Click += new System.EventHandler(On_click_OK);
            this._warningIcon.EditValue = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Warning32;
            resources.ApplyResources(this._warningIcon, "_warningIcon");
            this._warningIcon.Name = "_warningIcon";
            this._warningIcon.Properties.AllowFocused = false;
            this._warningIcon.Properties.AllowScrollViaMouseDrag = false;
            this._warningIcon.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("_warningIcon.Properties.Appearance.BackColor");
            this._warningIcon.Properties.Appearance.Options.UseBackColor = true;
            this._warningIcon.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._warningIcon.Properties.ReadOnly = true;
            this._warningIcon.Properties.ShowMenu = false;
            this._warningIcon.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
            this._warningIcon.ShowToolTips = false;
            resources.ApplyResources(this._extensionsInstallLink, "_extensionsInstallLink");
            this._extensionsInstallLink.Name = "_extensionsInstallLink";
            this._extensionsInstallLink.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("_extensionsInstallLink.Properties.Appearance.BackColor");
            this._extensionsInstallLink.Properties.Appearance.Options.UseBackColor = true;
            this._extensionsInstallLink.Visible = true;
            this._extensionsInstallLink.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this._extensionsInstallLink.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(_extensionsInstallLink_OpenLink);
            base.AcceptButton = this.btn_OK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ControlBox = false;
            base.Controls.Add(this._extensionsInstallLink);
            base.Controls.Add(this._warningIcon);
            base.Controls.Add(this.btn_OK);
            base.Controls.Add(this.BodyText);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ConnectExtensionErrorDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            ((System.ComponentModel.ISupportInitialize)this._warningIcon.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._extensionsInstallLink.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void On_click_OK(object sender, EventArgs e)
        {
            Close();
        }
    }
}
