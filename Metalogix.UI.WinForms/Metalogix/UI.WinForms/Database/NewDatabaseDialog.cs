using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Database
{
    public class NewDatabaseDialog : XtraForm
    {
        private IContainer components;

        private SimpleButton buttonCancel;

        private SimpleButton buttonOK;

        private TextEdit w_textBoxName;

        private LabelControl label1;

        public string DatabaseName
        {
            get
		{
			return w_textBoxName.Text;
		}
            set
		{
			w_textBoxName.Text = value;
		}
        }

        public NewDatabaseDialog()
	{
		InitializeComponent();
	}

        private void buttonOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(w_textBoxName.Text))
			{
				throw new Exception("Name cannot be empty.");
			}
			if (Regex.Matches(w_textBoxName.Text, "[0-9a-zA-Z$_]+").Count != 1)
			{
				throw new Exception("Invalid name specified.  Please provide a valid database name and try again.");
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show(exception.Message, "Validation Failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			base.DialogResult = DialogResult.None;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Database.NewDatabaseDialog));
		this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_textBoxName = new DevExpress.XtraEditors.TextEdit();
		this.label1 = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxName.Properties).BeginInit();
		base.SuspendLayout();
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(250, 40);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 2;
		this.buttonCancel.Text = "Cancel";
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(169, 40);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 1;
		this.buttonOK.Text = "OK";
		this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
		this.w_textBoxName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_textBoxName.Location = new System.Drawing.Point(56, 12);
		this.w_textBoxName.Name = "w_textBoxName";
		this.w_textBoxName.Size = new System.Drawing.Size(269, 20);
		this.w_textBoxName.TabIndex = 0;
		this.label1.Location = new System.Drawing.Point(12, 15);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(31, 13);
		this.label1.TabIndex = 5;
		this.label1.Text = "Name:";
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(337, 75);
		base.Controls.Add(this.w_textBoxName);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "NewDatabaseDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Create New Database";
		((System.ComponentModel.ISupportInitialize)this.w_textBoxName.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
    }
}
