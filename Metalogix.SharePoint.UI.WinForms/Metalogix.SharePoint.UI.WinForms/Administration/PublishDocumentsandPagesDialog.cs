using Metalogix.SharePoint.Options.Administration;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public class PublishDocumentsandPagesDialog : Form
	{
		private string m_sCheckinComment = string.Empty;

		private string m_sPublishComment = string.Empty;

		private string m_sApprovalComment = string.Empty;

		private bool m_bCheckin;

		private bool m_bPublish;

		private bool m_bApprove;

		private PublishDocumentsAndPagesOptions m_options;

		private IContainer components;

		private CheckBox w_cbCheckin;

		private CheckBox w_cbPublish;

		private CheckBox w_cbApprove;

		private Button w_btnOk;

		private Button w_btnCancel;

		private TextBox w_tbCheckinComment;

		private TextBox w_tbPublishComment;

		private TextBox w_tbApprovalComment;

		private Label w_lblCheckinComment;

		private Label w_lblPublishComment;

		private Label w_lblApprovalComment;

		public bool ApprovalAvailable
		{
			set
			{
				if (value)
				{
					this.w_cbPublish.Enabled = false;
					this.w_tbPublishComment.Enabled = false;
					this.w_lblPublishComment.Enabled = false;
					return;
				}
				this.w_tbApprovalComment.Enabled = false;
				this.w_cbApprove.Enabled = false;
				this.w_lblApprovalComment.Enabled = false;
			}
		}

		public PublishDocumentsAndPagesOptions Options
		{
			get
			{
				if (this.m_options != null)
				{
					this.m_options.Approve = this.m_bApprove;
					this.m_options.ApproveComment = this.m_sApprovalComment;
					this.m_options.Checkin = this.m_bCheckin;
					this.m_options.CheckinComment = this.m_sCheckinComment;
					this.m_options.Publish = this.m_bPublish;
					this.m_options.PublishComment = this.m_sPublishComment;
				}
				return this.m_options;
			}
			set
			{
				this.m_options = value;
			}
		}

		public PublishDocumentsandPagesDialog()
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PublishDocumentsandPagesDialog));
			this.w_cbCheckin = new CheckBox();
			this.w_cbPublish = new CheckBox();
			this.w_cbApprove = new CheckBox();
			this.w_btnOk = new Button();
			this.w_btnCancel = new Button();
			this.w_tbCheckinComment = new TextBox();
			this.w_tbPublishComment = new TextBox();
			this.w_tbApprovalComment = new TextBox();
			this.w_lblCheckinComment = new Label();
			this.w_lblPublishComment = new Label();
			this.w_lblApprovalComment = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_cbCheckin, "w_cbCheckin");
			this.w_cbCheckin.Name = "w_cbCheckin";
			this.w_cbCheckin.UseVisualStyleBackColor = true;
			this.w_cbCheckin.CheckedChanged += new EventHandler(this.On_Checkin_Checked);
			componentResourceManager.ApplyResources(this.w_cbPublish, "w_cbPublish");
			this.w_cbPublish.Name = "w_cbPublish";
			this.w_cbPublish.UseVisualStyleBackColor = true;
			this.w_cbPublish.CheckedChanged += new EventHandler(this.On_Publish_Checked);
			componentResourceManager.ApplyResources(this.w_cbApprove, "w_cbApprove");
			this.w_cbApprove.Name = "w_cbApprove";
			this.w_cbApprove.UseVisualStyleBackColor = true;
			this.w_cbApprove.CheckedChanged += new EventHandler(this.On_Approve_Checked);
			componentResourceManager.ApplyResources(this.w_btnOk, "w_btnOk");
			this.w_btnOk.Name = "w_btnOk";
			this.w_btnOk.UseVisualStyleBackColor = true;
			this.w_btnOk.Click += new EventHandler(this.On_OK_Clicked);
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_tbCheckinComment, "w_tbCheckinComment");
			this.w_tbCheckinComment.Name = "w_tbCheckinComment";
			componentResourceManager.ApplyResources(this.w_tbPublishComment, "w_tbPublishComment");
			this.w_tbPublishComment.Name = "w_tbPublishComment";
			componentResourceManager.ApplyResources(this.w_tbApprovalComment, "w_tbApprovalComment");
			this.w_tbApprovalComment.Name = "w_tbApprovalComment";
			componentResourceManager.ApplyResources(this.w_lblCheckinComment, "w_lblCheckinComment");
			this.w_lblCheckinComment.Name = "w_lblCheckinComment";
			componentResourceManager.ApplyResources(this.w_lblPublishComment, "w_lblPublishComment");
			this.w_lblPublishComment.Name = "w_lblPublishComment";
			componentResourceManager.ApplyResources(this.w_lblApprovalComment, "w_lblApprovalComment");
			this.w_lblApprovalComment.Name = "w_lblApprovalComment";
			base.AcceptButton = this.w_btnOk;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.w_lblApprovalComment);
			base.Controls.Add(this.w_lblPublishComment);
			base.Controls.Add(this.w_lblCheckinComment);
			base.Controls.Add(this.w_tbApprovalComment);
			base.Controls.Add(this.w_tbPublishComment);
			base.Controls.Add(this.w_tbCheckinComment);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOk);
			base.Controls.Add(this.w_cbApprove);
			base.Controls.Add(this.w_cbPublish);
			base.Controls.Add(this.w_cbCheckin);
			this.ForeColor = SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PublishDocumentsandPagesDialog";
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_Approve_Checked(object sender, EventArgs e)
		{
			this.w_tbApprovalComment.Enabled = this.w_cbApprove.Checked;
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void On_Checkin_Checked(object sender, EventArgs e)
		{
			this.w_tbCheckinComment.Enabled = this.w_cbCheckin.Checked;
		}

		private void On_OK_Clicked(object sender, EventArgs e)
		{
			this.m_bApprove = this.w_cbApprove.Checked;
			this.m_bCheckin = this.w_cbCheckin.Checked;
			this.m_bPublish = this.w_cbPublish.Checked;
			if (this.m_bApprove)
			{
				this.m_sApprovalComment = this.w_tbApprovalComment.Text;
			}
			if (this.m_bCheckin)
			{
				this.m_sCheckinComment = this.w_tbCheckinComment.Text;
			}
			if (this.m_bPublish)
			{
				this.m_sPublishComment = this.w_tbPublishComment.Text;
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void On_Publish_Checked(object sender, EventArgs e)
		{
			this.w_tbPublishComment.Enabled = this.w_cbPublish.Checked;
		}
	}
}