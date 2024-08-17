using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MappingOptions.png")]
	[ControlName("Mapping Options")]
	public class TCNavigationMappingOptions : ScopableTabbableControl
	{
		private TransformationTaskCollection m_ttcTasks;

		private SPMappingOptions m_options;

		private IContainer components;

		private CheckEdit w_cbSitesAndListsRenamed;

		private SimpleButton w_bEditNameMappings;

		private SimpleButton w_bImportSettings;

		internal CheckEdit w_cbMapAudiences;

		public SPMappingOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public TCNavigationMappingOptions()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCNavigationMappingOptions));
			this.w_cbSitesAndListsRenamed = new DevExpress.XtraEditors.CheckEdit();
			this.w_bEditNameMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_bImportSettings = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbMapAudiences = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSitesAndListsRenamed.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapAudiences.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbSitesAndListsRenamed, "w_cbSitesAndListsRenamed");
			this.w_cbSitesAndListsRenamed.Name = "w_cbSitesAndListsRenamed";
			this.w_cbSitesAndListsRenamed.Properties.AutoWidth = true;
			this.w_cbSitesAndListsRenamed.Properties.Caption = resources.GetString("w_cbSitesAndListsRenamed.Properties.Caption");
			this.w_cbSitesAndListsRenamed.CheckedChanged += new System.EventHandler(w_cbSitesAndListsRenamed_CheckedChanged);
			resources.ApplyResources(this.w_bEditNameMappings, "w_bEditNameMappings");
			this.w_bEditNameMappings.Name = "w_bEditNameMappings";
			this.w_bEditNameMappings.Click += new System.EventHandler(w_bEditNameMappings_Click);
			resources.ApplyResources(this.w_bImportSettings, "w_bImportSettings");
			this.w_bImportSettings.Name = "w_bImportSettings";
			this.w_bImportSettings.Click += new System.EventHandler(w_bImportSettings_Click);
			resources.ApplyResources(this.w_cbMapAudiences, "w_cbMapAudiences");
			this.w_cbMapAudiences.Name = "w_cbMapAudiences";
			this.w_cbMapAudiences.Properties.AutoWidth = true;
			this.w_cbMapAudiences.Properties.Caption = resources.GetString("w_cbMapAudiences.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_cbMapAudiences);
			base.Controls.Add(this.w_bImportSettings);
			base.Controls.Add(this.w_bEditNameMappings);
			base.Controls.Add(this.w_cbSitesAndListsRenamed);
			base.Name = "TCNavigationMappingOptions";
			((System.ComponentModel.ISupportInitialize)this.w_cbSitesAndListsRenamed.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapAudiences.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			m_ttcTasks = (TransformationTaskCollection)Options.TaskCollection.Clone();
			w_cbSitesAndListsRenamed.Checked = m_options.RenameSpecificNodes;
			w_cbMapAudiences.Checked = m_options.MapAudiences;
		}

		public override bool SaveUI()
		{
			Options.MapAudiences = w_cbMapAudiences.Checked;
			Options.RenameSpecificNodes = w_cbSitesAndListsRenamed.Checked;
			Options.TaskCollection = m_ttcTasks;
			return true;
		}

		protected override void UpdateEnabledState()
		{
			w_bEditNameMappings.Enabled = w_cbSitesAndListsRenamed.Checked && w_cbSitesAndListsRenamed.Enabled;
		}

		private void w_bEditNameMappings_Click(object sender, EventArgs e)
		{
			PropertyTransformationDialog propertyTransformationDialog = new PropertyTransformationDialog(SourceNodes, "Specify new names of Sites, Lists, and Folders");
			propertyTransformationDialog.Tasks = m_ttcTasks.Clone() as TransformationTaskCollection;
			PropertyTransformationDialog propertyTransformationDialog2 = propertyTransformationDialog;
			if (propertyTransformationDialog2.ShowDialog() == DialogResult.OK)
			{
				m_ttcTasks = propertyTransformationDialog2.Tasks;
			}
		}

		private void w_bImportSettings_Click(object sender, EventArgs e)
		{
		}

		private void w_cbSitesAndListsRenamed_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
