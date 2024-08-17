using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView;
using Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MigrationMode.png")]
    [ControlName("Migration Options")]
    public class TCMigrationOptionsBasicView : MigrationElementsScopabbleTabbableControl
    {
        public delegate void AdvancedOptionsMainControlStateChangedHandler(AdvancedOptionsMainControlStateEventArgs args, object sender);

        public delegate void UpdateNavigationOptionHandler(UpdateOptionsStateEventArgs args, object sender);

        public delegate void UpdateOptionsVisibilityHandler(UpdateOptionsStateEventArgs args, object sender);

        public delegate void UpdatePermissionsStateHandler(EventArgs args, object sender);

        private bool _isSiteCollectionOptionsExpanded;

        private bool _isSiteOptionsExpanded;

        private bool _isListContentOptionsExpanded;

        private AccordionContentContainer _accSiteCollectionOptions;

        private TCSiteCollectionOptionsBasicView _tcSiteCollectionOptionsBasicView;

        private AccordionControlElement _aceSiteCollectionOptions;

        private AccordionContentContainer _accSiteContentOptions;

        private TCSiteOptionsBasicView _tcSiteOptionsBasicView;

        private AccordionControlElement _aceSiteContentOptions;

        private AccordionContentContainer _accListContentOptions;

        private TCListContentOptionsBasicView _tcMigrationOptions;

        private AccordionControlElement _aceListContentOptions;

        private AccordionContentContainer _accListElementOptions;

        private TCListElementOptionsBasicView _tcListElementOptions;

        private AccordionControlElement _aceListElementOptions;

        private AccordionContentContainer _accFilteringOptions;

        private TCFilterOptionsBasicView _tcFilterOptionsBasicView;

        private AccordionControlElement _aceFilteringOptions;

        private bool _hasOnlyExternalLists;

        private bool _isTargetSharePointOnline;

        private SPFilterOptions _filterOptions;

        private SPFolderContentOptions _folderOptions;

        private SPSiteCollectionOptions _siteCollectionOptions;

        private SPMappingOptions _mappingOptions;

        private SPListContentOptions _listContentOptions;

        private SPWebPartOptions _webPartOptions;

        private SPPermissionsOptions _permissionsOptions;

        private SPTaxonomyOptions _mmdOptions;

        private SPWorkflowOptions _workflowOptions;

        private NodeCollection _sourceNodes;

        private NodeCollection _targetNodes;

        private SPSiteContentOptions _siteContentOptions;

        private IContainer components;

        private AccordionControl accordionControl;

        public bool EnableWorkflowSwitch
        {
            set
            {
                _tcListElementOptions.EnableWorkflowSwitch(value);
            }
        }

        public SPFilterOptions FilterOptions
        {
            get
            {
                return _filterOptions;
            }
            set
            {
                _filterOptions = value;
                _tcFilterOptionsBasicView.Options = _filterOptions;
            }
        }

        public SPFolderContentOptions FolderOptions
        {
            get
            {
                return _folderOptions;
            }
            set
            {
                _folderOptions = value;
                _tcMigrationOptions.FolderOptions = _folderOptions;
            }
        }

        public bool IsPreserveListID
        {
            set
            {
                _tcMigrationOptions.IsPreserveListID = value;
            }
        }

        public bool IsSetExplicitOptions
        {
            set
            {
                _tcMigrationOptions.IsSetExplicitOptions = value;
                _tcListElementOptions.IsSetExplicitOptions = value;
                if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
                {
                    _tcSiteOptionsBasicView.IsSetExplicitOptions = value;
                }
            }
        }

        public bool IsTargetSameAsSource => _tcSiteCollectionOptionsBasicView.IsTargetSameAsSource();

        public SPListContentOptions ListContentOptions
        {
            get
            {
                return _listContentOptions;
            }
            set
            {
                _listContentOptions = value;
                _tcMigrationOptions.Options = value;
            }
        }

        public SPMappingOptions MappingOptions
        {
            get
            {
                return _mappingOptions;
            }
            set
            {
                _mappingOptions = value;
                _tcFilterOptionsBasicView.ColumnMappings = _mappingOptions.ColumnMappings;
                _tcListElementOptions.MappingOptions = _mappingOptions;
            }
        }

        public SPTaxonomyOptions MMDOptions
        {
            get
            {
                return _mmdOptions;
            }
            set
            {
                _mmdOptions = value;
                _tcListElementOptions.MMDOptions = _mmdOptions;
            }
        }

        public SPPermissionsOptions PermissionsOptions
        {
            get
            {
                return _permissionsOptions;
            }
            set
            {
                _permissionsOptions = value;
                _tcListElementOptions.PermissionsOptions = _permissionsOptions;
            }
        }

        public SPSiteCollectionOptions SiteCollectionOptions
        {
            get
            {
                return _siteCollectionOptions;
            }
            set
            {
                _siteCollectionOptions = value;
                _tcSiteCollectionOptionsBasicView.Options = _siteCollectionOptions;
            }
        }

        public SPSiteContentOptions SiteContentOptions
        {
            get
            {
                return _siteContentOptions;
            }
            set
            {
                _siteContentOptions = value;
                _tcSiteOptionsBasicView.SiteContentOptions = value;
            }
        }

        public override NodeCollection SourceNodes
        {
            get
            {
                return _sourceNodes;
            }
            set
            {
                _sourceNodes = value;
                _tcFilterOptionsBasicView.SourceNodes = _sourceNodes;
                _tcMigrationOptions.SourceNodes = _sourceNodes;
                _tcListElementOptions.SourceNodes = _sourceNodes;
                if (base.Scope == SharePointObjectScope.SiteCollection)
                {
                    _tcSiteCollectionOptionsBasicView.SourceNodes = _sourceNodes;
                }
                if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
                {
                    _tcSiteOptionsBasicView.SourceNodes = _sourceNodes;
                }
            }
        }

        public override NodeCollection TargetNodes
        {
            get
            {
                return _targetNodes;
            }
            set
            {
                _targetNodes = value;
                _tcMigrationOptions.TargetNodes = _targetNodes;
                _tcListElementOptions.TargetNodes = _targetNodes;
                if (base.Scope == SharePointObjectScope.SiteCollection)
                {
                    _isTargetSharePointOnline = ((SPNode)_targetNodes[0]).Adapter.SharePointVersion.IsSharePointOnline;
                    _tcSiteCollectionOptionsBasicView.IsTargetSharePointOnline = _isTargetSharePointOnline;
                    _accSiteCollectionOptions.Size = (_isTargetSharePointOnline ? new Size(662, 407) : new Size(662, 374));
                    _tcSiteCollectionOptionsBasicView.TargetNodes = _targetNodes;
                }
                if (base.Scope == SharePointObjectScope.Site)
                {
                    _tcSiteOptionsBasicView.TargetNodes = _targetNodes;
                }
            }
        }

        public SPWebPartOptions WebPartOptions
        {
            get
            {
                return _webPartOptions;
            }
            set
            {
                _webPartOptions = value;
                _tcListElementOptions.WebPartOptions = _webPartOptions;
            }
        }

        public SPWorkflowOptions WorkflowOptions
        {
            get
            {
                return _workflowOptions;
            }
            set
            {
                _workflowOptions = value;
                _tcListElementOptions.WorkflowOptions = _workflowOptions;
            }
        }

        public TCMigrationOptionsBasicView(bool hasOnlyExternalLists)
        {
            InitializeComponent();
            _hasOnlyExternalLists = hasOnlyExternalLists;
        }

        private void AddFilterOptions()
        {
            _accFilteringOptions.Controls.Add(_tcFilterOptionsBasicView);
            _accFilteringOptions.Size = new Size(662, 267);
            _tcFilterOptionsBasicView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tcFilterOptionsBasicView.ControlName = "Filter Options";
            _tcFilterOptionsBasicView.Dock = DockStyle.Fill;
            _tcFilterOptionsBasicView.Name = "_tcFilterOptionsBasicView";
            _tcFilterOptionsBasicView.Padding = new Padding(5);
            _aceFilteringOptions.Name = "_aceFilteringOptions";
            _aceFilteringOptions.Text = "Filtering Options";
            SetAccordionControlElement(_aceFilteringOptions, _accFilteringOptions);
        }

        private void AddListContentOptions()
        {
            _accListContentOptions.Controls.Add(_tcMigrationOptions);
            _accListContentOptions.Size = new Size(662, 346);
            _tcMigrationOptions.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tcMigrationOptions.Dock = DockStyle.Fill;
            _tcMigrationOptions.Padding = new Padding(5);
            _tcMigrationOptions.Name = "_tcMigrationOptions";
            _aceListContentOptions.Name = "_aceListContentOptions";
            _aceListContentOptions.Text = ((base.Scope == SharePointObjectScope.Folder) ? "Folder Content Options" : "List Content Options");
            if (base.Scope == SharePointObjectScope.List || base.Scope == SharePointObjectScope.Item || base.Scope == SharePointObjectScope.Folder)
            {
                _aceListContentOptions.Expanded = true;
            }
            SetAccordionControlElement(_aceListContentOptions, _accListContentOptions);
        }

        private void AddListElementOptions()
        {
            _accListElementOptions.Controls.Add(_tcListElementOptions);
            _accListElementOptions.Size = new Size(663, (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) ? 568 : 573);
            _tcListElementOptions.Appearance.BackColor = Color.White;
            _tcListElementOptions.Appearance.Options.UseBackColor = true;
            _tcListElementOptions.AutoSize = true;
            _tcListElementOptions.Dock = DockStyle.Fill;
            _tcListElementOptions.Name = "_tcListElementOptions";
            _tcListElementOptions.Padding = new Padding(7, 14, 0, 0);
            _aceListElementOptions.Name = "_aceListElementOptions";
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                case SharePointObjectScope.Site:
                    _aceListElementOptions.Text = "Site Element Options";
                    break;
                case SharePointObjectScope.List:
                    _aceListElementOptions.Text = "List Element Options";
                    break;
                case SharePointObjectScope.Folder:
                    _aceListElementOptions.Text = "Folder Element Options";
                    break;
                case SharePointObjectScope.Item:
                    _aceListElementOptions.Text = "Item Element Options";
                    break;
            }
            SetAccordionControlElement(_aceListElementOptions, _accListElementOptions);
            _tcMigrationOptions.UpdatePermissionsStateChanged += UpdatePermissionStateChanged;
        }

        private void AddMigrationOptions()
        {
            if (base.Scope == SharePointObjectScope.SiteCollection)
            {
                _accSiteCollectionOptions = new AccordionContentContainer();
                _tcSiteCollectionOptionsBasicView = new TCSiteCollectionOptionsBasicView();
                _aceSiteCollectionOptions = new AccordionControlElement();
                accordionControl.Controls.Add(_accSiteCollectionOptions);
                accordionControl.Elements.Add(_aceSiteCollectionOptions);
                AddSiteCollectionOptions();
                _tcSiteCollectionOptionsBasicView.AdvancedOptionsMainControlStateChanged += SiteCollectionOptionsChangeSize;
            }
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)
            {
                _accSiteContentOptions = new AccordionContentContainer();
                _tcSiteOptionsBasicView = new TCSiteOptionsBasicView();
                _aceSiteContentOptions = new AccordionControlElement();
                accordionControl.Controls.Add(_accSiteContentOptions);
                accordionControl.Elements.Add(_aceSiteContentOptions);
                AddSiteContentOptions();
                _tcSiteOptionsBasicView.NavigationOptionChanged += UpdateOptionsNavigation;
            }
            _accListContentOptions = new AccordionContentContainer();
            _tcMigrationOptions = new TCListContentOptionsBasicView();
            _aceListContentOptions = new AccordionControlElement();
            _accListElementOptions = new AccordionContentContainer();
            _tcListElementOptions = new TCListElementOptionsBasicView();
            _aceListElementOptions = new AccordionControlElement();
            _accFilteringOptions = new AccordionContentContainer();
            _tcFilterOptionsBasicView = new TCFilterOptionsBasicView();
            _aceFilteringOptions = new AccordionControlElement();
            accordionControl.Controls.Add(_accListContentOptions);
            accordionControl.Controls.Add(_accListElementOptions);
            accordionControl.Controls.Add(_accFilteringOptions);
            accordionControl.Elements.Add(_aceListContentOptions);
            accordionControl.Elements.Add(_aceListElementOptions);
            accordionControl.Elements.Add(_aceFilteringOptions);
            AddListContentOptions();
            AddListElementOptions();
            AddFilterOptions();
            _tcMigrationOptions.HasOnlyExternalLists = _hasOnlyExternalLists;
            _tcListElementOptions.HasOnlyExternalLists = _hasOnlyExternalLists;
            _tcMigrationOptions.AdvancedOptionsMainControlStateChanged += ChangeLocation;
            _tcMigrationOptions.OptionsVisibilityChanged += UpdateOptionsVisibility;
            _tcListElementOptions.AdvancedOptionsMainControlStateChanged += ListElementsChangeLocation;
            _tcFilterOptionsBasicView.Scope = base.Scope;
            _tcListElementOptions.Scope = base.Scope;
            _tcMigrationOptions.Scope = base.Scope;
        }

        private void AddSiteCollectionOptions()
        {
            _accSiteCollectionOptions.Controls.Add(_tcSiteCollectionOptionsBasicView);
            _tcSiteCollectionOptionsBasicView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tcSiteCollectionOptionsBasicView.AutoSize = true;
            _tcSiteCollectionOptionsBasicView.Dock = DockStyle.Fill;
            _tcSiteCollectionOptionsBasicView.Name = "_tcSiteCollectionOptionsBasicView";
            _tcSiteCollectionOptionsBasicView.Padding = new Padding(5);
            _aceSiteCollectionOptions.Name = "_aceSiteCollectionOptions";
            _aceSiteCollectionOptions.Text = "Site Collection Options";
            _aceSiteCollectionOptions.Expanded = true;
            SetAccordionControlElement(_aceSiteCollectionOptions, _accSiteCollectionOptions);
        }

        private void AddSiteContentOptions()
        {
            _accSiteContentOptions.Controls.Add(_tcSiteOptionsBasicView);
            _accSiteContentOptions.Size = new Size(662, 176);
            _tcSiteOptionsBasicView.AutoSize = true;
            _tcSiteOptionsBasicView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tcSiteOptionsBasicView.Dock = DockStyle.Fill;
            _tcSiteOptionsBasicView.Padding = new Padding(5);
            _tcSiteOptionsBasicView.Name = "_tcSiteOptionsBasicView";
            _aceSiteContentOptions.Name = "_aceSiteContentOptions";
            _aceSiteContentOptions.Text = "Site Options";
            if (base.Scope == SharePointObjectScope.Site)
            {
                _aceSiteContentOptions.Expanded = true;
            }
            SetAccordionControlElement(_aceSiteContentOptions, _accSiteContentOptions);
            _tcSiteOptionsBasicView.OptionsVisibilityChanged += UpdateOptionsVisibility;
        }

        private void ChangeLocation(AdvancedOptionsMainControlStateEventArgs args, object sender)
        {
            if (args.IsDecreaseSize)
            {
                _accListContentOptions.Height -= args.Size;
            }
            else if (_accListContentOptions.Height != 346)
            {
                _accListContentOptions.Height += args.Size;
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

        public void ExpandSelectedElement()
        {
            if (_isSiteCollectionOptionsExpanded)
            {
                _aceSiteCollectionOptions.Expanded = true;
            }
            else if (_isSiteOptionsExpanded)
            {
                _aceSiteContentOptions.Expanded = true;
            }
            else if (_isListContentOptionsExpanded)
            {
                _aceListContentOptions.Expanded = true;
            }
        }

        public override ArrayList GetSummaryScreenDetails()
        {
            ArrayList arrayList = new ArrayList();
            switch (base.Scope)
            {
                case SharePointObjectScope.SiteCollection:
                    arrayList.AddRange(_tcSiteCollectionOptionsBasicView.GetSummaryScreenDetails());
                    arrayList.AddRange(_tcSiteOptionsBasicView.GetSummaryScreenDetails());
                    break;
                case SharePointObjectScope.Site:
                    arrayList.AddRange(_tcSiteOptionsBasicView.GetSummaryScreenDetails());
                    break;
            }
            arrayList.AddRange(_tcMigrationOptions.GetSummaryScreenDetails());
            arrayList.AddRange(_tcListElementOptions.GetSummaryScreenDetails());
            arrayList.AddRange(_tcFilterOptionsBasicView.GetSummaryScreenDetails());
            return arrayList;
        }

        private void InitializeComponent()
        {
            this.accordionControl = new DevExpress.XtraBars.Navigation.AccordionControl();
            ((System.ComponentModel.ISupportInitialize)this.accordionControl).BeginInit();
            this.accordionControl.SuspendLayout();
            base.SuspendLayout();
            this.accordionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accordionControl.Location = new System.Drawing.Point(0, 0);
            this.accordionControl.Name = "accordionControl";
            this.accordionControl.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Auto;
            this.accordionControl.Size = new System.Drawing.Size(679, 695);
            this.accordionControl.TabIndex = 0;
            this.accordionControl.Text = "accordionControl";
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            base.Controls.Add(this.accordionControl);
            base.Name = "TCMigrationOptionsBasicView";
            base.Size = new System.Drawing.Size(679, 695);
            ((System.ComponentModel.ISupportInitialize)this.accordionControl).EndInit();
            this.accordionControl.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private bool IsOverWriteSites()
        {
            if (base.ParentForm is ScopableLeftNavigableTabsFormBasicView)
            {
                ScopableLeftNavigableTabsFormBasicView scopableLeftNavigableTabsFormBasicView = (ScopableLeftNavigableTabsFormBasicView)base.ParentForm;
                Control[] array = scopableLeftNavigableTabsFormBasicView.Controls.Find("TCMigrationModeOptionsBasicView", searchAllChildren: true);
                if (array.Length > 1 && array[1] is TCMigrationModeOptionsBasicView)
                {
                    TCMigrationModeOptionsBasicView tCMigrationModeOptionsBasicView = (TCMigrationModeOptionsBasicView)array[1];
                    if (tCMigrationModeOptionsBasicView.Options == null)
                    {
                        return false;
                    }
                    return tCMigrationModeOptionsBasicView.Options.OverwriteSites;
                }
            }
            return false;
        }

        private void ListElementsChangeLocation(AdvancedOptionsMainControlStateEventArgs args, object sender)
        {
            if (args.IsDecreaseSize)
            {
                _accListElementOptions.Height -= args.Size;
            }
            else
            {
                _accListElementOptions.Height += args.Size;
            }
        }

        private void MakeFirstAccordionElementVisible()
        {
            AccordionControlElement element = accordionControl.Elements[0];
            accordionControl.MakeElementVisible(element);
        }

        protected override void MultiSelectUISetup()
        {
            _tcFilterOptionsBasicView.MultiSelectUI = base.MultiSelectUI;
        }

        public override bool SaveUI()
        {
            _isSiteCollectionOptionsExpanded = false;
            _isSiteOptionsExpanded = false;
            _isListContentOptionsExpanded = false;
            if (base.Scope == SharePointObjectScope.SiteCollection)
            {
                _tcSiteCollectionOptionsBasicView.IsModeSwitched = base.IsModeSwitched;
                if (!_tcSiteCollectionOptionsBasicView.SaveUI() || (IsOverWriteSites() && _tcSiteCollectionOptionsBasicView.IsTargetSameAsSource()))
                {
                    MakeFirstAccordionElementVisible();
                    _aceSiteCollectionOptions.Expanded = true;
                    _isSiteCollectionOptionsExpanded = true;
                    return false;
                }
            }
            if ((base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site) && !_tcSiteOptionsBasicView.SaveUI())
            {
                MakeFirstAccordionElementVisible();
                _aceSiteContentOptions.Expanded = true;
                _isSiteOptionsExpanded = true;
                return false;
            }
            _tcMigrationOptions.IsModeSwitched = base.IsModeSwitched;
            if (_tcMigrationOptions.SaveUI())
            {
                if (!_tcFilterOptionsBasicView.SaveUI())
                {
                    return false;
                }
                return _tcListElementOptions.SaveUI();
            }
            MakeFirstAccordionElementVisible();
            _aceListContentOptions.Expanded = true;
            _isListContentOptionsExpanded = true;
            return false;
        }

        private void SetAccordionControlElement(AccordionControlElement aceOption, AccordionContentContainer accOption)
        {
            Font font = new Font("Tahoma", 8.25f, FontStyle.Bold);
            Color backColor = Color.FromArgb(0, 114, 206);
            aceOption.Appearance.Hovered.BackColor = backColor;
            aceOption.Appearance.Hovered.Font = font;
            aceOption.Appearance.Hovered.Options.UseBackColor = true;
            aceOption.Appearance.Hovered.Options.UseFont = true;
            aceOption.Appearance.Normal.Font = font;
            aceOption.Appearance.Normal.Options.UseFont = true;
            aceOption.Appearance.Pressed.BackColor = backColor;
            aceOption.Appearance.Pressed.Font = font;
            aceOption.Appearance.Pressed.Options.UseBackColor = true;
            aceOption.Appearance.Pressed.Options.UseFont = true;
            aceOption.ContentContainer = accOption;
            aceOption.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
        }

        private void SiteCollectionOptionsChangeSize(AdvancedOptionsMainControlStateEventArgs args, object sender)
        {
            if (args.IsDecreaseSize)
            {
                _accSiteCollectionOptions.Height -= args.Size;
            }
            else
            {
                _accSiteCollectionOptions.Height += args.Size;
            }
        }

        private void UpdateOptionsNavigation(UpdateOptionsStateEventArgs args, object sender)
        {
            if (!(base.ParentForm is ScopableLeftNavigableTabsFormBasicView))
            {
                return;
            }
            ScopableLeftNavigableTabsFormBasicView scopableLeftNavigableTabsFormBasicView = (ScopableLeftNavigableTabsFormBasicView)base.ParentForm;
            Control[] array = scopableLeftNavigableTabsFormBasicView.Controls.Find("TCGeneralOptionsBasicView", searchAllChildren: true);
            if (array.Length > 1 && array[1] is TCGeneralOptionsBasicView)
            {
                TCGeneralOptionsBasicView tCGeneralOptionsBasicView = (TCGeneralOptionsBasicView)array[1];
                if (args.IsSiteEnabled.HasValue)
                {
                    tCGeneralOptionsBasicView.IsUpdateCorrectLinks = args.IsSiteEnabled.Value;
                }
            }
        }

        private void UpdateOptionsVisibility(UpdateOptionsStateEventArgs args, object sender)
        {
            if (args.IsSiteEnabled.HasValue)
            {
                _tcFilterOptionsBasicView.SitesAvailable = args.IsSiteEnabled.Value;
            }
            if (args.IsListEnabled.HasValue)
            {
                _tcFilterOptionsBasicView.ListsAvailable = args.IsListEnabled.Value;
                _tcListElementOptions.ListsAvailable = args.IsListEnabled.Value;
            }
            if (args.IsItemEnabled.HasValue)
            {
                _tcFilterOptionsBasicView.ItemsAvailable = args.IsItemEnabled.Value;
                _tcListElementOptions.ItemsAvailable = args.IsItemEnabled.Value;
            }
        }

        private void UpdatePermissionStateChanged(EventArgs args, object sender)
        {
            if (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.List)
            {
                _tcListElementOptions.UpdatePermissionsState((bool)sender, ListContentOptions);
            }
            else if (base.Scope == SharePointObjectScope.Folder)
            {
                _tcListElementOptions.UpdateFolderPermissionsState((bool)sender, FolderOptions);
            }
        }

        protected override void UpdateScope()
        {
            AddMigrationOptions();
        }
    }
}