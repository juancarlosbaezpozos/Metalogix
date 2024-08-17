using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping
{
    public class GenericSiteLevelMappingDialog : XtraForm
    {
        public class AdditionalOptionTab
        {
            public string TabName;

            public string ObjectColumnText;

            public string OptionColumnText;

            public int OptionColumnWidth;

            public IEnumerable<BarItem> MenuItems;

            public ContextMenuStrip ContextMenuStrip;

            public AdditionalOptionTab(string sTabName, string sObjectColumnText, string sOptionColumnText, IEnumerable<BarItem> menuItems, ContextMenuStrip cms)
            {
                TabName = sTabName;
                ObjectColumnText = sObjectColumnText;
                OptionColumnText = sOptionColumnText;
                MenuItems = menuItems;
                ContextMenuStrip = cms;
            }
        }

        protected class ContentTypeApplicationOptionsCollectionFilterParser : FilterParserBase
        {
            protected override FilterExpression GetFilterExpression(object obj)
            {
                ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = FilterParserBase.ConvertValue<ContentTypeApplicationOptionsCollection>(obj);
                if (contentTypeApplicationOptionsCollection == null)
                {
                    throw new ArgumentNullException();
                }
                return contentTypeApplicationOptionsCollection.AppliesToFilter as FilterExpression;
            }
        }

        protected class DocumentSetApplicationOptionsCollectionFilterParser : FilterParserBase
        {
            protected override FilterExpression GetFilterExpression(object obj)
            {
                DocumentSetApplicationOptionsCollection documentSetApplicationOptionsCollection = FilterParserBase.ConvertValue<DocumentSetApplicationOptionsCollection>(obj);
                if (documentSetApplicationOptionsCollection == null)
                {
                    throw new ArgumentNullException();
                }
                return documentSetApplicationOptionsCollection.AppliesToFilter as FilterExpression;
            }
        }

        protected abstract class FilterParserBase : IUrlParser
        {
            public virtual bool CanNavigate(object obj, string displayText)
            {
                FilterExpression filterExpression;
                return CanNavigate(obj, displayText, out filterExpression);
            }

            public virtual bool CanNavigate(object obj, string displayText, out FilterExpression filterExpression)
            {
                filterExpression = GetFilterExpression(obj);
                if (filterExpression == null)
                {
                    return false;
                }
                if ((filterExpression.Property.Equals("DisplayUrl", StringComparison.OrdinalIgnoreCase) && filterExpression.Operand == FilterOperand.Equals) || displayText.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return displayText.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            }

            protected static T ConvertValue<T>(object obj)
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }

            protected abstract FilterExpression GetFilterExpression(object obj);

            public virtual string GetNavigationUrl(object obj, string displayText)
            {
                if (!CanNavigate(obj, displayText, out var filterExpression))
                {
                    return string.Empty;
                }
                return filterExpression.Pattern;
            }
        }

        protected interface IUrlParser
        {
            bool CanNavigate(object obj, string displayText);

            string GetNavigationUrl(object obj, string displayText);
        }

        protected class MappingOption : INotifyPropertyChanged
        {
            private string _option;

            private string _optionAppliedValue;

            private bool _eventsBlocked;

            public string Option
            {
                get
                {
                    return _option;
                }
                set
                {
                    if (_option != value)
                    {
                        _option = value;
                        FirePropertyChanged("Option");
                    }
                }
            }

            public string OptionAppliedValue
            {
                get
                {
                    return _optionAppliedValue;
                }
                set
                {
                    if (_optionAppliedValue != value)
                    {
                        _optionAppliedValue = value;
                        FirePropertyChanged("OptionAppliedValue");
                    }
                }
            }

            public object SubObject { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public void BlockEvents()
            {
                _eventsBlocked = true;
            }

            private void FirePropertyChanged(string info)
            {
                if (!_eventsBlocked && this.PropertyChanged != null)
                {
                    PropertyChangedEventArgs e = new PropertyChangedEventArgs(info);
                    this.PropertyChanged(this, e);
                }
            }

            public void UnblockEvents()
            {
                _eventsBlocked = false;
            }
        }

        protected class PlainTextParser : IUrlParser
        {
            public bool CanNavigate(object obj, string displayText)
            {
                return Uri.IsWellFormedUriString(ParseDisplayText(displayText), UriKind.Absolute);
            }

            public string GetNavigationUrl(object obj, string displayText)
            {
                displayText = ParseDisplayText(displayText);
                if (!Uri.IsWellFormedUriString(displayText, UriKind.Absolute))
                {
                    return string.Empty;
                }
                return displayText;
            }

            private static string ParseDisplayText(string displayText)
            {
                if (string.IsNullOrEmpty(displayText))
                {
                    return "";
                }
                int num = displayText.IndexOf("http", StringComparison.OrdinalIgnoreCase);
                if (num >= 0)
                {
                    displayText = displayText.Substring(num, displayText.Length - num);
                }
                return displayText;
            }
        }

        protected class TransformationTaskFilterParser : FilterParserBase
        {
            protected override FilterExpression GetFilterExpression(object obj)
            {
                TransformationTask transformationTask = FilterParserBase.ConvertValue<TransformationTask>(obj);
                if (transformationTask == null)
                {
                    throw new ArgumentNullException();
                }
                return transformationTask.ApplyTo as FilterExpression;
            }
        }

        protected const string COLUMN_OPTION = "Option";

        protected const string COLUMN_APPLIED_VALUE = "OptionAppliedValue";

        protected const int DEFAULT_COLUMN_WIDTH_OPTION = 70;

        protected const int DEFAULT_COLUMN_WIDTH_APPLIED = 190;

        private DevExpress.XtraTab.XtraTabControl w_tabbedControl;

        private NodeCollection m_nodeCollection;

        private bool usingDefaultOptionAndValueColumns = true;

        private bool confirmRemoval;

        protected IUrlParser _urlParser;

        public List<AdditionalOptionTab> AdditionalTabs;

        private bool m_bAllowRuleModification = true;

        private bool m_bSouceIsListItem;

        private bool m_bCreateRuleCmbButtonVisible;

        private string m_sOriginalTabText = "Lists";

        private string m_sDialogTitle = "Title";

        private string m_sApplyOptionText = "Apply Option";

        private string m_sCreateRuleText = "Create Rule";

        private string m_sOptionsLabelText = "List of Options";

        private string m_sOptionColumnText = "Source Object";

        private string m_sOptionAppliedValuesText = "Applied Values";

        private string m_sEditOptionText = "Edit Option";

        private string m_sRemoveOptionText = "Remove Option";

        private string m_sCreateNewOptionText = "Apply Option";

        private string m_sCreateOptionToListText = "Apply Option to List";

        private List<Type> m_ExplorerSelectableTypes;

        private BindingList<MappingOption> _mappingList;

        private IContainer components;

        protected ExplorerControl w_ecwlExplorer;

        private XtraSizableSplitter w_spDividedContainer;

        private PanelControl w_plControlButtonPanel;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        private ContextMenuStrip w_cmsExplorerMenu;

        private ToolStripMenuItem applyOptionsToolStripMenuItem;

        private ToolStripMenuItem refreshToolStripMenuItem;

        private ToolStripMenuItem propertiesToolStripMenuItem;

        private ContextMenuStrip w_cmsOptionsMenu;

        private ToolStripMenuItem editOptionsToolStripMenuItem;

        private ToolStripMenuItem removeOptionsToolStripMenuItem;

        private ToolStripMenuItem navigateToToolStripMenuItem;

        private LabelControl w_lblOptionsView;

        private ToolStripMenuItem editRuleToolStripMenuitem;

        private ToolStripMenuItem applyListRuleToolStripMenuItem;

        private StandaloneBarDockControl _topBarDock;

        private StandaloneBarDockControl _mappingBarDock;

        private XtraBarManagerWithArrows _barManager;

        private Bar _explorerMenuBar;

        private BarButtonItem _createAppliedOptionButton;

        private Bar _mappingsMenuBar;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        protected BarButtonItem _createAppliedRuleButton;

        private BarButtonItem _createRuleButton;

        private BarButtonItem _applyListRuleButton;

        private BarButtonItem _applyRuleButton;

        private PopupMenu _createRuleMenu;

        private BarButtonItem _editRuleButton;

        private BarButtonItem _editTextButton;

        private BarButtonItem _removeButton;

        private BarButtonItem _navigateButton;

        private PanelControl _separatorPanel;

        private SimplifiedGridView _optionsView;

        public bool AllowRuleModification
        {
            get
            {
                return m_bAllowRuleModification;
            }
            set
            {
                m_bAllowRuleModification = value;
            }
        }

        public string ApplyOptionText
        {
            get
            {
                return m_sApplyOptionText;
            }
            set
            {
                m_sApplyOptionText = value;
            }
        }

        public bool ConfirmRemoval
        {
            get
            {
                return confirmRemoval;
            }
            set
            {
                confirmRemoval = value;
            }
        }

        public string CreateNewOptionText
        {
            get
            {
                return m_sCreateNewOptionText;
            }
            set
            {
                m_sCreateNewOptionText = value;
            }
        }

        public string CreateOptionToListText
        {
            get
            {
                return m_sCreateOptionToListText;
            }
            set
            {
                m_sCreateOptionToListText = value;
            }
        }

        public bool CreateRuleCmbButtonVisible
        {
            get
            {
                return m_bCreateRuleCmbButtonVisible;
            }
            set
            {
                m_bCreateRuleCmbButtonVisible = value;
            }
        }

        public string CreateRuleText
        {
            get
            {
                return m_sCreateRuleText;
            }
            set
            {
                m_sCreateRuleText = value;
            }
        }

        public string DialogTitle
        {
            get
            {
                return m_sDialogTitle;
            }
            set
            {
                m_sDialogTitle = value;
            }
        }

        public string EditOptionText
        {
            get
            {
                return m_sEditOptionText;
            }
            set
            {
                m_sEditOptionText = value;
            }
        }

        public List<Type> ExplorerSelectableTypes
        {
            set
            {
                m_ExplorerSelectableTypes = value;
            }
        }

        protected BindingList<MappingOption> MappingList
        {
            get
            {
                return _mappingList;
            }
            set
            {
                _mappingList = value;
                _optionsView.DataSource = value;
            }
        }

        public NodeCollection NodeCollection
        {
            get
            {
                if (m_nodeCollection == null)
                {
                    return null;
                }
                return m_nodeCollection;
            }
        }

        public string OptionAppliedValuesText
        {
            get
            {
                return m_sOptionAppliedValuesText;
            }
            set
            {
                m_sOptionAppliedValuesText = value;
            }
        }

        public string OptionColumnText
        {
            get
            {
                return m_sOptionColumnText;
            }
            set
            {
                m_sOptionColumnText = value;
            }
        }

        protected SimplifiedGridView OptionsGridView => _optionsView;

        public string OptionsLabelText
        {
            get
            {
                return m_sOptionsLabelText;
            }
            set
            {
                m_sOptionsLabelText = value;
            }
        }

        public string OriginalTabText
        {
            get
            {
                return m_sOriginalTabText;
            }
            set
            {
                m_sOriginalTabText = value;
            }
        }

        public string RemoveOptionText
        {
            get
            {
                return m_sRemoveOptionText;
            }
            set
            {
                m_sRemoveOptionText = value;
            }
        }

        public bool SelectedItemCanBeNavigatedTo
        {
            get
            {
                try
                {
                    bool result = false;
                    MappingOption[] selectedOptions = GetSelectedOptions();
                    if (selectedOptions.Length != 0)
                    {
                        MappingOption mappingOption = selectedOptions[0];
                        result = UrlParser.CanNavigate(mappingOption.SubObject, mappingOption.Option);
                    }
                    return result;
                }
                catch (Exception exc)
                {
                    GlobalServices.ErrorHandler.HandleException(exc);
                    return false;
                }
            }
        }

        public ExplorerTreeNode SelectedNode
        {
            get
            {
                if (w_ecwlExplorer.SelectedNode == null)
                {
                    return null;
                }
                return w_ecwlExplorer.SelectedNode;
            }
        }

        public bool SourceIsListItem
        {
            get
            {
                return m_bSouceIsListItem;
            }
            set
            {
                m_bSouceIsListItem = value;
            }
        }

        protected virtual IUrlParser UrlParser { get; private set; }

        public bool UsingDefaultOptionAndValueColumns => usingDefaultOptionAndValueColumns;

        public GenericSiteLevelMappingDialog()
        {
            AdditionalTabs = new List<AdditionalOptionTab>();
            InitializeComponent();
            InitializeOptionsDataSource();
        }

        public GenericSiteLevelMappingDialog(NodeCollection nodeCollection)
            : this()
        {
            m_nodeCollection = nodeCollection;
            w_ecwlExplorer.DataSource = nodeCollection;
            w_ecwlExplorer.Select();
        }

        private void _applyListRuleButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (w_ecwlExplorer.SelectedNodes.Count == 1)
            {
                CreateListRule();
            }
        }

        private void _applyRuleButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateRule();
        }

        private void _createAppliedOptionButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Handle_CreateNewApplyOptionsClick();
        }

        private void _createAppliedRuleButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreateRule();
        }

        private void _editRuleButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Handle_EditRuleClick();
        }

        private void _editTextButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Handle_EditOptionClick();
        }

        private void _navigateButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Handle_NavigationOptionClick();
        }

        private void _optionsView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void _removeButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Handle_RemoveOptionClick();
        }

        protected void AddOption(MappingOption option)
        {
            MappingList.Add(option);
        }

        protected virtual void CheckValidNode(ExplorerTreeNode treeNode, ref bool bEnabledCreateNodeOption, ref bool bEnabledCreateListRuleOption)
        {
            throw new NotImplementedException();
        }

        protected void ClearOptions()
        {
            MappingList.Clear();
        }

        protected virtual void CreateListRule()
        {
            throw new NotImplementedException();
        }

        private void CreateMenuBar(StandaloneBarDockControl dock, IEnumerable<BarItem> items)
        {
            Bar bar = new Bar();
            _barManager.DockControls.Add(dock);
            _barManager.Bars.Add(bar);
            bar.BarItemVertIndent = 0;
            bar.CanDockStyle = BarCanDockStyle.Standalone;
            bar.DockCol = 0;
            bar.DockRow = 0;
            bar.DockStyle = BarDockStyle.Standalone;
            bar.OptionsBar.AllowQuickCustomization = false;
            bar.OptionsBar.DrawBorder = false;
            bar.OptionsBar.DrawDragBorder = false;
            bar.OptionsBar.UseWholeRow = true;
            bar.StandaloneBarDockControl = dock;
            foreach (BarItem item in items)
            {
                _barManager.Items.Add(item);
                bar.ItemLinks.Add(item);
            }
        }

        protected virtual void CreateNewApplyOptions(ExplorerTreeNode treeNode)
        {
            throw new NotImplementedException();
        }

        protected virtual void CreateRule()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void EditOptionAppliedValue(MappingOption item)
        {
            throw new NotImplementedException();
        }

        protected virtual void EditRule(MappingOption item)
        {
            throw new NotImplementedException();
        }

        protected virtual void FormClosingConfirmation(ref FormClosingEventArgs e)
        {
        }

        private void Generic_CreateNewApplyOptions_OnClick(object sender, EventArgs e)
        {
            Handle_CreateNewApplyOptionsClick();
        }

        private void Generic_EditOption_OnClick(object sender, EventArgs e)
        {
            Handle_EditOptionClick();
        }

        private void Generic_EditRule_OnClick(object sender, EventArgs e)
        {
            Handle_EditRuleClick();
        }

        private void Generic_Navigate_OnClick(object sender, EventArgs e)
        {
            Handle_NavigationOptionClick();
        }

        private void Generic_RemoveOption_OnClick(object sender, EventArgs e)
        {
            Handle_RemoveOptionClick();
        }

        protected MappingOption[] GetSelectedOptions()
        {
            return _optionsView.SelectedItems.Select((object item) => (MappingOption)item).ToArray();
        }

        private void Handle_CreateNewApplyOptionsClick()
        {
            if (w_ecwlExplorer.SelectedNode != null)
            {
                CreateNewApplyOptions(w_ecwlExplorer.SelectedNode);
            }
        }

        private void Handle_EditOptionClick()
        {
            MappingOption[] selectedOptions = GetSelectedOptions();
            if (selectedOptions.Length == 1)
            {
                EditOptionAppliedValue(selectedOptions[0]);
            }
        }

        private void Handle_EditRuleClick()
        {
            MappingOption[] selectedOptions = GetSelectedOptions();
            if (selectedOptions.Length == 1)
            {
                EditRule(selectedOptions[0]);
            }
        }

        private void Handle_NavigationOptionClick()
        {
            MappingOption[] selectedOptions = GetSelectedOptions();
            if (selectedOptions.Length == 1)
            {
                NavigateTo(selectedOptions[0]);
            }
        }

        private void Handle_RemoveOptionClick()
        {
            MappingOption[] selectedOptions = GetSelectedOptions();
            if (selectedOptions.Length != 0 && (!confirmRemoval || FlatXtraMessageBox.Show(Resources.GSLMDConfirmRemoveMsg, Resources.GSLMDConfirmRemoveCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                MappingOption[] array = selectedOptions;
                for (int i = 0; i < array.Length; i++)
                {
                    RemoveOption(array[i]);
                }
            }
        }

        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.Mapping.GenericSiteLevelMappingDialog));
            this.w_cmsExplorerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyListRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.w_cmsOptionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editRuleToolStripMenuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.editOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigateToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.w_plControlButtonPanel = new DevExpress.XtraEditors.PanelControl();
            this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
            this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_spDividedContainer = new Metalogix.UI.WinForms.Components.XtraSizableSplitter();
            this.w_ecwlExplorer = new Metalogix.UI.WinForms.Widgets.ExplorerControl();
            this._topBarDock = new DevExpress.XtraBars.StandaloneBarDockControl();
            this._optionsView = new Metalogix.UI.WinForms.Components.SimplifiedGridView();
            this._mappingBarDock = new DevExpress.XtraBars.StandaloneBarDockControl();
            this.w_lblOptionsView = new DevExpress.XtraEditors.LabelControl();
            this._barManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
            this._explorerMenuBar = new DevExpress.XtraBars.Bar();
            this._createAppliedOptionButton = new DevExpress.XtraBars.BarButtonItem();
            this._createAppliedRuleButton = new DevExpress.XtraBars.BarButtonItem();
            this._createRuleButton = new DevExpress.XtraBars.BarButtonItem();
            this._createRuleMenu = new DevExpress.XtraBars.PopupMenu(this.components);
            this._applyListRuleButton = new DevExpress.XtraBars.BarButtonItem();
            this._applyRuleButton = new DevExpress.XtraBars.BarButtonItem();
            this._mappingsMenuBar = new DevExpress.XtraBars.Bar();
            this._editRuleButton = new DevExpress.XtraBars.BarButtonItem();
            this._editTextButton = new DevExpress.XtraBars.BarButtonItem();
            this._removeButton = new DevExpress.XtraBars.BarButtonItem();
            this._navigateButton = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this._separatorPanel = new DevExpress.XtraEditors.PanelControl();
            this.w_cmsExplorerMenu.SuspendLayout();
            this.w_cmsOptionsMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_plControlButtonPanel).BeginInit();
            this.w_plControlButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_spDividedContainer).BeginInit();
            this.w_spDividedContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._createRuleMenu).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this._separatorPanel).BeginInit();
            base.SuspendLayout();
            System.Windows.Forms.ToolStripItemCollection items = this.w_cmsExplorerMenu.Items;
            System.Windows.Forms.ToolStripItem[] toolStripItems = new System.Windows.Forms.ToolStripItem[4] { this.applyOptionsToolStripMenuItem, this.applyListRuleToolStripMenuItem, this.refreshToolStripMenuItem, this.propertiesToolStripMenuItem };
            items.AddRange(toolStripItems);
            this.w_cmsExplorerMenu.Name = "w_cmsExplorerMenu";
            resources.ApplyResources(this.w_cmsExplorerMenu, "w_cmsExplorerMenu");
            this.applyOptionsToolStripMenuItem.Name = "applyOptionsToolStripMenuItem";
            resources.ApplyResources(this.applyOptionsToolStripMenuItem, "applyOptionsToolStripMenuItem");
            this.applyOptionsToolStripMenuItem.Click += new System.EventHandler(Generic_CreateNewApplyOptions_OnClick);
            this.applyListRuleToolStripMenuItem.Name = "applyListRuleToolStripMenuItem";
            resources.ApplyResources(this.applyListRuleToolStripMenuItem, "applyListRuleToolStripMenuItem");
            this.applyListRuleToolStripMenuItem.Click += new System.EventHandler(w_tsmiCreateListRule_Click);
            this.refreshToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Refresh16;
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
            this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
            this.propertiesToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Properties16;
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            resources.ApplyResources(this.propertiesToolStripMenuItem, "propertiesToolStripMenuItem");
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(propertiesToolStripMenuItem_Click);
            System.Windows.Forms.ToolStripItemCollection items2 = this.w_cmsOptionsMenu.Items;
            System.Windows.Forms.ToolStripItem[] toolStripItems2 = new System.Windows.Forms.ToolStripItem[4] { this.editRuleToolStripMenuitem, this.editOptionsToolStripMenuItem, this.removeOptionsToolStripMenuItem, this.navigateToToolStripMenuItem };
            items2.AddRange(toolStripItems2);
            this.w_cmsOptionsMenu.Name = "w_cmsListViewMenu";
            resources.ApplyResources(this.w_cmsOptionsMenu, "w_cmsOptionsMenu");
            this.editRuleToolStripMenuitem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
            this.editRuleToolStripMenuitem.Name = "editRuleToolStripMenuitem";
            resources.ApplyResources(this.editRuleToolStripMenuitem, "editRuleToolStripMenuitem");
            this.editRuleToolStripMenuitem.Click += new System.EventHandler(Generic_EditRule_OnClick);
            this.editOptionsToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
            this.editOptionsToolStripMenuItem.Name = "editOptionsToolStripMenuItem";
            resources.ApplyResources(this.editOptionsToolStripMenuItem, "editOptionsToolStripMenuItem");
            this.editOptionsToolStripMenuItem.Click += new System.EventHandler(Generic_EditOption_OnClick);
            this.removeOptionsToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Minus16;
            this.removeOptionsToolStripMenuItem.Name = "removeOptionsToolStripMenuItem";
            resources.ApplyResources(this.removeOptionsToolStripMenuItem, "removeOptionsToolStripMenuItem");
            this.removeOptionsToolStripMenuItem.Click += new System.EventHandler(Generic_RemoveOption_OnClick);
            this.navigateToToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Navigate16;
            this.navigateToToolStripMenuItem.Name = "navigateToToolStripMenuItem";
            resources.ApplyResources(this.navigateToToolStripMenuItem, "navigateToToolStripMenuItem");
            this.navigateToToolStripMenuItem.Click += new System.EventHandler(Generic_Navigate_OnClick);
            this.w_plControlButtonPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plControlButtonPanel.Controls.Add(this.w_bOkay);
            this.w_plControlButtonPanel.Controls.Add(this.w_bCancel);
            resources.ApplyResources(this.w_plControlButtonPanel, "w_plControlButtonPanel");
            this.w_plControlButtonPanel.Name = "w_plControlButtonPanel";
            resources.ApplyResources(this.w_bOkay, "w_bOkay");
            this.w_bOkay.Name = "w_bOkay";
            this.w_bOkay.Click += new System.EventHandler(w_bOkay_Click);
            resources.ApplyResources(this.w_bCancel, "w_bCancel");
            this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_bCancel.Name = "w_bCancel";
            this.w_bCancel.Click += new System.EventHandler(w_bCancel_Click);
            resources.ApplyResources(this.w_spDividedContainer, "w_spDividedContainer");
            this.w_spDividedContainer.Horizontal = false;
            this.w_spDividedContainer.Name = "w_spDividedContainer";
            this.w_spDividedContainer.Panel1.Controls.Add(this.w_ecwlExplorer);
            this.w_spDividedContainer.Panel1.Controls.Add(this._topBarDock);
            this.w_spDividedContainer.Panel2.Controls.Add(this._optionsView);
            this.w_spDividedContainer.Panel2.Controls.Add(this._mappingBarDock);
            this.w_spDividedContainer.Panel2.Controls.Add(this.w_lblOptionsView);
            this.w_spDividedContainer.SplitterPosition = 209;
            this.w_spDividedContainer.SplitterWidth = 2;
            this.w_ecwlExplorer.Actions = new Metalogix.Actions.Action[0];
            this.w_ecwlExplorer.BackColor = System.Drawing.Color.Transparent;
            this.w_ecwlExplorer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.w_ecwlExplorer.CheckBoxes = false;
            this.w_ecwlExplorer.ContextMenuStrip = this.w_cmsExplorerMenu;
            this.w_ecwlExplorer.DataSource = null;
            resources.ApplyResources(this.w_ecwlExplorer, "w_ecwlExplorer");
            this.w_ecwlExplorer.MultiSelectEnabled = false;
            this.w_ecwlExplorer.MultiSelectLimitationMethod = null;
            this.w_ecwlExplorer.Name = "w_ecwlExplorer";
            this.w_ecwlExplorer.SelectedNodeChanged += new Metalogix.UI.WinForms.Widgets.ExplorerControl.SelectedNodeChangedHandler(w_ecwlExplorer_SelectedNodeChanged);
            this._topBarDock.CausesValidation = false;
            resources.ApplyResources(this._topBarDock, "_topBarDock");
            this._topBarDock.Name = "_topBarDock";
            this._optionsView.ColumnAutoWidth = false;
            this._optionsView.DataSource = null;
            resources.ApplyResources(this._optionsView, "_optionsView");
            this._optionsView.GridContextMenu = this.w_cmsOptionsMenu;
            this._optionsView.MultiSelect = false;
            this._optionsView.Name = "_optionsView";
            this._optionsView.SelectedItems = new object[0];
            this._optionsView.ShowColumnHeaders = true;
            this._optionsView.ShowGridLines = DevExpress.Utils.DefaultBoolean.False;
            this._optionsView.SelectionChanged += new System.EventHandler(_optionsView_SelectionChanged);
            this._mappingBarDock.CausesValidation = false;
            resources.ApplyResources(this._mappingBarDock, "_mappingBarDock");
            this._mappingBarDock.Name = "_mappingBarDock";
            resources.ApplyResources(this.w_lblOptionsView, "w_lblOptionsView");
            this.w_lblOptionsView.Name = "w_lblOptionsView";
            DevExpress.XtraBars.Bars bars = this._barManager.Bars;
            DevExpress.XtraBars.Bar[] bars2 = new DevExpress.XtraBars.Bar[2] { this._explorerMenuBar, this._mappingsMenuBar };
            bars.AddRange(bars2);
            this._barManager.DockControls.Add(this.barDockControlTop);
            this._barManager.DockControls.Add(this.barDockControlBottom);
            this._barManager.DockControls.Add(this.barDockControlLeft);
            this._barManager.DockControls.Add(this.barDockControlRight);
            this._barManager.DockControls.Add(this._topBarDock);
            this._barManager.DockControls.Add(this._mappingBarDock);
            this._barManager.Form = this;
            DevExpress.XtraBars.BarItems items3 = this._barManager.Items;
            DevExpress.XtraBars.BarItem[] items4 = new DevExpress.XtraBars.BarItem[9] { this._createAppliedOptionButton, this._createAppliedRuleButton, this._createRuleButton, this._applyListRuleButton, this._applyRuleButton, this._editRuleButton, this._editTextButton, this._removeButton, this._navigateButton };
            items3.AddRange(items4);
            this._barManager.MaxItemId = 9;
            this._explorerMenuBar.BarItemVertIndent = 0;
            this._explorerMenuBar.BarName = "Explorer Menu";
            this._explorerMenuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Standalone;
            this._explorerMenuBar.DockCol = 0;
            this._explorerMenuBar.DockRow = 0;
            this._explorerMenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this._explorerMenuBar.FloatLocation = new System.Drawing.Point(301, 190);
            DevExpress.XtraBars.LinksInfo linksPersistInfo = this._explorerMenuBar.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] links = new DevExpress.XtraBars.LinkPersistInfo[3]
            {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._createAppliedOptionButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._createAppliedRuleButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._createRuleButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksPersistInfo.AddRange(links);
            this._explorerMenuBar.OptionsBar.AllowQuickCustomization = false;
            this._explorerMenuBar.OptionsBar.DrawBorder = false;
            this._explorerMenuBar.OptionsBar.DrawDragBorder = false;
            this._explorerMenuBar.OptionsBar.UseWholeRow = true;
            this._explorerMenuBar.StandaloneBarDockControl = this._topBarDock;
            resources.ApplyResources(this._explorerMenuBar, "_explorerMenuBar");
            resources.ApplyResources(this._createAppliedOptionButton, "_createAppliedOptionButton");
            this._createAppliedOptionButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
            this._createAppliedOptionButton.Id = 0;
            this._createAppliedOptionButton.Name = "_createAppliedOptionButton";
            this._createAppliedOptionButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_createAppliedOptionButton_ItemClick);
            resources.ApplyResources(this._createAppliedRuleButton, "_createAppliedRuleButton");
            this._createAppliedRuleButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
            this._createAppliedRuleButton.Id = 1;
            this._createAppliedRuleButton.Name = "_createAppliedRuleButton";
            this._createAppliedRuleButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_createAppliedRuleButton_ItemClick);
            this._createRuleButton.ActAsDropDown = true;
            this._createRuleButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            resources.ApplyResources(this._createRuleButton, "_createRuleButton");
            this._createRuleButton.DropDownControl = this._createRuleMenu;
            this._createRuleButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
            this._createRuleButton.Id = 2;
            this._createRuleButton.Name = "_createRuleButton";
            this._createRuleButton.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            DevExpress.XtraBars.LinksInfo linksPersistInfo2 = this._createRuleMenu.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] links2 = new DevExpress.XtraBars.LinkPersistInfo[2]
            {
            new DevExpress.XtraBars.LinkPersistInfo(this._applyListRuleButton),
            new DevExpress.XtraBars.LinkPersistInfo(this._applyRuleButton)
            };
            linksPersistInfo2.AddRange(links2);
            this._createRuleMenu.Manager = this._barManager;
            this._createRuleMenu.Name = "_createRuleMenu";
            resources.ApplyResources(this._applyListRuleButton, "_applyListRuleButton");
            this._applyListRuleButton.Id = 3;
            this._applyListRuleButton.Name = "_applyListRuleButton";
            this._applyListRuleButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_applyListRuleButton_ItemClick);
            resources.ApplyResources(this._applyRuleButton, "_applyRuleButton");
            this._applyRuleButton.Id = 4;
            this._applyRuleButton.Name = "_applyRuleButton";
            this._applyRuleButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_applyRuleButton_ItemClick);
            this._mappingsMenuBar.BarItemVertIndent = 0;
            this._mappingsMenuBar.BarName = "Mappings Menu";
            this._mappingsMenuBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Standalone;
            this._mappingsMenuBar.DockCol = 0;
            this._mappingsMenuBar.DockRow = 0;
            this._mappingsMenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
            this._mappingsMenuBar.FloatLocation = new System.Drawing.Point(324, 525);
            DevExpress.XtraBars.LinksInfo linksPersistInfo3 = this._mappingsMenuBar.LinksPersistInfo;
            DevExpress.XtraBars.LinkPersistInfo[] links3 = new DevExpress.XtraBars.LinkPersistInfo[4]
            {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._editRuleButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._editTextButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._removeButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this._navigateButton, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)
            };
            linksPersistInfo3.AddRange(links3);
            this._mappingsMenuBar.OptionsBar.AllowQuickCustomization = false;
            this._mappingsMenuBar.OptionsBar.DrawBorder = false;
            this._mappingsMenuBar.OptionsBar.DrawDragBorder = false;
            this._mappingsMenuBar.OptionsBar.UseWholeRow = true;
            this._mappingsMenuBar.StandaloneBarDockControl = this._mappingBarDock;
            resources.ApplyResources(this._mappingsMenuBar, "_mappingsMenuBar");
            resources.ApplyResources(this._editRuleButton, "_editRuleButton");
            this._editRuleButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
            this._editRuleButton.Id = 5;
            this._editRuleButton.Name = "_editRuleButton";
            this._editRuleButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_editRuleButton_ItemClick);
            resources.ApplyResources(this._editTextButton, "_editTextButton");
            this._editTextButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
            this._editTextButton.Id = 6;
            this._editTextButton.Name = "_editTextButton";
            this._editTextButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_editTextButton_ItemClick);
            resources.ApplyResources(this._removeButton, "_removeButton");
            this._removeButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Minus16;
            this._removeButton.Id = 7;
            this._removeButton.Name = "_removeButton";
            this._removeButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_removeButton_ItemClick);
            resources.ApplyResources(this._navigateButton, "_navigateButton");
            this._navigateButton.Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Navigate16;
            this._navigateButton.Id = 8;
            this._navigateButton.Name = "_navigateButton";
            this._navigateButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(_navigateButton_ItemClick);
            this.barDockControlTop.CausesValidation = false;
            resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
            this.barDockControlBottom.CausesValidation = false;
            resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            this.barDockControlLeft.CausesValidation = false;
            resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            this.barDockControlRight.CausesValidation = false;
            resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
            this._separatorPanel.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("_separatorPanel.Appearance.BackColor");
            this._separatorPanel.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this._separatorPanel, "_separatorPanel");
            this._separatorPanel.Name = "_separatorPanel";
            base.AcceptButton = this.w_bOkay;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.Controls.Add(this.w_spDividedContainer);
            base.Controls.Add(this._separatorPanel);
            base.Controls.Add(this.w_plControlButtonPanel);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "GenericSiteLevelMappingDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(On_FormClosing);
            this.w_cmsExplorerMenu.ResumeLayout(false);
            this.w_cmsOptionsMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_plControlButtonPanel).EndInit();
            this.w_plControlButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.w_spDividedContainer).EndInit();
            this.w_spDividedContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._createRuleMenu).EndInit();
            ((System.ComponentModel.ISupportInitialize)this._separatorPanel).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeOptionsDataSource()
        {
            _optionsView.RowBuilderMethod = UpdateDataRow;
            _optionsView.CreateColumn("Option", "Option", 70);
            _optionsView.CreateColumn("OptionAppliedValue", "OptionAppliedValue", 190);
            MappingList = new BindingList<MappingOption>();
        }

        protected virtual void LoadOptionsUI()
        {
            throw new NotImplementedException();
        }

        protected void NavigateTo(MappingOption item)
        {
            try
            {
                Node nodeByUrl = NodeCollection.GetNodeByUrl(item.Option);
                if (nodeByUrl == null && UrlParser.CanNavigate(item.SubObject, item.Option))
                {
                    nodeByUrl = NodeCollection.GetNodeByUrl(UrlParser.GetNavigationUrl(item.SubObject, item.Option));
                }
                w_ecwlExplorer.NavigateToNode(nodeByUrl);
                w_ecwlExplorer.Focus();
            }
            catch (Exception exc)
            {
                GlobalServices.ErrorHandler.HandleException(exc);
            }
        }

        protected void NavigateToGeneric(MappingOption item, IUrlParser parser)
        {
            try
            {
                Node nodeByUrl = NodeCollection.GetNodeByUrl(item.Option);
                if (nodeByUrl == null && parser.CanNavigate(item.SubObject, item.Option))
                {
                    nodeByUrl = NodeCollection.GetNodeByUrl(parser.GetNavigationUrl(item.SubObject, item.Option));
                }
                NavigateToNode(nodeByUrl);
            }
            catch (Exception exc)
            {
                GlobalServices.ErrorHandler.HandleException(exc);
            }
        }

        protected void NavigateToNode(Node ExplorerNode)
        {
            w_ecwlExplorer.NavigateToNode(ExplorerNode);
            w_ecwlExplorer.Focus();
        }

        private void On_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormClosingConfirmation(ref e);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (w_ecwlExplorer.SelectedNode != null)
            {
                IXMLAbleList source = null;
                IXMLAbleList target = new CommonSerializableList<ExplorerNode> { (ExplorerNode)w_ecwlExplorer.SelectedNode.Node };
                new NodePropertiesAction().Configure(ref source, ref target);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (w_ecwlExplorer.SelectedNode != null)
            {
                SerializableList<ExplorerNode> target = new CommonSerializableList<ExplorerNode> { (ExplorerNode)w_ecwlExplorer.SelectedNode.Node };
                new RefreshAction().Run(null, target);
                w_ecwlExplorer.SelectedNode.UpdateChildrenUIAsync();
            }
        }

        protected virtual void RemoveOption(MappingOption item)
        {
            MappingList.Remove(item);
            UpdateOptionsColumnWidths();
        }

        protected virtual void SaveUI()
        {
            throw new NotImplementedException();
        }

        protected bool SelectedItemCanBeNavigatedToGeneric(IUrlParser parser)
        {
            try
            {
                bool result = false;
                MappingOption[] selectedOptions = GetSelectedOptions();
                if (selectedOptions.Length != 0)
                {
                    MappingOption mappingOption = selectedOptions[0];
                    result = parser.CanNavigate(mappingOption.SubObject, mappingOption.Option);
                }
                return result;
            }
            catch (Exception exc)
            {
                GlobalServices.ErrorHandler.HandleException(exc);
                return false;
            }
        }

        public void SelectTab(int iTabIndex)
        {
            if (w_tabbedControl != null && iTabIndex < w_tabbedControl.TabPages.Count)
            {
                w_tabbedControl.SelectedTabPageIndex = iTabIndex;
            }
        }

        protected void SetDialogText()
        {
            Text = DialogTitle;
            _createAppliedOptionButton.Caption = m_sApplyOptionText;
            _createAppliedRuleButton.Caption = m_sCreateRuleText;
            w_lblOptionsView.Text = m_sOptionsLabelText;
            _optionsView.GetColumnByFieldName("Option").Caption = m_sOptionColumnText;
            _optionsView.GetColumnByFieldName("OptionAppliedValue").Caption = m_sOptionAppliedValuesText;
            _removeButton.Caption = m_sRemoveOptionText;
            _editTextButton.Caption = m_sEditOptionText;
            editOptionsToolStripMenuItem.Text = m_sEditOptionText;
            removeOptionsToolStripMenuItem.Text = m_sRemoveOptionText;
            applyOptionsToolStripMenuItem.Text = m_sApplyOptionText;
            _createRuleButton.Caption = m_sCreateNewOptionText;
            _applyListRuleButton.Caption = m_sCreateOptionToListText;
            _applyRuleButton.Caption = m_sCreateRuleText;
            applyListRuleToolStripMenuItem.Text = m_sCreateOptionToListText;
        }

        public void SetupListViewColumns(Type classType)
        {
            usingDefaultOptionAndValueColumns = false;
            _optionsView.ClearColumns();
            _optionsView.CreateColumnsFromProperties(classType, onlyWithDisplayName: true);
        }

        protected void UpdateDataRow(DataRow row, object obj)
        {
            MappingOption mappingOption = null;
            if (!UsingDefaultOptionAndValueColumns)
            {
                mappingOption = obj as MappingOption;
                if (mappingOption != null)
                {
                    obj = mappingOption.SubObject;
                }
            }
            if (obj == null)
            {
                return;
            }
            Type type = obj.GetType();
            bool flag = true;
            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo property = type.GetProperty(column.ColumnName);
                if (property == null)
                {
                    continue;
                }
                object value = property.GetValue(obj, null);
                StringBuilder stringBuilder = new StringBuilder();
                if (value != null)
                {
                    stringBuilder.Append(value.ToString());
                }
                if (stringBuilder.Length > 259)
                {
                    stringBuilder.Length = 256;
                    stringBuilder.Append("...");
                }
                row[column] = stringBuilder.ToString();
                if (flag && !UsingDefaultOptionAndValueColumns)
                {
                    mappingOption.BlockEvents();
                    try
                    {
                        mappingOption.Option = stringBuilder.ToString();
                    }
                    finally
                    {
                        mappingOption.UnblockEvents();
                    }
                    flag = false;
                }
            }
        }

        protected void UpdateItemInUI(MappingOption option)
        {
            _optionsView.UpdateChangedItem(option);
        }

        protected void UpdateOptionsColumnWidths()
        {
            _optionsView.RunAutoWidthOnAllColumns();
        }

        protected List<SimplifiedGridView> UpdateTabs()
        {
            List<SimplifiedGridView> list = new List<SimplifiedGridView>();
            Control control = _optionsView.Parent;
            bool flag = AdditionalTabs.Count > 0;
            if (control is SplitGroupPanel && !flag)
            {
                return list;
            }
            if (control is SplitGroupPanel && flag)
            {
                DevExpress.XtraTab.XtraTabControl xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
                XtraTabPage xtraTabPage = null;
                SimplifiedGridView simplifiedGridView = null;
                foreach (AdditionalOptionTab additionalTab in AdditionalTabs)
                {
                    simplifiedGridView = new SimplifiedGridView();
                    simplifiedGridView.CreateColumn(additionalTab.ObjectColumnText, "Option", 70);
                    simplifiedGridView.CreateColumn(additionalTab.OptionColumnText, "OptionAppliedValue", additionalTab.OptionColumnWidth);
                    simplifiedGridView.ContextMenuStrip = additionalTab.ContextMenuStrip;
                    StandaloneBarDockControl standaloneBarDockControl = new StandaloneBarDockControl
                    {
                        Size = _mappingBarDock.Size
                    };
                    xtraTabPage = new XtraTabPage
                    {
                        Text = additionalTab.TabName
                    };
                    list.Add(simplifiedGridView);
                    xtraTabPage.Controls.Add(simplifiedGridView);
                    xtraTabPage.Controls.Add(standaloneBarDockControl);
                    simplifiedGridView.Dock = DockStyle.Fill;
                    standaloneBarDockControl.Dock = DockStyle.Bottom;
                    CreateMenuBar(standaloneBarDockControl, additionalTab.MenuItems);
                    xtraTabControl.TabPages.Add(xtraTabPage);
                }
                control.Controls.Remove(w_lblOptionsView);
                control.Controls.Remove(_optionsView);
                control.Controls.Remove(_mappingBarDock);
                xtraTabPage = new XtraTabPage
                {
                    Text = OriginalTabText
                };
                xtraTabPage.Controls.Add(_optionsView);
                xtraTabPage.Controls.Add(_mappingBarDock);
                xtraTabControl.TabPages.Add(xtraTabPage);
                xtraTabControl.Dock = DockStyle.Fill;
                control.Controls.Add(xtraTabControl);
                w_tabbedControl = xtraTabControl;
                _separatorPanel.Visible = false;
            }
            return list;
        }

        protected void UpdateUI()
        {
            ExplorerTreeNode selectedNode = w_ecwlExplorer.SelectedNode;
            _createAppliedOptionButton.Enabled = false;
            _applyListRuleButton.Enabled = false;
            applyListRuleToolStripMenuItem.Enabled = false;
            if (selectedNode != null && selectedNode.Node != null)
            {
                try
                {
                    bool bEnabledCreateNodeOption = false;
                    bool bEnabledCreateListRuleOption = false;
                    CheckValidNode(selectedNode, ref bEnabledCreateNodeOption, ref bEnabledCreateListRuleOption);
                    _applyListRuleButton.Enabled = bEnabledCreateListRuleOption;
                    _createAppliedOptionButton.Enabled = bEnabledCreateNodeOption;
                    applyListRuleToolStripMenuItem.Enabled = bEnabledCreateListRuleOption;
                }
                catch (NotImplementedException)
                {
                    if (selectedNode.Node != m_nodeCollection[0])
                    {
                        foreach (Type explorerSelectableType in m_ExplorerSelectableTypes)
                        {
                            if (!selectedNode.Node.GetType().Equals(explorerSelectableType))
                            {
                                continue;
                            }
                            _createAppliedOptionButton.Enabled = true;
                            break;
                        }
                    }
                }
            }
            _applyRuleButton.Enabled = AllowRuleModification;
            _createRuleButton.Visibility = ((!m_bCreateRuleCmbButtonVisible) ? BarItemVisibility.Never : BarItemVisibility.Always);
            _createAppliedRuleButton.Visibility = (m_bCreateRuleCmbButtonVisible ? BarItemVisibility.Never : BarItemVisibility.Always);
            applyListRuleToolStripMenuItem.Visible = _applyListRuleButton.Enabled;
            applyOptionsToolStripMenuItem.Visible = _createAppliedOptionButton.Enabled;
            object[] selectedItems = _optionsView.SelectedItems;
            _editRuleButton.Enabled = AllowRuleModification && selectedItems.Length == 1;
            _editTextButton.Enabled = selectedItems.Length == 1;
            _removeButton.Enabled = selectedItems.Length != 0;
            _navigateButton.Enabled = SelectedItemCanBeNavigatedTo;
            editRuleToolStripMenuitem.Enabled = _editRuleButton.Enabled;
            editOptionsToolStripMenuItem.Enabled = _editTextButton.Enabled;
            removeOptionsToolStripMenuItem.Enabled = _removeButton.Enabled;
            navigateToToolStripMenuItem.Enabled = _navigateButton.Enabled;
        }

        private void w_bCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void w_bOkay_Click(object sender, EventArgs e)
        {
            SaveUI();
            base.DialogResult = DialogResult.OK;
            Close();
        }

        private void w_ecwlExplorer_SelectedNodeChanged(ReadOnlyCollection<ExplorerTreeNode> node)
        {
            UpdateUI();
        }

        private void w_tsmiCreateListRule_Click(object sender, EventArgs e)
        {
            if (w_ecwlExplorer.SelectedNodes.Count == 1)
            {
                CreateListRule();
            }
        }

        private void w_tsmiCreateRule_Click(object sender, EventArgs e)
        {
            CreateRule();
        }
    }
}