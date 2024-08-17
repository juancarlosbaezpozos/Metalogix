using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.UI.WinForms.Transformers
{
    [ControlImage("Metalogix.UI.WinForms.Icons.Transformations.png")]
    [ControlName("Transformations")]
    [IsAdvancedPrecondition(true)]
    [RequiresFullEdition(false)]
    public class TCTransformation : TabbableControl
    {
        private class TransformerWrapper
        {
            public readonly ITransformer _transformer;

            public string Name => _transformer.Name;

            public TransformerWrapper(ITransformer transformer)
		{
			_transformer = transformer;
		}

            public bool GetIsReadOnly()
		{
			return _transformer.ReadOnly;
		}

            public ITransformer GetTransformer()
		{
			return _transformer;
		}
        }

        private Metalogix.Actions.Action m_action;

        private TransformerCollection m_PersistentTransformers;

        private TransformerCollection m_currentTransformers;

        private TransformerCollection _outputTransformers;

        private Color _activeColor;

        private Color _readOnlyColor;

        private Dictionary<ITransformerDefinition, TransformerCollection> m_primeDictionary = new Dictionary<ITransformerDefinition, TransformerCollection>();

        private Dictionary<ITransformerDefinition, TransformerCollection> m_transformerDictionary = new Dictionary<ITransformerDefinition, TransformerCollection>();

        private BindingList<TransformerWrapper> _visibleTransformers = new BindingList<TransformerWrapper>();

        private IContainer components;

        private SplitContainerControl _splitter;

        private GroupControl _grpAvailablDefinitions;

        private ListBoxControl _listAvailableDefinitions;

        private GroupControl _grpAppliedTransformers;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlTop;

        private XtraBarManagerWithArrows _barManager;

        private Bar _barAddTransformers;

        private BarSubItem _barMenuAddTransformer;

        private Bar _barOrganize;

        private HelpTipButton w_helpAddTransformer;

        private BarButtonItem _barBtnUp;

        private BarButtonItem _barBtnDown;

        private BarButtonItem _barBtnEdit;

        private BarButtonItem _barBtnDelete;

        private BarButtonItem barButtonItem1;

        private PanelControl panelControl1;

        private GridControl _gridTransformers;

        private GridView _gridView;

        public Metalogix.Actions.Action Action => m_action;

        public TransformerCollection CurrentTransformerCollection => m_currentTransformers;

        public TransformerCollection OutputTransformerCollection => _outputTransformers;

        public TransformerCollection PersistentTransformerCollection
        {
            get
		{
			return m_PersistentTransformers;
		}
            set
		{
			m_PersistentTransformers = value;
			LoadUI();
		}
        }

        public TCTransformation(Metalogix.Actions.Action action, ActionContext context)
	{
		InitializeComponent();
		m_action = action;
		Context = context;
		Skin skin = CommonSkins.GetSkin(_gridTransformers.LookAndFeel);
		_activeColor = skin.Colors.GetColor(CommonColors.ControlText);
		_readOnlyColor = skin.Colors.GetColor(CommonColors.DisabledText);
		Type type = GetType();
		w_helpAddTransformer.SetResourceString(type.FullName + _barMenuAddTransformer.Name, type);
		UpdateUI();
	}

        private void ClearAddTransformerMenu()
	{
		List<BarItem> barItems = new List<BarItem>();
		foreach (BarItem item in _barManager.Items)
		{
			if (item.Links.Count > 0 && item.Links[0].OwnerItem == _barMenuAddTransformer)
			{
				barItems.Add(item);
			}
		}
		_barMenuAddTransformer.ClearLinks();
		foreach (BarItem barItem in barItems)
		{
			_barManager.Items.Remove(barItem);
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

        public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
	{
		if (string.Equals(sender.Name, "TCTaxonomyOptions") && string.Equals(sMessage, "TransformerCollectionChanged"))
		{
			ReloadTransformerUI();
		}
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Transformers.TCTransformation));
		this._splitter = new DevExpress.XtraEditors.SplitContainerControl();
		this._grpAvailablDefinitions = new DevExpress.XtraEditors.GroupControl();
		this._listAvailableDefinitions = new DevExpress.XtraEditors.ListBoxControl();
		this._grpAppliedTransformers = new DevExpress.XtraEditors.GroupControl();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this._gridTransformers = new DevExpress.XtraGrid.GridControl();
		this._gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this._barManager = new Metalogix.UI.WinForms.Components.XtraBarManagerWithArrows(this.components);
		this._barAddTransformers = new DevExpress.XtraBars.Bar();
		this._barMenuAddTransformer = new DevExpress.XtraBars.BarSubItem();
		this._barOrganize = new DevExpress.XtraBars.Bar();
		this._barBtnUp = new DevExpress.XtraBars.BarButtonItem();
		this._barBtnDown = new DevExpress.XtraBars.BarButtonItem();
		this._barBtnEdit = new DevExpress.XtraBars.BarButtonItem();
		this._barBtnDelete = new DevExpress.XtraBars.BarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.w_helpAddTransformer = new TooltipsTest.HelpTipButton();
		((System.ComponentModel.ISupportInitialize)this._splitter).BeginInit();
		this._splitter.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this._grpAvailablDefinitions).BeginInit();
		this._grpAvailablDefinitions.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this._listAvailableDefinitions).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._grpAppliedTransformers).BeginInit();
		this._grpAppliedTransformers.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this._gridTransformers).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_helpAddTransformer).BeginInit();
		base.SuspendLayout();
		this._splitter.Dock = System.Windows.Forms.DockStyle.Fill;
		this._splitter.Location = new System.Drawing.Point(0, 0);
		this._splitter.Name = "_splitter";
		this._splitter.Panel1.Controls.Add(this._grpAvailablDefinitions);
		this._splitter.Panel1.MinSize = 150;
		this._splitter.Panel1.Text = "Panel1";
		this._splitter.Panel2.Controls.Add(this._grpAppliedTransformers);
		this._splitter.Panel2.MinSize = 300;
		this._splitter.Panel2.Text = "Panel2";
		this._splitter.Size = new System.Drawing.Size(480, 285);
		this._splitter.SplitterPosition = 180;
		this._splitter.TabIndex = 0;
		this._splitter.Text = "splitContainerControl1";
		this._grpAvailablDefinitions.Controls.Add(this._listAvailableDefinitions);
		this._grpAvailablDefinitions.Dock = System.Windows.Forms.DockStyle.Fill;
		this._grpAvailablDefinitions.Location = new System.Drawing.Point(0, 0);
		this._grpAvailablDefinitions.Name = "_grpAvailablDefinitions";
		this._grpAvailablDefinitions.Size = new System.Drawing.Size(175, 285);
		this._grpAvailablDefinitions.TabIndex = 0;
		this._grpAvailablDefinitions.Text = "Available Definitions";
		this._listAvailableDefinitions.Dock = System.Windows.Forms.DockStyle.Fill;
		this._listAvailableDefinitions.Location = new System.Drawing.Point(2, 21);
		this._listAvailableDefinitions.Name = "_listAvailableDefinitions";
		this._listAvailableDefinitions.Size = new System.Drawing.Size(171, 262);
		this._listAvailableDefinitions.TabIndex = 0;
		this._listAvailableDefinitions.SelectedIndexChanged += new System.EventHandler(On_SelectedDefinition_Changed);
		this._grpAppliedTransformers.Controls.Add(this.panelControl1);
		this._grpAppliedTransformers.Controls.Add(this.w_helpAddTransformer);
		this._grpAppliedTransformers.Controls.Add(this.barDockControlLeft);
		this._grpAppliedTransformers.Controls.Add(this.barDockControlRight);
		this._grpAppliedTransformers.Controls.Add(this.barDockControlBottom);
		this._grpAppliedTransformers.Controls.Add(this.barDockControlTop);
		this._grpAppliedTransformers.Dock = System.Windows.Forms.DockStyle.Fill;
		this._grpAppliedTransformers.Location = new System.Drawing.Point(0, 0);
		this._grpAppliedTransformers.Name = "_grpAppliedTransformers";
		this._grpAppliedTransformers.Size = new System.Drawing.Size(300, 285);
		this._grpAppliedTransformers.TabIndex = 0;
		this._grpAppliedTransformers.Text = "Applied Transformers for Object";
		this.panelControl1.Appearance.BackColor = System.Drawing.Color.White;
		this.panelControl1.Appearance.Options.UseBackColor = true;
		this.panelControl1.Controls.Add(this._gridTransformers);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(2, 52);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(265, 231);
		this.panelControl1.TabIndex = 11;
		this._gridTransformers.Dock = System.Windows.Forms.DockStyle.Fill;
		this._gridTransformers.Location = new System.Drawing.Point(2, 2);
		this._gridTransformers.MainView = this._gridView;
		this._gridTransformers.MenuManager = this._barManager;
		this._gridTransformers.Name = "_gridTransformers";
		this._gridTransformers.Size = new System.Drawing.Size(261, 227);
		this._gridTransformers.TabIndex = 0;
		this._gridTransformers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this._gridView });
		this._gridView.GridControl = this._gridTransformers;
		this._gridView.Name = "_gridView";
		this._gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
		this._gridView.OptionsBehavior.AutoUpdateTotalSummary = false;
		this._gridView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
		this._gridView.OptionsBehavior.Editable = false;
		this._gridView.OptionsBehavior.ReadOnly = true;
		this._gridView.OptionsCustomization.AllowColumnMoving = false;
		this._gridView.OptionsCustomization.AllowColumnResizing = false;
		this._gridView.OptionsCustomization.AllowFilter = false;
		this._gridView.OptionsCustomization.AllowGroup = false;
		this._gridView.OptionsCustomization.AllowQuickHideColumns = false;
		this._gridView.OptionsCustomization.AllowSort = false;
		this._gridView.OptionsDetail.AllowZoomDetail = false;
		this._gridView.OptionsDetail.EnableMasterViewMode = false;
		this._gridView.OptionsDetail.ShowDetailTabs = false;
		this._gridView.OptionsDetail.SmartDetailExpand = false;
		this._gridView.OptionsMenu.EnableColumnMenu = false;
		this._gridView.OptionsMenu.EnableFooterMenu = false;
		this._gridView.OptionsMenu.ShowAutoFilterRowItem = false;
		this._gridView.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
		this._gridView.OptionsMenu.ShowGroupSortSummaryItems = false;
		this._gridView.OptionsNavigation.AutoFocusNewRow = true;
		this._gridView.OptionsPrint.PrintFooter = false;
		this._gridView.OptionsPrint.PrintGroupFooter = false;
		this._gridView.OptionsPrint.PrintHeader = false;
		this._gridView.OptionsPrint.PrintHorzLines = false;
		this._gridView.OptionsPrint.PrintVertLines = false;
		this._gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this._gridView.OptionsSelection.MultiSelect = true;
		this._gridView.OptionsView.ShowColumnHeaders = false;
		this._gridView.OptionsView.ShowDetailButtons = false;
		this._gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this._gridView.OptionsView.ShowGroupExpandCollapseButtons = false;
		this._gridView.OptionsView.ShowGroupPanel = false;
		this._gridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(On_Row_Style);
		this._gridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(On_Transformer_Selected_Changed);
		this._barManager.AllowCustomization = false;
		this._barManager.AllowMoveBarOnToolbar = false;
		this._barManager.AllowQuickCustomization = false;
		DevExpress.XtraBars.Bars bars = this._barManager.Bars;
		DevExpress.XtraBars.Bar[] barArray = new DevExpress.XtraBars.Bar[2] { this._barAddTransformers, this._barOrganize };
		bars.AddRange(barArray);
		this._barManager.DockControls.Add(this.barDockControlTop);
		this._barManager.DockControls.Add(this.barDockControlBottom);
		this._barManager.DockControls.Add(this.barDockControlLeft);
		this._barManager.DockControls.Add(this.barDockControlRight);
		this._barManager.Form = this._grpAppliedTransformers;
		DevExpress.XtraBars.BarItems items = this._barManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[6] { this._barMenuAddTransformer, this.barButtonItem1, this._barBtnUp, this._barBtnDown, this._barBtnEdit, this._barBtnDelete };
		items.AddRange(barItemArray);
		this._barManager.MaxItemId = 7;
		this._barAddTransformers.BarName = "Add Transformers";
		this._barAddTransformers.DockCol = 0;
		this._barAddTransformers.DockRow = 0;
		this._barAddTransformers.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this._barAddTransformers.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._barMenuAddTransformer)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this._barAddTransformers.OptionsBar.AllowQuickCustomization = false;
		this._barAddTransformers.OptionsBar.DisableCustomization = true;
		this._barAddTransformers.OptionsBar.DrawDragBorder = false;
		this._barAddTransformers.Text = "Tools";
		this._barMenuAddTransformer.Caption = "Add Transformer";
		this._barMenuAddTransformer.Glyph = Metalogix.UI.WinForms.Properties.Resources.Add16;
		this._barMenuAddTransformer.Id = 0;
		this._barMenuAddTransformer.Name = "_barMenuAddTransformer";
		this._barMenuAddTransformer.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this._barOrganize.BarName = "Organize Transformers";
		this._barOrganize.DockCol = 0;
		this._barOrganize.DockRow = 0;
		this._barOrganize.DockStyle = DevExpress.XtraBars.BarDockStyle.Right;
		this._barOrganize.FloatLocation = new System.Drawing.Point(815, 202);
		DevExpress.XtraBars.LinksInfo linksInfo = this._barOrganize.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfoArray = new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnUp),
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnDown),
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnEdit),
			new DevExpress.XtraBars.LinkPersistInfo(this._barBtnDelete)
		};
		linksInfo.AddRange(linkPersistInfoArray);
		this._barOrganize.OptionsBar.AllowQuickCustomization = false;
		this._barOrganize.OptionsBar.DisableCustomization = true;
		this._barOrganize.OptionsBar.DrawDragBorder = false;
		this._barOrganize.Text = "Organize Transformers";
		this._barBtnUp.Caption = "Up";
		this._barBtnUp.Glyph = Metalogix.UI.WinForms.Properties.Resources.Up16;
		this._barBtnUp.Id = 2;
		this._barBtnUp.Name = "_barBtnUp";
		this._barBtnUp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Up_Clicked);
		this._barBtnDown.Caption = "Down";
		this._barBtnDown.Glyph = Metalogix.UI.WinForms.Properties.Resources.Down16;
		this._barBtnDown.Id = 3;
		this._barBtnDown.Name = "_barBtnDown";
		this._barBtnDown.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Down_Clicked);
		this._barBtnEdit.Caption = "Edit";
		this._barBtnEdit.Glyph = Metalogix.UI.WinForms.Properties.Resources.Edit16;
		this._barBtnEdit.Id = 4;
		this._barBtnEdit.Name = "_barBtnEdit";
		this._barBtnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Edit_Clicked);
		this._barBtnDelete.Caption = "Delete";
		this._barBtnDelete.Glyph = Metalogix.UI.WinForms.Properties.Resources.Delete16;
		this._barBtnDelete.Id = 6;
		this._barBtnDelete.Name = "_barBtnDelete";
		this._barBtnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(On_Delete_Clicked);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(2, 21);
		this.barDockControlTop.Size = new System.Drawing.Size(296, 31);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(2, 283);
		this.barDockControlBottom.Size = new System.Drawing.Size(296, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(2, 52);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 231);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(267, 52);
		this.barDockControlRight.Size = new System.Drawing.Size(31, 231);
		this.barButtonItem1.Caption = "Add";
		this.barButtonItem1.Id = 1;
		this.barButtonItem1.Name = "barButtonItem1";
		this.w_helpAddTransformer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_helpAddTransformer.AnchoringControl = null;
		this.w_helpAddTransformer.BackColor = System.Drawing.Color.Transparent;
		this.w_helpAddTransformer.CommonParentControl = null;
		this.w_helpAddTransformer.Image = (System.Drawing.Image)componentResourceManager.GetObject("w_helpAddTransformer.Image");
		this.w_helpAddTransformer.Location = new System.Drawing.Point(271, 26);
		this.w_helpAddTransformer.MaximumSize = new System.Drawing.Size(20, 20);
		this.w_helpAddTransformer.MinimumSize = new System.Drawing.Size(20, 20);
		this.w_helpAddTransformer.Name = "w_helpAddTransformer";
		this.w_helpAddTransformer.RealOffset = null;
		this.w_helpAddTransformer.RelativeOffset = null;
		this.w_helpAddTransformer.Size = new System.Drawing.Size(20, 20);
		this.w_helpAddTransformer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.w_helpAddTransformer.TabIndex = 6;
		this.w_helpAddTransformer.TabStop = false;
		base.Controls.Add(this._splitter);
		this.MinimumSize = new System.Drawing.Size(480, 285);
		base.Name = "TCTransformation";
		base.Size = new System.Drawing.Size(480, 285);
		base.Load += new System.EventHandler(On_Shown);
		base.Resize += new System.EventHandler(On_Resize);
		((System.ComponentModel.ISupportInitialize)this._splitter).EndInit();
		this._splitter.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this._grpAvailablDefinitions).EndInit();
		this._grpAvailablDefinitions.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this._listAvailableDefinitions).EndInit();
		((System.ComponentModel.ISupportInitialize)this._grpAppliedTransformers).EndInit();
		this._grpAppliedTransformers.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this._gridTransformers).EndInit();
		((System.ComponentModel.ISupportInitialize)this._gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_helpAddTransformer).EndInit();
		base.ResumeLayout(false);
	}

        protected override void LoadUI()
	{
		base.LoadUI();
		m_currentTransformers = ((PersistentTransformerCollection != null) ? PersistentTransformerCollection.Clone() : new TransformerCollection());
		ReloadTransformerUI();
	}

        private void On_AddTransformer_Clicked(object sender, ItemClickEventArgs e)
	{
		BarItem item = e.Item;
		ITransformer tag = item.Tag as ITransformer;
		ITransformerDefinition selectedItem = _listAvailableDefinitions.SelectedItem as ITransformerDefinition;
		if (tag == null)
		{
			return;
		}
		ITransformer transformer = tag.Clone();
		if (transformer.Configure(Action, Context.Sources, Context.Targets))
		{
			SuspendLayout();
			_visibleTransformers.Add(new TransformerWrapper(transformer));
			m_transformerDictionary[selectedItem].Add(transformer);
			if (TransformerIsSingleUse(transformer))
			{
				_barManager.Items.Remove(item);
				_barMenuAddTransformer.Enabled = _barMenuAddTransformer.ItemLinks.Count > 0;
			}
			ResumeLayout();
		}
	}

        private void On_Delete_Clicked(object sender, ItemClickEventArgs e)
	{
		SuspendLayout();
		ITransformerDefinition selectedItem = ((_listAvailableDefinitions.SelectedItems.Count <= 0) ? null : (_listAvailableDefinitions.SelectedItem as ITransformerDefinition));
		ITransformerDefinition transformerDefinition = selectedItem;
		TransformerCollection item = ((transformerDefinition == null) ? null : m_transformerDictionary[transformerDefinition]);
		TransformerCollection transformerCollection = item;
		int[] selectedRows = _gridView.GetSelectedRows();
		for (int i = selectedRows.Length - 1; i >= 0; i--)
		{
			int num = selectedRows[i];
			TransformerWrapper row = _gridView.GetRow(num) as TransformerWrapper;
			if (transformerCollection != null)
			{
				ITransformer transformer = row.GetTransformer();
				SendMessage("TransformerCollectionItemRemoved", transformer.GetType().ToString());
				transformerCollection.Remove(transformer);
			}
			_visibleTransformers.RemoveAt(num);
		}
		UpdateTransformerUI();
		ResumeLayout();
		SetSidebarEnabledState();
	}

        private void On_Down_Clicked(object sender, ItemClickEventArgs e)
	{
		SuspendLayout();
		ITransformerDefinition selectedItem = _listAvailableDefinitions.SelectedItem as ITransformerDefinition;
		TransformerCollection item = null;
		if (selectedItem != null && m_transformerDictionary.ContainsKey(selectedItem))
		{
			item = m_transformerDictionary[selectedItem];
		}
		int[] selectedRows = _gridView.GetSelectedRows();
		for (int i = selectedRows.Length - 1; i >= 0; i--)
		{
			int num = selectedRows[i];
			if (_gridView.GetRow(num) is TransformerWrapper row)
			{
				_visibleTransformers.RemoveAt(num);
				item.RemoveAt(num);
				_visibleTransformers.Insert(num + 1, row);
				item.Insert(num + 1, row.GetTransformer());
			}
		}
		int[] numArray = selectedRows;
		foreach (int num1 in numArray)
		{
			_gridView.SelectRow(num1 + 1);
		}
		ResumeLayout();
		SetSidebarEnabledState();
	}

        private void On_Edit_Clicked(object sender, ItemClickEventArgs e)
	{
		int[] selectedRows = _gridView.GetSelectedRows();
		if (selectedRows.Length == 1 && _gridView.GetRow(selectedRows[0]) is TransformerWrapper row)
		{
			row.GetTransformer()?.Configure(Action, Context.Sources, Context.Targets);
		}
	}

        private void On_Resize(object sender, EventArgs e)
	{
		_splitter.Size = base.Size;
	}

        private void On_Row_Style(object sender, RowStyleEventArgs e)
	{
		if (!(_gridView.GetRow(e.RowHandle) is TransformerWrapper row))
		{
			e.Appearance.ForeColor = _activeColor;
			return;
		}
		ITransformer transformer = row.GetTransformer();
		e.Appearance.ForeColor = (transformer.ReadOnly ? _readOnlyColor : _activeColor);
	}

        private void On_SelectedDefinition_Changed(object sender, EventArgs e)
	{
		UpdateTransformerUI();
		SetSidebarEnabledState();
	}

        private void On_Shown(object sender, EventArgs e)
	{
		if (_listAvailableDefinitions.Items.Count > 0 && _listAvailableDefinitions.SelectedIndices.Count == 0)
		{
			_listAvailableDefinitions.SelectedIndex = 0;
		}
		_splitter.Size = base.Size;
	}

        private void On_Transformer_Selected_Changed(object sender, SelectionChangedEventArgs e)
	{
		SetSidebarEnabledState();
	}

        private void On_Up_Clicked(object sender, ItemClickEventArgs e)
	{
		SuspendLayout();
		ITransformerDefinition selectedItem = _listAvailableDefinitions.SelectedItem as ITransformerDefinition;
		TransformerCollection item = null;
		if (selectedItem != null && m_transformerDictionary.ContainsKey(selectedItem))
		{
			item = m_transformerDictionary[selectedItem];
		}
		int[] selectedRows = _gridView.GetSelectedRows();
		int[] numArray = selectedRows;
		foreach (int num in numArray)
		{
			if (_gridView.GetRow(num) is TransformerWrapper row)
			{
				_visibleTransformers.RemoveAt(num);
				item.RemoveAt(num);
				_visibleTransformers.Insert(num - 1, row);
				item.Insert(num - 1, row.GetTransformer());
			}
		}
		int[] numArray1 = selectedRows;
		foreach (int num1 in numArray1)
		{
			_gridView.SelectRow(num1 - 1);
		}
		ResumeLayout();
		SetSidebarEnabledState();
	}

        protected virtual void ReloadTransformerUI()
	{
		m_transformerDictionary.Clear();
		foreach (ITransformerDefinition supportedDefinition in Action.SupportedDefinitions)
		{
			if (!m_transformerDictionary.ContainsKey(supportedDefinition))
			{
				m_transformerDictionary.Add(supportedDefinition, supportedDefinition.GetMatchingTransformers(m_currentTransformers));
			}
		}
		UpdateTransformerUI();
	}

        public override bool SaveUI()
	{
		if (!base.SaveUI())
		{
			return false;
		}
		_outputTransformers = new TransformerCollection();
		foreach (TransformerCollection value in m_transformerDictionary.Values)
		{
			foreach (ITransformer transformer in value)
			{
				if (!_outputTransformers.Contains(transformer))
				{
					_outputTransformers.Add(transformer);
				}
			}
		}
		return true;
	}

        private void SetSidebarEnabledState()
	{
		int[] selectedRows = _gridView.GetSelectedRows();
		if (selectedRows.Length == 0)
		{
			_barBtnUp.Enabled = false;
			_barBtnDown.Enabled = false;
			_barBtnEdit.Enabled = false;
			_barBtnDelete.Enabled = false;
			return;
		}
		int num = selectedRows[0];
		int num1 = selectedRows[selectedRows.Length - 1];
		_barBtnUp.Enabled = num > 0;
		_barBtnDown.Enabled = num1 < _gridView.RowCount - 1;
		bool flag = false;
		int[] numArray = selectedRows;
		foreach (int num3 in numArray)
		{
			if (_gridView.GetRow(num3) is TransformerWrapper row && row.GetIsReadOnly())
			{
				flag = true;
				break;
			}
		}
		_barBtnDelete.Enabled = !flag;
		_barBtnEdit.Enabled = selectedRows.Length == 1 && !flag;
	}

        private static bool TransformerIsSingleUse(ITransformer transformer)
	{
		return (transformer.Cardinality & Cardinality.MoreThanOne) != Cardinality.MoreThanOne;
	}

        private void UpdateTransformerUI()
	{
		SuspendLayout();
		try
		{
			ClearAddTransformerMenu();
			_gridTransformers.DataSource = null;
			_visibleTransformers.Clear();
			if (m_primeDictionary.Count == 0)
			{
				return;
			}
			if (_listAvailableDefinitions.SelectedItems.Count <= 0)
			{
				_grpAppliedTransformers.Text = string.Format(Resources.No_Transformer_Definition);
			}
			else
			{
				ITransformerDefinition item = _listAvailableDefinitions.SelectedItems[0] as ITransformerDefinition;
				_grpAppliedTransformers.Text = string.Format(Resources.Applied_Transformers, item.Name);
				List<Type> types = new List<Type>();
				foreach (ITransformer transformer in m_transformerDictionary[item])
				{
					_visibleTransformers.Add(new TransformerWrapper(transformer));
					if (TransformerIsSingleUse(transformer))
					{
						types.Add(transformer.GetType());
					}
				}
				foreach (ITransformer item1 in m_primeDictionary[item])
				{
					if (!types.Contains(item1.GetType()))
					{
						BarItem barItem = _barManager.Items.CreateButton(item1.Name);
						barItem.Tag = item1;
						barItem.ItemClick += On_AddTransformer_Clicked;
						_barMenuAddTransformer.AddItem(barItem);
					}
				}
			}
			_gridTransformers.DataSource = _visibleTransformers;
			_barMenuAddTransformer.Enabled = _barMenuAddTransformer.ItemLinks.Count > 0;
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException("Error loading transformer menus", exception);
		}
		ResumeLayout();
	}

        private void UpdateUI()
	{
		m_primeDictionary.Clear();
		_listAvailableDefinitions.Items.Clear();
		foreach (ITransformerDefinition supportedDefinition in Action.SupportedDefinitions)
		{
			if (supportedDefinition.Hidden || m_primeDictionary.ContainsKey(supportedDefinition))
			{
				continue;
			}
			_listAvailableDefinitions.Items.Add(supportedDefinition);
			TransformerCollection matchingAvailableTransformers = supportedDefinition.GetMatchingAvailableTransformers();
			if (matchingAvailableTransformers == null)
			{
				continue;
			}
			TransformerCollection transformerCollection = new TransformerCollection();
			foreach (ITransformer matchingAvailableTransformer in matchingAvailableTransformers)
			{
				if (!matchingAvailableTransformer.GetType().IsDefined(typeof(TransformerVisibleAttribute), inherit: true) || ((TransformerVisibleAttribute)matchingAvailableTransformer.GetType().GetCustomAttributes(typeof(TransformerVisibleAttribute), inherit: true)[0]).IsVisible)
				{
					transformerCollection.Add(matchingAvailableTransformer);
				}
			}
			m_primeDictionary.Add(supportedDefinition, transformerCollection);
		}
	}
    }
}
