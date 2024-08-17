using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	public class ServiceWindow : Form
	{
		private IContainer components;

		private Button Stop;

		public ServiceWindow()
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
			this.Stop = new Button();
			base.SuspendLayout();
			this.Stop.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Stop.Location = new Point(64, 23);
			this.Stop.Name = "Stop";
			this.Stop.Size = new System.Drawing.Size(75, 23);
			this.Stop.TabIndex = 0;
			this.Stop.Text = "Stop";
			this.Stop.UseVisualStyleBackColor = true;
			this.Stop.Click += new EventHandler(this.On_Click);
			base.AcceptButton = this.Stop;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(211, 79);
			base.Controls.Add(this.Stop);
			base.Name = "ServiceWindow";
			this.Text = "ServiceWindow";
			base.ResumeLayout(false);
		}

		private void On_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}