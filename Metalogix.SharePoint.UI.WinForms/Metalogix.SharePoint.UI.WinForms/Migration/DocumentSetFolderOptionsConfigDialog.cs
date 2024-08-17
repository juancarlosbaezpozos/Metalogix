using Metalogix.SharePoint;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class DocumentSetFolderOptionsConfigDialog : Form
	{
		private IEnumerable<SPContentType> m_contentTypes;

		private string m_sSelectedCT;

		private IContainer components;

		private Label w_lbCaption;

		private ComboBox w_cbContentTypes;

		private Button w_bCancel;

		private Button w_bOkay;

		private Label w_lbContentType;

		public string SelectedCT
		{
			get
			{
				return this.m_sSelectedCT;
			}
			set
			{
				this.m_sSelectedCT = value;
				this.LoadUI();
			}
		}

		public DocumentSetFolderOptionsConfigDialog(IEnumerable<SPContentType> targetContentTypes)
		{
			this.InitializeComponent();
			this.m_contentTypes = targetContentTypes;
			int num = 0;
			foreach (SPContentType targetContentType in targetContentTypes)
			{
				this.w_cbContentTypes.Items.Add(targetContentType.Name);
				num++;
			}
			if (num != 0)
			{
				this.w_cbContentTypes.SelectedIndex = 0;
				return;
			}
			FlatXtraMessageBox.Show("There are no Document Set content types on the target site.  You must create atleast 1 Document Set content type to continue.", "No Document Set Content Types", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DocumentSetFolderOptionsConfigDialog));
			this.w_lbCaption = new Label();
			this.w_cbContentTypes = new ComboBox();
			this.w_bCancel = new Button();
			this.w_bOkay = new Button();
			this.w_lbContentType = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_lbCaption, "w_lbCaption");
			this.w_lbCaption.Name = "w_lbCaption";
			this.w_cbContentTypes.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_cbContentTypes.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.w_cbContentTypes, "w_cbContentTypes");
			this.w_cbContentTypes.Name = "w_cbContentTypes";
			componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bCancel.Click += new EventHandler(this.w_bCancel_Click);
			componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bOkay.Click += new EventHandler(this.w_bOkay_Click);
			componentResourceManager.ApplyResources(this.w_lbContentType, "w_lbContentType");
			this.w_lbContentType.Name = "w_lbContentType";
			base.AcceptButton = this.w_bOkay;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_lbContentType);
			base.Controls.Add(this.w_bCancel);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_cbContentTypes);
			base.Controls.Add(this.w_lbCaption);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DocumentSetFolderOptionsConfigDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			if (this.m_sSelectedCT != null)
			{
				for (int i = 0; i < this.w_cbContentTypes.Items.Count; i++)
				{
					if (this.w_cbContentTypes.Items[i].ToString().Equals(this.m_sSelectedCT))
					{
						this.w_cbContentTypes.SelectedIndex = i;
					}
				}
			}
		}

		private bool SaveUI()
		{
			this.m_sSelectedCT = this.w_cbContentTypes.SelectedItem.ToString();
			return true;
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			if (this.SaveUI())
			{
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				base.Close();
			}
		}
	}
}