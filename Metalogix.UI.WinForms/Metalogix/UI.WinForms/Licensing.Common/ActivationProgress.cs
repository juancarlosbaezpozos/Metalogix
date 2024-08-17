using Metalogix.UI.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	public class ActivationProgress : Form
	{
		private IContainer components;

		private Label lblMessage;

		private PictureBox pictureBox1;

		public string Status
		{
			set
			{
				this.lblMessage.Text = value;
			}
		}

		public ActivationProgress()
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ActivationProgress));
			this.lblMessage = new Label();
			this.pictureBox1 = new PictureBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.lblMessage.BackColor = Color.Transparent;
			this.lblMessage.Dock = DockStyle.Fill;
			this.lblMessage.Font = new System.Drawing.Font("Verdana", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblMessage.ForeColor = Color.White;
			this.lblMessage.Location = new Point(67, 0);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
			this.lblMessage.Size = new System.Drawing.Size(365, 88);
			this.lblMessage.TabIndex = 0;
			this.lblMessage.Text = "Connecting...";
			this.pictureBox1.BackColor = Color.Transparent;
			this.pictureBox1.Dock = DockStyle.Left;
			this.pictureBox1.Image = Resources.activation;
			this.pictureBox1.Location = new Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(67, 88);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(7f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = (Image)componentResourceManager.GetObject("$this.BackgroundImage");
			base.ClientSize = new System.Drawing.Size(432, 88);
			base.ControlBox = false;
			base.Controls.Add(this.lblMessage);
			base.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("Verdana", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ActivationProgress";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Activation Progress";
			base.TopMost = true;
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}