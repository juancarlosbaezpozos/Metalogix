using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public class FileAlreadyExistsDialog : Form
	{
		private string m_sFilename = "";

		private bool m_bApplyToAllItems;

		private IContainer components;

		private Button w_bCancel;

		private Button w_bSkip;

		private Button w_bOverwrite;

		private PictureBox w_pbIcon;

		private CheckBox w_cbApplyToAll;

		private Label w_lDescription;

		public bool ApplyToAllItems
		{
			get
			{
				return this.m_bApplyToAllItems;
			}
			set
			{
				this.m_bApplyToAllItems = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.m_sFilename;
			}
			set
			{
				this.m_sFilename = value;
			}
		}

		public FileAlreadyExistsDialog()
		{
			this.InitializeComponent();
			this.w_pbIcon.Image = this.GetBitmap(SystemIcons.Warning);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FileAlreadyExistsDialog_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.FileName))
			{
				int height = this.w_lDescription.Height;
				string str = string.Format("The file \"{0}\" already exists.\nWould you like to overwrite it?", this.FileName);
				this.w_lDescription.Text = str;
				int num = this.w_lDescription.Height - height;
				FileAlreadyExistsDialog fileAlreadyExistsDialog = this;
				fileAlreadyExistsDialog.Height = fileAlreadyExistsDialog.Height + num;
			}
		}

		public Bitmap GetBitmap(System.Drawing.Icon icon)
		{
			Bitmap bitmap = new Bitmap(icon.Width, icon.Height);
			Graphics graphic = Graphics.FromImage(bitmap);
			graphic.DrawIcon(icon, 0, 0);
			graphic.Dispose();
			return bitmap;
		}

		private void InitializeComponent()
		{
			this.w_bCancel = new Button();
			this.w_bSkip = new Button();
			this.w_bOverwrite = new Button();
			this.w_pbIcon = new PictureBox();
			this.w_cbApplyToAll = new CheckBox();
			this.w_lDescription = new Label();
			((ISupportInitialize)this.w_pbIcon).BeginInit();
			base.SuspendLayout();
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Location = new Point(417, 81);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 4;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bSkip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bSkip.DialogResult = System.Windows.Forms.DialogResult.No;
			this.w_bSkip.Location = new Point(336, 81);
			this.w_bSkip.Name = "w_bSkip";
			this.w_bSkip.Size = new System.Drawing.Size(75, 23);
			this.w_bSkip.TabIndex = 3;
			this.w_bSkip.Text = "Skip";
			this.w_bSkip.UseVisualStyleBackColor = true;
			this.w_bSkip.Click += new EventHandler(this.w_bSkip_Click);
			this.w_bOverwrite.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOverwrite.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.w_bOverwrite.Location = new Point(255, 81);
			this.w_bOverwrite.Name = "w_bOverwrite";
			this.w_bOverwrite.Size = new System.Drawing.Size(75, 23);
			this.w_bOverwrite.TabIndex = 2;
			this.w_bOverwrite.Text = "Overwrite";
			this.w_bOverwrite.UseVisualStyleBackColor = true;
			this.w_bOverwrite.Click += new EventHandler(this.w_bOverwrite_Click);
			this.w_pbIcon.Location = new Point(32, 32);
			this.w_pbIcon.Name = "w_pbIcon";
			this.w_pbIcon.Size = new System.Drawing.Size(32, 32);
			this.w_pbIcon.TabIndex = 3;
			this.w_pbIcon.TabStop = false;
			this.w_cbApplyToAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.w_cbApplyToAll.AutoSize = true;
			this.w_cbApplyToAll.Location = new Point(32, 85);
			this.w_cbApplyToAll.Name = "w_cbApplyToAll";
			this.w_cbApplyToAll.Size = new System.Drawing.Size(190, 17);
			this.w_cbApplyToAll.TabIndex = 1;
			this.w_cbApplyToAll.Text = "Do this for all files that already exist";
			this.w_cbApplyToAll.UseVisualStyleBackColor = true;
			this.w_lDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			this.w_lDescription.AutoSize = true;
			this.w_lDescription.Location = new Point(77, 32);
			this.w_lDescription.MaximumSize = new System.Drawing.Size(391, 0);
			this.w_lDescription.Name = "w_lDescription";
			this.w_lDescription.Size = new System.Drawing.Size(149, 26);
			this.w_lDescription.TabIndex = 0;
			this.w_lDescription.Text = "The file already exists. \r\nWould you like to overwrite it?";
			base.AcceptButton = this.w_bOverwrite;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(504, 116);
			base.Controls.Add(this.w_lDescription);
			base.Controls.Add(this.w_cbApplyToAll);
			base.Controls.Add(this.w_pbIcon);
			base.Controls.Add(this.w_bOverwrite);
			base.Controls.Add(this.w_bSkip);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FileAlreadyExistsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "File Already Exists";
			base.Load += new EventHandler(this.FileAlreadyExistsDialog_Load);
			((ISupportInitialize)this.w_pbIcon).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void w_bOverwrite_Click(object sender, EventArgs e)
		{
			this.ApplyToAllItems = this.w_cbApplyToAll.Checked;
		}

		private void w_bSkip_Click(object sender, EventArgs e)
		{
			this.ApplyToAllItems = this.w_cbApplyToAll.Checked;
		}
	}
}