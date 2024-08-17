using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Proxy
{
    internal class NewAddressDialog : XtraForm
    {
        private IContainer components;

        private SimpleButton buttonCancel;

        private SimpleButton buttonOK;

        private TextEdit w_textBoxAddress;

        public string ServerAddress
        {
            get
		{
			return w_textBoxAddress.Text;
		}
            set
		{
			w_textBoxAddress.Text = value;
		}
        }

        public NewAddressDialog()
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
		this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_textBoxAddress = new DevExpress.XtraEditors.TextEdit();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxAddress.Properties).BeginInit();
		base.SuspendLayout();
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(202, 33);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 2;
		this.buttonCancel.Text = "Cancel";
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(121, 33);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 1;
		this.buttonOK.Text = "OK";
		this.w_textBoxAddress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_textBoxAddress.EditValue = "http://";
		this.w_textBoxAddress.Location = new System.Drawing.Point(12, 6);
		this.w_textBoxAddress.Name = "w_textBoxAddress";
		this.w_textBoxAddress.Size = new System.Drawing.Size(265, 20);
		this.w_textBoxAddress.TabIndex = 0;
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(289, 68);
		base.Controls.Add(this.w_textBoxAddress);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "NewAddressDialog";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "New Server Address";
		((System.ComponentModel.ISupportInitialize)this.w_textBoxAddress.Properties).EndInit();
		base.ResumeLayout(false);
	}
    }
}
