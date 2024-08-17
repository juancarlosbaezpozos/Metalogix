using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Components.AnchoredControls;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MappingOptions.png")]
	[ControlName("Mapping Options")]
	public class TCMappingOptions : MigrationElementsScopabbleTabbableControl
	{
		private const string RENAMESITELEVEL = "Rename Specific Sites. Lists and Folders";

		private const string RENAMELISTLEVEL = "Rename Specific Lists and Folders";

		private const string RENAMEFOLDERLEVEL = "Rename Specific Folders";

		private SPMappingOptions m_options;

		private SPWebTemplateCollection m_sourceWebTemplates;

		private SPWebTemplateCollection m_targetWebTemplates;

		private bool m_bChildSitesAvailable;

		private TransformationTaskCollection m_ttcTasks;

		private ColumnMappings m_columnMappings;

		private bool m_bSuspendRenameUpdateEvents;

		private IContainer components;

		private SimpleButton w_btnEditRenameAll;

		private CheckEdit w_cbChangeProperties;

		private SimpleButton w_btnEditTemplateMappings;

		private CheckEdit w_cbMapWebTemplates;

		private PanelControl w_plSite;

		private PanelControl w_plAboveItemScope;

		private PanelControl w_plBelowListScope;

		private CheckEdit w_cbMapColumns;

		private SimpleButton w_btnEditColumnMappings;

		private SimpleButton w_btnEditGlobalMappings;

		private PanelControl w_plWebServiceOnly;

		private CheckEdit w_cbAutomapUserMetadata;

		internal CheckEdit w_cbOverwriteGroups;

		internal CheckEdit w_cbMapGroupsByName;

		private PanelControl w_plMissingUsers;

		private AnchoredTextBox w_tbMissingUsers;

		private CheckEdit w_cbMissingUsers;

		internal CheckEdit w_cbMapAudiences;

		internal CheckEdit w_cbAllowDBWrite;

		private HelpTipButton w_helpMissingUsers;

		private HelpTipButton w_helpEditGlobalMappings;

		private HelpTipButton w_helpAllowDBWrite;

		public bool ChildSitesAvailable
		{
			get
			{
				return m_bChildSitesAvailable;
			}
			set
			{
				m_bChildSitesAvailable = value;
				UpdateEnabledState();
			}
		}

		public bool DisplayDBWritingOption
		{
			get
			{
				return w_cbAllowDBWrite.Visible;
			}
			set
			{
				if (value != DisplayDBWritingOption)
				{
					if (value)
					{
						ShowControl(w_cbAllowDBWrite, this);
					}
					else
					{
						HideControl(w_cbAllowDBWrite);
					}
				}
			}
		}

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

		public override NodeCollection SourceNodes
		{
			get
			{
				return base.SourceNodes;
			}
			set
			{
				base.SourceNodes = value;
				if (base.SourceNodes != null && base.SourceNodes.Count > 0)
				{
					if (PasteActionUtils.CollectionContainsMultipleLists(base.SourceNodes))
					{
						HideControl(w_plBelowListScope);
					}
					if (base.SourceNodes[0] is SPWeb)
					{
						SourceWebTemplates = ((SPWeb)base.SourceNodes[0]).Templates;
					}
					else if (base.SourceNodes[0] is SPServer)
					{
						SourceWebTemplates = ((SPServer)base.SourceNodes[0]).Languages[0].Templates;
					}
				}
			}
		}

		public SPWebTemplateCollection SourceWebTemplates
		{
			get
			{
				return m_sourceWebTemplates;
			}
			set
			{
				m_sourceWebTemplates = value;
				ResetWebMappingTable();
			}
		}

		public override NodeCollection TargetNodes
		{
			get
			{
				return base.TargetNodes;
			}
			set
			{
				base.TargetNodes = value;
				UpdateAutomapVisible();
				UpdateMapColumnsEnabled();
				if (base.TargetNodes != null && base.TargetNodes.Count > 0)
				{
					if (PasteActionUtils.CollectionContainsMultipleLists(base.TargetNodes))
					{
						HideControl(w_plBelowListScope);
					}
					if (base.TargetNodes[0] is SPWeb)
					{
						TargetWebTemplates = ((SPWeb)base.TargetNodes[0]).Templates;
					}
				}
			}
		}

		private SPWebTemplateCollection TargetWebTemplates
		{
			get
			{
				return m_targetWebTemplates;
			}
			set
			{
				m_targetWebTemplates = value;
				ResetWebMappingTable();
			}
		}

		public TCMappingOptions()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpAllowDBWrite.SetResourceString(type.FullName + w_cbAllowDBWrite.Name, type);
			w_helpMissingUsers.SetResourceString(type.FullName + w_cbMissingUsers.Name, type);
			w_helpEditGlobalMappings.SetResourceString(type.FullName + w_btnEditGlobalMappings.Name, type);
		}

		private void CheckTransformationsTasks()
		{
			bool @checked = w_cbChangeProperties != null && Convert.ToBoolean(m_ttcTasks.Count);
			w_cbChangeProperties.Checked = @checked;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FireColumnMappingsSet()
		{
			SendMessage("ColumnMappingsSet", m_columnMappings);
		}

		private void FireTopLevelRenameStateChanged(string sNewName, string sNewTitle)
		{
			RenameInfo renameInfo = default(RenameInfo);
			renameInfo.Name = sNewName;
			renameInfo.Title = sNewTitle;
			RenameInfo renameInfo2 = renameInfo;
			SendMessage("TopLevelNodeRenamed", renameInfo2);
		}

		private CommonSerializableTable<object, object> GetTemplateTable()
		{
			if (w_btnEditTemplateMappings.Tag == null)
			{
				ResetWebMappingTable();
			}
			return (CommonSerializableTable<object, object>)w_btnEditTemplateMappings.Tag;
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			switch (sMessage)
			{
			case "TopLevelNodeRenamed":
			{
				RenameInfo renameInfo = (RenameInfo)oValue;
				TopLevelNodeRenamed(renameInfo.Name, renameInfo.Title);
				break;
			}
			case "AvailableTemplatesChanged":
				TargetWebTemplates = oValue as SPWebTemplateCollection;
				break;
			case "CopySubSitesChanged":
				ChildSitesAvailable = (bool)oValue;
				break;
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCMappingOptions));
			this.w_plSite = new DevExpress.XtraEditors.PanelControl();
			this.w_cbMapWebTemplates = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnEditTemplateMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnEditRenameAll = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbChangeProperties = new DevExpress.XtraEditors.CheckEdit();
			this.w_plAboveItemScope = new DevExpress.XtraEditors.PanelControl();
			this.w_plBelowListScope = new DevExpress.XtraEditors.PanelControl();
			this.w_btnEditColumnMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbMapColumns = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnEditGlobalMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_plWebServiceOnly = new DevExpress.XtraEditors.PanelControl();
			this.w_cbAutomapUserMetadata = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbOverwriteGroups = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbMapGroupsByName = new DevExpress.XtraEditors.CheckEdit();
			this.w_plMissingUsers = new DevExpress.XtraEditors.PanelControl();
			this.w_helpMissingUsers = new TooltipsTest.HelpTipButton();
			this.w_tbMissingUsers = new Metalogix.UI.WinForms.Components.AnchoredControls.AnchoredTextBox();
			this.w_cbMissingUsers = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbMapAudiences = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbAllowDBWrite = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpEditGlobalMappings = new TooltipsTest.HelpTipButton();
			this.w_helpAllowDBWrite = new TooltipsTest.HelpTipButton();
			((System.ComponentModel.ISupportInitialize)this.w_plSite).BeginInit();
			this.w_plSite.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapWebTemplates.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbChangeProperties.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plAboveItemScope).BeginInit();
			this.w_plAboveItemScope.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plBelowListScope).BeginInit();
			this.w_plBelowListScope.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapColumns.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebServiceOnly).BeginInit();
			this.w_plWebServiceOnly.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbAutomapUserMetadata.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteGroups.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapGroupsByName.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMissingUsers).BeginInit();
			this.w_plMissingUsers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpMissingUsers).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbMissingUsers).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbMissingUsers.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMissingUsers.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapAudiences.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbAllowDBWrite.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpEditGlobalMappings).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpAllowDBWrite).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_plSite, "w_plSite");
			this.w_plSite.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSite.Controls.Add(this.w_cbMapWebTemplates);
			this.w_plSite.Controls.Add(this.w_btnEditTemplateMappings);
			this.w_plSite.Name = "w_plSite";
			resources.ApplyResources(this.w_cbMapWebTemplates, "w_cbMapWebTemplates");
			this.w_cbMapWebTemplates.Name = "w_cbMapWebTemplates";
			this.w_cbMapWebTemplates.Properties.AutoWidth = true;
			this.w_cbMapWebTemplates.Properties.Caption = resources.GetString("w_cbMapWebTemplates.Properties.Caption");
			this.w_cbMapWebTemplates.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			resources.ApplyResources(this.w_btnEditTemplateMappings, "w_btnEditTemplateMappings");
			this.w_btnEditTemplateMappings.Name = "w_btnEditTemplateMappings";
			this.w_btnEditTemplateMappings.Click += new System.EventHandler(On_MapChildTemplates_Clicked);
			resources.ApplyResources(this.w_btnEditRenameAll, "w_btnEditRenameAll");
			this.w_btnEditRenameAll.Name = "w_btnEditRenameAll";
			this.w_btnEditRenameAll.Click += new System.EventHandler(On_RenameSpecificNodes_Clicked);
			resources.ApplyResources(this.w_cbChangeProperties, "w_cbChangeProperties");
			this.w_cbChangeProperties.Name = "w_cbChangeProperties";
			this.w_cbChangeProperties.Properties.AutoWidth = true;
			this.w_cbChangeProperties.Properties.Caption = resources.GetString("w_cbChangeProperties.Properties.Caption");
			this.w_cbChangeProperties.CheckedChanged += new System.EventHandler(On_Rename_CheckedChanged);
			this.w_plAboveItemScope.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plAboveItemScope.Controls.Add(this.w_cbChangeProperties);
			this.w_plAboveItemScope.Controls.Add(this.w_btnEditRenameAll);
			resources.ApplyResources(this.w_plAboveItemScope, "w_plAboveItemScope");
			this.w_plAboveItemScope.Name = "w_plAboveItemScope";
			this.w_plBelowListScope.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plBelowListScope.Controls.Add(this.w_btnEditColumnMappings);
			this.w_plBelowListScope.Controls.Add(this.w_cbMapColumns);
			resources.ApplyResources(this.w_plBelowListScope, "w_plBelowListScope");
			this.w_plBelowListScope.Name = "w_plBelowListScope";
			resources.ApplyResources(this.w_btnEditColumnMappings, "w_btnEditColumnMappings");
			this.w_btnEditColumnMappings.Name = "w_btnEditColumnMappings";
			this.w_btnEditColumnMappings.Click += new System.EventHandler(On_EditColumnMappings_Clicked);
			resources.ApplyResources(this.w_cbMapColumns, "w_cbMapColumns");
			this.w_cbMapColumns.Name = "w_cbMapColumns";
			this.w_cbMapColumns.Properties.AutoWidth = true;
			this.w_cbMapColumns.Properties.Caption = resources.GetString("w_cbMapColumns.Properties.Caption");
			this.w_cbMapColumns.CheckedChanged += new System.EventHandler(On_MapColumns_CheckedChanged);
			resources.ApplyResources(this.w_btnEditGlobalMappings, "w_btnEditGlobalMappings");
			this.w_btnEditGlobalMappings.Name = "w_btnEditGlobalMappings";
			this.w_btnEditGlobalMappings.Click += new System.EventHandler(On_EditGlobalMappings_Clicked);
			this.w_plWebServiceOnly.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plWebServiceOnly.Controls.Add(this.w_cbAutomapUserMetadata);
			resources.ApplyResources(this.w_plWebServiceOnly, "w_plWebServiceOnly");
			this.w_plWebServiceOnly.Name = "w_plWebServiceOnly";
			resources.ApplyResources(this.w_cbAutomapUserMetadata, "w_cbAutomapUserMetadata");
			this.w_cbAutomapUserMetadata.Name = "w_cbAutomapUserMetadata";
			this.w_cbAutomapUserMetadata.Properties.AutoWidth = true;
			this.w_cbAutomapUserMetadata.Properties.Caption = resources.GetString("w_cbAutomapUserMetadata.Properties.Caption");
			resources.ApplyResources(this.w_cbOverwriteGroups, "w_cbOverwriteGroups");
			this.w_cbOverwriteGroups.Name = "w_cbOverwriteGroups";
			this.w_cbOverwriteGroups.Properties.AutoWidth = true;
			this.w_cbOverwriteGroups.Properties.Caption = resources.GetString("w_cbOverwriteGroups.Properties.Caption");
			resources.ApplyResources(this.w_cbMapGroupsByName, "w_cbMapGroupsByName");
			this.w_cbMapGroupsByName.Name = "w_cbMapGroupsByName";
			this.w_cbMapGroupsByName.Properties.AutoWidth = true;
			this.w_cbMapGroupsByName.Properties.Caption = resources.GetString("w_cbMapGroupsByName.Properties.Caption");
			this.w_cbMapGroupsByName.CheckedChanged += new System.EventHandler(On_MapGroupsByName_CheckedChanged);
			this.w_plMissingUsers.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plMissingUsers.Controls.Add(this.w_helpMissingUsers);
			this.w_plMissingUsers.Controls.Add(this.w_tbMissingUsers);
			this.w_plMissingUsers.Controls.Add(this.w_cbMissingUsers);
			resources.ApplyResources(this.w_plMissingUsers, "w_plMissingUsers");
			this.w_plMissingUsers.Name = "w_plMissingUsers";
			this.w_helpMissingUsers.AnchoringControl = this.w_tbMissingUsers;
			this.w_helpMissingUsers.BackColor = System.Drawing.Color.Transparent;
			this.w_helpMissingUsers.CommonParentControl = null;
			resources.ApplyResources(this.w_helpMissingUsers, "w_helpMissingUsers");
			this.w_helpMissingUsers.Name = "w_helpMissingUsers";
			this.w_helpMissingUsers.TabStop = false;
			this.w_tbMissingUsers.AnchoringControl = this.w_cbMissingUsers;
			this.w_tbMissingUsers.CommonParentControl = null;
			resources.ApplyResources(this.w_tbMissingUsers, "w_tbMissingUsers");
			this.w_tbMissingUsers.Name = "w_tbMissingUsers";
			this.w_tbMissingUsers.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_tbMissingUsers.Properties.Appearance.BackColor");
			this.w_tbMissingUsers.Properties.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this.w_cbMissingUsers, "w_cbMissingUsers");
			this.w_cbMissingUsers.Name = "w_cbMissingUsers";
			this.w_cbMissingUsers.Properties.AutoWidth = true;
			this.w_cbMissingUsers.Properties.Caption = resources.GetString("w_cbMissingUsers.Properties.Caption");
			this.w_cbMissingUsers.CheckedChanged += new System.EventHandler(w_cbMissingUsers_CheckedChanged);
			resources.ApplyResources(this.w_cbMapAudiences, "w_cbMapAudiences");
			this.w_cbMapAudiences.Name = "w_cbMapAudiences";
			this.w_cbMapAudiences.Properties.AutoWidth = true;
			this.w_cbMapAudiences.Properties.Caption = resources.GetString("w_cbMapAudiences.Properties.Caption");
			resources.ApplyResources(this.w_cbAllowDBWrite, "w_cbAllowDBWrite");
			this.w_cbAllowDBWrite.Name = "w_cbAllowDBWrite";
			this.w_cbAllowDBWrite.Properties.AutoWidth = true;
			this.w_cbAllowDBWrite.Properties.Caption = resources.GetString("w_cbAllowDBWrite.Properties.Caption");
			this.w_helpEditGlobalMappings.AnchoringControl = this.w_btnEditGlobalMappings;
			this.w_helpEditGlobalMappings.BackColor = System.Drawing.Color.Transparent;
			this.w_helpEditGlobalMappings.CommonParentControl = null;
			resources.ApplyResources(this.w_helpEditGlobalMappings, "w_helpEditGlobalMappings");
			this.w_helpEditGlobalMappings.Name = "w_helpEditGlobalMappings";
			this.w_helpEditGlobalMappings.TabStop = false;
			this.w_helpAllowDBWrite.AnchoringControl = this.w_cbAllowDBWrite;
			this.w_helpAllowDBWrite.BackColor = System.Drawing.Color.Transparent;
			this.w_helpAllowDBWrite.CommonParentControl = null;
			resources.ApplyResources(this.w_helpAllowDBWrite, "w_helpAllowDBWrite");
			this.w_helpAllowDBWrite.Name = "w_helpAllowDBWrite";
			this.w_helpAllowDBWrite.TabStop = false;
			base.Controls.Add(this.w_helpAllowDBWrite);
			base.Controls.Add(this.w_helpEditGlobalMappings);
			base.Controls.Add(this.w_cbAllowDBWrite);
			base.Controls.Add(this.w_cbMapAudiences);
			base.Controls.Add(this.w_plMissingUsers);
			base.Controls.Add(this.w_cbOverwriteGroups);
			base.Controls.Add(this.w_cbMapGroupsByName);
			base.Controls.Add(this.w_plWebServiceOnly);
			base.Controls.Add(this.w_btnEditGlobalMappings);
			base.Controls.Add(this.w_plBelowListScope);
			base.Controls.Add(this.w_plAboveItemScope);
			base.Controls.Add(this.w_plSite);
			base.Name = "TCMappingOptions";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)this.w_plSite).EndInit();
			this.w_plSite.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbMapWebTemplates.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbChangeProperties.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plAboveItemScope).EndInit();
			this.w_plAboveItemScope.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plBelowListScope).EndInit();
			this.w_plBelowListScope.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbMapColumns.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebServiceOnly).EndInit();
			this.w_plWebServiceOnly.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbAutomapUserMetadata.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteGroups.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapGroupsByName.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMissingUsers).EndInit();
			this.w_plMissingUsers.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpMissingUsers).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbMissingUsers.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbMissingUsers).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMissingUsers.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapAudiences.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbAllowDBWrite.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpEditGlobalMappings).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpAllowDBWrite).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbAllowDBWrite.Enabled = SharePointConfigurationVariables.AllowDBWriting || ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.IsSharePointOnline;
			w_cbAllowDBWrite.CheckedChanged -= w_cbAllowDBWrite_CheckedChanged;
			w_cbAllowDBWrite.Checked = w_cbAllowDBWrite.Enabled && Options.AllowDBUserWriting;
			w_cbAllowDBWrite.CheckedChanged += w_cbAllowDBWrite_CheckedChanged;
			w_cbMapAudiences.Checked = m_options.MapAudiences;
			w_cbMapWebTemplates.Checked = m_options.MapChildWebTemplates;
			w_btnEditTemplateMappings.Tag = null;
			ResetWebMappingTable();
			m_ttcTasks = (TransformationTaskCollection)Options.TaskCollection.Clone();
			w_cbChangeProperties.Checked = m_options.RenameSpecificNodes;
			PropagateTopLevelRename();
			UpdateMapColumnsEnabled();
			ColumnMappings columnMappings = ((m_options.ColumnMappings != null) ? ((ColumnMappings)m_options.ColumnMappings.Clone()) : null);
			SetColumnMappings(columnMappings);
			w_cbAutomapUserMetadata.Checked = m_options.ColumnMappings != null && m_options.ColumnMappings.AutoMapCreatedAndModified;
			w_cbMapColumns.Checked = m_options.MapColumns && w_cbMapColumns.Enabled;
			w_cbMapGroupsByName.Checked = m_options.MapGroupsByName;
			w_cbOverwriteGroups.Checked = m_options.OverwriteGroups;
			w_cbMissingUsers.Checked = m_options.MapMissingUsers;
			w_tbMissingUsers.Text = m_options.MapMissingUsersToLoginName;
		}

		private void On_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void On_EditColumnMappings_Clicked(object sender, EventArgs e)
		{
			if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
			{
				ColumnMappingDialog columnMappingDialog = new ColumnMappingDialog((Node)SourceNodes[0], (Node)TargetNodes[0])
				{
					SummaryItems = m_columnMappings.ToArray()
				};
				if (columnMappingDialog.ShowDialog() == DialogResult.OK)
				{
					m_columnMappings.Clear();
					m_columnMappings.AddRange(columnMappingDialog.SummaryItems);
				}
			}
		}

		private void On_EditGlobalMappings_Clicked(object sender, EventArgs e)
		{
			OpenUserMappingDialog();
		}

		private void On_MapChildTemplates_Clicked(object sender, EventArgs e)
		{
			SerializableWebTemplateMapper serializableWebTemplateMapper = new SerializableWebTemplateMapper
			{
				Mappings = (SerializableTable<object, object>)w_btnEditTemplateMappings.Tag,
				SourceTemplateCollection = SourceWebTemplates,
				TargetTemplateCollection = TargetWebTemplates
			};
			serializableWebTemplateMapper.ShowDialog();
		}

		private void On_MapColumns_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void On_MapGroupsByName_CheckedChanged(object sender, EventArgs e)
		{
			w_cbOverwriteGroups.Enabled = w_cbMapGroupsByName.Checked;
		}

		private void On_Rename_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			if (!m_bSuspendRenameUpdateEvents)
			{
				PropagateTopLevelRename();
			}
		}

		private void On_RenameSpecificNodes_Clicked(object sender, EventArgs e)
		{
			NodeCollection nodeCollection = null;
			nodeCollection = ((base.Scope != SharePointObjectScope.SiteCollection || SourceNodes.Count != 1) ? SourceNodes : (SourceNodes[0] as SPNode).Children);
			PropertyTransformationDialog propertyTransformationDialog = new PropertyTransformationDialog(nodeCollection, w_cbChangeProperties.Text);
			propertyTransformationDialog.Tasks = m_ttcTasks.Clone() as TransformationTaskCollection;
			PropertyTransformationDialog propertyTransformationDialog2 = propertyTransformationDialog;
			if (propertyTransformationDialog2.ShowDialog() == DialogResult.OK)
			{
				m_ttcTasks = propertyTransformationDialog2.Tasks;
				PropagateTopLevelRename();
			}
		}

		private void PropagateTopLevelRename()
		{
			if (m_ttcTasks.Count <= 0 || SourceNodes == null || SourceNodes.Count != 1 || !w_cbChangeProperties.Checked)
			{
				FireTopLevelRenameStateChanged(null, null);
			}
			else if (SourceNodes[0] is SPNode sPNode)
			{
				int indexofTask = m_ttcTasks.GetIndexofTask(sPNode.GetType(), sPNode.DisplayUrl);
				if (indexofTask >= 0)
				{
					TransformationTask transformationTask = m_ttcTasks[indexofTask];
					string text = ((!transformationTask.ChangeOperations.ContainsKey("Name")) ? null : transformationTask.ChangeOperations["Name"]);
					string sNewName = text;
					string key = ((sPNode.GetType() == typeof(SPFolder)) ? "FileLeafRef" : "Title");
					string sNewTitle = ((!transformationTask.ChangeOperations.ContainsKey(key)) ? null : transformationTask.ChangeOperations[key]);
					FireTopLevelRenameStateChanged(sNewName, sNewTitle);
				}
			}
		}

		private void ResetWebMappingTable()
		{
			if (Options == null || SourceWebTemplates == null || TargetWebTemplates == null)
			{
				return;
			}
			CommonSerializableTable<object, object> commonSerializableTable = new CommonSerializableTable<object, object>();
			if (w_btnEditTemplateMappings.Tag != null)
			{
				CommonSerializableTable<object, object> commonSerializableTable2 = (CommonSerializableTable<object, object>)w_btnEditTemplateMappings.Tag;
				foreach (object key in commonSerializableTable2.Keys)
				{
					string name = ((SPWebTemplate)key).Name;
					string name2 = ((SPWebTemplate)commonSerializableTable2[key]).Name;
					SPWebTemplate sPWebTemplate = SourceWebTemplates[name];
					SPWebTemplate sPWebTemplate2 = TargetWebTemplates[name2];
					if (sPWebTemplate != null && sPWebTemplate2 != null)
					{
						commonSerializableTable.Add(sPWebTemplate, sPWebTemplate2);
					}
				}
			}
			else
			{
				foreach (string key2 in m_options.WebTemplateMappingTable.Keys)
				{
					SPWebTemplate sPWebTemplate3 = SourceWebTemplates[key2];
					SPWebTemplate sPWebTemplate4 = TargetWebTemplates[m_options.WebTemplateMappingTable[key2]];
					if (sPWebTemplate3 != null && sPWebTemplate4 != null)
					{
						commonSerializableTable.Add(sPWebTemplate3, sPWebTemplate4);
					}
				}
			}
			w_btnEditTemplateMappings.Tag = commonSerializableTable;
		}

		public override bool SaveUI()
		{
			Options.AllowDBUserWriting = w_cbAllowDBWrite.Enabled && w_cbAllowDBWrite.Checked;
			Options.MapAudiences = w_cbMapAudiences.Checked;
			Options.RenameSpecificNodes = w_cbChangeProperties.Checked;
			Options.TaskCollection = m_ttcTasks;
			Options.MapChildWebTemplates = w_cbMapWebTemplates.Checked;
			CommonSerializableTable<object, object> commonSerializableTable = (CommonSerializableTable<object, object>)w_btnEditTemplateMappings.Tag;
			if (commonSerializableTable != null)
			{
				foreach (SPWebTemplate key in commonSerializableTable.Keys)
				{
					if (!m_options.WebTemplateMappingTable.ContainsKey(key.Name))
					{
						m_options.WebTemplateMappingTable.Add(key.Name, ((SPWebTemplate)commonSerializableTable[key]).Name);
					}
					else
					{
						m_options.WebTemplateMappingTable[key.Name] = ((SPWebTemplate)commonSerializableTable[key]).Name;
					}
				}
			}
			Options.MapColumns = w_cbMapColumns.Checked && w_cbMapColumns.Enabled;
			Options.ColumnMappings = m_columnMappings;
			if (Options.ColumnMappings != null)
			{
				Options.ColumnMappings.AutoMapCreatedAndModified = w_cbAutomapUserMetadata.Checked && w_plWebServiceOnly.Visible;
			}
			else if (w_cbAutomapUserMetadata.Checked && w_plWebServiceOnly.Visible)
			{
				Options.ColumnMappings = new ColumnMappings
				{
					AutoMapCreatedAndModified = true
				};
			}
			m_options.MapGroupsByName = w_cbMapGroupsByName.Checked && w_cbMapGroupsByName.Enabled;
			m_options.OverwriteGroups = w_cbOverwriteGroups.Checked && w_cbOverwriteGroups.Enabled;
			m_options.MapMissingUsers = w_cbMissingUsers.Enabled && w_cbMissingUsers.Checked;
			SPMappingOptions options = m_options;
			string mapMissingUsersToLoginName = ((!m_options.MapMissingUsers) ? null : w_tbMissingUsers.Text);
			options.MapMissingUsersToLoginName = mapMissingUsersToLoginName;
			return true;
		}

		private void SetColumnMappings(ColumnMappings mappings)
		{
			m_columnMappings = mappings;
			if (m_columnMappings == null)
			{
				m_columnMappings = new ColumnMappings();
			}
			FireColumnMappingsSet();
		}

		private void TopLevelNodeRenamed(string sNewName, string sNewTitle)
		{
			m_bSuspendRenameUpdateEvents = true;
			if (SourceNodes[0] is SPNode sPNode)
			{
				TransformationTask transformationTask = null;
				int indexofTask = m_ttcTasks.GetIndexofTask(sPNode.GetType(), sPNode.DisplayUrl);
				if (indexofTask >= 0)
				{
					m_ttcTasks.TransformationTasks.RemoveAt(indexofTask);
				}
				if (sNewName != null || sNewTitle != null)
				{
					transformationTask = new TransformationTask
					{
						ApplyTo = new FilterExpression(FilterOperand.Equals, sPNode.GetType(), "DisplayUrl", sPNode.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false)
					};
					if (sNewTitle != null)
					{
						transformationTask.ChangeOperations.Add((sPNode.GetType() == typeof(SPFolder)) ? "FileLeafRef" : "Title", sNewTitle);
					}
					if (sNewName != null)
					{
						transformationTask.ChangeOperations.Add("Name", sNewName);
					}
				}
				if (transformationTask != null)
				{
					m_ttcTasks.TransformationTasks.Add(transformationTask);
				}
				CheckTransformationsTasks();
			}
			m_bSuspendRenameUpdateEvents = false;
		}

		private void UpdateAutomapVisible()
		{
			if (TargetNodes == null || TargetNodes.Count == 0 || !(TargetNodes[0] is SPNode) || ((SPNode)TargetNodes[0]).CanWriteCreatedModifiedMetaInfo)
			{
				HideControl(w_plWebServiceOnly);
			}
		}

		protected override void UpdateEnabledState()
		{
			w_plSite.Enabled = ChildSitesAvailable;
			w_btnEditTemplateMappings.Enabled = w_cbMapWebTemplates.Checked && w_cbMapWebTemplates.Enabled;
			w_btnEditRenameAll.Enabled = w_cbChangeProperties.Checked && w_cbChangeProperties.Enabled;
			w_btnEditColumnMappings.Enabled = w_cbMapColumns.Checked && w_cbMapColumns.Enabled;
			w_cbOverwriteGroups.Enabled = w_cbMapGroupsByName.Checked;
			w_tbMissingUsers.Enabled = w_cbMissingUsers.Checked;
		}

		private void UpdateMapColumnsEnabled()
		{
			SPList sPList = null;
			if (TargetNodes == null || TargetNodes.Count == 0 || !(TargetNodes[0] is SPFolder))
			{
				w_cbMapColumns.Enabled = false;
				w_cbMapColumns.Checked = w_cbMapColumns.Checked && w_cbMapColumns.Enabled;
			}
			else
			{
				sPList = ((!(TargetNodes[0] is SPList)) ? ((SPFolder)TargetNodes[0]).ParentList : ((SPList)TargetNodes[0]));
				w_cbMapColumns.Enabled = sPList.BaseType != ListType.DiscussionForum && sPList.BaseType != ListType.Surveys && sPList.BaseTemplate != ListTemplateType.DiscussionBoard;
				w_cbMapColumns.Checked = w_cbMapColumns.Checked && w_cbMapColumns.Enabled;
			}
		}

		protected override void UpdateScope()
		{
			switch (base.Scope)
			{
			case SharePointObjectScope.SiteCollection:
			case SharePointObjectScope.Site:
				w_cbChangeProperties.Text = "Rename Specific Sites. Lists and Folders";
				HideControl(w_plBelowListScope);
				break;
			case SharePointObjectScope.List:
				w_cbChangeProperties.Text = "Rename Specific Lists and Folders";
				HideControl(w_plSite);
				HideControl(w_plBelowListScope);
				if (BCSHelper.HasExternalListsOnly(SourceNodes))
				{
					HideControl(w_cbMapAudiences);
				}
				break;
			case SharePointObjectScope.Folder:
				w_cbChangeProperties.Text = "Rename Specific Folders";
				HideControl(w_plSite);
				break;
			case SharePointObjectScope.Item:
				HideControl(w_plSite);
				HideControl(w_plAboveItemScope);
				break;
			case SharePointObjectScope.Permissions:
				HideControl(w_plSite);
				HideControl(w_plAboveItemScope);
				HideControl(w_plBelowListScope);
				HideControl(w_plWebServiceOnly);
				HideControl(w_cbMapAudiences);
				break;
			}
		}

		private void w_cbAllowDBWrite_CheckedChanged(object sender, EventArgs e)
		{
			if (w_cbAllowDBWrite.Checked && FlatXtraMessageBox.Show(Resources.MsgUserDbWriteWarning, Resources.WarningString, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				w_cbAllowDBWrite.Checked = false;
			}
		}

		private void w_cbMissingUsers_CheckedChanged(object sender, EventArgs e)
		{
			w_tbMissingUsers.Enabled = w_cbMissingUsers.Checked;
		}
	}
}
