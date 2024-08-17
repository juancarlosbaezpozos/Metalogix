using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Getting Started")]
    public class TCGettingStarted : AgentWizardTabbableControl
    {
        private IContainer components;

        private RichTextBox tbxGettingStarted;

        public TCGettingStarted()
	{
		InitializeComponent();
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCGettingStarted));
		this.tbxGettingStarted = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.tbxGettingStarted.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxGettingStarted.BackColor = System.Drawing.Color.White;
		this.tbxGettingStarted.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxGettingStarted.Cursor = System.Windows.Forms.Cursors.Default;
		this.tbxGettingStarted.Location = new System.Drawing.Point(10, 10);
		this.tbxGettingStarted.Name = "tbxGettingStarted";
		this.tbxGettingStarted.ReadOnly = true;
		this.tbxGettingStarted.Size = new System.Drawing.Size(500, 221);
		this.tbxGettingStarted.TabIndex = 0;
		this.tbxGettingStarted.Text = componentResourceManager.GetString("tbxGettingStarted.Text");
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tbxGettingStarted);
		base.Name = "TCGettingStarted";
		base.Size = new System.Drawing.Size(525, 256);
		base.Load += new System.EventHandler(TCGettingStarted_Load);
		base.ResumeLayout(false);
	}

        private void TCGettingStarted_Load(object sender, EventArgs e)
	{
		if (base.TopLevelControl is AgentWizard && !((AgentWizard)base.TopLevelControl).IsMetabaseTabSupported())
		{
			tbxGettingStarted.Text = tbxGettingStarted.Text.Replace("• Configure the Content Matrix Metabase to reside in SQL rather than local to this machine.\n", string.Empty);
		}
	}
    }
}
