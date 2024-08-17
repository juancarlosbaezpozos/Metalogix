using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.FilterOptions.png")]
    [ControlName("Filter Options")]
    public class TCFilterOptionsBasicView : FilterOptionsScopableTabbableControl
    {
        private bool _isWarningShown;

        private SPFilterOptions _options;

        private IContainer components;

        private TableLayoutPanel tlpFilter;

        private FilterControlBasicView fcChildSites;

        private FilterControlBasicView fcItemsDocuments;

        private FilterControlBasicView fcListsLibraries;

        private FilterControlBasicView fcListColumns;

        private FilterControlBasicView fcSiteColumns;

        public ColumnMappings ColumnMappings { get; set; }

        public SPFilterOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                LoadUI();
            }
        }

        public TCFilterOptionsBasicView()
        {
            InitializeComponent();
            FilterControlBasicView filterControlBasicView = fcChildSites;
            Type[] collection = new Type[1] { typeof(SPWeb) };
            filterControlBasicView.FilterType = new FilterBuilderType(new List<Type>(collection), bAllowFreeFormEntry: false);
            fcChildSites.TypeHeader = "Child Sites";
            fcChildSites.IsFilterExpressionEditorDialog = true;
            FilterControlBasicView filterControlBasicView2 = fcItemsDocuments;
            Type[] collection2 = new Type[1] { typeof(SPListItem) };
            filterControlBasicView2.FilterType = new FilterBuilderType(new List<Type>(collection2), bAllowFreeFormEntry: true);
            fcItemsDocuments.TypeHeader = "Items and Documents";
            fcItemsDocuments.IsFilterExpressionEditorDialog = true;
            FilterControlBasicView filterControlBasicView3 = fcListsLibraries;
            Type[] collection3 = new Type[1] { typeof(SPList) };
            filterControlBasicView3.FilterType = new FilterBuilderType(new List<Type>(collection3), bAllowFreeFormEntry: false);
            fcListsLibraries.TypeHeader = "Lists and Libraries";
            fcListsLibraries.IsFilterExpressionEditorDialog = true;
            FilterControlBasicView filterControlBasicView4 = fcListColumns;
            Type[] collection4 = new Type[1] { typeof(SPList) };
            filterControlBasicView4.FilterType = new FilterBuilderType(new List<Type>(collection4), bAllowFreeFormEntry: false);
            fcListColumns.TypeHeader = "List Columns";
            fcListColumns.IsFilterExpressionEditorDialog = false;
            FilterControlBasicView filterControlBasicView5 = fcSiteColumns;
            Type[] collection5 = new Type[1] { typeof(SPWeb) };
            filterControlBasicView5.FilterType = new FilterBuilderType(new List<Type>(collection5), bAllowFreeFormEntry: false);
            fcSiteColumns.TypeHeader = "Site Columns";
            fcSiteColumns.IsFilterExpressionEditorDialog = false;
            foreach (Control control in tlpFilter.Controls)
            {
                if (control is FilterControlBasicView)
                {
                    ((FilterControlBasicView)control).ToggleSwitched += ToggleSwitched;
                }
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

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            if ((base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && fcChildSites.Enabled && fcChildSites.IsFiltered)
            {
                arrayList.Add(new OptionsSummary($"Apply filter on {fcChildSites.TypeHeader} : {fcChildSites.IsFiltered}", 1));
            }
            if (fcItemsDocuments.Enabled && fcItemsDocuments.IsFiltered)
            {
                arrayList.Add(new OptionsSummary($"Apply filter on {fcItemsDocuments.TypeHeader} : {fcItemsDocuments.IsFiltered}", 1));
            }
            if (base.Scope != SharePointObjectScope.Item && fcListsLibraries.Enabled && fcListsLibraries.IsFiltered)
            {
                arrayList.Add(new OptionsSummary($"Apply filter on {fcListsLibraries.TypeHeader} : {fcListsLibraries.IsFiltered}", 1));
            }
            if (base.Scope != SharePointObjectScope.SiteCollection && base.Scope != SharePointObjectScope.Site && fcListColumns.Enabled && fcListColumns.IsFiltered)
            {
                arrayList.Add(new OptionsSummary($"Apply filter on {fcListColumns.TypeHeader} : {fcListColumns.IsFiltered}", 1));
            }
            if ((base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && fcSiteColumns.Enabled && fcSiteColumns.IsFiltered)
            {
                arrayList.Add(new OptionsSummary($"Apply filter on {fcSiteColumns.TypeHeader} : {fcSiteColumns.IsFiltered}", 1));
            }
            if (arrayList.Count > 0)
            {
                arrayList.Insert(0, new OptionsSummary("Filter Options", 0));
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCFilterOptionsBasicView));
            this.tlpFilter = new System.Windows.Forms.TableLayoutPanel();
            this.fcChildSites = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.FilterControlBasicView();
            this.fcItemsDocuments = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.FilterControlBasicView();
            this.fcListsLibraries = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.FilterControlBasicView();
            this.fcListColumns = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.FilterControlBasicView();
            this.fcSiteColumns = new Metalogix.SharePoint.UI.WinForms.Migration.BasicView.FilterControlBasicView();
            this.tlpFilter.SuspendLayout();
            base.SuspendLayout();
            resources.ApplyResources(this.tlpFilter, "tlpFilter");
            this.tlpFilter.Controls.Add(this.fcChildSites, 0, 0);
            this.tlpFilter.Controls.Add(this.fcItemsDocuments, 0, 1);
            this.tlpFilter.Controls.Add(this.fcListsLibraries, 0, 2);
            this.tlpFilter.Controls.Add(this.fcListColumns, 0, 3);
            this.tlpFilter.Controls.Add(this.fcSiteColumns, 0, 4);
            this.tlpFilter.Name = "tlpFilter";
            this.fcChildSites.Filters = null;
            this.fcChildSites.IsFilterExpressionEditorDialog = false;
            resources.ApplyResources(this.fcChildSites, "fcChildSites");
            this.fcChildSites.Name = "fcChildSites";
            this.fcChildSites.SiteFieldsFilter = null;
            this.fcChildSites.SourceList = null;
            this.fcChildSites.SourceNodes = null;
            this.fcChildSites.SourceWeb = null;
            this.fcChildSites.TypeHeader = "Object";
            this.fcItemsDocuments.Filters = null;
            this.fcItemsDocuments.IsFilterExpressionEditorDialog = false;
            resources.ApplyResources(this.fcItemsDocuments, "fcItemsDocuments");
            this.fcItemsDocuments.Name = "fcItemsDocuments";
            this.fcItemsDocuments.SiteFieldsFilter = null;
            this.fcItemsDocuments.SourceList = null;
            this.fcItemsDocuments.SourceNodes = null;
            this.fcItemsDocuments.SourceWeb = null;
            this.fcItemsDocuments.TypeHeader = "Object";
            this.fcListsLibraries.Filters = null;
            this.fcListsLibraries.IsFilterExpressionEditorDialog = false;
            resources.ApplyResources(this.fcListsLibraries, "fcListsLibraries");
            this.fcListsLibraries.Name = "fcListsLibraries";
            this.fcListsLibraries.SiteFieldsFilter = null;
            this.fcListsLibraries.SourceList = null;
            this.fcListsLibraries.SourceNodes = null;
            this.fcListsLibraries.SourceWeb = null;
            this.fcListsLibraries.TypeHeader = "Object";
            this.fcListColumns.Filters = null;
            this.fcListColumns.IsFilterExpressionEditorDialog = false;
            resources.ApplyResources(this.fcListColumns, "fcListColumns");
            this.fcListColumns.Name = "fcListColumns";
            this.fcListColumns.SiteFieldsFilter = null;
            this.fcListColumns.SourceList = null;
            this.fcListColumns.SourceNodes = null;
            this.fcListColumns.SourceWeb = null;
            this.fcListColumns.TypeHeader = "Object";
            this.fcSiteColumns.Filters = null;
            this.fcSiteColumns.IsFilterExpressionEditorDialog = false;
            resources.ApplyResources(this.fcSiteColumns, "fcSiteColumns");
            this.fcSiteColumns.Name = "fcSiteColumns";
            this.fcSiteColumns.SiteFieldsFilter = null;
            this.fcSiteColumns.SourceList = null;
            this.fcSiteColumns.SourceNodes = null;
            this.fcSiteColumns.SourceWeb = null;
            this.fcSiteColumns.TypeHeader = "Object";
            resources.ApplyResources(this, "$this");
            base.Controls.Add(this.tlpFilter);
            base.Name = "TCFilterOptionsBasicView";
            base.Load += new System.EventHandler(TCFilterOptions_Load);
            this.tlpFilter.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsAnyFilterOptionsSelected()
        {
            if (fcChildSites.IsFiltered || fcItemsDocuments.IsFiltered || fcListsLibraries.IsFiltered || fcListColumns.IsFiltered)
            {
                return true;
            }
            return fcSiteColumns.IsFiltered;
        }

        protected override void LoadUI()
        {
            fcChildSites.IsEventSuppresed = true;
            fcListsLibraries.IsEventSuppresed = true;
            fcItemsDocuments.IsEventSuppresed = true;
            fcListColumns.IsEventSuppresed = true;
            fcSiteColumns.IsEventSuppresed = true;
            fcChildSites.IsFiltered = _options.FilterSites;
            fcChildSites.Filters = FilterExpression.ParseExpression(_options.SiteFilterExpression.ToXML());
            fcListsLibraries.IsFiltered = _options.FilterLists;
            fcListsLibraries.Filters = FilterExpression.ParseExpression(_options.ListFilterExpression.ToXML());
            fcItemsDocuments.IsFiltered = _options.FilterItems;
            fcItemsDocuments.Filters = FilterExpression.ParseExpression(_options.ItemFilterExpression.ToXML());
            fcListColumns.IsFiltered = _options.FilterFields;
            fcListColumns.Filters = FilterExpression.ParseExpression(_options.ListFieldsFilterExpression.ToXML());
            fcSiteColumns.IsFiltered = _options.FilterSiteFields;
            fcSiteColumns.SiteFieldsFilter = FilterExpression.ParseExpression(_options.SiteFieldsFilterExpression.ToXML());
            fcListColumns.SourceList = base.SourceList;
            fcListColumns.CurrentColumnMappings = ColumnMappings;
            fcSiteColumns.SourceNodes = SourceNodes;
            fcSiteColumns.SourceWeb = base.SourceWeb;
            fcChildSites.IsEventSuppresed = false;
            fcListsLibraries.IsEventSuppresed = false;
            fcItemsDocuments.IsEventSuppresed = false;
            fcListColumns.IsEventSuppresed = false;
            fcSiteColumns.IsEventSuppresed = false;
        }

        public override bool SaveUI()
        {
            _options.FilterSites = fcChildSites.IsFiltered;
            _options.SiteFilterExpression = fcChildSites.Filters;
            _options.FilterLists = fcListsLibraries.IsFiltered;
            _options.ListFilterExpression = fcListsLibraries.Filters;
            _options.FilterItems = fcItemsDocuments.IsFiltered;
            _options.ItemFilterExpression = fcItemsDocuments.Filters;
            _options.FilterFields = fcListColumns.IsFiltered;
            _options.ListFieldsFilterExpression = fcListColumns.Filters;
            _options.FilterSiteFields = fcSiteColumns.IsFiltered;
            _options.SiteFieldsFilterExpression = fcSiteColumns.Filters;
            return true;
        }

        private void TCFilterOptions_Load(object sender, EventArgs e)
        {
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                if (base.SourceWeb != null && base.SourceWeb.Adapter.SharePointVersion.IsSharePoint2003 && base.SourceIsServerNode)
                {
                    fcSiteColumns.Hide();
                }
                fcListColumns.Hide();
                return;
            }
            fcSiteColumns.Hide();
            fcChildSites.Hide();
            if (base.Scope != SharePointObjectScope.List)
            {
                fcListsLibraries.Hide();
            }
            else if (base.MultiSelectUI)
            {
                fcListColumns.Hide();
            }
        }

        private void ToggleSwitched(object sender, CancelEventArgs e)
        {
            if (IsAnyFilterOptionsSelected() && !_isWarningShown)
            {
                e.Cancel = !VerifyUserActionDialog.GetUserVerification(SharePointConfigurationVariables.ShowFilterOptionInformationDialog, Resources.FilterWorkflowsWarning);
                _isWarningShown = true;
            }
        }

        protected override void UpdateEnabledState()
        {
            fcItemsDocuments.Enabled = base.ItemsAvailable;
            fcChildSites.Enabled = base.SitesAvailable;
            fcListsLibraries.Enabled = base.ListsAvailable;
        }
    }
}
