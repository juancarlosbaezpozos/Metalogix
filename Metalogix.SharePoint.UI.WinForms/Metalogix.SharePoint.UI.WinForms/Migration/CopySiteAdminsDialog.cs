using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    public class CopySiteAdminsDialog : XtraForm
    {
        private IContainer components;

        public MemoEdit w_tbSiteCollectionAdmins;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        public CopySiteAdminsDialog()
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
            this.w_tbSiteCollectionAdmins = new DevExpress.XtraEditors.MemoEdit();
            this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
            this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)this.w_tbSiteCollectionAdmins.Properties).BeginInit();
            base.SuspendLayout();
            this.w_tbSiteCollectionAdmins.Location = new System.Drawing.Point(4, 8);
            this.w_tbSiteCollectionAdmins.Name = "w_tbSiteCollectionAdmins";
            this.w_tbSiteCollectionAdmins.Size = new System.Drawing.Size(418, 72);
            this.w_tbSiteCollectionAdmins.TabIndex = 2;
            this.w_bOkay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.w_bOkay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_bOkay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.w_bOkay.Location = new System.Drawing.Point(266, 88);
            this.w_bOkay.Name = "w_bOkay";
            this.w_bOkay.Size = new System.Drawing.Size(75, 23);
            this.w_bOkay.TabIndex = 4;
            this.w_bOkay.Text = "OK";
            this.w_bCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_bCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.w_bCancel.Location = new System.Drawing.Point(347, 88);
            this.w_bCancel.Name = "w_bCancel";
            this.w_bCancel.Size = new System.Drawing.Size(75, 23);
            this.w_bCancel.TabIndex = 5;
            this.w_bCancel.Text = "Cancel";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(427, 118);
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_bCancel);
            base.Controls.Add(this.w_tbSiteCollectionAdmins);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CopySiteAdminsDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Site Collection Administrators";
            ((System.ComponentModel.ISupportInitialize)this.w_tbSiteCollectionAdmins.Properties).EndInit();
            base.ResumeLayout(false);
        }
    }
}