using Metalogix.Deployment;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Deployment
{
	public class AnotherProcessesDlg : Form
	{
		private IContainer components;

		private Button roundedButtonRetry;

		private Button roundedButtonClose;

		private Label labelDesc1;

		private LinkLabel linkLabelProxySettings;

		private Label label1;

		private PictureBox pictureBox;

		private ListView listViewProcesses;

		private ColumnHeader columnHeaderUser;

		private ColumnHeader columnHeaderProcess;

		private ColumnHeader columnHeaderId;

		public AnotherProcessInfo[] AnotherProcesses
		{
			set
			{
				this.listViewProcesses.Items.Clear();
				AnotherProcessInfo[] anotherProcessInfoArray = value;
				for (int i = 0; i < (int)anotherProcessInfoArray.Length; i++)
				{
					AnotherProcessInfo anotherProcessInfo = anotherProcessInfoArray[i];
					ListView.ListViewItemCollection items = this.listViewProcesses.Items;
					string[] str = new string[] { anotherProcessInfo.Id.ToString(), anotherProcessInfo.UserName, anotherProcessInfo.ProcessName };
					items.Add(new ListViewItem(str));
				}
			}
		}

		public AnotherProcessesDlg()
		{
			this.InitializeComponent();
			this.pictureBox.Image = Resources.Download;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AnotherProcessesDlg));
			this.roundedButtonRetry = new Button();
			this.roundedButtonClose = new Button();
			this.labelDesc1 = new Label();
			this.linkLabelProxySettings = new LinkLabel();
			this.label1 = new Label();
			this.pictureBox = new PictureBox();
			this.listViewProcesses = new ListView();
			this.columnHeaderId = new ColumnHeader();
			this.columnHeaderUser = new ColumnHeader();
			this.columnHeaderProcess = new ColumnHeader();
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			this.roundedButtonRetry.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonRetry.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.roundedButtonRetry.Location = new Point(311, 170);
			this.roundedButtonRetry.Name = "roundedButtonRetry";
			this.roundedButtonRetry.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonRetry.TabIndex = 4;
			this.roundedButtonRetry.Text = "Retry";
			this.roundedButtonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.roundedButtonClose.Location = new Point(392, 170);
			this.roundedButtonClose.Name = "roundedButtonClose";
			this.roundedButtonClose.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonClose.TabIndex = 5;
			this.roundedButtonClose.Text = "Close";
			this.labelDesc1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelDesc1.Location = new Point(46, 43);
			this.labelDesc1.Name = "labelDesc1";
			this.labelDesc1.Size = new System.Drawing.Size(421, 16);
			this.labelDesc1.TabIndex = 1;
			this.labelDesc1.Text = "Another instance of this Application is running.";
			this.labelDesc1.TextAlign = ContentAlignment.MiddleLeft;
			this.linkLabelProxySettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.linkLabelProxySettings.LinkArea = new LinkArea(0, 0);
			this.linkLabelProxySettings.Location = new Point(47, 63);
			this.linkLabelProxySettings.Name = "linkLabelProxySettings";
			this.linkLabelProxySettings.Size = new System.Drawing.Size(418, 16);
			this.linkLabelProxySettings.TabIndex = 2;
			this.linkLabelProxySettings.Text = "Please close all instances listed below: ";
			this.linkLabelProxySettings.TextAlign = ContentAlignment.MiddleLeft;
			this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.label1.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 238);
			this.label1.ForeColor = Color.DarkSlateGray;
			this.label1.Location = new Point(46, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(421, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "New version is available for download.";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.pictureBox.Image = (Image)componentResourceManager.GetObject("pictureBox.Image");
			this.pictureBox.Location = new Point(8, 8);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(32, 32);
			this.pictureBox.TabIndex = 13;
			this.pictureBox.TabStop = false;
			ListView.ColumnHeaderCollection columns = this.listViewProcesses.Columns;
			ColumnHeader[] columnHeaderArray = new ColumnHeader[] { this.columnHeaderId, this.columnHeaderUser, this.columnHeaderProcess };
			columns.AddRange(columnHeaderArray);
			this.listViewProcesses.FullRowSelect = true;
			this.listViewProcesses.Location = new Point(49, 82);
			this.listViewProcesses.Name = "listViewProcesses";
			this.listViewProcesses.Size = new System.Drawing.Size(418, 82);
			this.listViewProcesses.TabIndex = 3;
			this.listViewProcesses.UseCompatibleStateImageBehavior = false;
			this.listViewProcesses.View = View.Details;
			this.columnHeaderId.Text = "Process Id";
			this.columnHeaderId.Width = 90;
			this.columnHeaderUser.Text = "User Name";
			this.columnHeaderUser.Width = 100;
			this.columnHeaderProcess.Text = "Instance Name";
			this.columnHeaderProcess.Width = 200;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.ClientSize = new System.Drawing.Size(479, 199);
			base.Controls.Add(this.listViewProcesses);
			base.Controls.Add(this.linkLabelProxySettings);
			base.Controls.Add(this.pictureBox);
			base.Controls.Add(this.labelDesc1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.roundedButtonClose);
			base.Controls.Add(this.roundedButtonRetry);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AnotherProcessesDlg";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Download and Install Update";
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
		}
	}
}