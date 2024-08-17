using Metalogix.Actions;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class RunSaveCancelForm : Form
	{
		private Metalogix.Actions.ConfigurationResult m_result = Metalogix.Actions.ConfigurationResult.Cancel;

		private IContainer components;

		protected Button w_btnCancel;

		protected Button w_btnSave;

		protected Button w_btnRun;

		private Label w_lblMessage;

		public Metalogix.Actions.ConfigurationResult ConfigurationResult
		{
			get
			{
				return this.m_result;
			}
			set
			{
				this.m_result = value;
			}
		}

		public RunSaveCancelForm(string sMessage, string sTitle, Image iconImage)
		{
			this.InitializeComponent();
			this.Text = sTitle;
			this.w_lblMessage.Text = sMessage;
			Bitmap bitmap = iconImage as Bitmap;
			if (bitmap == null)
			{
				return;
			}
			System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
			base.Icon = icon;
			base.ShowIcon = icon != null;
			int width = base.Width - this.w_lblMessage.Width + this.w_lblMessage.PreferredWidth;
			int height = base.Height - this.w_lblMessage.Height + this.w_lblMessage.PreferredHeight;
			System.Drawing.Size size = new System.Drawing.Size(width, height);
			base.Size = size;
			this.MinimumSize = size;
			this.MaximumSize = size;
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
			this.w_btnCancel = new Button();
			this.w_btnSave = new Button();
			this.w_btnRun = new Button();
			this.w_lblMessage = new Label();
			base.SuspendLayout();
			this.w_btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnCancel.Location = new Point(315, 60);
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.w_btnCancel.TabIndex = 20;
			this.w_btnCancel.Text = "&Cancel";
			this.w_btnCancel.Click += new EventHandler(this.On_Cancel_Clicked);
			this.w_btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnSave.Location = new Point(225, 60);
			this.w_btnSave.Name = "w_btnSave";
			this.w_btnSave.Size = new System.Drawing.Size(75, 23);
			this.w_btnSave.TabIndex = 19;
			this.w_btnSave.Text = "&Save";
			this.w_btnSave.Click += new EventHandler(this.On_Save_Clicked);
			this.w_btnRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnRun.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnRun.Location = new Point(135, 60);
			this.w_btnRun.Name = "w_btnRun";
			this.w_btnRun.Size = new System.Drawing.Size(75, 23);
			this.w_btnRun.TabIndex = 18;
			this.w_btnRun.Text = "&Run";
			this.w_btnRun.Click += new EventHandler(this.On_Run_Clicked);
			this.w_lblMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_lblMessage.Location = new Point(26, 12);
			this.w_lblMessage.Name = "w_lblMessage";
			this.w_lblMessage.Size = new System.Drawing.Size(364, 30);
			this.w_lblMessage.TabIndex = 21;
			this.w_lblMessage.Text = "Message";
			base.AcceptButton = this.w_btnRun;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(403, 92);
			base.Controls.Add(this.w_lblMessage);
			base.Controls.Add(this.w_btnRun);
			base.Controls.Add(this.w_btnSave);
			base.Controls.Add(this.w_btnCancel);
			base.Name = "RunSaveCancelForm";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "RunSaveCancelForm";
			base.Shown += new EventHandler(this.On_Shown);
			base.ResumeLayout(false);
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			this.m_result = Metalogix.Actions.ConfigurationResult.Cancel;
		}

		private void On_Run_Clicked(object sender, EventArgs e)
		{
			this.m_result = Metalogix.Actions.ConfigurationResult.Run;
		}

		private void On_Save_Clicked(object sender, EventArgs e)
		{
			this.m_result = Metalogix.Actions.ConfigurationResult.Save;
		}

		private void On_Shown(object sender, EventArgs e)
		{
			this.w_btnRun.Focus();
		}

		public static Metalogix.Actions.ConfigurationResult ShowDialog(string sMessage, string sTitle, Image iconImage)
		{
			RunSaveCancelForm runSaveCancelForm = new RunSaveCancelForm(sMessage, sTitle, iconImage);
			runSaveCancelForm.ShowDialog();
			return runSaveCancelForm.m_result;
		}
	}
}