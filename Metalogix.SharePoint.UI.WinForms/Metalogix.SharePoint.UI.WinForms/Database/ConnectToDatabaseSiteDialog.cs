using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class ConnectToDatabaseSiteDialog : Form
	{
		private IContainer components;

		private Label label2;

		private Label label1;

		private Button w_btnOK;

		private TextBox w_txtServerRelativeUrl;

		private Button w_btnCancel;

		public string ServerRelativeUrl
		{
			get
			{
				return this.w_txtServerRelativeUrl.Text;
			}
			set
			{
				this.w_txtServerRelativeUrl.Text = value;
			}
		}

		public ConnectToDatabaseSiteDialog()
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

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ConnectToDatabaseSiteDialog));
			this.label2 = new Label();
			this.label1 = new Label();
			this.w_btnOK = new Button();
			this.w_txtServerRelativeUrl = new TextBox();
			this.w_btnCancel = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			componentResourceManager.ApplyResources(this.w_txtServerRelativeUrl, "w_txtServerRelativeUrl");
			this.w_txtServerRelativeUrl.Name = "w_txtServerRelativeUrl";
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			base.AcceptButton = this.w_btnOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_txtServerRelativeUrl);
			base.Controls.Add(this.w_btnCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ConnectToDatabaseSiteDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}