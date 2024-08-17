using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.ListContentOptions.png")]
	[ControlName("List Content Options")]
	public class TCListContentOptions : ListContentScopableTabbableControl
	{
		private const string LISTNAMELABEL = "Change List Name To";

		private const string LISTTITLELABEL = "Change List Title To";

		private const int INDENTWIDTH = 18;

		public const string DOCUMENT_SET_ID = "0x0120D520";

		private CommonSerializableList<ContentTypeApplicationOptionsCollection> m_contentTypeApplicationOptions;

		private CommonSerializableList<DocumentSetApplicationOptionsCollection> m_documentSetApplicationOptions;

		private CommonSerializableList<DocumentSetFolderOptions> m_folderToDocumentSetApplicationOptions;

		public bool isAzureUserMappingWarningMessageRepeating;

		private string m_sNewListName;

		private string m_sNewListTitle;

		private bool m_bOriginalPreserveItemIdsValue;

		private bool m_bSuspendRenameEvents = true;

		private bool m_bSuppressPreserveIdsChangedEvent;

		private IContainer components;

		internal PanelControl w_plMaxVersions;

		internal LabelControl w_lblMaxVersions;

		internal CheckEdit w_rbMaxVerisons;

		internal CheckEdit w_rbCopyAllVersions;

		internal CheckEdit w_cbCopyItems;

		internal CheckEdit w_cbVersions;

		internal CheckEdit w_cbCopySubfolders;

		internal CheckEdit w_cbCopyLists;

		internal SimpleButton w_btnEditRename;

		internal CheckEdit w_cbRenameList;

		private PanelControl w_plListScoped;

		private PanelControl w_plSiteScoped;

		internal PanelControl w_plAboveItemLevel;

		internal CheckEdit w_cbPreserveIds;

		private CheckEdit w_cbPreserveDocumentIDs;

		private CheckEdit w_cbReattachPageLayouts;

		private CheckEdit w_cbApplyContentTypes;

		private SimpleButton w_bEditApplyContentTypes;

		private PanelControl w_plApplyNewContentTypes;

		internal CheckEdit w_cbDisableParsing;

		private CheckEdit w_cbApplyDocumentSets;

		private SimpleButton w_bEditAndApplyDocumentSets;

		private CheckEdit w_cbPreserveSharePointDocumentIDs;

		private CheckEdit _cbCopySPDCustomizedForms;

		private HelpTipButton w_helpApplyDocumentSets;

		private HelpTipButton w_helpReattachPageLayouts;

		private HelpTipButton w_helpPreserveIds;

		private HelpTipButton w_helpPreserveDocumentIDs;

		private HelpTipButton w_helpDisableParsing;

		private HelpTipButton w_helpPreserveSharePointDocumentIDs;

		private SpinEdit w_nudVersions;

		internal CheckEdit _cbUseAzureUpload;

		private HelpTipButton _helpAzureUpload;

		internal CheckEdit cbEncryptAzureMigrationJobs;

		public string NewListName
		{
			get
			{
				return m_sNewListName;
			}
			set
			{
				m_sNewListName = value;
				UpdateRenameUI();
			}
		}

		public string NewListTitle
		{
			get
			{
				return m_sNewListTitle;
			}
			set
			{
				m_sNewListTitle = value;
				UpdateRenameUI();
			}
		}

		public override string TabName
		{
			get
			{
				if (base.Scope == SharePointObjectScope.Item)
				{
					return "Items Options";
				}
				return base.TabName;
			}
		}

		public bool TargetHasDocumentSetFeature
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count == 0 || !(TargetNodes[0] is SPNode))
				{
					return false;
				}
				SPNode sPNode = TargetNodes[0] as SPNode;
				bool flag = false;
				while (!flag)
				{
					if (sPNode == null)
					{
						return false;
					}
					if (!(sPNode is SPWeb))
					{
						sPNode = sPNode.Parent as SPNode;
					}
					else
					{
						flag = true;
					}
				}
				return (sPNode as SPWeb).HasSharePoint2010DocumentSetFeature;
			}
		}

		public TCListContentOptions()
		{
			InitializeComponent();
			if (!SharePointConfigurationVariables.AllowDBWriting)
			{
				w_cbPreserveDocumentIDs.Enabled = false;
			}
			Type type = GetType();
			w_helpApplyDocumentSets.SetResourceString(type.FullName + w_cbApplyDocumentSets.Name, type);
			w_helpDisableParsing.SetResourceString(type.FullName + w_cbDisableParsing.Name, type);
			w_helpPreserveDocumentIDs.SetResourceString(type.FullName + w_cbPreserveDocumentIDs.Name, type);
			w_helpPreserveIds.SetResourceString(type.FullName + w_cbPreserveIds.Name, type);
			w_helpPreserveSharePointDocumentIDs.SetResourceString(type.FullName + w_cbPreserveSharePointDocumentIDs.Name, type);
			w_helpReattachPageLayouts.SetResourceString(type.FullName + w_cbReattachPageLayouts.Name, type);
			_helpAzureUpload.SetResourceString(type.FullName + _cbUseAzureUpload.Name, type);
		}

		private void _cbCopySPDCustomizedForms_CheckedChanged(object sender, EventArgs e)
		{
			SendMessage("CopySPDCustomizedFormPages", _cbCopySPDCustomizedForms.Checked && _cbCopySPDCustomizedForms.Enabled);
		}

		private void _cbUseAzureUpload_EditValueChanging(object sender, ChangingEventArgs e)
		{
			bool flag = (bool)e.NewValue;
			if (!base.CanFocus || !flag)
			{
				cbEncryptAzureMigrationJobs.Checked = false;
				cbEncryptAzureMigrationJobs.Enabled = false;
			}
			else if (MessageBox.Show(Metalogix.SharePoint.UI.WinForms.Properties.Resources.AzureUploadWarning, Metalogix.SharePoint.UI.WinForms.Properties.Resources.AzureUploadWarningCaption, MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
			else
			{
				cbEncryptAzureMigrationJobs.Enabled = true;
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

		private void EditRename()
		{
			string text = NewListName ?? base.SourceList.Name;
			string text2 = NewListTitle ?? base.SourceList.Title;
			string title = w_cbRenameList.Text;
			string[] labels = new string[2] { "Change List Name To", "Change List Title To" };
			string[] defaultTexts = new string[2] { text, text2 };
			Dictionary<string, object> dictionary = MLShortTextEntryDialog.ShowDialog(title, labels, defaultTexts);
			if (dictionary != null)
			{
				string text3 = dictionary["Change List Name To"].ToString();
				string text4 = dictionary["Change List Title To"].ToString();
				string sNewListName = ((!string.IsNullOrEmpty(text3)) ? text3 : null);
				m_sNewListName = sNewListName;
				string sNewListTitle = ((!string.IsNullOrEmpty(text4)) ? text4 : null);
				m_sNewListTitle = sNewListTitle;
				FireListRenameStateChanged();
			}
			UpdateRenameUI();
		}

		private void FireListRenameStateChanged()
		{
			RenameInfo renameInfo = default(RenameInfo);
			if (!w_cbRenameList.Checked)
			{
				renameInfo.Name = null;
				renameInfo.Title = null;
			}
			else
			{
				renameInfo.Name = NewListName;
				renameInfo.Title = NewListTitle;
			}
			SendMessage("TopLevelNodeRenamed", renameInfo);
		}

		private SPWeb GetLowestCommonWeb(NodeCollection nodes)
		{
			if (nodes.Count == 0)
			{
				return null;
			}
			List<SPWeb> mainWebChain = new List<SPWeb>();
			for (Node node = nodes[0] as Node; node != null; node = node.Parent)
			{
				if (node is SPWeb item)
				{
					mainWebChain.Add(item);
				}
			}
			if (mainWebChain.Count == 0)
			{
				return null;
			}
			for (int i = 1; i < nodes.Count; i++)
			{
				if (!TrimWebsBelowLowestIntersection(nodes[i] as Node, ref mainWebChain))
				{
					return null;
				}
			}
			if (mainWebChain.Count <= 0)
			{
				return null;
			}
			return mainWebChain[0];
		}

		private IEnumerable<SPContentType> GetTargetContentTypes()
		{
			if (TargetNodes == null)
			{
				return new SPContentType[0];
			}
			SPWeb lowestCommonWeb = GetLowestCommonWeb(TargetNodes);
			if (lowestCommonWeb == null)
			{
				return new SPContentType[0];
			}
			List<SPContentType> list = new List<SPContentType>(lowestCommonWeb.ContentTypes.Count);
			foreach (SPContentType contentType in lowestCommonWeb.ContentTypes)
			{
				list.Add(contentType);
			}
			return list;
		}

		private IEnumerable<SPContentType> GetTargetDocumentSetContentTypes()
		{
			if (TargetNodes == null)
			{
				return new SPContentType[0];
			}
			SPWeb lowestCommonWeb = GetLowestCommonWeb(TargetNodes);
			if (lowestCommonWeb == null)
			{
				return new SPContentType[0];
			}
			List<SPContentType> list = new List<SPContentType>(lowestCommonWeb.ContentTypes.Count);
			foreach (SPContentType contentType in lowestCommonWeb.ContentTypes)
			{
				if (contentType.ContentTypeID.StartsWith("0x0120D520"))
				{
					list.Add(contentType);
				}
			}
			return list;
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			if (sMessage == "MigrationModeChanged")
			{
				MigrationModeChangedInfo migrationModeChangedInfo = (MigrationModeChangedInfo)oValue;
				MigrationModeChanged(migrationModeChangedInfo.NewMigrationMode, migrationModeChangedInfo.OverwritingOrUpdatingItems, migrationModeChangedInfo.PropagatingItemDeletions);
			}
			else if (sMessage == "TopLevelNodeRenamed")
			{
				RenameInfo renameInfo = (RenameInfo)oValue;
				ListRenamingChanged(renameInfo.Name, renameInfo.Title);
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCListContentOptions));
			this.w_helpPreserveSharePointDocumentIDs = new TooltipsTest.HelpTipButton();
			this.w_cbPreserveSharePointDocumentIDs = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpDisableParsing = new TooltipsTest.HelpTipButton();
			this.w_cbDisableParsing = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpPreserveDocumentIDs = new TooltipsTest.HelpTipButton();
			this.w_cbPreserveDocumentIDs = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpPreserveIds = new TooltipsTest.HelpTipButton();
			this.w_cbPreserveIds = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpReattachPageLayouts = new TooltipsTest.HelpTipButton();
			this.w_cbReattachPageLayouts = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpApplyDocumentSets = new TooltipsTest.HelpTipButton();
			this.w_bEditAndApplyDocumentSets = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbApplyDocumentSets = new DevExpress.XtraEditors.CheckEdit();
			this.w_plApplyNewContentTypes = new DevExpress.XtraEditors.PanelControl();
			this.w_cbApplyContentTypes = new DevExpress.XtraEditors.CheckEdit();
			this.w_bEditApplyContentTypes = new DevExpress.XtraEditors.SimpleButton();
			this.w_plMaxVersions = new DevExpress.XtraEditors.PanelControl();
			this.w_nudVersions = new DevExpress.XtraEditors.SpinEdit();
			this.w_lblMaxVersions = new DevExpress.XtraEditors.LabelControl();
			this.w_rbMaxVerisons = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbCopyAllVersions = new DevExpress.XtraEditors.CheckEdit();
			this.w_plAboveItemLevel = new DevExpress.XtraEditors.PanelControl();
			this._cbCopySPDCustomizedForms = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopySubfolders = new DevExpress.XtraEditors.CheckEdit();
			this.w_plListScoped = new DevExpress.XtraEditors.PanelControl();
			this.w_cbRenameList = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnEditRename = new DevExpress.XtraEditors.SimpleButton();
			this.w_plSiteScoped = new DevExpress.XtraEditors.PanelControl();
			this.w_cbCopyLists = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbVersions = new DevExpress.XtraEditors.CheckEdit();
			this._cbUseAzureUpload = new DevExpress.XtraEditors.CheckEdit();
			this._helpAzureUpload = new TooltipsTest.HelpTipButton();
			this.cbEncryptAzureMigrationJobs = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveSharePointDocumentIDs).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveSharePointDocumentIDs.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpDisableParsing).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbDisableParsing.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveDocumentIDs).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveDocumentIDs.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveIds).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveIds.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpReattachPageLayouts).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbReattachPageLayouts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpApplyDocumentSets).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyDocumentSets.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plApplyNewContentTypes).BeginInit();
			this.w_plApplyNewContentTypes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyContentTypes.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMaxVersions).BeginInit();
			this.w_plMaxVersions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_nudVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbMaxVerisons.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyAllVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plAboveItemLevel).BeginInit();
			this.w_plAboveItemLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._cbCopySPDCustomizedForms.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubfolders.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plListScoped).BeginInit();
			this.w_plListScoped.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbRenameList.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plSiteScoped).BeginInit();
			this.w_plSiteScoped.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbUseAzureUpload.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._helpAzureUpload).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbEncryptAzureMigrationJobs.Properties).BeginInit();
			base.SuspendLayout();
			this.w_helpPreserveSharePointDocumentIDs.AnchoringControl = this.w_cbPreserveSharePointDocumentIDs;
			this.w_helpPreserveSharePointDocumentIDs.BackColor = System.Drawing.Color.Transparent;
			this.w_helpPreserveSharePointDocumentIDs.CommonParentControl = null;
			resources.ApplyResources(this.w_helpPreserveSharePointDocumentIDs, "w_helpPreserveSharePointDocumentIDs");
			this.w_helpPreserveSharePointDocumentIDs.Name = "w_helpPreserveSharePointDocumentIDs";
			this.w_helpPreserveSharePointDocumentIDs.TabStop = false;
			resources.ApplyResources(this.w_cbPreserveSharePointDocumentIDs, "w_cbPreserveSharePointDocumentIDs");
			this.w_cbPreserveSharePointDocumentIDs.Name = "w_cbPreserveSharePointDocumentIDs";
			this.w_cbPreserveSharePointDocumentIDs.Properties.AutoWidth = true;
			this.w_cbPreserveSharePointDocumentIDs.Properties.Caption = resources.GetString("w_cbPreserveSharePointDocumentIDs.Properties.Caption");
			this.w_helpDisableParsing.AnchoringControl = this.w_cbDisableParsing;
			this.w_helpDisableParsing.BackColor = System.Drawing.Color.Transparent;
			this.w_helpDisableParsing.CommonParentControl = null;
			resources.ApplyResources(this.w_helpDisableParsing, "w_helpDisableParsing");
			this.w_helpDisableParsing.Name = "w_helpDisableParsing";
			this.w_helpDisableParsing.TabStop = false;
			resources.ApplyResources(this.w_cbDisableParsing, "w_cbDisableParsing");
			this.w_cbDisableParsing.Name = "w_cbDisableParsing";
			this.w_cbDisableParsing.Properties.AutoWidth = true;
			this.w_cbDisableParsing.Properties.Caption = resources.GetString("w_cbDisableParsing.Properties.Caption");
			this.w_helpPreserveDocumentIDs.AnchoringControl = this.w_cbPreserveDocumentIDs;
			this.w_helpPreserveDocumentIDs.BackColor = System.Drawing.Color.Transparent;
			this.w_helpPreserveDocumentIDs.CommonParentControl = null;
			resources.ApplyResources(this.w_helpPreserveDocumentIDs, "w_helpPreserveDocumentIDs");
			this.w_helpPreserveDocumentIDs.Name = "w_helpPreserveDocumentIDs";
			this.w_helpPreserveDocumentIDs.TabStop = false;
			resources.ApplyResources(this.w_cbPreserveDocumentIDs, "w_cbPreserveDocumentIDs");
			this.w_cbPreserveDocumentIDs.Name = "w_cbPreserveDocumentIDs";
			this.w_cbPreserveDocumentIDs.Properties.AutoWidth = true;
			this.w_cbPreserveDocumentIDs.Properties.Caption = resources.GetString("w_cbPreserveDocumentIDs.Properties.Caption");
			this.w_cbPreserveDocumentIDs.CheckedChanged += new System.EventHandler(On_PreserveDocumentIDs_Checked);
			this.w_helpPreserveIds.AnchoringControl = this.w_cbPreserveIds;
			this.w_helpPreserveIds.BackColor = System.Drawing.Color.Transparent;
			this.w_helpPreserveIds.CommonParentControl = null;
			resources.ApplyResources(this.w_helpPreserveIds, "w_helpPreserveIds");
			this.w_helpPreserveIds.Name = "w_helpPreserveIds";
			this.w_helpPreserveIds.TabStop = false;
			resources.ApplyResources(this.w_cbPreserveIds, "w_cbPreserveIds");
			this.w_cbPreserveIds.Name = "w_cbPreserveIds";
			this.w_cbPreserveIds.Properties.AutoWidth = true;
			this.w_cbPreserveIds.Properties.Caption = resources.GetString("w_cbPreserveIds.Properties.Caption");
			this.w_cbPreserveIds.CheckedChanged += new System.EventHandler(On_cbPreserveIds_CheckedChanged);
			this.w_helpReattachPageLayouts.AnchoringControl = this.w_cbReattachPageLayouts;
			this.w_helpReattachPageLayouts.BackColor = System.Drawing.Color.Transparent;
			this.w_helpReattachPageLayouts.CommonParentControl = null;
			resources.ApplyResources(this.w_helpReattachPageLayouts, "w_helpReattachPageLayouts");
			this.w_helpReattachPageLayouts.Name = "w_helpReattachPageLayouts";
			this.w_helpReattachPageLayouts.TabStop = false;
			resources.ApplyResources(this.w_cbReattachPageLayouts, "w_cbReattachPageLayouts");
			this.w_cbReattachPageLayouts.Name = "w_cbReattachPageLayouts";
			this.w_cbReattachPageLayouts.Properties.AutoWidth = true;
			this.w_cbReattachPageLayouts.Properties.Caption = resources.GetString("w_cbReattachPageLayouts.Properties.Caption");
			this.w_helpApplyDocumentSets.AnchoringControl = this.w_bEditAndApplyDocumentSets;
			this.w_helpApplyDocumentSets.BackColor = System.Drawing.Color.Transparent;
			this.w_helpApplyDocumentSets.CommonParentControl = null;
			resources.ApplyResources(this.w_helpApplyDocumentSets, "w_helpApplyDocumentSets");
			this.w_helpApplyDocumentSets.Name = "w_helpApplyDocumentSets";
			this.w_helpApplyDocumentSets.TabStop = false;
			resources.ApplyResources(this.w_bEditAndApplyDocumentSets, "w_bEditAndApplyDocumentSets");
			this.w_bEditAndApplyDocumentSets.Name = "w_bEditAndApplyDocumentSets";
			this.w_bEditAndApplyDocumentSets.Click += new System.EventHandler(w_bEditApplyDocumentSets_Click);
			resources.ApplyResources(this.w_cbApplyDocumentSets, "w_cbApplyDocumentSets");
			this.w_cbApplyDocumentSets.Name = "w_cbApplyDocumentSets";
			this.w_cbApplyDocumentSets.Properties.AutoWidth = true;
			this.w_cbApplyDocumentSets.Properties.Caption = resources.GetString("w_cbApplyDocumentSets.Properties.Caption");
			this.w_cbApplyDocumentSets.CheckedChanged += new System.EventHandler(w_cbApplyDocumentSets_CheckedChanged);
			this.w_plApplyNewContentTypes.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plApplyNewContentTypes.Controls.Add(this.w_cbApplyContentTypes);
			this.w_plApplyNewContentTypes.Controls.Add(this.w_bEditApplyContentTypes);
			resources.ApplyResources(this.w_plApplyNewContentTypes, "w_plApplyNewContentTypes");
			this.w_plApplyNewContentTypes.Name = "w_plApplyNewContentTypes";
			resources.ApplyResources(this.w_cbApplyContentTypes, "w_cbApplyContentTypes");
			this.w_cbApplyContentTypes.Name = "w_cbApplyContentTypes";
			this.w_cbApplyContentTypes.Properties.AutoWidth = true;
			this.w_cbApplyContentTypes.Properties.Caption = resources.GetString("w_cbApplyContentTypes.Properties.Caption");
			this.w_cbApplyContentTypes.CheckedChanged += new System.EventHandler(w_cbApplyContentTypes_CheckedChanged);
			resources.ApplyResources(this.w_bEditApplyContentTypes, "w_bEditApplyContentTypes");
			this.w_bEditApplyContentTypes.Name = "w_bEditApplyContentTypes";
			this.w_bEditApplyContentTypes.Click += new System.EventHandler(w_bEditApplyContentTypes_Click);
			resources.ApplyResources(this.w_plMaxVersions, "w_plMaxVersions");
			this.w_plMaxVersions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plMaxVersions.Controls.Add(this.w_nudVersions);
			this.w_plMaxVersions.Controls.Add(this.w_lblMaxVersions);
			this.w_plMaxVersions.Controls.Add(this.w_rbMaxVerisons);
			this.w_plMaxVersions.Controls.Add(this.w_rbCopyAllVersions);
			this.w_plMaxVersions.Name = "w_plMaxVersions";
			resources.ApplyResources(this.w_nudVersions, "w_nudVersions");
			this.w_nudVersions.Name = "w_nudVersions";
			this.w_nudVersions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this.w_nudVersions.Properties.Mask.EditMask = resources.GetString("w_nudVersions.Properties.Mask.EditMask");
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_nudVersions.Properties;
			int[] bits = new int[4] { 2147483647, 0, 0, 0 };
			properties.MaxValue = new decimal(bits);
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.w_nudVersions.Properties;
			int[] bits2 = new int[4] { 1, 0, 0, 0 };
			properties2.MinValue = new decimal(bits2);
			resources.ApplyResources(this.w_lblMaxVersions, "w_lblMaxVersions");
			this.w_lblMaxVersions.Name = "w_lblMaxVersions";
			resources.ApplyResources(this.w_rbMaxVerisons, "w_rbMaxVerisons");
			this.w_rbMaxVerisons.Name = "w_rbMaxVerisons";
			this.w_rbMaxVerisons.Properties.AutoWidth = true;
			this.w_rbMaxVerisons.Properties.Caption = resources.GetString("w_rbMaxVerisons.Properties.Caption");
			this.w_rbMaxVerisons.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbMaxVerisons.Properties.RadioGroupIndex = 1;
			this.w_rbMaxVerisons.TabStop = false;
			this.w_rbMaxVerisons.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			resources.ApplyResources(this.w_rbCopyAllVersions, "w_rbCopyAllVersions");
			this.w_rbCopyAllVersions.Name = "w_rbCopyAllVersions";
			this.w_rbCopyAllVersions.Properties.AutoWidth = true;
			this.w_rbCopyAllVersions.Properties.Caption = resources.GetString("w_rbCopyAllVersions.Properties.Caption");
			this.w_rbCopyAllVersions.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyAllVersions.Properties.RadioGroupIndex = 1;
			this.w_rbCopyAllVersions.TabStop = false;
			this.w_plAboveItemLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plAboveItemLevel.Controls.Add(this._cbCopySPDCustomizedForms);
			this.w_plAboveItemLevel.Controls.Add(this.w_cbCopyItems);
			this.w_plAboveItemLevel.Controls.Add(this.w_cbCopySubfolders);
			this.w_plAboveItemLevel.Controls.Add(this.w_plListScoped);
			this.w_plAboveItemLevel.Controls.Add(this.w_plSiteScoped);
			resources.ApplyResources(this.w_plAboveItemLevel, "w_plAboveItemLevel");
			this.w_plAboveItemLevel.Name = "w_plAboveItemLevel";
			resources.ApplyResources(this._cbCopySPDCustomizedForms, "_cbCopySPDCustomizedForms");
			this._cbCopySPDCustomizedForms.Name = "_cbCopySPDCustomizedForms";
			this._cbCopySPDCustomizedForms.Properties.AutoWidth = true;
			this._cbCopySPDCustomizedForms.Properties.Caption = resources.GetString("_cbCopySPDCustomizedForms.Properties.Caption");
			this._cbCopySPDCustomizedForms.CheckedChanged += new System.EventHandler(_cbCopySPDCustomizedForms_CheckedChanged);
			resources.ApplyResources(this.w_cbCopyItems, "w_cbCopyItems");
			this.w_cbCopyItems.Name = "w_cbCopyItems";
			this.w_cbCopyItems.Properties.AutoWidth = true;
			this.w_cbCopyItems.Properties.Caption = resources.GetString("w_cbCopyItems.Properties.Caption");
			this.w_cbCopyItems.CheckedChanged += new System.EventHandler(On_ListItems_CheckedChanged);
			resources.ApplyResources(this.w_cbCopySubfolders, "w_cbCopySubfolders");
			this.w_cbCopySubfolders.Name = "w_cbCopySubfolders";
			this.w_cbCopySubfolders.Properties.AutoWidth = true;
			this.w_cbCopySubfolders.Properties.Caption = resources.GetString("w_cbCopySubfolders.Properties.Caption");
			this.w_cbCopySubfolders.CheckedChanged += new System.EventHandler(On_SubFolders_CheckedChanged);
			resources.ApplyResources(this.w_plListScoped, "w_plListScoped");
			this.w_plListScoped.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plListScoped.Controls.Add(this.w_cbRenameList);
			this.w_plListScoped.Controls.Add(this.w_btnEditRename);
			this.w_plListScoped.Name = "w_plListScoped";
			resources.ApplyResources(this.w_cbRenameList, "w_cbRenameList");
			this.w_cbRenameList.Name = "w_cbRenameList";
			this.w_cbRenameList.Properties.AutoWidth = true;
			this.w_cbRenameList.Properties.Caption = resources.GetString("w_cbRenameList.Properties.Caption");
			this.w_cbRenameList.CheckedChanged += new System.EventHandler(On_RenameList_CheckedChanged);
			resources.ApplyResources(this.w_btnEditRename, "w_btnEditRename");
			this.w_btnEditRename.Name = "w_btnEditRename";
			this.w_btnEditRename.Click += new System.EventHandler(On_EditRename_Clicked);
			resources.ApplyResources(this.w_plSiteScoped, "w_plSiteScoped");
			this.w_plSiteScoped.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSiteScoped.Controls.Add(this.w_cbCopyLists);
			this.w_plSiteScoped.Name = "w_plSiteScoped";
			resources.ApplyResources(this.w_cbCopyLists, "w_cbCopyLists");
			this.w_cbCopyLists.Name = "w_cbCopyLists";
			this.w_cbCopyLists.Properties.AutoWidth = true;
			this.w_cbCopyLists.Properties.Caption = resources.GetString("w_cbCopyLists.Properties.Caption");
			this.w_cbCopyLists.CheckedChanged += new System.EventHandler(On_CopyLists_CheckedChanged);
			resources.ApplyResources(this.w_cbVersions, "w_cbVersions");
			this.w_cbVersions.Name = "w_cbVersions";
			this.w_cbVersions.Properties.AutoWidth = true;
			this.w_cbVersions.Properties.Caption = resources.GetString("w_cbVersions.Properties.Caption");
			this.w_cbVersions.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			resources.ApplyResources(this._cbUseAzureUpload, "_cbUseAzureUpload");
			this._cbUseAzureUpload.Name = "_cbUseAzureUpload";
			this._cbUseAzureUpload.Properties.AutoWidth = true;
			this._cbUseAzureUpload.Properties.Caption = resources.GetString("_cbUseAzureUpload.Properties.Caption");
			this._cbUseAzureUpload.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(_cbUseAzureUpload_EditValueChanging);
			this._helpAzureUpload.AnchoringControl = this._cbUseAzureUpload;
			this._helpAzureUpload.BackColor = System.Drawing.Color.Transparent;
			this._helpAzureUpload.CommonParentControl = null;
			resources.ApplyResources(this._helpAzureUpload, "_helpAzureUpload");
			this._helpAzureUpload.Name = "_helpAzureUpload";
			this._helpAzureUpload.TabStop = false;
			resources.ApplyResources(this.cbEncryptAzureMigrationJobs, "cbEncryptAzureMigrationJobs");
			this.cbEncryptAzureMigrationJobs.Name = "cbEncryptAzureMigrationJobs";
			this.cbEncryptAzureMigrationJobs.Properties.AutoWidth = true;
			this.cbEncryptAzureMigrationJobs.Properties.Caption = resources.GetString("cbEncryptAzureMigrationJobs.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.cbEncryptAzureMigrationJobs);
			base.Controls.Add(this._helpAzureUpload);
			base.Controls.Add(this._cbUseAzureUpload);
			base.Controls.Add(this.w_helpPreserveSharePointDocumentIDs);
			base.Controls.Add(this.w_helpDisableParsing);
			base.Controls.Add(this.w_helpPreserveDocumentIDs);
			base.Controls.Add(this.w_helpPreserveIds);
			base.Controls.Add(this.w_helpReattachPageLayouts);
			base.Controls.Add(this.w_helpApplyDocumentSets);
			base.Controls.Add(this.w_cbPreserveSharePointDocumentIDs);
			base.Controls.Add(this.w_cbApplyDocumentSets);
			base.Controls.Add(this.w_bEditAndApplyDocumentSets);
			base.Controls.Add(this.w_cbDisableParsing);
			base.Controls.Add(this.w_plApplyNewContentTypes);
			base.Controls.Add(this.w_cbReattachPageLayouts);
			base.Controls.Add(this.w_cbPreserveDocumentIDs);
			base.Controls.Add(this.w_cbPreserveIds);
			base.Controls.Add(this.w_plMaxVersions);
			base.Controls.Add(this.w_plAboveItemLevel);
			base.Controls.Add(this.w_cbVersions);
			base.Name = "TCListContentOptions";
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveSharePointDocumentIDs).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveSharePointDocumentIDs.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpDisableParsing).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbDisableParsing.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveDocumentIDs).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveDocumentIDs.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpPreserveIds).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveIds.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpReattachPageLayouts).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbReattachPageLayouts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpApplyDocumentSets).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyDocumentSets.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plApplyNewContentTypes).EndInit();
			this.w_plApplyNewContentTypes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyContentTypes.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMaxVersions).EndInit();
			this.w_plMaxVersions.ResumeLayout(false);
			this.w_plMaxVersions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_nudVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbMaxVerisons.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyAllVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plAboveItemLevel).EndInit();
			this.w_plAboveItemLevel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._cbCopySPDCustomizedForms.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubfolders.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plListScoped).EndInit();
			this.w_plListScoped.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbRenameList.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plSiteScoped).EndInit();
			this.w_plSiteScoped.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbUseAzureUpload.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._helpAzureUpload).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbEncryptAzureMigrationJobs.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void ListRenamingChanged(string sNewName, string sNewTitle)
		{
			m_sNewListName = sNewName;
			m_sNewListTitle = sNewTitle;
			UpdateRenameUI();
		}

		protected override void LoadUI()
		{
			w_cbCopyLists.Checked = base.Options.CopyLists;
			w_cbCopyItems.Checked = base.Options.CopyListItems;
			w_cbCopySubfolders.Checked = base.Options.CopySubFolders;
			_cbCopySPDCustomizedForms.Checked = base.Options.CopyCustomizedFormPages;
			w_rbMaxVerisons.Checked = base.Options.CopyMaxVersions;
			w_rbCopyAllVersions.Checked = !base.Options.CopyMaxVersions;
			w_cbVersions.Checked = base.Options.CopyVersions;
			w_nudVersions.Value = base.Options.MaximumVersionCount;
			m_sNewListName = base.Options.NewListName;
			m_sNewListTitle = base.Options.NewListTitle;
			w_cbRenameList.Checked = base.Options.RenameList;
			w_cbApplyContentTypes.Checked = base.Options.ApplyNewContentTypes;
			w_cbApplyDocumentSets.Checked = base.Options.ApplyNewDocumentSets;
			w_cbPreserveDocumentIDs.Checked = base.Options.PreserveDocumentIDs && SharePointConfigurationVariables.AllowDBWriting;
			w_cbPreserveSharePointDocumentIDs.Checked = base.Options.PreserveSharePointDocumentIDs;
			m_bOriginalPreserveItemIdsValue = base.Options.PreserveItemIDs;
			w_cbPreserveIds.Checked = base.Options.PreserveItemIDs;
			CommonSerializableList<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptions = ((base.Options.ContentTypeApplicationObjects != null) ? ((CommonSerializableList<ContentTypeApplicationOptionsCollection>)base.Options.ContentTypeApplicationObjects.Clone()) : null);
			m_contentTypeApplicationOptions = contentTypeApplicationOptions;
			CommonSerializableList<DocumentSetApplicationOptionsCollection> documentSetApplicationOptions = ((base.Options.DocumentSetApplicationObjects != null) ? ((CommonSerializableList<DocumentSetApplicationOptionsCollection>)base.Options.DocumentSetApplicationObjects.Clone()) : null);
			m_documentSetApplicationOptions = documentSetApplicationOptions;
			CommonSerializableList<DocumentSetFolderOptions> folderToDocumentSetApplicationOptions = ((base.Options.FolderToDocumentSetApplicationObjects != null) ? ((CommonSerializableList<DocumentSetFolderOptions>)base.Options.FolderToDocumentSetApplicationObjects.Clone()) : null);
			m_folderToDocumentSetApplicationOptions = folderToDocumentSetApplicationOptions;
			bool flag = false;
			if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
			{
				flag = ((SPNode)SourceNodes[0]).Adapter.SharePointVersion.MajorVersion == ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.MajorVersion;
			}
			w_cbReattachPageLayouts.Checked = base.Options.ReattachPageLayouts || !flag;
			w_cbDisableParsing.Checked = base.Options.DisableDocumentParsing;
			_cbUseAzureUpload.Enabled = SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes);
			_cbUseAzureUpload.Checked = _cbUseAzureUpload.Enabled && base.Options.UseAzureOffice365Upload;
			if (_cbUseAzureUpload.Enabled && _cbUseAzureUpload.Checked)
			{
				cbEncryptAzureMigrationJobs.Enabled = true;
				cbEncryptAzureMigrationJobs.Checked = base.Options.EncryptAzureMigrationJobs;
			}
			UpdateEnabledState();
			UpdateRenameUI();
			UpdateAllAvailabilities();
		}

		private void MigrationModeChanged(MigrationMode newMigrationMode, bool bOverwritingOrUpdatingItems, bool bPropagatingItemDeletions)
		{
			if (base.IsTargetOMAdapter && !base.IsTargetClientOM)
			{
				m_bSuppressPreserveIdsChangedEvent = true;
				if (newMigrationMode == MigrationMode.Incremental || (newMigrationMode == MigrationMode.Custom && (bOverwritingOrUpdatingItems || bPropagatingItemDeletions)))
				{
					w_cbPreserveIds.Enabled = true;
					w_cbPreserveIds.Checked = true;
				}
				else
				{
					w_cbPreserveIds.Enabled = true;
					w_cbPreserveIds.Checked = m_bOriginalPreserveItemIdsValue;
				}
				m_bSuppressPreserveIdsChangedEvent = false;
			}
		}

		protected override void MultiSelectUISetup()
		{
			if (base.MultiSelectUI)
			{
				HideControl(w_plListScoped);
			}
		}

		private void On_cbPreserveIds_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_bSuppressPreserveIdsChangedEvent)
			{
				m_bOriginalPreserveItemIdsValue = w_cbPreserveIds.Checked;
			}
		}

		private void On_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void On_CopyLists_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.List, w_cbCopyLists.Checked));
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Folder, w_cbCopyLists.Checked && w_cbCopySubfolders.Checked));
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Item, w_cbCopyLists.Checked && w_cbCopyItems.Checked));
		}

		private void On_EditRename_Clicked(object sender, EventArgs e)
		{
			EditRename();
		}

		private void On_ListItems_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Item, w_cbCopyItems.Checked));
		}

		private void On_PreserveDocumentIDs_Checked(object sender, EventArgs e)
		{
			if (w_cbPreserveDocumentIDs.Checked && base.ContainsFocus && FlatXtraMessageBox.Show(string.Format(Metalogix.SharePoint.Properties.Resources.Feature_Requires_DBWriting, Metalogix.SharePoint.Properties.Resources.Preserve_DocumentIDs), Metalogix.SharePoint.Properties.Resources.Information, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
			{
				w_cbPreserveDocumentIDs.Checked = false;
			}
		}

		private void On_RenameList_CheckedChanged(object sender, EventArgs e)
		{
			w_btnEditRename.Enabled = w_cbRenameList.Checked;
			if (!m_bSuspendRenameEvents)
			{
				if (w_cbRenameList.Checked)
				{
					EditRename();
				}
				else
				{
					FireListRenameStateChanged();
				}
			}
		}

		private void On_SubFolders_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Folder, w_cbCopySubfolders.Checked));
		}

		public override bool SaveUI()
		{
			base.Options.CopyLists = w_cbCopyLists.Checked;
			base.Options.CopyListItems = w_cbCopyItems.Checked;
			base.Options.CopySubFolders = w_cbCopySubfolders.Checked;
			base.Options.CopyCustomizedFormPages = _cbCopySPDCustomizedForms.Checked;
			base.Options.CopyMaxVersions = w_rbMaxVerisons.Checked;
			base.Options.CopyVersions = w_cbVersions.Checked;
			base.Options.MaximumVersionCount = (int)w_nudVersions.Value;
			base.Options.NewListName = m_sNewListName;
			base.Options.NewListTitle = m_sNewListTitle;
			base.Options.RenameList = w_cbRenameList.Checked;
			base.Options.ReattachPageLayouts = w_cbReattachPageLayouts.Checked;
			base.Options.ApplyNewContentTypes = w_cbApplyContentTypes.Checked;
			base.Options.ApplyNewDocumentSets = w_cbApplyDocumentSets.Checked;
			base.Options.PreserveItemIDs = (base.IsTargetOMAdapter || base.IsTargetClientOM) && w_cbPreserveIds.Checked;
			base.Options.PreserveDocumentIDs = w_cbPreserveDocumentIDs.Checked && w_cbPreserveDocumentIDs.Enabled;
			base.Options.ContentTypeApplicationObjects = m_contentTypeApplicationOptions;
			base.Options.DocumentSetApplicationObjects = m_documentSetApplicationOptions;
			base.Options.FolderToDocumentSetApplicationObjects = m_folderToDocumentSetApplicationOptions;
			base.Options.PreserveSharePointDocumentIDs = w_cbPreserveSharePointDocumentIDs.Checked;
			base.Options.DisableDocumentParsing = w_cbDisableParsing.Checked;
			base.Options.UseAzureOffice365Upload = _cbUseAzureUpload.Checked;
			base.Options.EncryptAzureMigrationJobs = cbEncryptAzureMigrationJobs.Checked;
			bool flag = false;
			if (!base.IsModeSwitched && base.Options.UseAzureOffice365Upload && SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
			{
				flag = SPUIUtils.IsAzureConnectionStringEmpty((SPNode)SourceNodes[0], (SPNode)TargetNodes[0], base.Options.EncryptAzureMigrationJobs, ref isAzureUserMappingWarningMessageRepeating);
			}
			return !flag;
		}

		private bool TrimWebsBelowLowestIntersection(Node node, ref List<SPWeb> mainWebChain)
		{
			if (node == null)
			{
				return false;
			}
			if (node is SPWeb item)
			{
				int num = mainWebChain.IndexOf(item);
				if (num >= 0)
				{
					mainWebChain.RemoveRange(0, num);
					return true;
				}
			}
			return TrimWebsBelowLowestIntersection(node.Parent, ref mainWebChain);
		}

		private void UpdateAllAvailabilities()
		{
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.List, w_cbCopyLists.Checked));
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Folder, w_cbCopyLists.Checked && w_cbCopySubfolders.Checked));
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Item, w_cbCopyLists.Checked && w_cbCopyItems.Checked));
		}

		protected override void UpdateEnabledState()
		{
			w_cbCopySubfolders.Enabled = w_cbCopyLists.Checked;
			w_cbRenameList.Enabled = w_cbCopyLists.Checked;
			w_btnEditRename.Enabled = w_cbRenameList.Enabled && w_cbRenameList.Checked;
			w_btnEditRename.Enabled = w_cbRenameList.Checked && w_cbRenameList.Enabled;
			_cbCopySPDCustomizedForms.Enabled = w_cbCopyLists.Checked;
			w_cbCopyItems.Enabled = w_cbCopyLists.Checked;
			bool flag = w_cbCopyItems.Checked && w_cbCopyItems.Enabled;
			w_cbVersions.Enabled = flag;
			w_plMaxVersions.Enabled = w_cbVersions.Checked && w_cbVersions.Enabled;
			w_nudVersions.Enabled = w_rbMaxVerisons.Checked;
			w_cbApplyContentTypes.Enabled = w_cbCopyLists.Checked;
			w_bEditApplyContentTypes.Enabled = w_cbApplyContentTypes.Checked && w_cbApplyContentTypes.Enabled;
			w_cbApplyDocumentSets.Enabled = false;
			w_bEditAndApplyDocumentSets.Enabled = false;
			if (base.IsTarget2010)
			{
				if (!(TargetNodes[0] is SPFolder sPFolder) || !sPFolder.IsDocumentSet)
				{
					w_cbApplyDocumentSets.Enabled = w_cbCopyLists.Checked;
					w_bEditAndApplyDocumentSets.Enabled = w_cbApplyDocumentSets.Checked && w_cbApplyDocumentSets.Enabled;
				}
				else
				{
					w_cbApplyDocumentSets.Enabled = false;
					w_bEditAndApplyDocumentSets.Enabled = false;
				}
			}
			CheckEdit checkEdit = w_cbPreserveIds;
			bool enabled = (flag || (w_cbCopySubfolders.Enabled && w_cbCopySubfolders.Checked)) && (base.IsTargetOMAdapter || base.IsTargetClientOM);
			checkEdit.Enabled = enabled;
			w_cbPreserveIds.Checked = w_cbPreserveIds.Checked && w_cbPreserveIds.Enabled;
			CheckEdit checkEdit2 = w_cbPreserveDocumentIDs;
			bool enabled2 = (flag || (w_cbCopySubfolders.Enabled && w_cbCopySubfolders.Checked)) && SharePointConfigurationVariables.AllowDBWriting && base.IsTargetOMAdapter && !base.IsTargetClientOM;
			checkEdit2.Enabled = enabled2;
			w_cbPreserveDocumentIDs.Checked = w_cbPreserveDocumentIDs.Checked && w_cbPreserveDocumentIDs.Enabled;
			w_cbPreserveSharePointDocumentIDs.Enabled = flag && base.IsTarget2010 && base.IsSource2010;
			w_cbPreserveSharePointDocumentIDs.Checked = w_cbPreserveSharePointDocumentIDs.Checked && w_cbPreserveSharePointDocumentIDs.Enabled;
			bool flag2 = false;
			bool flag3 = false;
			if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
			{
				flag2 = ((SPNode)SourceNodes[0]).Adapter.SharePointVersion.MajorVersion == ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.MajorVersion;
				if (((SPNode)SourceNodes[0]).Adapter.ExternalizationSupport == ExternalizationSupport.Supported)
				{
					ExternalizationSupport externalizationSupport = ((SPNode)TargetNodes[0]).Adapter.ExternalizationSupport;
				}
				ExternalizationSupport externalizationSupport2 = ((SPNode)TargetNodes[0]).Adapter.ExternalizationSupport;
				ExternalizationSupport externalizationSupport3 = ((SPNode)SourceNodes[0]).Adapter.ExternalizationSupport;
				flag3 = ((SPNode)TargetNodes[0]).Adapter.IsNws;
			}
			w_cbReattachPageLayouts.Enabled = w_cbCopyLists.Checked && flag2;
			if (!flag2)
			{
				w_cbReattachPageLayouts.Checked = true;
			}
			w_cbDisableParsing.Enabled = w_cbCopyLists.Checked && !flag3;
			w_cbDisableParsing.Checked = w_cbDisableParsing.Checked && w_cbDisableParsing.Enabled;
			_cbUseAzureUpload.Enabled = SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes) && w_cbCopyItems.Enabled && w_cbCopyItems.Checked;
			_cbUseAzureUpload.Checked = _cbUseAzureUpload.Checked && _cbUseAzureUpload.Enabled;
			if (_cbUseAzureUpload.Enabled && _cbUseAzureUpload.Checked)
			{
				cbEncryptAzureMigrationJobs.Enabled = true;
				cbEncryptAzureMigrationJobs.Checked = cbEncryptAzureMigrationJobs.Checked;
			}
		}

		private void UpdateRenameUI()
		{
			m_bSuspendRenameEvents = true;
			w_cbRenameList.Checked = NewListName != null || NewListTitle != null;
			m_bSuspendRenameEvents = false;
		}

		protected override void UpdateScope()
		{
			if (base.Scope == SharePointObjectScope.SiteCollection)
			{
				HideControl(w_plApplyNewContentTypes);
			}
			if (base.Scope == SharePointObjectScope.Site || base.Scope == SharePointObjectScope.SiteCollection)
			{
				HideControl(w_plListScoped);
			}
			if (base.Scope == SharePointObjectScope.List)
			{
				HideControl(w_plSiteScoped);
				CheckEdit checkEdit = w_cbRenameList;
				Point location = w_cbRenameList.Location;
				Point location2 = w_cbRenameList.Location;
				checkEdit.Location = new Point(location.X - 18, location2.Y);
				SimpleButton simpleButton = w_btnEditRename;
				Point location3 = w_btnEditRename.Location;
				Point location4 = w_btnEditRename.Location;
				simpleButton.Location = new Point(location3.X - 18, location4.Y);
				CheckEdit checkEdit2 = w_cbCopySubfolders;
				Point location5 = w_cbCopySubfolders.Location;
				Point location6 = w_cbCopySubfolders.Location;
				checkEdit2.Location = new Point(location5.X - 18, location6.Y);
				CheckEdit cbCopySPDCustomizedForms = _cbCopySPDCustomizedForms;
				Point location7 = _cbCopySPDCustomizedForms.Location;
				Point location8 = _cbCopySPDCustomizedForms.Location;
				cbCopySPDCustomizedForms.Location = new Point(location7.X - 18, location8.Y);
				CheckEdit checkEdit3 = w_cbCopyItems;
				Point location9 = w_cbCopyItems.Location;
				Point location10 = w_cbCopyItems.Location;
				checkEdit3.Location = new Point(location9.X - 18, location10.Y);
				CheckEdit checkEdit4 = w_cbVersions;
				Point location11 = w_cbVersions.Location;
				Point location12 = w_cbVersions.Location;
				checkEdit4.Location = new Point(location11.X - 18, location12.Y);
				PanelControl panelControl = w_plMaxVersions;
				Point location13 = w_plMaxVersions.Location;
				Point location14 = w_plMaxVersions.Location;
				panelControl.Location = new Point(location13.X - 18, location14.Y);
				PanelControl panelControl2 = w_plApplyNewContentTypes;
				Point location15 = w_plApplyNewContentTypes.Location;
				Point location16 = w_plApplyNewContentTypes.Location;
				panelControl2.Location = new Point(location15.X - 18, location16.Y);
				CheckEdit checkEdit5 = w_cbApplyDocumentSets;
				Point location17 = w_cbApplyDocumentSets.Location;
				Point location18 = w_cbApplyDocumentSets.Location;
				checkEdit5.Location = new Point(location17.X - 18, location18.Y);
				SimpleButton simpleButton2 = w_bEditAndApplyDocumentSets;
				Point location19 = w_bEditAndApplyDocumentSets.Location;
				Point location20 = w_bEditAndApplyDocumentSets.Location;
				simpleButton2.Location = new Point(location19.X - 18, location20.Y);
				CheckEdit checkEdit6 = w_cbPreserveIds;
				Point location21 = w_cbPreserveIds.Location;
				Point location22 = w_cbPreserveIds.Location;
				checkEdit6.Location = new Point(location21.X - 18, location22.Y);
				CheckEdit checkEdit7 = w_cbPreserveDocumentIDs;
				Point location23 = w_cbPreserveDocumentIDs.Location;
				Point location24 = w_cbPreserveDocumentIDs.Location;
				checkEdit7.Location = new Point(location23.X - 18, location24.Y);
				CheckEdit checkEdit8 = w_cbReattachPageLayouts;
				Point location25 = w_cbReattachPageLayouts.Location;
				Point location26 = w_cbReattachPageLayouts.Location;
				checkEdit8.Location = new Point(location25.X - 18, location26.Y);
				CheckEdit checkEdit9 = w_cbDisableParsing;
				Point location27 = w_cbDisableParsing.Location;
				Point location28 = w_cbDisableParsing.Location;
				checkEdit9.Location = new Point(location27.X - 18, location28.Y);
				CheckEdit checkEdit10 = w_cbPreserveSharePointDocumentIDs;
				Point location29 = w_cbPreserveSharePointDocumentIDs.Location;
				Point location30 = w_cbPreserveSharePointDocumentIDs.Location;
				checkEdit10.Location = new Point(location29.X - 18, location30.Y);
				CheckEdit cbUseAzureUpload = _cbUseAzureUpload;
				Point location31 = _cbUseAzureUpload.Location;
				Point location32 = _cbUseAzureUpload.Location;
				cbUseAzureUpload.Location = new Point(location31.X - 18, location32.Y);
				CheckEdit checkEdit11 = cbEncryptAzureMigrationJobs;
				Point location33 = cbEncryptAzureMigrationJobs.Location;
				Point location34 = cbEncryptAzureMigrationJobs.Location;
				checkEdit11.Location = new Point(location33.X - 18, location34.Y);
			}
			if (base.Scope == SharePointObjectScope.Item)
			{
				HideControl(w_plAboveItemLevel);
				CheckEdit checkEdit12 = w_cbVersions;
				Point location35 = w_cbVersions.Location;
				Point location36 = w_cbVersions.Location;
				checkEdit12.Location = new Point(location35.X - 36, location36.Y);
				PanelControl panelControl3 = w_plMaxVersions;
				Point location37 = w_plMaxVersions.Location;
				Point location38 = w_plMaxVersions.Location;
				panelControl3.Location = new Point(location37.X - 36, location38.Y);
				PanelControl panelControl4 = w_plApplyNewContentTypes;
				Point location39 = w_plApplyNewContentTypes.Location;
				Point location40 = w_plApplyNewContentTypes.Location;
				panelControl4.Location = new Point(location39.X - 18, location40.Y);
				CheckEdit checkEdit13 = w_cbApplyDocumentSets;
				Point location41 = w_cbApplyDocumentSets.Location;
				Point location42 = w_cbApplyDocumentSets.Location;
				checkEdit13.Location = new Point(location41.X - 18, location42.Y);
				SimpleButton simpleButton3 = w_bEditAndApplyDocumentSets;
				Point location43 = w_bEditAndApplyDocumentSets.Location;
				Point location44 = w_bEditAndApplyDocumentSets.Location;
				simpleButton3.Location = new Point(location43.X - 18, location44.Y);
				CheckEdit checkEdit14 = w_cbPreserveIds;
				Point location45 = w_cbPreserveIds.Location;
				Point location46 = w_cbPreserveIds.Location;
				checkEdit14.Location = new Point(location45.X - 18, location46.Y);
				CheckEdit checkEdit15 = w_cbPreserveDocumentIDs;
				Point location47 = w_cbPreserveDocumentIDs.Location;
				Point location48 = w_cbPreserveDocumentIDs.Location;
				checkEdit15.Location = new Point(location47.X - 18, location48.Y);
				CheckEdit checkEdit16 = w_cbReattachPageLayouts;
				Point location49 = w_cbReattachPageLayouts.Location;
				Point location50 = w_cbReattachPageLayouts.Location;
				checkEdit16.Location = new Point(location49.X - 18, location50.Y);
				CheckEdit checkEdit17 = w_cbDisableParsing;
				Point location51 = w_cbDisableParsing.Location;
				Point location52 = w_cbDisableParsing.Location;
				checkEdit17.Location = new Point(location51.X - 18, location52.Y);
				CheckEdit checkEdit18 = w_cbPreserveSharePointDocumentIDs;
				Point location53 = w_cbPreserveSharePointDocumentIDs.Location;
				Point location54 = w_cbPreserveSharePointDocumentIDs.Location;
				checkEdit18.Location = new Point(location53.X - 18, location54.Y);
				CheckEdit cbUseAzureUpload2 = _cbUseAzureUpload;
				Point location55 = _cbUseAzureUpload.Location;
				Point location56 = _cbUseAzureUpload.Location;
				cbUseAzureUpload2.Location = new Point(location55.X - 18, location56.Y);
				CheckEdit checkEdit19 = cbEncryptAzureMigrationJobs;
				Point location57 = cbEncryptAzureMigrationJobs.Location;
				Point location58 = cbEncryptAzureMigrationJobs.Location;
				checkEdit19.Location = new Point(location57.X - 18, location58.Y);
			}
		}

		private void w_bEditApplyContentTypes_Click(object sender, EventArgs e)
		{
			CommonSerializableList<ContentTypeApplicationOptionsCollection> commonSerializableList = m_contentTypeApplicationOptions;
			if (!PasteActionUtils.CollectionContainsMultipleLists(SourceNodes))
			{
				if (commonSerializableList == null)
				{
					commonSerializableList = new CommonSerializableList<ContentTypeApplicationOptionsCollection>();
				}
				ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = ((commonSerializableList.Count == 0) ? new ContentTypeApplicationOptionsCollection() : ((ContentTypeApplicationOptionsCollection)commonSerializableList[0]));
				SPList sPList = base.SourceList;
				if (sPList == null)
				{
					if (SourceNodes == null || SourceNodes.Count == 0 || !(SourceNodes[0] is SPListItem))
					{
						return;
					}
					sPList = ((SPListItem)SourceNodes[0]).ParentList;
				}
				ContentTypeApplicationOptionsConfigDialog contentTypeApplicationOptionsConfigDialog = new ContentTypeApplicationOptionsConfigDialog(sPList, GetTargetContentTypes())
				{
					Options = contentTypeApplicationOptionsCollection
				};
				if (contentTypeApplicationOptionsConfigDialog.ShowDialog() == DialogResult.OK)
				{
					if (contentTypeApplicationOptionsCollection.Data.Count > 0)
					{
						commonSerializableList.Clear();
						commonSerializableList.Add(contentTypeApplicationOptionsCollection);
						m_contentTypeApplicationOptions = commonSerializableList;
					}
					else
					{
						m_contentTypeApplicationOptions = null;
					}
				}
			}
			else
			{
				if (commonSerializableList == null)
				{
					commonSerializableList = new CommonSerializableList<ContentTypeApplicationOptionsCollection>();
				}
				ContentTypeApplicationOptionsSiteLevelConfigDialog contentTypeApplicationOptionsSiteLevelConfigDialog = new ContentTypeApplicationOptionsSiteLevelConfigDialog(SourceNodes)
				{
					TargetContentTypes = GetTargetContentTypes(),
					Options = commonSerializableList
				};
				if (contentTypeApplicationOptionsSiteLevelConfigDialog.ShowDialog() == DialogResult.OK)
				{
					CommonSerializableList<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptions = ((commonSerializableList.Count != 0) ? commonSerializableList : null);
					m_contentTypeApplicationOptions = contentTypeApplicationOptions;
				}
			}
		}

		private void w_bEditApplyDocumentSets_Click(object sender, EventArgs e)
		{
			if (!TargetHasDocumentSetFeature)
			{
				FlatXtraMessageBox.Show("Cannot apply Document Sets because Document Sets are not enabled on target Site Collection. Please enable Document Sets on the target and try again.", "Cannot Apply Document Sets", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			CommonSerializableList<DocumentSetApplicationOptionsCollection> commonSerializableList = m_documentSetApplicationOptions;
			CommonSerializableList<DocumentSetFolderOptions> commonSerializableList2 = m_folderToDocumentSetApplicationOptions;
			if (commonSerializableList == null)
			{
				commonSerializableList = new CommonSerializableList<DocumentSetApplicationOptionsCollection>();
			}
			if (commonSerializableList2 == null)
			{
				commonSerializableList2 = new CommonSerializableList<DocumentSetFolderOptions>();
			}
			DocumentSetApplicationOptionsSiteLevelConfigDialog documentSetApplicationOptionsSiteLevelConfigDialog = new DocumentSetApplicationOptionsSiteLevelConfigDialog(SourceNodes)
			{
				TargetContentTypes = GetTargetDocumentSetContentTypes(),
				Options = commonSerializableList,
				FolderOptions = commonSerializableList2
			};
			if (documentSetApplicationOptionsSiteLevelConfigDialog.ShowDialog() == DialogResult.OK)
			{
				CommonSerializableList<DocumentSetApplicationOptionsCollection> documentSetApplicationOptions = ((commonSerializableList.Count != 0) ? commonSerializableList : null);
				m_documentSetApplicationOptions = documentSetApplicationOptions;
				CommonSerializableList<DocumentSetFolderOptions> folderToDocumentSetApplicationOptions = ((commonSerializableList2.Count != 0) ? commonSerializableList2 : null);
				m_folderToDocumentSetApplicationOptions = folderToDocumentSetApplicationOptions;
			}
		}

		private void w_cbApplyContentTypes_CheckedChanged(object sender, EventArgs e)
		{
			w_bEditApplyContentTypes.Enabled = w_cbApplyContentTypes.Checked && w_cbApplyContentTypes.Enabled;
		}

		private void w_cbApplyDocumentSets_CheckedChanged(object sender, EventArgs e)
		{
			w_bEditAndApplyDocumentSets.Enabled = w_cbApplyDocumentSets.Checked && w_cbApplyDocumentSets.Enabled;
		}
	}
}
