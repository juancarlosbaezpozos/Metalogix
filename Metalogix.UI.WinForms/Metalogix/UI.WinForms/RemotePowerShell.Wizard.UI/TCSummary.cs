using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Actions.Remoting;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Summary")]
    public class TCSummary : AgentWizardTabbableControl
    {
        private const string NotAvailable = "NA";

        private IContainer components;

        private LabelControl lblSummary;

        private LabelControl lblAgents;

        private XtraParentSelectableGrid gvAgentsSummary;

        private GridView grdAgentSummary;

        private LabelControl lblAgentDatabase;

        private LabelControl lblMetabaseDatabase;

        private LabelControl lblCertificate;

        private LabelControl lblAgentDbConnectionString;

        private LabelControl lblCertificatePath;

        private LabelControl lblMetabaseDbConnectionString;

        public TCSummary()
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

        private DataTable GetDataTable(List<Agent> agents)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add(Constants.MachineName);
		dataTable.Columns.Add(Constants.UserName);
		dataTable.Columns.Add(Constants.Status);
		foreach (Agent agent in agents)
		{
			DataRowCollection rows = dataTable.Rows;
			object[] machineName = new object[3] { agent.MachineName, agent.UserName, agent.Status };
			rows.Add(machineName);
		}
		return dataTable;
	}

        private void grdAgentSummary_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
	{
		Font font = new Font("Tahoma", float.Parse("8.25"), FontStyle.Regular);
		Rectangle bounds = e.Bounds;
		Rectangle rectangle = e.Bounds;
		Rectangle bounds1 = e.Bounds;
		Rectangle rectangle1 = e.Bounds;
		Rectangle rectangle2 = new Rectangle(bounds.Left + 5, rectangle.Top + 5, bounds1.Width - 5, rectangle1.Height - 5);
		e.Graphics.DrawString("No agent found.", font, Brushes.Black, rectangle2);
	}

        private void InitializeComponent()
	{
		this.lblSummary = new DevExpress.XtraEditors.LabelControl();
		this.lblAgents = new DevExpress.XtraEditors.LabelControl();
		this.gvAgentsSummary = new Metalogix.UI.WinForms.Components.XtraParentSelectableGrid();
		this.grdAgentSummary = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.lblAgentDatabase = new DevExpress.XtraEditors.LabelControl();
		this.lblMetabaseDatabase = new DevExpress.XtraEditors.LabelControl();
		this.lblCertificate = new DevExpress.XtraEditors.LabelControl();
		this.lblAgentDbConnectionString = new DevExpress.XtraEditors.LabelControl();
		this.lblCertificatePath = new DevExpress.XtraEditors.LabelControl();
		this.lblMetabaseDbConnectionString = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.gvAgentsSummary).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdAgentSummary).BeginInit();
		base.SuspendLayout();
		this.lblSummary.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblSummary.Location = new System.Drawing.Point(10, 10);
		this.lblSummary.Name = "lblSummary";
		this.lblSummary.Size = new System.Drawing.Size(55, 13);
		this.lblSummary.TabIndex = 0;
		this.lblSummary.Text = "Summary";
		this.lblAgents.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblAgents.Location = new System.Drawing.Point(10, 183);
		this.lblAgents.Name = "lblAgents";
		this.lblAgents.Size = new System.Drawing.Size(43, 13);
		this.lblAgents.TabIndex = 4;
		this.lblAgents.Text = "Agents:";
		this.gvAgentsSummary.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gvAgentsSummary.Location = new System.Drawing.Point(10, 202);
		this.gvAgentsSummary.MainView = this.grdAgentSummary;
		this.gvAgentsSummary.Name = "gvAgentsSummary";
		this.gvAgentsSummary.Size = new System.Drawing.Size(496, 294);
		this.gvAgentsSummary.TabIndex = 0;
		this.gvAgentsSummary.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdAgentSummary });
		this.grdAgentSummary.GridControl = this.gvAgentsSummary;
		this.grdAgentSummary.Name = "grdAgentSummary";
		this.grdAgentSummary.OptionsBehavior.Editable = false;
		this.grdAgentSummary.OptionsCustomization.AllowColumnMoving = false;
		this.grdAgentSummary.OptionsCustomization.AllowFilter = false;
		this.grdAgentSummary.OptionsMenu.EnableColumnMenu = false;
		this.grdAgentSummary.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.grdAgentSummary.OptionsView.ShowGroupPanel = false;
		this.grdAgentSummary.CustomDrawEmptyForeground += new DevExpress.XtraGrid.Views.Base.CustomDrawEventHandler(grdAgentSummary_CustomDrawEmptyForeground);
		this.lblAgentDatabase.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblAgentDatabase.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblAgentDatabase.Location = new System.Drawing.Point(10, 29);
		this.lblAgentDatabase.Name = "lblAgentDatabase";
		this.lblAgentDatabase.Size = new System.Drawing.Size(82, 13);
		this.lblAgentDatabase.TabIndex = 5;
		this.lblAgentDatabase.Text = "Agent Database:";
		this.lblMetabaseDatabase.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblMetabaseDatabase.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblMetabaseDatabase.Location = new System.Drawing.Point(10, 76);
		this.lblMetabaseDatabase.Name = "lblMetabaseDatabase";
		this.lblMetabaseDatabase.Size = new System.Drawing.Size(100, 13);
		this.lblMetabaseDatabase.TabIndex = 6;
		this.lblMetabaseDatabase.Text = "Metabase Database:";
		this.lblCertificate.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblCertificate.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblCertificate.Location = new System.Drawing.Point(10, 123);
		this.lblCertificate.Name = "lblCertificate";
		this.lblCertificate.Size = new System.Drawing.Size(54, 13);
		this.lblCertificate.TabIndex = 7;
		this.lblCertificate.Text = "Certificate:";
		this.lblAgentDbConnectionString.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblAgentDbConnectionString.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.lblAgentDbConnectionString.Location = new System.Drawing.Point(10, 44);
		this.lblAgentDbConnectionString.Name = "lblAgentDbConnectionString";
		this.lblAgentDbConnectionString.Size = new System.Drawing.Size(500, 0);
		this.lblAgentDbConnectionString.TabIndex = 8;
		this.lblCertificatePath.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblCertificatePath.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.lblCertificatePath.Location = new System.Drawing.Point(10, 139);
		this.lblCertificatePath.Name = "lblCertificatePath";
		this.lblCertificatePath.Size = new System.Drawing.Size(500, 0);
		this.lblCertificatePath.TabIndex = 10;
		this.lblMetabaseDbConnectionString.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblMetabaseDbConnectionString.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.lblMetabaseDbConnectionString.Location = new System.Drawing.Point(10, 90);
		this.lblMetabaseDbConnectionString.Name = "lblMetabaseDbConnectionString";
		this.lblMetabaseDbConnectionString.Size = new System.Drawing.Size(500, 0);
		this.lblMetabaseDbConnectionString.TabIndex = 9;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lblCertificatePath);
		base.Controls.Add(this.lblMetabaseDbConnectionString);
		base.Controls.Add(this.lblAgentDbConnectionString);
		base.Controls.Add(this.lblCertificate);
		base.Controls.Add(this.lblMetabaseDatabase);
		base.Controls.Add(this.lblAgentDatabase);
		base.Controls.Add(this.gvAgentsSummary);
		base.Controls.Add(this.lblAgents);
		base.Controls.Add(this.lblSummary);
		base.Name = "TCSummary";
		base.Size = new System.Drawing.Size(525, 517);
		((System.ComponentModel.ISupportInitialize)this.gvAgentsSummary).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdAgentSummary).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void LoadGridView()
	{
		grdAgentSummary.ClearSelection();
		gvAgentsSummary.DataSource = null;
		if (AgentWizardTabbableControl.Agents == null)
		{
			return;
		}
		List<Agent> list = AgentWizardTabbableControl.Agents.GetList();
		if (list != null && list.Count > 0)
		{
			DataTable dataTable = GetDataTable(list);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gvAgentsSummary.DataSource = dataTable;
			}
		}
	}

        private void LoadSummary()
	{
		try
		{
			lblAgentDbConnectionString.Text = JobUtils.MaskAdapterContext(AgentWizardTabbableControl.AgentDetails.AgentDBConnectionString);
			lblMetabaseDbConnectionString.Text = (string.IsNullOrEmpty(AgentWizardTabbableControl.AgentDetails.MetabaseDBConnectionString) ? "NA" : JobUtils.MaskAdapterContext(AgentWizardTabbableControl.AgentDetails.MetabaseDBConnectionString));
			lblCertificatePath.Text = (string.IsNullOrEmpty(AgentWizardTabbableControl.AgentDetails.CertificateLocation) ? "NA" : AgentWizardTabbableControl.AgentDetails.CertificateLocation);
			LoadGridView();
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while loading wizard summary", exception, ErrorIcon.Error);
		}
	}

        public override bool LoadUI()
	{
		LoadSummary();
		return true;
	}
    }
}
