using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class HostNameDialog : XtraForm
    {
        private string m_HostName;

        private IContainer components;

        private SimpleButton w_btnOK;

        private SimpleButton w_btnCancelSkip;

        private LabelControl w_lblHostName;

        private TextEdit w_tbHostName;

        private LabelControl label4;

        private PictureEdit w_warning;

        public string CancelSkipButtonText
        {
            set
            {
                w_btnCancelSkip.Text = value;
            }
        }

        public string HostName
        {
            get
            {
                return m_HostName.TrimEnd('/');
            }
            set
            {
                m_HostName = value;
                w_tbHostName.Text = value;
            }
        }

        public HostNameDialog()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.HostNameDialog));
            this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.w_btnCancelSkip = new DevExpress.XtraEditors.SimpleButton();
            this.w_lblHostName = new DevExpress.XtraEditors.LabelControl();
            this.w_tbHostName = new DevExpress.XtraEditors.TextEdit();
            this.label4 = new DevExpress.XtraEditors.LabelControl();
            this.w_warning = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbHostName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_warning.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new System.EventHandler(w_btnOK_Click);
            this.w_btnCancelSkip.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.w_btnCancelSkip, "w_btnCancelSkip");
            this.w_btnCancelSkip.Name = "w_btnCancelSkip";
            this.w_btnCancelSkip.Click += new System.EventHandler(w_btnCancelSkip_Click);
            resources.ApplyResources(this.w_lblHostName, "w_lblHostName");
            this.w_lblHostName.Name = "w_lblHostName";
            resources.ApplyResources(this.w_tbHostName, "w_tbHostName");
            this.w_tbHostName.Name = "w_tbHostName";
            this.label4.Appearance.Font = (System.Drawing.Font)resources.GetObject("label4.Appearance.Font");
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.w_warning.EditValue = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Warning32;
            resources.ApplyResources(this.w_warning, "w_warning");
            this.w_warning.Name = "w_warning";
            this.w_warning.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_warning.Properties.ErrorImage = (System.Drawing.Image)resources.GetObject("w_warning.Properties.ErrorImage");
            this.w_warning.Properties.InitialImage = (System.Drawing.Image)resources.GetObject("w_warning.Properties.InitialImage");
            this.w_warning.Properties.ShowMenu = false;
            base.AcceptButton = this.w_btnOK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancelSkip;
            base.Controls.Add(this.w_warning);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.w_tbHostName);
            base.Controls.Add(this.w_lblHostName);
            base.Controls.Add(this.w_btnCancelSkip);
            base.Controls.Add(this.w_btnOK);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "HostNameDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            ((System.ComponentModel.ISupportInitialize)this.w_tbHostName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_warning.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void w_btnCancelSkip_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
        }

        private void w_btnOK_Click(object sender, EventArgs e)
        {
            if (w_tbHostName.Text == "")
            {
                FlatXtraMessageBox.Show("You must enter a value in the text Box", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            m_HostName = w_tbHostName.Text;
            base.DialogResult = DialogResult.OK;
        }
    }
}
