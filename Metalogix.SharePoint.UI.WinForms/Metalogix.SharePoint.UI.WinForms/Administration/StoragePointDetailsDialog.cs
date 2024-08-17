using Metalogix.SharePoint;
using Metalogix.SharePoint.StoragePoint;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public class StoragePointDetailsDialog : Form
	{
		private SPListItem m_item;

		private IContainer components;

		private TextBox w_txtNodeAddress;

		private Label w_lblAddress;

		private TextBox w_txtFolder;

		private Label w_lblFolder;

		private TextBox w_txtFileName;

		private Label w_lblFilename;

		private Button w_btnClose;

		public SPListItem Item
		{
			get
			{
				return this.m_item;
			}
			set
			{
				this.m_item = value;
				this.UpdateUI();
			}
		}

		public StoragePointDetailsDialog()
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(StoragePointDetailsDialog));
			this.w_txtNodeAddress = new TextBox();
			this.w_lblAddress = new Label();
			this.w_txtFolder = new TextBox();
			this.w_lblFolder = new Label();
			this.w_txtFileName = new TextBox();
			this.w_lblFilename = new Label();
			this.w_btnClose = new Button();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_txtNodeAddress, "w_txtNodeAddress");
			this.w_txtNodeAddress.Name = "w_txtNodeAddress";
			this.w_txtNodeAddress.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_lblAddress, "w_lblAddress");
			this.w_lblAddress.Name = "w_lblAddress";
			componentResourceManager.ApplyResources(this.w_txtFolder, "w_txtFolder");
			this.w_txtFolder.Name = "w_txtFolder";
			this.w_txtFolder.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_lblFolder, "w_lblFolder");
			this.w_lblFolder.Name = "w_lblFolder";
			componentResourceManager.ApplyResources(this.w_txtFileName, "w_txtFileName");
			this.w_txtFileName.Name = "w_txtFileName";
			this.w_txtFileName.ReadOnly = true;
			componentResourceManager.ApplyResources(this.w_lblFilename, "w_lblFilename");
			this.w_lblFilename.Name = "w_lblFilename";
			componentResourceManager.ApplyResources(this.w_btnClose, "w_btnClose");
			this.w_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnClose.Name = "w_btnClose";
			base.AcceptButton = this.w_btnClose;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			base.CancelButton = this.w_btnClose;
			componentResourceManager.ApplyResources(this, "$this");
			base.Controls.Add(this.w_btnClose);
			base.Controls.Add(this.w_txtFileName);
			base.Controls.Add(this.w_lblFilename);
			base.Controls.Add(this.w_txtFolder);
			base.Controls.Add(this.w_lblFolder);
			base.Controls.Add(this.w_txtNodeAddress);
			base.Controls.Add(this.w_lblAddress);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "StoragePointDetailsDialog";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void ParseBlobRef(byte[] blobRef, out string sFileName, out string sFolderPath)
		{
			if (blobRef == null)
			{
				sFileName = "";
				sFolderPath = "";
				return;
			}
			BlobReference blobReference = new BlobReference(blobRef);
			sFileName = blobReference.EndPointReference.FileName;
			sFolderPath = blobReference.EndPointReference.Folder;
		}

		private void UpdateUI()
		{
			base.SuspendLayout();
			this.w_txtNodeAddress.Text = this.Item.DisplayUrl;
			string str = null;
			string str1 = null;
			this.ParseBlobRef(this.Item.GetBlobRef(), out str, out str1);
			this.w_txtFileName.Text = str;
			this.w_txtFolder.Text = str1;
			base.ResumeLayout();
		}
	}
}