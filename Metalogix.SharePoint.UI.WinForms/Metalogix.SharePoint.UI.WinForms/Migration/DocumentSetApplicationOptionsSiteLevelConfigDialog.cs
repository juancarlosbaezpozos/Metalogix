using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Filters;
using Metalogix.UI.WinForms.Explorer;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class DocumentSetApplicationOptionsSiteLevelConfigDialog : GenericSiteLevelMappingDialog
	{
		private SimplifiedGridView foldersListView;

		private BindingList<MappingOption> _folderDataSource;

		private int m_iFoldersViewValuesColumnMinWidth;

		private BarButtonItem _editButton;

		private BarButtonItem _removeButton;

		private BarButtonItem _navigateButton;

		private IEnumerable<SPContentType> m_targetContentTypes;

		private SerializableList<DocumentSetApplicationOptionsCollection> m_ListOptions;

		private SerializableList<DocumentSetFolderOptions> m_FolderOptions;

		private bool m_bTreatListAsFolder;

		private IContainer components;

		private ContextMenuStrip w_cmsFoldersMenu;

		private ToolStripMenuItem editContentTypeToolStripMenuItem;

		private ToolStripMenuItem removeFolderDocSetToolStripMenuItem;

		private ToolStripMenuItem navigateToFolderStripMenuItem;

		public SerializableList<DocumentSetFolderOptions> FolderOptions
		{
			get
			{
				return m_FolderOptions;
			}
			set
			{
				m_FolderOptions = value;
				LoadFoldersUI();
			}
		}

		public SerializableList<DocumentSetApplicationOptionsCollection> Options
		{
			get
			{
				return m_ListOptions;
			}
			set
			{
				m_ListOptions = value;
				LoadOptionsUI();
			}
		}

		public IEnumerable<SPContentType> TargetContentTypes
		{
			get
			{
				return m_targetContentTypes;
			}
			set
			{
				m_targetContentTypes = value;
			}
		}

		public bool TreatListAsFolder
		{
			get
			{
				return m_bTreatListAsFolder;
			}
			set
			{
				m_bTreatListAsFolder = value;
			}
		}

		protected override IUrlParser UrlParser
		{
			get
			{
				IUrlParser urlParser = _urlParser;
				if (urlParser == null)
				{
					urlParser = (_urlParser = new DocumentSetApplicationOptionsCollectionFilterParser());
				}
				return urlParser;
			}
		}

		public DocumentSetApplicationOptionsSiteLevelConfigDialog(NodeCollection nodeCollection)
			: base(nodeCollection)
		{
			InitializeComponent();
			base.ExplorerSelectableTypes = new List<Type>(new Type[1] { typeof(SPFolder) });
			base.DialogTitle = Metalogix.SharePoint.Properties.Resources.DocSetDialogTitle;
			base.ApplyOptionText = Metalogix.SharePoint.Properties.Resources.DocSetDialogApplyOption;
			base.CreateRuleText = Metalogix.SharePoint.Properties.Resources.DocSetDialogCreateOptionViaRule;
			base.CreateNewOptionText = Metalogix.SharePoint.Properties.Resources.DocSetDialogCreateOption;
			base.CreateOptionToListText = Metalogix.SharePoint.Properties.Resources.DocSetDialogCreateOptionToList;
			base.OptionsLabelText = Metalogix.SharePoint.Properties.Resources.DocSetDialogOptionsLabel;
			base.OptionColumnText = Metalogix.SharePoint.Properties.Resources.DocSetDialogOptionsColumn;
			base.OptionAppliedValuesText = Metalogix.SharePoint.Properties.Resources.DocSetDialogOptionsValue;
			base.EditOptionText = Metalogix.SharePoint.Properties.Resources.DocSetDialogEditOption;
			base.RemoveOptionText = Metalogix.SharePoint.Properties.Resources.DocSetDialogRemoveOption;
			base.OriginalTabText = Metalogix.SharePoint.Properties.Resources.DocSetDialogOriginalTabText;
			InitializeMenuBarItems();
			AdditionalOptionTab item = new AdditionalOptionTab(Metalogix.SharePoint.Properties.Resources.DocSetDialogFolderTabTitle, Metalogix.SharePoint.Properties.Resources.DocSetDialogFolderTabObjectColumn, Metalogix.SharePoint.Properties.Resources.DocSetDialogFolderTabOptionColumn, GetMenuBarItems(), w_cmsFoldersMenu)
			{
				OptionColumnWidth = 160
			};
			AdditionalTabs.Add(item);
			List<SimplifiedGridView> list = UpdateTabs();
			if (list.Count == 1)
			{
				foldersListView = list[0];
				_folderDataSource = new BindingList<MappingOption>();
				foldersListView.DataSource = _folderDataSource;
				foldersListView.SelectionChanged += foldersListView_SelectedIndexChanged;
				UpdateFoldersUI();
				m_iFoldersViewValuesColumnMinWidth = foldersListView.GetColumnByFieldName("OptionAppliedValue").Width;
			}
			base.CreateRuleCmbButtonVisible = true;
			if (((Node)nodeCollection[0]) is SPList || ((Node)nodeCollection[0]) is SPFolder)
			{
				base.AllowRuleModification = false;
			}
			if (((Node)nodeCollection[0]) is SPListItem)
			{
				base.SourceIsListItem = true;
			}
			UpdateUI();
			SetDialogText();
		}

		protected override void CheckValidNode(ExplorerTreeNode treeNode, ref bool bEnabledCreateNodeOption, ref bool bEnabledCreateListRuleOption)
		{
			bEnabledCreateListRuleOption = false;
			bEnabledCreateNodeOption = false;
			if (treeNode != null)
			{
				if (treeNode.Node is SPList && (treeNode.Node as SPList).IsDocumentLibrary)
				{
					bEnabledCreateListRuleOption = true;
					bEnabledCreateNodeOption = false;
				}
				else if (treeNode.Node is SPFolder && (treeNode.Node as SPFolder).ParentList.IsDocumentLibrary)
				{
					bEnabledCreateListRuleOption = false;
					bEnabledCreateNodeOption = true;
				}
			}
		}

		protected override void CreateListRule()
		{
			ExplorerTreeNode selectedNode = base.SelectedNode;
			if (selectedNode == null || !(selectedNode.Node is SPList) || !(selectedNode.Node as SPList).IsDocumentLibrary)
			{
				return;
			}
			if (TreatListAsFolder)
			{
				CreateNewApplyOptions(selectedNode);
				return;
			}
			foreach (MappingOption mapping in base.MappingList)
			{
				if (((DocumentSetApplicationOptionsCollection)mapping.SubObject).AppliesToFilter is FilterExpression)
				{
					SPNode sPNode = (SPNode)selectedNode.Node;
					if (!((string)sPNode.GetProperties()["DisplayUrl"].GetValue(sPNode) != ((FilterExpression)((DocumentSetApplicationOptionsCollection)mapping.SubObject).AppliesToFilter).Pattern))
					{
						EditOptionAppliedValue(mapping);
						SelectTab(1);
						return;
					}
				}
			}
			DocumentSetApplicationOptionsCollection documentSetApplicationOptionsCollection = new DocumentSetApplicationOptionsCollection();
			documentSetApplicationOptionsCollection.AppliesToFilter = new FilterExpression(FilterOperand.Equals, typeof(SPList), "DisplayUrl", selectedNode.Node.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false);
			DocumentSetApplicationOptionsCollection documentSetApplicationOptionsCollection2 = documentSetApplicationOptionsCollection;
			DocumentSetDocumentMappingDialog documentSetDocumentMappingDialog = new DocumentSetDocumentMappingDialog(m_targetContentTypes)
			{
				Options = documentSetApplicationOptionsCollection2
			};
			if (documentSetDocumentMappingDialog.ShowDialog() == DialogResult.OK && documentSetDocumentMappingDialog.Options.Data.Count > 0)
			{
				MappingOption option = new MappingOption
				{
					SubObject = documentSetApplicationOptionsCollection2,
					Option = ((FilterExpression)documentSetApplicationOptionsCollection2.AppliesToFilter).Pattern,
					OptionAppliedValue = GetOptionsCollectionDisplayString(documentSetApplicationOptionsCollection2)
				};
				AddOption(option);
				UpdateFoldersViewColumnWidths();
			}
			SelectTab(1);
		}

		protected override void CreateNewApplyOptions(ExplorerTreeNode treeNode)
		{
			ExplorerTreeNode selectedNode = base.SelectedNode;
			if (selectedNode == null)
			{
				return;
			}
			foreach (MappingOption item2 in _folderDataSource)
			{
				if (item2.SubObject is FilterExpression)
				{
					SPNode sPNode = (SPNode)selectedNode.Node;
					if (!((string)sPNode.GetProperties()["DisplayUrl"].GetValue(sPNode) != ((FilterExpression)item2.SubObject).Pattern))
					{
						EditFolderOptionContentType(item2);
						SelectTab(0);
						return;
					}
				}
			}
			DocumentSetFolderOptionsConfigDialog documentSetFolderOptionsConfigDialog = new DocumentSetFolderOptionsConfigDialog(m_targetContentTypes);
			if (documentSetFolderOptionsConfigDialog.ShowDialog() == DialogResult.OK && documentSetFolderOptionsConfigDialog.SelectedCT != null)
			{
				FilterExpression filterExpression = new FilterExpression(FilterOperand.Equals, typeof(SPListItem), "DisplayUrl", selectedNode.Node.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false);
				MappingOption item = new MappingOption
				{
					SubObject = filterExpression,
					Option = filterExpression.Pattern,
					OptionAppliedValue = documentSetFolderOptionsConfigDialog.SelectedCT
				};
				_folderDataSource.Add(item);
				UpdateFoldersViewColumnWidths();
			}
			SelectTab(0);
		}

		protected override void CreateRule()
		{
			bool sourceIsListItem = base.SourceIsListItem;
			bool flag = false;
			IFilterExpression filterExpression = null;
			string text = "";
			if (base.SourceIsListItem)
			{
				FilterExpression filterExpression2 = new FilterExpression(FilterOperand.NotEquals, typeof(SPList), "ID", "0", bIsCaseSensitive: false, bIsBaseFilter: false, null);
				filterExpression = filterExpression2;
				flag = true;
				text = "Items";
			}
			else
			{
				FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog();
				filterExpressionEditorDialog.Title = "List Filter Conditions";
				filterExpressionEditorDialog.LabelText = "Specify filters to determine which lists and libraries documents will be taken from.";
				filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type> { typeof(SPList) }, bAllowFreeFormEntry: false);
				FilterExpressionEditorDialog filterExpressionEditorDialog2 = filterExpressionEditorDialog;
				filterExpressionEditorDialog2.ShowDialog();
				filterExpression = filterExpressionEditorDialog2.FilterExpression;
				flag = filterExpressionEditorDialog2.DialogResult == DialogResult.OK && filterExpression != null;
				text = ((filterExpression != null) ? filterExpression.GetLogicString() : "");
			}
			if (flag)
			{
				DocumentSetApplicationOptionsCollection options = new DocumentSetApplicationOptionsCollection();
				DocumentSetDocumentMappingDialog documentSetDocumentMappingDialog = new DocumentSetDocumentMappingDialog(m_targetContentTypes)
				{
					Options = options
				};
				MappingOption mappingOption = new MappingOption
				{
					Option = text
				};
				if (documentSetDocumentMappingDialog.ShowDialog() == DialogResult.OK)
				{
					ListViewItem.ListViewSubItem listViewSubItem = new ListViewItem.ListViewSubItem();
					options = documentSetDocumentMappingDialog.Options;
					options.AppliesToFilter = filterExpression;
					mappingOption.SubObject = options;
					mappingOption.OptionAppliedValue = GetOptionsCollectionDisplayString(documentSetDocumentMappingDialog.Options);
					AddOption(mappingOption);
				}
				UpdateOptionsColumnWidths();
			}
			SelectTab(1);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EditFolderOptionContentType(MappingOption folderOption)
		{
			if (folderOption != null)
			{
				DocumentSetFolderOptionsConfigDialog documentSetFolderOptionsConfigDialog = new DocumentSetFolderOptionsConfigDialog(m_targetContentTypes)
				{
					SelectedCT = folderOption.OptionAppliedValue
				};
				if (documentSetFolderOptionsConfigDialog.ShowDialog() == DialogResult.OK && documentSetFolderOptionsConfigDialog.SelectedCT != null && !documentSetFolderOptionsConfigDialog.SelectedCT.Equals(folderOption.OptionAppliedValue))
				{
					folderOption.OptionAppliedValue = documentSetFolderOptionsConfigDialog.SelectedCT;
					UpdateFoldersViewColumnWidths();
				}
			}
		}

		protected override void EditOptionAppliedValue(MappingOption option)
		{
			DocumentSetApplicationOptionsCollection documentSetApplicationOptionsCollection = (DocumentSetApplicationOptionsCollection)option.SubObject;
			DocumentSetDocumentMappingDialog documentSetDocumentMappingDialog = new DocumentSetDocumentMappingDialog(m_targetContentTypes)
			{
				Options = (DocumentSetApplicationOptionsCollection)option.SubObject
			};
			if (documentSetDocumentMappingDialog.ShowDialog() == DialogResult.OK)
			{
				documentSetApplicationOptionsCollection = (DocumentSetApplicationOptionsCollection)(option.SubObject = documentSetDocumentMappingDialog.Options);
				option.OptionAppliedValue = GetOptionsCollectionDisplayString(documentSetApplicationOptionsCollection);
				UpdateOptionsColumnWidths();
			}
		}

		protected override void EditRule(MappingOption option)
		{
			DocumentSetApplicationOptionsCollection documentSetApplicationOptionsCollection = (DocumentSetApplicationOptionsCollection)option.SubObject;
			FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog();
			filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type> { typeof(SPList) }, bAllowFreeFormEntry: false);
			filterExpressionEditorDialog.FilterExpression = documentSetApplicationOptionsCollection.AppliesToFilter;
			filterExpressionEditorDialog.Title = "List Filter Conditions";
			filterExpressionEditorDialog.LabelText = "Specify filters to determine which lists and libraries documents will be taken from.";
			FilterExpressionEditorDialog filterExpressionEditorDialog2 = filterExpressionEditorDialog;
			if (filterExpressionEditorDialog2.ShowDialog() == DialogResult.OK)
			{
				documentSetApplicationOptionsCollection.AppliesToFilter = filterExpressionEditorDialog2.FilterExpression;
				option.SubObject = documentSetApplicationOptionsCollection.AppliesToFilter;
				option.Option = ((filterExpressionEditorDialog2.FilterExpression != null) ? filterExpressionEditorDialog2.FilterExpression.GetLogicString() : "");
			}
		}

		private void foldersListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateFoldersUI();
		}

		private MappingOption GetMappingOption(DocumentSetApplicationOptionsCollection collection)
		{
			MappingOption mappingOption = new MappingOption
			{
				Option = ""
			};
			if (collection != null && collection.AppliesToFilter != null)
			{
				IFilterExpression appliesToFilter = collection.AppliesToFilter;
				if (appliesToFilter != null)
				{
					mappingOption.Option = appliesToFilter.GetLogicString();
				}
			}
			mappingOption.SubObject = collection;
			mappingOption.OptionAppliedValue = GetOptionsCollectionDisplayString(collection);
			return mappingOption;
		}

		private IEnumerable<BarItem> GetMenuBarItems()
		{
			return new List<BarItem> { _editButton, _removeButton, _navigateButton };
		}

		private string GetOptionsCollectionDisplayString(DocumentSetApplicationOptionsCollection collection)
		{
			string text = "";
			foreach (DocumentSetApplicationOptions datum in collection.Data)
			{
				text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + datum.ContentTypeName;
			}
			return text;
		}

		private new void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.DocumentSetApplicationOptionsSiteLevelConfigDialog));
			this.w_cmsFoldersMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.editContentTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeFolderDocSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.navigateToFolderStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.w_cmsFoldersMenu.SuspendLayout();
			base.SuspendLayout();
			System.Windows.Forms.ToolStripItemCollection items = this.w_cmsFoldersMenu.Items;
			System.Windows.Forms.ToolStripItem[] toolStripItems = new System.Windows.Forms.ToolStripItem[3] { this.editContentTypeToolStripMenuItem, this.removeFolderDocSetToolStripMenuItem, this.navigateToFolderStripMenuItem };
			items.AddRange(toolStripItems);
			this.w_cmsFoldersMenu.Name = "w_cmsListViewMenu";
			resources.ApplyResources(this.w_cmsFoldersMenu, "w_cmsFoldersMenu");
			this.editContentTypeToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16;
			resources.ApplyResources(this.editContentTypeToolStripMenuItem, "editContentTypeToolStripMenuItem");
			this.editContentTypeToolStripMenuItem.Name = "editContentTypeToolStripMenuItem";
			this.editContentTypeToolStripMenuItem.Click += new System.EventHandler(w_tsbEditContentType_Click);
			this.removeFolderDocSetToolStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Minus16;
			this.removeFolderDocSetToolStripMenuItem.Name = "removeFolderDocSetToolStripMenuItem";
			resources.ApplyResources(this.removeFolderDocSetToolStripMenuItem, "removeFolderDocSetToolStripMenuItem");
			this.removeFolderDocSetToolStripMenuItem.Click += new System.EventHandler(w_tsbRemoveFolderRule_Click);
			this.navigateToFolderStripMenuItem.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Navigate16;
			resources.ApplyResources(this.navigateToFolderStripMenuItem, "navigateToFolderStripMenuItem");
			this.navigateToFolderStripMenuItem.Name = "navigateToFolderStripMenuItem";
			this.navigateToFolderStripMenuItem.Click += new System.EventHandler(w_tsbNavigateToFolder_Click);
			resources.ApplyResources(this, "$this");
			base.Name = "DocumentSetApplicationOptionsSiteLevelConfigDialog";
			this.w_cmsFoldersMenu.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void InitializeMenuBarItems()
		{
			_editButton = new BarButtonItem
			{
				Caption = "Edit Content Type",
				Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Edit16,
				PaintStyle = BarItemPaintStyle.CaptionGlyph
			};
			_editButton.ItemClick += w_tsbEditContentType_Click;
			_removeButton = new BarButtonItem
			{
				Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Minus16,
				Caption = "Remove",
				PaintStyle = BarItemPaintStyle.CaptionGlyph
			};
			_removeButton.ItemClick += w_tsbRemoveFolderRule_Click;
			_navigateButton = new BarButtonItem
			{
				Glyph = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Navigate16,
				Caption = "Navigate To",
				PaintStyle = BarItemPaintStyle.CaptionGlyph
			};
			_navigateButton.ItemClick += w_tsbNavigateToFolder_Click;
		}

		private void LoadFoldersUI()
		{
			_folderDataSource.Clear();
			if (FolderOptions == null)
			{
				UpdateFoldersUI();
				return;
			}
			foreach (DocumentSetFolderOptions folderOption in FolderOptions)
			{
				MappingOption item = new MappingOption
				{
					SubObject = folderOption.FolderFilter,
					Option = folderOption.FolderFilter.Pattern,
					OptionAppliedValue = folderOption.ContentTypeName
				};
				_folderDataSource.Add(item);
			}
			UpdateFoldersUI();
			UpdateFoldersViewColumnWidths();
		}

		protected override void LoadOptionsUI()
		{
			ClearOptions();
			if (Options == null)
			{
				UpdateUI();
				return;
			}
			foreach (DocumentSetApplicationOptionsCollection option in Options)
			{
				AddOption(GetMappingOption(option.Clone()));
			}
			UpdateUI();
			UpdateOptionsColumnWidths();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
		}

		protected override void SaveUI()
		{
			Options.Clear();
			FolderOptions.Clear();
			foreach (MappingOption mapping in base.MappingList)
			{
				Options.Add((DocumentSetApplicationOptionsCollection)mapping.SubObject);
			}
			DocumentSetFolderOptions documentSetFolderOptions = null;
			foreach (MappingOption item in _folderDataSource)
			{
				documentSetFolderOptions = new DocumentSetFolderOptions(item.OptionAppliedValue)
				{
					FolderFilter = (FilterExpression)item.SubObject
				};
				FolderOptions.Add(documentSetFolderOptions);
			}
		}

		protected void UpdateFoldersUI()
		{
			object[] selectedItems = foldersListView.SelectedItems;
			_editButton.Enabled = selectedItems.Length == 1;
			_navigateButton.Enabled = selectedItems.Length == 1;
			_removeButton.Enabled = selectedItems.Length != 0;
			editContentTypeToolStripMenuItem.Enabled = _editButton.Enabled;
			removeFolderDocSetToolStripMenuItem.Enabled = _removeButton.Enabled;
			navigateToFolderStripMenuItem.Enabled = _navigateButton.Enabled;
		}

		protected void UpdateFoldersViewColumnWidths()
		{
			foldersListView.RunAutoWidthOnAllColumns();
		}

		private void w_tsbEditContentType_Click(object sender, EventArgs e)
		{
			if (foldersListView != null && foldersListView.SelectedItems.Length == 1)
			{
				EditFolderOptionContentType((MappingOption)foldersListView.SelectedItems[0]);
			}
		}

		private void w_tsbNavigateToFolder_Click(object sender, EventArgs e)
		{
			if (foldersListView != null && foldersListView.SelectedItems.Length == 1)
			{
				NavigateTo((MappingOption)foldersListView.SelectedItems[0]);
			}
		}

		private void w_tsbRemoveFolderRule_Click(object sender, EventArgs e)
		{
			if (foldersListView != null && foldersListView.SelectedItems.Length != 0)
			{
				List<MappingOption> list = new List<MappingOption>();
				object[] selectedItems = foldersListView.SelectedItems;
				for (int i = 0; i < selectedItems.Length; i++)
				{
					MappingOption item = (MappingOption)selectedItems[i];
					_folderDataSource.Remove(item);
				}
			}
		}
	}
}
