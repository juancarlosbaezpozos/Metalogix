using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.Standard.Explorer;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
	public class SearchDialog : XtraForm, IHasSelectableObjects
	{
		private delegate void CursorDelegate(Cursor cursor);

		private delegate void FillItemsViewDelegate(ListItemCollection items, FieldCollection viewFields);

		private string m_sContentTypeText;

		private Point m_location;

		private SPNode m_searchNode;

		private Thread m_searchThread;

		private bool m_bJustEnteredSearchTerm;

		private bool m_bCancelled;

		private bool m_bDoingBackgroundFetch;

		private IContainer components;

		private ExplorerControlWithLocation _searchLocation;

		private GroupControl _groupAdvanced;

		private STItemCollectionView _itemsView;

		private CheckEdit _cbMatchExactly;

		private CheckEdit _cbRecursive;

		private CheckEdit _cbModifiedBefore;

		private CheckEdit _cbCreatedBefore;

		private CheckEdit _cbModifiedAfter;

		private CheckEdit _cbCreatedAfter;

		private CheckEdit _cbModifiedBy;

		private CheckEdit _cbCreatedBy;

		private TextEdit _tbModifiedBy;

		private TextEdit _tbCreatedBy;

		private CheckEdit _cbSites;

		private CheckEdit _cbLists;

		private CheckEdit _cbFolders;

		private CheckEdit _cbDocuments;

		private CheckEdit _cbItems;

		private LabelControl _lblTypes;

		private CheckEdit _cbContentType;

		private LayoutControl _layoutControl;

		private LayoutControlGroup _layoutControlGroup;

		private SimpleButton _btnAdvanced;

		private LayoutControlItem _layoutAdvancedButton;

		private SimpleButton _btnSearch;

		private TextEdit _teSearch;

		private SimpleButton _btnShowPropertyGrids;

		private LayoutControlItem _layoutSearch;

		private LayoutControlItem _layoutSearchButton;

		private DateEdit _dtpCreatedBefore;

		private ComboBoxEdit _cmbContentType;

		private DateEdit _dtpModifiedAfter;

		private DateEdit _dtpModifiedBefore;

		private DateEdit _dtpCreatedAfter;

		private LayoutControlItem _layoutSearchLocation;

		private LayoutControlItem _layoutAdvancedOptions;

		private LabelControl _lblStatus;

		private SimpleButton _tsBtnCancel;

		private LayoutControlItem _layoutCancelButton;

		private SearchResultsActionPalette _searchResultsActionPalette;

		private LayoutControlItem _layoutPropertiesButton;

		private LayoutControlItem _layoutStatus;

		private LayoutControlItem _layoutItemsView;

		private LabelControl _lblLine;

		private LayoutControlItem _layoutLine;

		public SPSearchParameters Parameters
		{
			get
			{
				return GetParameters();
			}
			set
			{
				LoadParameters(value);
			}
		}

		public SPNode SearchNode
		{
			get
			{
				return m_searchNode;
			}
			set
			{
				m_searchNode = value;
				LoadUI();
			}
		}

		public IXMLAbleList SelectedObjects
		{
			get
			{
				List<SPNode> list = new List<SPNode>();
				foreach (SPSearchResult selectedObject in _itemsView.SelectedObjects)
				{
					list.Add(selectedObject.Node);
				}
				return new NodeCollection(list.ToArray());
			}
		}

		public Point StartLocation
		{
			get
			{
				return m_location;
			}
			set
			{
				m_location.X = value.X;
				m_location.Y = value.Y;
			}
		}

		public SearchDialog()
		{
			InitializeComponent();
			_searchLocation.LocationDescriptor = "Search in:";
			ResultsFieldCollection resultsFieldCollection = new ResultsFieldCollection();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Field item in resultsFieldCollection)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(item.Name);
			}
			_itemsView.UpdateColumnWidths(stringBuilder.ToString(), 110);
			_itemsView.UpdateColumnWidths("Created,Modified", 150);
			_itemsView.UpdateColumnWidths("Path", 200);
			_itemsView.ViewFields = resultsFieldCollection;
			_itemsView.DataSource = new ListItemCollection(null, null, null);
			_itemsView.SelectedItemsChanged += ItemsView_SelectedItems_Changed;
		}

		private void CancelSearch()
		{
			m_bCancelled = true;
			if (!m_bDoingBackgroundFetch)
			{
				m_searchThread.Abort();
			}
			else
			{
				m_searchThread.Join();
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

		private void FillItemsView(ListItemCollection items, FieldCollection viewFields)
		{
			if (!base.InvokeRequired)
			{
				try
				{
					_itemsView.ViewFields = viewFields;
					_itemsView.DataSource = items;
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			FillItemsViewDelegate method = FillItemsView;
			object[] args = new object[2] { items, viewFields };
			Invoke(method, args);
		}

		private SPSearchParameters GetParameters()
		{
			SPSearchParameters sPSearchParameters = new SPSearchParameters
			{
				MatchExactly = _cbMatchExactly.Checked,
				Recursive = _cbRecursive.Checked,
				IncludeItems = _cbItems.Checked,
				IncludeDocuments = _cbDocuments.Checked,
				IncludeFolders = _cbFolders.Checked,
				IncludeLists = _cbLists.Checked,
				IncludeSites = _cbSites.Checked
			};
			if (_cbContentType.Checked)
			{
				sPSearchParameters.ContentType = m_sContentTypeText;
			}
			if (_cbCreatedBy.Checked)
			{
				sPSearchParameters.CreatedBy = _tbCreatedBy.Text;
			}
			if (_cbModifiedBy.Checked)
			{
				sPSearchParameters.ModifiedBy = _tbModifiedBy.Text;
			}
			if (_cbCreatedBefore.Checked)
			{
				sPSearchParameters.CreatedBefore = _dtpCreatedBefore.DateTime;
			}
			if (_cbCreatedAfter.Checked)
			{
				sPSearchParameters.CreatedAfter = _dtpCreatedAfter.DateTime;
			}
			if (_cbModifiedBefore.Checked)
			{
				sPSearchParameters.ModifiedBefore = _dtpModifiedBefore.DateTime;
			}
			if (_cbModifiedAfter.Checked)
			{
				sPSearchParameters.ModifiedAfter = _dtpModifiedAfter.DateTime;
			}
			return sPSearchParameters;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Reporting.SearchDialog));
			Metalogix.DataStructures.Generic.CommonSerializableList<object> sourceOverride = new Metalogix.DataStructures.Generic.CommonSerializableList<object>();
			this._searchLocation = new Metalogix.UI.WinForms.Widgets.ExplorerControlWithLocation();
			this._groupAdvanced = new DevExpress.XtraEditors.GroupControl();
			this._cmbContentType = new DevExpress.XtraEditors.ComboBoxEdit();
			this._cbMatchExactly = new DevExpress.XtraEditors.CheckEdit();
			this._tbModifiedBy = new DevExpress.XtraEditors.TextEdit();
			this._cbContentType = new DevExpress.XtraEditors.CheckEdit();
			this._dtpModifiedAfter = new DevExpress.XtraEditors.DateEdit();
			this._cbLists = new DevExpress.XtraEditors.CheckEdit();
			this._dtpModifiedBefore = new DevExpress.XtraEditors.DateEdit();
			this._cbModifiedBefore = new DevExpress.XtraEditors.CheckEdit();
			this._dtpCreatedAfter = new DevExpress.XtraEditors.DateEdit();
			this._cbSites = new DevExpress.XtraEditors.CheckEdit();
			this._dtpCreatedBefore = new DevExpress.XtraEditors.DateEdit();
			this._cbDocuments = new DevExpress.XtraEditors.CheckEdit();
			this._cbCreatedBy = new DevExpress.XtraEditors.CheckEdit();
			this._cbModifiedAfter = new DevExpress.XtraEditors.CheckEdit();
			this._tbCreatedBy = new DevExpress.XtraEditors.TextEdit();
			this._cbItems = new DevExpress.XtraEditors.CheckEdit();
			this._cbFolders = new DevExpress.XtraEditors.CheckEdit();
			this._cbCreatedBefore = new DevExpress.XtraEditors.CheckEdit();
			this._cbModifiedBy = new DevExpress.XtraEditors.CheckEdit();
			this._cbRecursive = new DevExpress.XtraEditors.CheckEdit();
			this._lblTypes = new DevExpress.XtraEditors.LabelControl();
			this._cbCreatedAfter = new DevExpress.XtraEditors.CheckEdit();
			this._itemsView = new Metalogix.UI.Standard.Explorer.STItemCollectionView();
			this._searchResultsActionPalette = new Metalogix.SharePoint.UI.WinForms.Reporting.SearchResultsActionPalette(this.components);
			this._layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this._lblLine = new DevExpress.XtraEditors.LabelControl();
			this._btnShowPropertyGrids = new DevExpress.XtraEditors.SimpleButton();
			this._lblStatus = new DevExpress.XtraEditors.LabelControl();
			this._tsBtnCancel = new DevExpress.XtraEditors.SimpleButton();
			this._btnSearch = new DevExpress.XtraEditors.SimpleButton();
			this._teSearch = new DevExpress.XtraEditors.TextEdit();
			this._btnAdvanced = new DevExpress.XtraEditors.SimpleButton();
			this._layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this._layoutAdvancedButton = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutSearch = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutSearchButton = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutSearchLocation = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutAdvancedOptions = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutCancelButton = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutPropertiesButton = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutStatus = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutItemsView = new DevExpress.XtraLayout.LayoutControlItem();
			this._layoutLine = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)this._groupAdvanced).BeginInit();
			this._groupAdvanced.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._cmbContentType.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbMatchExactly.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._tbModifiedBy.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbContentType.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedAfter.Properties.VistaTimeProperties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedAfter.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedBefore.Properties.VistaTimeProperties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedBefore.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedBefore.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedAfter.Properties.VistaTimeProperties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedAfter.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbSites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedBefore.Properties.VistaTimeProperties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedBefore.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbDocuments.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedBy.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedAfter.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._tbCreatedBy.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbFolders.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedBefore.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedBy.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbRecursive.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedAfter.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutControl).BeginInit();
			this._layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._teSearch.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutControlGroup).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutAdvancedButton).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearch).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearchButton).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearchLocation).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutAdvancedOptions).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutCancelButton).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutPropertiesButton).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutStatus).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutItemsView).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._layoutLine).BeginInit();
			base.SuspendLayout();
			this._searchLocation.Actions = new Metalogix.Actions.Action[0];
			this._searchLocation.BackColor = System.Drawing.Color.White;
			this._searchLocation.CheckBoxes = false;
			this._searchLocation.DataSource = null;
			resources.ApplyResources(this._searchLocation, "_searchLocation");
			this._searchLocation.LocationDescriptor = "Location:";
			this._searchLocation.MultiSelectEnabled = false;
			this._searchLocation.MultiSelectLimitationMethod = null;
			this._searchLocation.Name = "_searchLocation";
			this._searchLocation.TabStop = false;
			this._searchLocation.SelectedNodeChanged += new Metalogix.UI.WinForms.Widgets.ExplorerControl.SelectedNodeChangedHandler(On_ExplorerNode_Changed);
			this._groupAdvanced.Controls.Add(this._cmbContentType);
			this._groupAdvanced.Controls.Add(this._cbMatchExactly);
			this._groupAdvanced.Controls.Add(this._tbModifiedBy);
			this._groupAdvanced.Controls.Add(this._cbContentType);
			this._groupAdvanced.Controls.Add(this._dtpModifiedAfter);
			this._groupAdvanced.Controls.Add(this._cbLists);
			this._groupAdvanced.Controls.Add(this._dtpModifiedBefore);
			this._groupAdvanced.Controls.Add(this._cbModifiedBefore);
			this._groupAdvanced.Controls.Add(this._dtpCreatedAfter);
			this._groupAdvanced.Controls.Add(this._cbSites);
			this._groupAdvanced.Controls.Add(this._dtpCreatedBefore);
			this._groupAdvanced.Controls.Add(this._cbDocuments);
			this._groupAdvanced.Controls.Add(this._cbCreatedBy);
			this._groupAdvanced.Controls.Add(this._cbModifiedAfter);
			this._groupAdvanced.Controls.Add(this._tbCreatedBy);
			this._groupAdvanced.Controls.Add(this._cbItems);
			this._groupAdvanced.Controls.Add(this._cbFolders);
			this._groupAdvanced.Controls.Add(this._cbCreatedBefore);
			this._groupAdvanced.Controls.Add(this._cbModifiedBy);
			this._groupAdvanced.Controls.Add(this._cbRecursive);
			this._groupAdvanced.Controls.Add(this._lblTypes);
			this._groupAdvanced.Controls.Add(this._cbCreatedAfter);
			resources.ApplyResources(this._groupAdvanced, "_groupAdvanced");
			this._groupAdvanced.Name = "_groupAdvanced";
			resources.ApplyResources(this._cmbContentType, "_cmbContentType");
			this._cmbContentType.Name = "_cmbContentType";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this._cmbContentType.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons.AddRange(buttons2);
			this._cmbContentType.TextChanged += new System.EventHandler(On_ContentType_TextChanged);
			this._cmbContentType.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbMatchExactly, "_cbMatchExactly");
			this._cbMatchExactly.Name = "_cbMatchExactly";
			this._cbMatchExactly.Properties.AutoWidth = true;
			this._cbMatchExactly.Properties.Caption = resources.GetString("_cbMatchExactly.Properties.Caption");
			resources.ApplyResources(this._tbModifiedBy, "_tbModifiedBy");
			this._tbModifiedBy.Name = "_tbModifiedBy";
			this._tbModifiedBy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbContentType, "_cbContentType");
			this._cbContentType.Name = "_cbContentType";
			this._cbContentType.Properties.AutoWidth = true;
			this._cbContentType.Properties.Caption = resources.GetString("_cbContentType.Properties.Caption");
			this._cbContentType.CheckedChanged += new System.EventHandler(On_ContentType_CheckChanged);
			resources.ApplyResources(this._dtpModifiedAfter, "_dtpModifiedAfter");
			this._dtpModifiedAfter.Name = "_dtpModifiedAfter";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons3 = this._dtpModifiedAfter.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons4 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons3.AddRange(buttons4);
			this._dtpModifiedAfter.Properties.EditFormat.FormatString = "g";
			this._dtpModifiedAfter.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this._dtpModifiedAfter.Properties.Mask.EditMask = resources.GetString("_dtpModifiedAfter.Properties.Mask.EditMask");
			this._dtpModifiedAfter.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
			this._dtpModifiedAfter.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
			this._dtpModifiedAfter.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this._dtpModifiedAfter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbLists, "_cbLists");
			this._cbLists.Name = "_cbLists";
			this._cbLists.Properties.AutoWidth = true;
			this._cbLists.Properties.Caption = resources.GetString("_cbLists.Properties.Caption");
			this._cbLists.CheckedChanged += new System.EventHandler(On_Type_CheckedChanged);
			resources.ApplyResources(this._dtpModifiedBefore, "_dtpModifiedBefore");
			this._dtpModifiedBefore.Name = "_dtpModifiedBefore";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons5 = this._dtpModifiedBefore.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons6 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons5.AddRange(buttons6);
			this._dtpModifiedBefore.Properties.EditFormat.FormatString = "g";
			this._dtpModifiedBefore.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this._dtpModifiedBefore.Properties.Mask.EditMask = resources.GetString("_dtpModifiedBefore.Properties.Mask.EditMask");
			this._dtpModifiedBefore.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
			this._dtpModifiedBefore.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
			this._dtpModifiedBefore.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this._dtpModifiedBefore.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbModifiedBefore, "_cbModifiedBefore");
			this._cbModifiedBefore.Name = "_cbModifiedBefore";
			this._cbModifiedBefore.Properties.AutoWidth = true;
			this._cbModifiedBefore.Properties.Caption = resources.GetString("_cbModifiedBefore.Properties.Caption");
			this._cbModifiedBefore.CheckedChanged += new System.EventHandler(On_ModifiedBefore_CheckedChanged);
			resources.ApplyResources(this._dtpCreatedAfter, "_dtpCreatedAfter");
			this._dtpCreatedAfter.Name = "_dtpCreatedAfter";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons7 = this._dtpCreatedAfter.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons8 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons7.AddRange(buttons8);
			this._dtpCreatedAfter.Properties.EditFormat.FormatString = "g";
			this._dtpCreatedAfter.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this._dtpCreatedAfter.Properties.Mask.EditMask = resources.GetString("_dtpCreatedAfter.Properties.Mask.EditMask");
			this._dtpCreatedAfter.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
			this._dtpCreatedAfter.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
			this._dtpCreatedAfter.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this._dtpCreatedAfter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbSites, "_cbSites");
			this._cbSites.Name = "_cbSites";
			this._cbSites.Properties.AutoWidth = true;
			this._cbSites.Properties.Caption = resources.GetString("_cbSites.Properties.Caption");
			this._cbSites.CheckedChanged += new System.EventHandler(On_Type_CheckedChanged);
			resources.ApplyResources(this._dtpCreatedBefore, "_dtpCreatedBefore");
			this._dtpCreatedBefore.Name = "_dtpCreatedBefore";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons9 = this._dtpCreatedBefore.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons10 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons9.AddRange(buttons10);
			this._dtpCreatedBefore.Properties.EditFormat.FormatString = "g";
			this._dtpCreatedBefore.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
			this._dtpCreatedBefore.Properties.Mask.EditMask = resources.GetString("_dtpCreatedBefore.Properties.Mask.EditMask");
			this._dtpCreatedBefore.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
			this._dtpCreatedBefore.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.True;
			this._dtpCreatedBefore.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this._dtpCreatedBefore.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbDocuments, "_cbDocuments");
			this._cbDocuments.Name = "_cbDocuments";
			this._cbDocuments.Properties.AutoWidth = true;
			this._cbDocuments.Properties.Caption = resources.GetString("_cbDocuments.Properties.Caption");
			this._cbDocuments.CheckedChanged += new System.EventHandler(On_Type_CheckedChanged);
			resources.ApplyResources(this._cbCreatedBy, "_cbCreatedBy");
			this._cbCreatedBy.Name = "_cbCreatedBy";
			this._cbCreatedBy.Properties.AutoWidth = true;
			this._cbCreatedBy.Properties.Caption = resources.GetString("_cbCreatedBy.Properties.Caption");
			this._cbCreatedBy.CheckedChanged += new System.EventHandler(On_CreatedBy_CheckedChanged);
			resources.ApplyResources(this._cbModifiedAfter, "_cbModifiedAfter");
			this._cbModifiedAfter.Name = "_cbModifiedAfter";
			this._cbModifiedAfter.Properties.AutoWidth = true;
			this._cbModifiedAfter.Properties.Caption = resources.GetString("_cbModifiedAfter.Properties.Caption");
			this._cbModifiedAfter.CheckedChanged += new System.EventHandler(On_ModifiedAfter_CheckedChanged);
			resources.ApplyResources(this._tbCreatedBy, "_tbCreatedBy");
			this._tbCreatedBy.Name = "_tbCreatedBy";
			this._tbCreatedBy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			resources.ApplyResources(this._cbItems, "_cbItems");
			this._cbItems.Name = "_cbItems";
			this._cbItems.Properties.AutoWidth = true;
			this._cbItems.Properties.Caption = resources.GetString("_cbItems.Properties.Caption");
			this._cbItems.CheckedChanged += new System.EventHandler(On_Type_CheckedChanged);
			resources.ApplyResources(this._cbFolders, "_cbFolders");
			this._cbFolders.Name = "_cbFolders";
			this._cbFolders.Properties.AutoWidth = true;
			this._cbFolders.Properties.Caption = resources.GetString("_cbFolders.Properties.Caption");
			this._cbFolders.CheckedChanged += new System.EventHandler(On_Type_CheckedChanged);
			resources.ApplyResources(this._cbCreatedBefore, "_cbCreatedBefore");
			this._cbCreatedBefore.Name = "_cbCreatedBefore";
			this._cbCreatedBefore.Properties.AutoWidth = true;
			this._cbCreatedBefore.Properties.Caption = resources.GetString("_cbCreatedBefore.Properties.Caption");
			this._cbCreatedBefore.CheckedChanged += new System.EventHandler(On_CreatedBefore_CheckedChanged);
			resources.ApplyResources(this._cbModifiedBy, "_cbModifiedBy");
			this._cbModifiedBy.Name = "_cbModifiedBy";
			this._cbModifiedBy.Properties.AutoWidth = true;
			this._cbModifiedBy.Properties.Caption = resources.GetString("_cbModifiedBy.Properties.Caption");
			this._cbModifiedBy.CheckedChanged += new System.EventHandler(On_ModifiedBy_CheckedChanged);
			resources.ApplyResources(this._cbRecursive, "_cbRecursive");
			this._cbRecursive.Name = "_cbRecursive";
			this._cbRecursive.Properties.AutoWidth = true;
			this._cbRecursive.Properties.Caption = resources.GetString("_cbRecursive.Properties.Caption");
			resources.ApplyResources(this._lblTypes, "_lblTypes");
			this._lblTypes.Name = "_lblTypes";
			resources.ApplyResources(this._cbCreatedAfter, "_cbCreatedAfter");
			this._cbCreatedAfter.Name = "_cbCreatedAfter";
			this._cbCreatedAfter.Properties.AutoWidth = true;
			this._cbCreatedAfter.Properties.Caption = resources.GetString("_cbCreatedAfter.Properties.Caption");
			this._cbCreatedAfter.CheckedChanged += new System.EventHandler(On_CreatedAfter_CheckedChanged);
			this._itemsView.BackColor = System.Drawing.Color.White;
			this._itemsView.ContextMenuStrip = this._searchResultsActionPalette;
			this._itemsView.DataConverter = null;
			this._itemsView.DataSource = null;
			this._itemsView.Filter = null;
			resources.ApplyResources(this._itemsView, "_itemsView");
			this._itemsView.MultiSelect = true;
			this._itemsView.Name = "_itemsView";
			this._itemsView.ShowPropertyGrid = false;
			this._itemsView.ViewFields = null;
			this._searchResultsActionPalette.HostingControl = null;
			this._searchResultsActionPalette.LegalType = null;
			this._searchResultsActionPalette.MonitoredSearchResults = null;
			this._searchResultsActionPalette.Name = "_searchResultsActionPalette";
			resources.ApplyResources(this._searchResultsActionPalette, "_searchResultsActionPalette");
			this._searchResultsActionPalette.SourceOverride = sourceOverride;
			this._searchResultsActionPalette.UseSourceOverride = false;
			this._layoutControl.Controls.Add(this._lblLine);
			this._layoutControl.Controls.Add(this._itemsView);
			this._layoutControl.Controls.Add(this._btnShowPropertyGrids);
			this._layoutControl.Controls.Add(this._lblStatus);
			this._layoutControl.Controls.Add(this._groupAdvanced);
			this._layoutControl.Controls.Add(this._searchLocation);
			this._layoutControl.Controls.Add(this._tsBtnCancel);
			this._layoutControl.Controls.Add(this._btnSearch);
			this._layoutControl.Controls.Add(this._teSearch);
			this._layoutControl.Controls.Add(this._btnAdvanced);
			resources.ApplyResources(this._layoutControl, "_layoutControl");
			this._layoutControl.Name = "_layoutControl";
			this._layoutControl.Root = this._layoutControlGroup;
			this._lblLine.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this._lblLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this._lblLine.LineVisible = true;
			resources.ApplyResources(this._lblLine, "_lblLine");
			this._lblLine.Name = "_lblLine";
			this._lblLine.StyleController = this._layoutControl;
			this._btnShowPropertyGrids.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Properties16;
			resources.ApplyResources(this._btnShowPropertyGrids, "_btnShowPropertyGrids");
			this._btnShowPropertyGrids.Name = "_btnShowPropertyGrids";
			this._btnShowPropertyGrids.StyleController = this._layoutControl;
			this._btnShowPropertyGrids.Click += new System.EventHandler(On_Properties_Clicked);
			resources.ApplyResources(this._lblStatus, "_lblStatus");
			this._lblStatus.Name = "_lblStatus";
			this._lblStatus.StyleController = this._layoutControl;
			resources.ApplyResources(this._tsBtnCancel, "_tsBtnCancel");
			this._tsBtnCancel.Name = "_tsBtnCancel";
			this._tsBtnCancel.StyleController = this._layoutControl;
			this._tsBtnCancel.Click += new System.EventHandler(On_Cancel_Clicked);
			this._btnSearch.Image = (System.Drawing.Image)resources.GetObject("_btnSearch.Image");
			resources.ApplyResources(this._btnSearch, "_btnSearch");
			this._btnSearch.Name = "_btnSearch";
			this._btnSearch.StyleController = this._layoutControl;
			this._btnSearch.Click += new System.EventHandler(On_Search);
			resources.ApplyResources(this._teSearch, "_teSearch");
			this._teSearch.Name = "_teSearch";
			this._teSearch.StyleController = this._layoutControl;
			this._teSearch.Click += new System.EventHandler(On_SearchTerm_Click);
			this._teSearch.Enter += new System.EventHandler(On_SearchTerm_Entered);
			this._teSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(On_KeyPress_SearchTerm);
			this._btnAdvanced.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
			resources.ApplyResources(this._btnAdvanced, "_btnAdvanced");
			this._btnAdvanced.Name = "_btnAdvanced";
			this._btnAdvanced.StyleController = this._layoutControl;
			this._btnAdvanced.Click += new System.EventHandler(On_Advanced_Clicked);
			resources.ApplyResources(this._layoutControlGroup, "_layoutControlGroup");
			this._layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this._layoutControlGroup.GroupBordersVisible = false;
			DevExpress.XtraLayout.Utils.LayoutGroupItemCollection items = this._layoutControlGroup.Items;
			DevExpress.XtraLayout.BaseLayoutItem[] items2 = new DevExpress.XtraLayout.BaseLayoutItem[10] { this._layoutAdvancedButton, this._layoutSearch, this._layoutSearchButton, this._layoutSearchLocation, this._layoutAdvancedOptions, this._layoutCancelButton, this._layoutPropertiesButton, this._layoutStatus, this._layoutItemsView, this._layoutLine };
			items.AddRange(items2);
			this._layoutControlGroup.Location = new System.Drawing.Point(0, 0);
			this._layoutControlGroup.Name = "_layoutControlGroup";
			this._layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(7, 7, 7, 7);
			this._layoutControlGroup.Size = new System.Drawing.Size(892, 583);
			this._layoutControlGroup.TextVisible = false;
			this._layoutAdvancedButton.Control = this._btnAdvanced;
			resources.ApplyResources(this._layoutAdvancedButton, "_layoutAdvancedButton");
			this._layoutAdvancedButton.Location = new System.Drawing.Point(754, 0);
			this._layoutAdvancedButton.Name = "_layoutAdvancedButton";
			this._layoutAdvancedButton.Size = new System.Drawing.Size(124, 26);
			this._layoutAdvancedButton.TextSize = new System.Drawing.Size(0, 0);
			this._layoutAdvancedButton.TextToControlDistance = 0;
			this._layoutAdvancedButton.TextVisible = false;
			this._layoutSearch.Control = this._teSearch;
			resources.ApplyResources(this._layoutSearch, "_layoutSearch");
			this._layoutSearch.Location = new System.Drawing.Point(0, 0);
			this._layoutSearch.Name = "_layoutSearch";
			this._layoutSearch.Size = new System.Drawing.Size(586, 26);
			this._layoutSearch.TextSize = new System.Drawing.Size(64, 13);
			this._layoutSearchButton.Control = this._btnSearch;
			resources.ApplyResources(this._layoutSearchButton, "_layoutSearchButton");
			this._layoutSearchButton.Location = new System.Drawing.Point(586, 0);
			this._layoutSearchButton.Name = "_layoutSearchButton";
			this._layoutSearchButton.Size = new System.Drawing.Size(84, 26);
			this._layoutSearchButton.TextSize = new System.Drawing.Size(0, 0);
			this._layoutSearchButton.TextToControlDistance = 0;
			this._layoutSearchButton.TextVisible = false;
			this._layoutSearchLocation.BestFitWeight = 66;
			this._layoutSearchLocation.Control = this._searchLocation;
			resources.ApplyResources(this._layoutSearchLocation, "_layoutSearchLocation");
			this._layoutSearchLocation.Location = new System.Drawing.Point(0, 26);
			this._layoutSearchLocation.Name = "_layoutSearchLocation";
			this._layoutSearchLocation.Size = new System.Drawing.Size(494, 284);
			this._layoutSearchLocation.TextSize = new System.Drawing.Size(0, 0);
			this._layoutSearchLocation.TextToControlDistance = 0;
			this._layoutSearchLocation.TextVisible = false;
			this._layoutAdvancedOptions.BestFitWeight = 66;
			this._layoutAdvancedOptions.Control = this._groupAdvanced;
			resources.ApplyResources(this._layoutAdvancedOptions, "_layoutAdvancedOptions");
			this._layoutAdvancedOptions.Location = new System.Drawing.Point(494, 26);
			this._layoutAdvancedOptions.Name = "_layoutAdvancedOptions";
			this._layoutAdvancedOptions.Size = new System.Drawing.Size(384, 284);
			this._layoutAdvancedOptions.TextSize = new System.Drawing.Size(0, 0);
			this._layoutAdvancedOptions.TextToControlDistance = 0;
			this._layoutAdvancedOptions.TextVisible = false;
			this._layoutAdvancedOptions.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this._layoutCancelButton.Control = this._tsBtnCancel;
			resources.ApplyResources(this._layoutCancelButton, "_layoutCancelButton");
			this._layoutCancelButton.Location = new System.Drawing.Point(670, 0);
			this._layoutCancelButton.Name = "_layoutCancelButton";
			this._layoutCancelButton.Size = new System.Drawing.Size(84, 26);
			this._layoutCancelButton.TextSize = new System.Drawing.Size(0, 0);
			this._layoutCancelButton.TextToControlDistance = 0;
			this._layoutCancelButton.TextVisible = false;
			this._layoutCancelButton.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this._layoutPropertiesButton.Control = this._btnShowPropertyGrids;
			this._layoutPropertiesButton.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
			resources.ApplyResources(this._layoutPropertiesButton, "_layoutPropertiesButton");
			this._layoutPropertiesButton.Location = new System.Drawing.Point(31, 323);
			this._layoutPropertiesButton.Name = "_layoutPropertiesButton";
			this._layoutPropertiesButton.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this._layoutPropertiesButton.Size = new System.Drawing.Size(847, 22);
			this._layoutPropertiesButton.TextSize = new System.Drawing.Size(0, 0);
			this._layoutPropertiesButton.TextToControlDistance = 0;
			this._layoutPropertiesButton.TextVisible = false;
			this._layoutStatus.Control = this._lblStatus;
			resources.ApplyResources(this._layoutStatus, "_layoutStatus");
			this._layoutStatus.Location = new System.Drawing.Point(0, 323);
			this._layoutStatus.Name = "_layoutStatus";
			this._layoutStatus.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this._layoutStatus.Size = new System.Drawing.Size(31, 22);
			this._layoutStatus.TextSize = new System.Drawing.Size(0, 0);
			this._layoutStatus.TextToControlDistance = 0;
			this._layoutStatus.TextVisible = false;
			this._layoutItemsView.Control = this._itemsView;
			resources.ApplyResources(this._layoutItemsView, "_layoutItemsView");
			this._layoutItemsView.Location = new System.Drawing.Point(0, 345);
			this._layoutItemsView.Name = "_layoutItemsView";
			this._layoutItemsView.Size = new System.Drawing.Size(878, 224);
			this._layoutItemsView.TextSize = new System.Drawing.Size(0, 0);
			this._layoutItemsView.TextToControlDistance = 0;
			this._layoutItemsView.TextVisible = false;
			this._layoutLine.Control = this._lblLine;
			this._layoutLine.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			resources.ApplyResources(this._layoutLine, "_layoutLine");
			this._layoutLine.Location = new System.Drawing.Point(0, 310);
			this._layoutLine.Name = "_layoutLine";
			this._layoutLine.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this._layoutLine.Size = new System.Drawing.Size(878, 13);
			this._layoutLine.TextSize = new System.Drawing.Size(0, 0);
			this._layoutLine.TextToControlDistance = 0;
			this._layoutLine.TextVisible = false;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this._layoutControl);
			base.Name = "SearchDialog";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(On_Closing);
			base.Load += new System.EventHandler(On_Load);
			base.Shown += new System.EventHandler(On_Shown);
			((System.ComponentModel.ISupportInitialize)this._groupAdvanced).EndInit();
			this._groupAdvanced.ResumeLayout(false);
			this._groupAdvanced.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this._cmbContentType.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbMatchExactly.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._tbModifiedBy.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbContentType.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedAfter.Properties.VistaTimeProperties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedAfter.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedBefore.Properties.VistaTimeProperties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpModifiedBefore.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedBefore.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedAfter.Properties.VistaTimeProperties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedAfter.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbSites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedBefore.Properties.VistaTimeProperties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._dtpCreatedBefore.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbDocuments.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedBy.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedAfter.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._tbCreatedBy.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbFolders.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedBefore.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbModifiedBy.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbRecursive.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbCreatedAfter.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutControl).EndInit();
			this._layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._teSearch.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutControlGroup).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutAdvancedButton).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearch).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearchButton).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutSearchLocation).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutAdvancedOptions).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutCancelButton).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutPropertiesButton).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutStatus).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutItemsView).EndInit();
			((System.ComponentModel.ISupportInitialize)this._layoutLine).EndInit();
			base.ResumeLayout(false);
		}

		private void ItemsView_SelectedItems_Changed(ListItemCollection item)
		{
			SPSearchResultsCollection sPSearchResultsCollection = item as SPSearchResultsCollection;
			_searchResultsActionPalette.MonitoredSearchResults = sPSearchResultsCollection;
			if (sPSearchResultsCollection == null)
			{
				return;
			}
			foreach (SPSearchResult item2 in sPSearchResultsCollection)
			{
				try
				{
					SPNode node = item2.Node;
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
		}

		private void LoadParameters(SPSearchParameters parameters)
		{
			_cbRecursive.Checked = parameters.Recursive;
			_cbMatchExactly.Checked = parameters.MatchExactly;
			_cbItems.Checked = parameters.IncludeItems;
			_cbDocuments.Checked = parameters.IncludeDocuments;
			_cbFolders.Checked = parameters.IncludeFolders;
			_cbLists.Checked = parameters.IncludeLists;
			_cbSites.Checked = parameters.IncludeSites;
			_cbContentType.Checked = parameters.ContentType != null;
			_cmbContentType.Text = parameters.ContentType;
			_cbCreatedBy.Checked = parameters.CreatedBy != null;
			_tbCreatedBy.Text = parameters.CreatedBy;
			_cbModifiedBy.Checked = parameters.ModifiedBy != null;
			_tbModifiedBy.Text = parameters.ModifiedBy;
			_cbCreatedBefore.Checked = parameters.CreatedBefore != DateTime.MinValue;
			if (_cbCreatedBefore.Checked)
			{
				_dtpCreatedBefore.DateTime = parameters.CreatedBefore;
			}
			_cbCreatedAfter.Checked = parameters.CreatedAfter != DateTime.MinValue;
			if (_cbCreatedAfter.Checked)
			{
				_dtpCreatedAfter.DateTime = parameters.CreatedAfter;
			}
			_cbModifiedBefore.Checked = parameters.ModifiedBefore != DateTime.MinValue;
			if (_cbModifiedBefore.Checked)
			{
				_dtpModifiedBefore.DateTime = parameters.ModifiedBefore;
			}
			_cbModifiedAfter.Checked = parameters.ModifiedAfter != DateTime.MinValue;
			if (_cbModifiedAfter.Checked)
			{
				_dtpModifiedAfter.DateTime = parameters.ModifiedAfter;
			}
		}

		private void LoadUI()
		{
			if (SearchNode != null && _searchLocation.DataSource != null)
			{
				_searchLocation.NavigateToNode(SearchNode);
			}
		}

		private void On_Advanced_Clicked(object sender, EventArgs e)
		{
			_layoutAdvancedOptions.Visibility = ((_layoutAdvancedOptions.Visibility == LayoutVisibility.Always) ? LayoutVisibility.Never : LayoutVisibility.Always);
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			CancelSearch();
		}

		private void On_Closing(object sender, FormClosingEventArgs e)
		{
			StartLocation = base.Location;
			_teSearch.Focus();
			m_bJustEnteredSearchTerm = false;
			_teSearch.SelectAll();
			Hide();
			e.Cancel = true;
		}

		private void On_ContentType_CheckChanged(object sender, EventArgs e)
		{
			_cmbContentType.Enabled = _cbContentType.Checked;
		}

		private void On_ContentType_TextChanged(object sender, EventArgs e)
		{
			m_sContentTypeText = _cmbContentType.Text;
		}

		private void On_CreatedAfter_CheckedChanged(object sender, EventArgs e)
		{
			_dtpCreatedAfter.Enabled = _cbCreatedAfter.Checked;
		}

		private void On_CreatedBefore_CheckedChanged(object sender, EventArgs e)
		{
			_dtpCreatedBefore.Enabled = _cbCreatedBefore.Checked;
		}

		private void On_CreatedBy_CheckedChanged(object sender, EventArgs e)
		{
			_tbCreatedBy.Enabled = _cbCreatedBy.Enabled && _cbCreatedBy.Checked;
		}

		private void On_ExplorerNode_Changed(ReadOnlyCollection<ExplorerTreeNode> selectedNodes)
		{
			if (selectedNodes.Count > 0)
			{
				m_searchNode = selectedNodes[selectedNodes.Count - 1].Node as SPNode;
				UpdateUI();
			}
		}

		private void On_KeyPress_SearchTerm(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				if (_layoutSearchButton.Visible)
				{
					RunSearch();
				}
				else
				{
					CancelSearch();
				}
			}
		}

		private void On_Load(object sender, EventArgs e)
		{
			_searchLocation.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
			base.Location = StartLocation;
			LoadUI();
		}

		private void On_ModifiedAfter_CheckedChanged(object sender, EventArgs e)
		{
			_dtpModifiedAfter.Enabled = _cbModifiedAfter.Checked;
		}

		private void On_ModifiedBefore_CheckedChanged(object sender, EventArgs e)
		{
			_dtpModifiedBefore.Enabled = _cbModifiedBefore.Checked;
		}

		private void On_ModifiedBy_CheckedChanged(object sender, EventArgs e)
		{
			_tbModifiedBy.Enabled = _cbModifiedBy.Enabled && _cbModifiedBy.Checked;
		}

		private void On_Properties_Clicked(object sender, EventArgs e)
		{
			_itemsView.ShowPropertyGrid = !_itemsView.ShowPropertyGrid;
			if (_itemsView.ShowPropertyGrid)
			{
				_btnShowPropertyGrids.Text = "Hide Properties";
				_btnShowPropertyGrids.ToolTip = "Hide Item Properties";
			}
			else
			{
				_btnShowPropertyGrids.Text = "Show Properties";
				_btnShowPropertyGrids.ToolTip = "Show Item Properties";
			}
		}

		private void On_Search(object sender, EventArgs e)
		{
			RunSearch();
		}

		private void On_SearchTerm_Click(object sender, EventArgs e)
		{
			if (m_bJustEnteredSearchTerm)
			{
				_teSearch.SelectAll();
				m_bJustEnteredSearchTerm = false;
			}
		}

		private void On_SearchTerm_Entered(object sender, EventArgs e)
		{
			m_bJustEnteredSearchTerm = true;
		}

		private void On_Shown(object sender, EventArgs e)
		{
			_teSearch.Focus();
		}

		private void On_Type_CheckedChanged(object sender, EventArgs e)
		{
			bool flag = _cbItems.Checked || _cbDocuments.Checked || _cbFolders.Checked;
			_cbContentType.Enabled = flag;
			_cbCreatedBy.Enabled = flag;
			_cbModifiedBy.Enabled = flag;
			_cmbContentType.Enabled = flag && _cbContentType.Checked;
			_tbCreatedBy.Enabled = flag && _cbCreatedBy.Checked;
			_tbModifiedBy.Enabled = flag && _cbModifiedBy.Checked;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			_searchResultsActionPalette.BuildActionMenu();
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void RunSearch()
		{
			bool flag = false;
			if (_searchLocation.SelectedNode != null && (_teSearch.Text.Length > 0 || _cbCreatedAfter.Checked || _cbCreatedBefore.Checked || _cbCreatedBy.Checked || _cbModifiedAfter.Checked || _cbModifiedBefore.Checked || _cbModifiedBy.Checked || _cbContentType.Checked))
			{
				SetSearchButtonEnabled(bEnabled: false);
				if (m_searchThread != null && m_searchThread.ThreadState == ThreadState.Running)
				{
					CancelSearch();
				}
				m_searchThread = new Thread(Search);
				m_searchThread.Start();
			}
		}

		private void Search()
		{
			SPSearchResultsCollection sPSearchResultsCollection = new SPSearchResultsCollection(SearchNode, _teSearch.Text);
			try
			{
				SetCursor(Cursors.WaitCursor);
				SetStatusText("Searching...");
				sPSearchResultsCollection.Parameters = GetParameters();
				bool flag = sPSearchResultsCollection.ExecuteSearch();
				FillItemsView(sPSearchResultsCollection, sPSearchResultsCollection.ResultFields);
				SetCursor(Cursors.Default);
				object[] array = new object[4] { "Search Complete - ", sPSearchResultsCollection.Count, " results found", null };
				array[3] = (flag ? "" : " - Site collections where the user is not an admin were skipped");
				SetStatusText(string.Concat(array));
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (!m_bCancelled)
				{
					SetStatusText("Search failed: " + ex2.Message);
				}
				else
				{
					SetStatusText("Search Cancelled");
				}
				m_bCancelled = false;
			}
			finally
			{
				SetSearchButtonEnabled(bEnabled: true);
				SetCursor(Cursors.Default);
			}
		}

		private void SetCursor(Cursor cursor)
		{
			if (!base.InvokeRequired)
			{
				Cursor = cursor;
				return;
			}
			Delegate method = new CursorDelegate(SetCursor);
			object[] args = new object[1] { cursor };
			Invoke(method, args);
		}

		private void SetSearchButtonEnabled(bool bEnabled)
		{
			if (base.InvokeRequired)
			{
				Invoke((MethodInvoker)delegate
				{
					SetSearchButtonEnabled(bEnabled);
				});
			}
			else
			{
				_layoutSearchButton.Visibility = ((!bEnabled) ? LayoutVisibility.Never : LayoutVisibility.Always);
				_layoutCancelButton.Visibility = (bEnabled ? LayoutVisibility.Never : LayoutVisibility.Always);
			}
		}

		private void SetStatusText(string sStatusText)
		{
			if (!base.InvokeRequired)
			{
				_lblStatus.Text = sStatusText;
				return;
			}
			Invoke((MethodInvoker)delegate
			{
				SetStatusText(sStatusText);
			});
		}

		private void UpdateUI()
		{
			_cmbContentType.Properties.Items.Clear();
			_cmbContentType.MaskBox.AutoCompleteCustomSource.Clear();
			if (m_searchNode is SPServer)
			{
				return;
			}
			if (!typeof(SPFolder).IsAssignableFrom(m_searchNode.GetType()))
			{
				_cbSites.Enabled = true;
				_cbLists.Enabled = true;
				if (!(m_searchNode is SPWeb))
				{
					return;
				}
				{
					foreach (SPContentType contentType in ((SPWeb)m_searchNode).ContentTypes)
					{
						if (contentType.Name != null)
						{
							_cmbContentType.Properties.Items.Add(contentType.Name);
							_cmbContentType.MaskBox.AutoCompleteCustomSource.Add(contentType.Name);
						}
					}
					return;
				}
			}
			_cbLists.Enabled = false;
			_cbSites.Enabled = false;
			foreach (SPContentType contentType2 in ((SPFolder)m_searchNode).ParentList.ContentTypes)
			{
				if (contentType2.Name != null)
				{
					_cmbContentType.Properties.Items.Add(contentType2.Name);
					_cmbContentType.MaskBox.AutoCompleteCustomSource.Add(contentType2.Name);
				}
			}
		}
	}
}
