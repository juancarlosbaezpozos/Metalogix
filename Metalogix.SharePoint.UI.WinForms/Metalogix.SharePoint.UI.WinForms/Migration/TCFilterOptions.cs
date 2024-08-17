using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Filters;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Filters;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.FilterOptions.png")]
	[ControlName("Filter Options")]
	public class TCFilterOptions : FilterOptionsScopableTabbableControl
	{
		private ColumnMappings m_currentColumnMappings;

		private IFilterExpression m_currentFilter;

		private SPFilterOptions m_options;

		private bool m_bSubFoldersAvailable;

		private bool m_bChildSitesAvailable;

		private bool m_bCopyingCustomContent;

		private IFilterExpression m_siteFieldsFilter;

		private bool _hasShownAnnouncement;

		private IContainer components;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcLists;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcFolders;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcItems;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcSites;

		private PanelControl w_pColumnFiltering;

		private SimpleButton w_bEditColumnFilters;

		private CheckEdit w_cbFilterColumns;

		private MemoEdit w_tbColumnsFilterText;

		private PanelControl w_pSiteColumnFiltering;

		private MemoEdit w_tbSiteColumnsFilterText;

		private SimpleButton w_bEditSiteColumnFilters;

		private CheckEdit w_cbFilterSiteColumns;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcCustomFolders;

		private Metalogix.UI.WinForms.Data.Filters.FilterControl w_fcCustomFiles;

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

		protected bool CopyingCustomContent
		{
			get
			{
				return m_bCopyingCustomContent;
			}
			set
			{
				m_bCopyingCustomContent = value;
				UpdateEnabledState();
			}
		}

		public IFilterExpression ListFieldsFilter
		{
			get
			{
				return m_currentFilter;
			}
			set
			{
				m_currentFilter = value;
				if (m_currentFilter != null)
				{
					UpdateFilterUI(m_currentFilter, w_tbColumnsFilterText);
				}
			}
		}

		public SPFilterOptions Options
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

		public IFilterExpression SiteFieldsFilter
		{
			get
			{
				return m_siteFieldsFilter;
			}
			set
			{
				m_siteFieldsFilter = value;
				if (m_siteFieldsFilter != null)
				{
					UpdateFilterUI(m_siteFieldsFilter, w_tbSiteColumnsFilterText);
				}
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
				UpdateScope();
			}
		}

		public bool SubFoldersAvailable
		{
			get
			{
				return m_bSubFoldersAvailable;
			}
			set
			{
				m_bSubFoldersAvailable = value;
				UpdateEnabledState();
			}
		}

		public FilterSetupOptions VisOptions { get; set; }

		public TCFilterOptions()
		{
			InitializeComponent();
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl = w_fcSites;
			Type[] collection = new Type[1] { typeof(SPWeb) };
			filterControl.FilterType = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: false);
			w_fcSites.TypeHeader = "Child Sites";
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl2 = w_fcLists;
			Type[] collection2 = new Type[1] { typeof(SPList) };
			filterControl2.FilterType = new FilterBuilderType(new List<Type>(collection2), bAllowFreeFormEntry: false);
			w_fcLists.TypeHeader = "Lists and Libraries";
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl3 = w_fcFolders;
			Type[] collection3 = new Type[1] { typeof(SPFolder) };
			filterControl3.FilterType = new FilterBuilderType(new List<Type>(collection3), bAllowFreeFormEntry: true);
			w_fcFolders.TypeHeader = "Folders";
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl4 = w_fcItems;
			Type[] collection4 = new Type[1] { typeof(SPListItem) };
			filterControl4.FilterType = new FilterBuilderType(new List<Type>(collection4), bAllowFreeFormEntry: true);
			w_fcItems.TypeHeader = "Items and Documents";
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl5 = w_fcCustomFolders;
			Type[] collection5 = new Type[1] { typeof(SPFolderBasic) };
			filterControl5.FilterType = new FilterBuilderType(new List<Type>(collection5), bAllowFreeFormEntry: true);
			w_fcCustomFolders.TypeHeader = "Other Root Folders";
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl6 = w_fcCustomFiles;
			Type[] collection6 = new Type[1] { typeof(SPFile) };
			filterControl6.FilterType = new FilterBuilderType(new List<Type>(collection6), bAllowFreeFormEntry: true);
			w_fcCustomFiles.TypeHeader = "Other Root Files";
			foreach (Control control in base.Controls)
			{
				if (control is Metalogix.UI.WinForms.Data.Filters.FilterControl)
				{
					((Metalogix.UI.WinForms.Data.Filters.FilterControl)control).CheckedChanged += CheckedChanged;
				}
			}
		}

		private void BuildExclusionString(IFilterExpression iFilter, StringBuilder sb)
		{
			if (!(iFilter is FilterExpressionList))
			{
				FilterExpression filterExpression = (FilterExpression)iFilter;
				sb.Append("\"" + filterExpression.Property + "\" must " + Metalogix.UI.WinForms.Data.Filters.FilterControl.Translator[filterExpression.Operand]);
				if (filterExpression.Pattern != null)
				{
					sb.Append(" \"" + filterExpression.Pattern + "\"");
				}
				return;
			}
			FilterExpressionList filterExpressionList = (FilterExpressionList)iFilter;
			string value = " " + filterExpressionList.Logic.ToString() + " \n";
			int num = 0;
			foreach (IFilterExpression item in filterExpressionList)
			{
				BuildExclusionString(item, sb);
				num++;
				if (num != filterExpressionList.Count)
				{
					sb.Append(value);
				}
			}
		}

		private void CheckedChanged(object sender, CancelEventArgs e)
		{
			if (IsOptionActive() && ShowInformationDialog())
			{
				e.Cancel = !VerifyUserActionDialog.GetUserVerification(SharePointConfigurationVariables.ShowFilterOptionInformationDialog, Resources.FilterWorkflowsWarning);
				_hasShownAnnouncement = true;
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

		private string GetExclusionString(IFilterExpression filter)
		{
			if (filter == null || (filter is FilterExpressionList && ((FilterExpressionList)filter).Count == 0))
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			BuildExclusionString(filter, stringBuilder);
			return stringBuilder.ToString();
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			switch (sMessage)
			{
			case "ColumnMappingsSet":
				SetCurrentColumnMappings(oValue as ColumnMappings);
				break;
			case "CopySubSitesChanged":
				ChildSitesAvailable = (bool)oValue;
				break;
			case "CopyCustContentChanged":
				CopyingCustomContent = (bool)oValue;
				break;
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCFilterOptions));
			this.w_pSiteColumnFiltering = new DevExpress.XtraEditors.PanelControl();
			this.w_tbSiteColumnsFilterText = new DevExpress.XtraEditors.MemoEdit();
			this.w_bEditSiteColumnFilters = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbFilterSiteColumns = new DevExpress.XtraEditors.CheckEdit();
			this.w_pColumnFiltering = new DevExpress.XtraEditors.PanelControl();
			this.w_tbColumnsFilterText = new DevExpress.XtraEditors.MemoEdit();
			this.w_bEditColumnFilters = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbFilterColumns = new DevExpress.XtraEditors.CheckEdit();
			this.w_fcCustomFiles = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			this.w_fcCustomFolders = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			this.w_fcSites = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			this.w_fcItems = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			this.w_fcFolders = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			this.w_fcLists = new Metalogix.UI.WinForms.Data.Filters.FilterControl();
			((System.ComponentModel.ISupportInitialize)this.w_pSiteColumnFiltering).BeginInit();
			this.w_pSiteColumnFiltering.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_tbSiteColumnsFilterText.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFilterSiteColumns.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_pColumnFiltering).BeginInit();
			this.w_pColumnFiltering.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_tbColumnsFilterText.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFilterColumns.Properties).BeginInit();
			base.SuspendLayout();
			this.w_pSiteColumnFiltering.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_pSiteColumnFiltering.Controls.Add(this.w_tbSiteColumnsFilterText);
			this.w_pSiteColumnFiltering.Controls.Add(this.w_bEditSiteColumnFilters);
			this.w_pSiteColumnFiltering.Controls.Add(this.w_cbFilterSiteColumns);
			resources.ApplyResources(this.w_pSiteColumnFiltering, "w_pSiteColumnFiltering");
			this.w_pSiteColumnFiltering.Name = "w_pSiteColumnFiltering";
			resources.ApplyResources(this.w_tbSiteColumnsFilterText, "w_tbSiteColumnsFilterText");
			this.w_tbSiteColumnsFilterText.Name = "w_tbSiteColumnsFilterText";
			this.w_tbSiteColumnsFilterText.Properties.ReadOnly = true;
			resources.ApplyResources(this.w_bEditSiteColumnFilters, "w_bEditSiteColumnFilters");
			this.w_bEditSiteColumnFilters.Name = "w_bEditSiteColumnFilters";
			this.w_bEditSiteColumnFilters.Click += new System.EventHandler(w_bEditSiteColumnFilters_Click);
			resources.ApplyResources(this.w_cbFilterSiteColumns, "w_cbFilterSiteColumns");
			this.w_cbFilterSiteColumns.Name = "w_cbFilterSiteColumns";
			this.w_cbFilterSiteColumns.Properties.AutoWidth = true;
			this.w_cbFilterSiteColumns.Properties.Caption = resources.GetString("w_cbFilterSiteColumns.Properties.Caption");
			this.w_cbFilterSiteColumns.CheckedChanged += new System.EventHandler(w_cbFilterSiteColumns_CheckedChanged);
			this.w_pColumnFiltering.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_pColumnFiltering.Controls.Add(this.w_tbColumnsFilterText);
			this.w_pColumnFiltering.Controls.Add(this.w_bEditColumnFilters);
			this.w_pColumnFiltering.Controls.Add(this.w_cbFilterColumns);
			resources.ApplyResources(this.w_pColumnFiltering, "w_pColumnFiltering");
			this.w_pColumnFiltering.Name = "w_pColumnFiltering";
			resources.ApplyResources(this.w_tbColumnsFilterText, "w_tbColumnsFilterText");
			this.w_tbColumnsFilterText.Name = "w_tbColumnsFilterText";
			this.w_tbColumnsFilterText.Properties.ReadOnly = true;
			resources.ApplyResources(this.w_bEditColumnFilters, "w_bEditColumnFilters");
			this.w_bEditColumnFilters.Name = "w_bEditColumnFilters";
			this.w_bEditColumnFilters.Click += new System.EventHandler(w_bEditColumnFilters_Click);
			resources.ApplyResources(this.w_cbFilterColumns, "w_cbFilterColumns");
			this.w_cbFilterColumns.Name = "w_cbFilterColumns";
			this.w_cbFilterColumns.Properties.AutoWidth = true;
			this.w_cbFilterColumns.Properties.Caption = resources.GetString("w_cbFilterColumns.Properties.Caption");
			this.w_cbFilterColumns.CheckedChanged += new System.EventHandler(w_cbFilterColumns_CheckedChanged);
			resources.ApplyResources(this.w_fcCustomFiles, "w_fcCustomFiles");
			this.w_fcCustomFiles.Filters = null;
			this.w_fcCustomFiles.IsFiltered = false;
			this.w_fcCustomFiles.Name = "w_fcCustomFiles";
			this.w_fcCustomFiles.TypeHeader = "Object";
			resources.ApplyResources(this.w_fcCustomFolders, "w_fcCustomFolders");
			this.w_fcCustomFolders.Filters = null;
			this.w_fcCustomFolders.IsFiltered = false;
			this.w_fcCustomFolders.Name = "w_fcCustomFolders";
			this.w_fcCustomFolders.TypeHeader = "Object";
			resources.ApplyResources(this.w_fcSites, "w_fcSites");
			this.w_fcSites.Filters = null;
			this.w_fcSites.IsFiltered = false;
			this.w_fcSites.Name = "w_fcSites";
			this.w_fcSites.TypeHeader = "Object";
			resources.ApplyResources(this.w_fcItems, "w_fcItems");
			this.w_fcItems.Filters = null;
			this.w_fcItems.IsFiltered = false;
			this.w_fcItems.Name = "w_fcItems";
			this.w_fcItems.TypeHeader = "Object";
			resources.ApplyResources(this.w_fcFolders, "w_fcFolders");
			this.w_fcFolders.Filters = null;
			this.w_fcFolders.IsFiltered = false;
			this.w_fcFolders.Name = "w_fcFolders";
			this.w_fcFolders.TypeHeader = "Object";
			resources.ApplyResources(this.w_fcLists, "w_fcLists");
			this.w_fcLists.Filters = null;
			this.w_fcLists.IsFiltered = false;
			this.w_fcLists.Name = "w_fcLists";
			this.w_fcLists.TypeHeader = "Object";
			base.Controls.Add(this.w_fcCustomFiles);
			base.Controls.Add(this.w_fcCustomFolders);
			base.Controls.Add(this.w_pSiteColumnFiltering);
			base.Controls.Add(this.w_pColumnFiltering);
			base.Controls.Add(this.w_fcLists);
			base.Controls.Add(this.w_fcFolders);
			base.Controls.Add(this.w_fcItems);
			base.Controls.Add(this.w_fcSites);
			base.Name = "TCFilterOptions";
			resources.ApplyResources(this, "$this");
			base.Load += new System.EventHandler(TCFilterOptions_Load);
			((System.ComponentModel.ISupportInitialize)this.w_pSiteColumnFiltering).EndInit();
			this.w_pSiteColumnFiltering.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_tbSiteColumnsFilterText.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFilterSiteColumns.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_pColumnFiltering).EndInit();
			this.w_pColumnFiltering.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_tbColumnsFilterText.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFilterColumns.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private bool IsOptionActive()
		{
			if (w_fcSites.IsFiltered || w_fcLists.IsFiltered || w_fcFolders.IsFiltered || w_fcItems.IsFiltered || w_fcCustomFolders.IsFiltered || w_fcCustomFiles.IsFiltered || w_cbFilterColumns.Checked)
			{
				return true;
			}
			return w_cbFilterSiteColumns.Checked;
		}

		protected override void LoadUI()
		{
			HideControl(w_fcFolders);
			w_fcSites.IsFiltered = m_options.FilterSites;
			w_fcSites.Filters = FilterExpression.ParseExpression(m_options.SiteFilterExpression.ToXML());
			w_fcLists.IsFiltered = m_options.FilterLists;
			w_fcLists.Filters = FilterExpression.ParseExpression(m_options.ListFilterExpression.ToXML());
			w_fcFolders.IsFiltered = m_options.FilterFolders;
			w_fcFolders.Filters = FilterExpression.ParseExpression(m_options.FolderFilterExpression.ToXML());
			w_fcItems.IsFiltered = m_options.FilterItems;
			w_fcItems.Filters = FilterExpression.ParseExpression(m_options.ItemFilterExpression.ToXML());
			w_cbFilterColumns.Checked = m_options.FilterFields;
			ListFieldsFilter = FilterExpression.ParseExpression(m_options.ListFieldsFilterExpression.ToXML());
			w_cbFilterSiteColumns.Checked = m_options.FilterSiteFields;
			SiteFieldsFilter = FilterExpression.ParseExpression(m_options.SiteFieldsFilterExpression.ToXML());
			w_fcCustomFolders.IsFiltered = m_options.FilterCustomFolders;
			w_fcCustomFolders.Filters = FilterExpression.ParseExpression(m_options.CustomFolderFilterExpression.ToXML());
			w_fcCustomFiles.IsFiltered = m_options.FilterCustomFiles;
			w_fcCustomFiles.Filters = FilterExpression.ParseExpression(m_options.CustomFileFilterExpression.ToXML());
			if (VisOptions > FilterSetupOptions.None)
			{
				if (!VisOptions.HasFlag(FilterSetupOptions.Sites))
				{
					HideControl(w_fcSites);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.Lists))
				{
					HideControl(w_fcLists);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.Folders))
				{
					HideControl(w_fcFolders);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.Items))
				{
					HideControl(w_fcItems);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.ListColumns))
				{
					HideControl(w_pColumnFiltering);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.SiteColumns))
				{
					HideControl(w_pSiteColumnFiltering);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.CustomFolders))
				{
					HideControl(w_fcCustomFolders);
				}
				if (!VisOptions.HasFlag(FilterSetupOptions.CustomFiles))
				{
					HideControl(w_fcCustomFiles);
				}
			}
		}

		protected override void MultiSelectUISetup()
		{
			HideControl(w_pColumnFiltering);
		}

		public override bool SaveUI()
		{
			m_options.FilterSites = w_fcSites.IsFiltered;
			m_options.SiteFilterExpression = w_fcSites.Filters;
			m_options.FilterLists = w_fcLists.IsFiltered;
			m_options.ListFilterExpression = w_fcLists.Filters;
			m_options.FilterFolders = w_fcFolders.IsFiltered;
			m_options.FolderFilterExpression = w_fcFolders.Filters;
			m_options.FilterItems = w_fcItems.IsFiltered;
			m_options.ItemFilterExpression = w_fcItems.Filters;
			m_options.FilterFields = w_cbFilterColumns.Checked;
			m_options.ListFieldsFilterExpression = ListFieldsFilter;
			m_options.FilterSiteFields = w_cbFilterSiteColumns.Checked;
			m_options.SiteFieldsFilterExpression = SiteFieldsFilter;
			m_options.FilterCustomFolders = w_fcCustomFolders.IsFiltered;
			m_options.CustomFolderFilterExpression = w_fcCustomFolders.Filters;
			m_options.FilterCustomFiles = w_fcCustomFiles.IsFiltered;
			m_options.CustomFileFilterExpression = w_fcCustomFiles.Filters;
			return true;
		}

		private void SetCurrentColumnMappings(ColumnMappings mappings)
		{
			m_currentColumnMappings = mappings;
			m_currentFilter = mappings.FieldsFilter;
			if (m_currentColumnMappings != null)
			{
				IFilterExpression fieldsFilter = m_currentColumnMappings.FieldsFilter;
			}
			UpdateFilterUI(m_currentFilter, w_tbColumnsFilterText);
		}

		private bool ShowInformationDialog()
		{
			if (_hasShownAnnouncement || base.ActiveControl == null)
			{
				return false;
			}
			return this == base.ActiveControl.Parent;
		}

		private void TCFilterOptions_Load(object sender, EventArgs e)
		{
			w_fcSites.Width = base.ClientSize.Width - 12;
			w_fcLists.Width = base.ClientSize.Width - 12;
			w_fcFolders.Width = base.ClientSize.Width - 12;
			w_fcItems.Width = base.ClientSize.Width - 12;
			w_pSiteColumnFiltering.Width = base.ClientSize.Width - 8;
			w_pColumnFiltering.Width = base.ClientSize.Width - 8;
			w_fcCustomFolders.Width = base.ClientSize.Width - 12;
			w_fcCustomFiles.Width = base.ClientSize.Width - 12;
		}

		protected override void UpdateEnabledState()
		{
			w_fcSites.Enabled = ChildSitesAvailable;
			w_fcLists.Enabled = base.ListsAvailable;
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl = w_fcFolders;
			bool enabled = (base.FoldersAvailable && base.Scope != SharePointObjectScope.Folder) || (SubFoldersAvailable && base.Scope == SharePointObjectScope.Folder && ((base.Scope == SharePointObjectScope.List && !BCSHelper.HasExternalListsOnly(SourceNodes)) || base.Scope != SharePointObjectScope.List));
			filterControl.Enabled = enabled;
			Metalogix.UI.WinForms.Data.Filters.FilterControl filterControl2 = w_fcItems;
			bool enabled2 = base.ItemsAvailable && ((base.Scope == SharePointObjectScope.List && !BCSHelper.HasExternalListsOnly(SourceNodes)) || base.Scope != SharePointObjectScope.List);
			filterControl2.Enabled = enabled2;
			w_pColumnFiltering.Enabled = (base.Scope == SharePointObjectScope.List && !BCSHelper.HasExternalListsOnly(SourceNodes)) || base.Scope != SharePointObjectScope.List;
			w_tbSiteColumnsFilterText.Enabled = w_cbFilterSiteColumns.Checked;
			w_bEditSiteColumnFilters.Enabled = w_tbSiteColumnsFilterText.Enabled;
			w_tbColumnsFilterText.Enabled = w_cbFilterColumns.Checked;
			w_bEditColumnFilters.Enabled = w_tbColumnsFilterText.Enabled;
			w_fcCustomFiles.Enabled = (base.Scope == SharePointObjectScope.Server || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && CopyingCustomContent;
			w_fcCustomFolders.Enabled = (base.Scope == SharePointObjectScope.Server || base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && CopyingCustomContent;
		}

		private void UpdateFilterUI(IFilterExpression filter, MemoEdit textBox)
		{
			if (filter == null)
			{
				textBox.Text = "";
				textBox.Properties.ScrollBars = ScrollBars.None;
				return;
			}
			string exclusionString = GetExclusionString(filter);
			char[] separator = new char[1] { '\n' };
			textBox.Lines = exclusionString.Split(separator);
			bool flag = textBox.PreferredSize.Height > textBox.Height;
			int num = textBox.PreferredSize.Width;
			int num2 = textBox.Width;
			if (flag)
			{
				textBox.Properties.ScrollBars = ScrollBars.Vertical;
			}
			else
			{
				textBox.Properties.ScrollBars = ScrollBars.None;
			}
		}

		protected override void UpdateScope()
		{
			if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
			{
				if ((base.SourceWeb != null && base.SourceWeb.Adapter.SharePointVersion.IsSharePoint2003) || base.SourceIsServerNode)
				{
					HideControl(w_pSiteColumnFiltering);
				}
				HideControl(w_pColumnFiltering);
				return;
			}
			HideControl(w_pSiteColumnFiltering);
			HideControl(w_fcSites);
			if (base.Scope != SharePointObjectScope.List)
			{
				HideControl(w_fcLists);
			}
		}

		private void w_bEditColumnFilters_Click(object sender, EventArgs e)
		{
			if (base.SourceList == null)
			{
				return;
			}
			ColumnFilteringConfigDialog columnFilteringConfigDialog = new ColumnFilteringConfigDialog(base.SourceList);
			if (m_currentColumnMappings != null && m_currentColumnMappings.FieldsFilter != null)
			{
				columnFilteringConfigDialog.FilterExpression = m_currentColumnMappings.FieldsFilter;
			}
			if (columnFilteringConfigDialog.ShowDialog() == DialogResult.OK)
			{
				if (m_currentColumnMappings == null)
				{
					m_currentColumnMappings = new ColumnMappings();
				}
				m_currentColumnMappings.FieldsFilter = columnFilteringConfigDialog.FilterExpression;
				m_currentFilter = m_currentColumnMappings.FieldsFilter;
				UpdateFilterUI(m_currentFilter, w_tbColumnsFilterText);
			}
		}

		private void w_bEditSiteColumnFilters_Click(object sender, EventArgs e)
		{
			if (base.SourceWeb != null)
			{
				ColumnFilteringConfigDialog columnFilteringConfigDialog = new ColumnFilteringConfigDialog(SourceNodes)
				{
					FilterExpression = SiteFieldsFilter
				};
				if (columnFilteringConfigDialog.ShowDialog() == DialogResult.OK)
				{
					SiteFieldsFilter = columnFilteringConfigDialog.FilterExpression;
					UpdateFilterUI(SiteFieldsFilter, w_tbSiteColumnsFilterText);
				}
			}
		}

		private void w_cbFilterColumns_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			CheckedChanged(null, null);
		}

		private void w_cbFilterSiteColumns_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			CheckedChanged(null, null);
		}
	}
}
