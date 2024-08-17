using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Application Mappings")]
    public class TCApplicationMappings : AgentWizardTabbableControl
    {
        private bool _isUserSettingsCopied;

        private IContainer components;

        private SimpleButton btnCopySettings;

        private RichTextBox tbxMessage;

        public JobListFullControl JobListFullControl { get; set; }

        public TCApplicationMappings()
	{
		InitializeComponent();
		InitializeMessage();
	}

        private void btnCopySettings_Click(object sender, EventArgs e)
	{
		try
		{
			JobListFullControl.SaveAllSettingsToDb(JobsSettings.AdapterContext.ToInsecureString());
			_isUserSettingsCopied = true;
			FlatXtraMessageBox.Show("Settings copied successfully.", "Settings Copied", MessageBoxButtons.OK);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while saving settings in database.", exception, ErrorIcon.Error);
		}
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCApplicationMappings));
		this.btnCopySettings = new DevExpress.XtraEditors.SimpleButton();
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.btnCopySettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btnCopySettings.Location = new System.Drawing.Point(198, 241);
		this.btnCopySettings.Name = "btnCopySettings";
		this.btnCopySettings.Size = new System.Drawing.Size(151, 30);
		this.btnCopySettings.TabIndex = 1;
		this.btnCopySettings.Text = "Copy Settings";
		this.btnCopySettings.Click += new System.EventHandler(btnCopySettings_Click);
		this.tbxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(500, 202);
		this.tbxMessage.TabIndex = 3;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.btnCopySettings);
		base.Controls.Add(this.tbxMessage);
		base.Name = "TCApplicationMappings";
		base.Size = new System.Drawing.Size(525, 384);
		base.ResumeLayout(false);
	}

        private void InitializeMessage()
	{
		if (JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.OrdinalIgnoreCase))
		{
			tbxMessage.Text = tbxMessage.Text.Replace("the local machine over to the Agent Database", "current Agent Database to the new Agent Database");
			tbxMessage.Text = tbxMessage.Text.Replace("locally that should be used for all Agents", "that would be desired for usage in the new Agent Database");
			tbxMessage.Text = tbxMessage.Text.Replace("the local machine (this machine) to the Agent Database", "current Agent Database to the new Agent Database");
		}
	}

        public override bool ValidatePage()
	{
		if (!_isUserSettingsCopied)
		{
			try
			{
				if (base.ParentForm is AgentWizard parentForm && parentForm.IsMetabaseTabSupported())
				{
					List<string> strs = new List<string> { "DefaultMetabaseAdapter", "AutoProvisionNewMetabaseFile", "FileMetabaseContext", "SQLMetabaseContext" };
					JobListFullControl.SaveValuesToDb(ResourceScope.ApplicationAndUserSpecific, JobsSettings.AdapterContext.ToInsecureString(), strs);
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while saving metabase settings in database.", exception, ErrorIcon.Error);
			}
		}
		return true;
	}
    }
}
