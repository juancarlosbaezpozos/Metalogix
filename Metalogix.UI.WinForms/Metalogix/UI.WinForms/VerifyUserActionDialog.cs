using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
    public partial class VerifyUserActionDialog : CollapsableForm
    {
        private bool m_bShowDialogAgain = true;

        private IContainer components;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOk;

        private CheckEdit w_cbShowDialogAgain;

        private LabelControl w_lblInfoMessage;

        private PictureEdit w_warning;

        private HyperLinkEdit _infoLink;

        private PanelControl _linkPanel;

        public string InformationLink
        {
            get
            {
                return this._infoLink.Text;
            }
            set
            {
                this._infoLink.Text = value;
            }
        }

        public string InformationLinkText
        {
            get
            {
                return this._infoLink.Properties.Caption;
            }
            set
            {
                this._infoLink.Properties.Caption = value;
            }
        }

        public string InformationMessage
        {
            get
            {
                return this.w_lblInfoMessage.Text;
            }
            set
            {
                this.w_lblInfoMessage.Text = value;
            }
        }

        public bool ShowDialogAgain
        {
            get
            {
                return this.m_bShowDialogAgain;
            }
        }

        private VerifyUserActionDialog()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static bool GetUserVerification(IConfigurationVariable configurationVariable, string informationMessage, string title = "Information", string infoLink = null, string infoLinkText = "Click here for more information", MessageBoxButtons messageBoxButtons = MessageBoxButtons.OKCancel)
        {
            if (!configurationVariable.GetValue<bool>())
            {
                return true;
            }
            VerifyUserActionDialog verifyUserActionDialog = VerifyUserActionDialog.GetVerifyUserActionDialog(informationMessage, title, infoLink, infoLinkText);
            if (messageBoxButtons == MessageBoxButtons.OK)
            {
                verifyUserActionDialog.w_btnCancel.Visible = false;
                verifyUserActionDialog.w_btnOk.Location = verifyUserActionDialog.w_btnCancel.Location;
            }
            if (verifyUserActionDialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }
            if (!verifyUserActionDialog.ShowDialogAgain)
            {
                configurationVariable.SetValue(false);
            }
            return true;
        }

        public static bool GetUserVerification(string informationMessage, string title = "Information", string infoLink = null, string infoLinkText = "Click here for more information")
        {
            VerifyUserActionDialog verifyUserActionDialog = VerifyUserActionDialog.GetVerifyUserActionDialog(informationMessage, title, infoLink, infoLinkText);
            verifyUserActionDialog.w_cbShowDialogAgain.Visible = false;
            if (verifyUserActionDialog.ShowDialog() == DialogResult.OK)
            {
                return true;
            }
            return false;
        }

        private static VerifyUserActionDialog GetVerifyUserActionDialog(string informationMessage, string title, string infoLink, string infoLinkText)
        {
            VerifyUserActionDialog verifyUserActionDialog = new VerifyUserActionDialog()
            {
                Text = title,
                InformationMessage = informationMessage,
                InformationLink = infoLink,
                InformationLinkText = infoLinkText
            };
            return verifyUserActionDialog;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(VerifyUserActionDialog));
            this.w_btnCancel = new SimpleButton();
            this.w_btnOk = new SimpleButton();
            this.w_cbShowDialogAgain = new CheckEdit();
            this.w_lblInfoMessage = new LabelControl();
            this.w_warning = new PictureEdit();
            this._infoLink = new HyperLinkEdit();
            this._linkPanel = new PanelControl();
            ((ISupportInitialize)this.w_cbShowDialogAgain.Properties).BeginInit();
            ((ISupportInitialize)this.w_warning.Properties).BeginInit();
            ((ISupportInitialize)this._infoLink.Properties).BeginInit();
            ((ISupportInitialize)this._linkPanel).BeginInit();
            this._linkPanel.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            this.w_btnCancel.Click += new EventHandler(this.On_Cancel_Click);
            componentResourceManager.ApplyResources(this.w_btnOk, "w_btnOk");
            this.w_btnOk.Name = "w_btnOk";
            this.w_btnOk.Click += new EventHandler(this.On_Ok_Click);
            componentResourceManager.ApplyResources(this.w_cbShowDialogAgain, "w_cbShowDialogAgain");
            this.w_cbShowDialogAgain.Name = "w_cbShowDialogAgain";
            this.w_cbShowDialogAgain.Properties.Caption = componentResourceManager.GetString("w_cbShowDialogAgain.Properties.Caption");
            this.w_cbShowDialogAgain.CheckedChanged += new EventHandler(this.On_ShowDialogAgain_Changed);
            componentResourceManager.ApplyResources(this.w_lblInfoMessage, "w_lblInfoMessage");
            this.w_lblInfoMessage.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblInfoMessage.Appearance.Font");
            this.w_lblInfoMessage.Name = "w_lblInfoMessage";
            this.w_warning.EditValue = Resources.JobStatus_Warning_32;
            componentResourceManager.ApplyResources(this.w_warning, "w_warning");
            this.w_warning.Name = "w_warning";
            this.w_warning.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_warning.Properties.ErrorImage = Resources.JobStatus_Failed_32;
            this.w_warning.Properties.InitialImage = Resources.Item_Status_Warning;
            this.w_warning.Properties.ShowMenu = false;
            componentResourceManager.ApplyResources(this._infoLink, "_infoLink");
            this._infoLink.Name = "_infoLink";
            this._infoLink.Properties.AllowFocused = false;
            this._infoLink.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("_infoLink.Properties.Appearance.BackColor");
            this._infoLink.Properties.Appearance.Options.UseBackColor = true;
            this._infoLink.Properties.BorderStyle = BorderStyles.NoBorder;
            this._linkPanel.BorderStyle = BorderStyles.NoBorder;
            this._linkPanel.Controls.Add(this._infoLink);
            componentResourceManager.ApplyResources(this._linkPanel, "_linkPanel");
            this._linkPanel.Name = "_linkPanel";
            base.AcceptButton = this.w_btnOk;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.ControlBox = false;
            base.Controls.Add(this._linkPanel);
            base.Controls.Add(this.w_warning);
            base.Controls.Add(this.w_lblInfoMessage);
            base.Controls.Add(this.w_cbShowDialogAgain);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOk);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "VerifyUserActionDialog";
            base.SizeGripStyle = SizeGripStyle.Hide;
            base.Load += new EventHandler(this.On_Load);
            ((ISupportInitialize)this.w_cbShowDialogAgain.Properties).EndInit();
            ((ISupportInitialize)this.w_warning.Properties).EndInit();
            ((ISupportInitialize)this._infoLink.Properties).EndInit();
            ((ISupportInitialize)this._linkPanel).EndInit();
            this._linkPanel.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void On_Cancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void On_Load(object sender, EventArgs e)
        {
            int height = this.w_lblInfoMessage.Height - 13;
            base.Height = base.Height + height;
            PanelControl point = this._linkPanel;
            int x = this._linkPanel.Location.X;
            Point location = this._linkPanel.Location;
            point.Location = new Point(x, location.Y + height);
            if (string.IsNullOrEmpty(this.InformationLink))
            {
                base.HideControl(this._linkPanel);
            }
        }

        private void On_Ok_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        private void On_ShowDialogAgain_Changed(object sender, EventArgs e)
        {
            this.m_bShowDialogAgain = !this.w_cbShowDialogAgain.Checked;
        }
    }
}