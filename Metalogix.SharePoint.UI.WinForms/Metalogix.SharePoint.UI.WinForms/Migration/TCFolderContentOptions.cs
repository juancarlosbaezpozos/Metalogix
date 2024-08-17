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
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.FolderContentOptions32.png")]
	[ControlName("Folder Content Options")]
	public class TCFolderContentOptions : ScopableTabbableControl
	{
		private const string FOLDERRENAMELABEL = "Change Folder Name and Title To";

		public const string DOCUMENT_SET_ID = "0x0120D520";

		private CommonSerializableList<ContentTypeApplicationOptionsCollection> m_contentTypeApplicationOptions;

		private CommonSerializableList<DocumentSetApplicationOptionsCollection> m_documentSetApplicationOptions;

		private CommonSerializableList<DocumentSetFolderOptions> m_folderToDocumentSetApplicationOptions;

		public bool isAzureUserMappingWarningMessageRepeating;

		private SPFolderContentOptions m_options;

		private string m_sNewFolderName;

		private bool m_bOriginalPreserveItemIdsValue;

		private bool m_bSuppressPreserveIdsChangedEvent;

		private bool m_bSuspendRenameEvents = true;

		private IContainer components;

		internal PanelControl w_plMaxVersions;

		internal LabelControl w_lblMaxVersions;

		internal CheckEdit w_rbMaxVerisons;

		internal CheckEdit w_rbCopyAllVersions;

		internal CheckEdit w_cbVersions;

		internal CheckEdit w_cbPreserveIds;

		internal SimpleButton w_btnEditRename;

		internal CheckEdit w_cbRenameFolder;

		internal CheckEdit w_cbCopyItems;

		internal CheckEdit w_cbCopySubfolders;

		private CheckEdit w_cbPreserveDocumentIDs;

		private CheckEdit w_cbReattachPageLayouts;

		private PanelControl w_pnlRename;

		private PanelControl w_plApplyNewContentTypes;

		private CheckEdit w_cbApplyContentTypes;

		private SimpleButton w_bEditApplyContentTypes;

		internal CheckEdit w_cbDisableParsing;

		private CheckEdit w_cbApplyDocumentSets;

		private SimpleButton w_bEditAndApplyDocumentSets;

		private SpinEdit w_nudVersions;

		internal CheckEdit _cbUseAzureUpload;

		private HelpTipButton _helpAzureUpload;

		internal CheckEdit cbEncryptAzureMigrationJobs;

		public string NewFolderName
		{
			get
			{
				return m_sNewFolderName;
			}
			set
			{
				m_sNewFolderName = value;
				UpdateRenameUI();
			}
		}

		public SPFolderContentOptions Options
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

		private SPFolder SourceFolder
		{
			get
			{
				if (SourceNodes == null || SourceNodes.Count == 0)
				{
					return null;
				}
				return SourceNodes[0] as SPFolder;
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

		public bool TargetIs2010
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count <= 0)
				{
					return false;
				}
				return ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.IsSharePoint2010OrLater;
			}
		}

		public bool TargetIsClientOM
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count <= 0 || !((SPNode)TargetNodes[0]).Adapter.SharePointVersion.IsSharePoint2010OrLater || !((SPNode)TargetNodes[0]).Adapter.IsNws)
				{
					return false;
				}
				return ((SPNode)TargetNodes[0]).Adapter.IsClientOM;
			}
		}

		public bool TargetIsOMAdapter
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count == 0 || !(TargetNodes[0] is SPNode))
				{
					return true;
				}
				return !((SPNode)TargetNodes[0]).Adapter.IsNws;
			}
		}

		public TCFolderContentOptions()
		{
			InitializeComponent();
			Type type = GetType();
			_helpAzureUpload.SetResourceString(type.FullName + _cbUseAzureUpload.Name, type);
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
			string defaultText = ((NewFolderName != null) ? NewFolderName : SourceFolder.Name);
			object obj = MLShortTextEntryDialog.ShowDialog(w_cbRenameFolder.Text, "Change Folder Name and Title To", defaultText);
			if (obj != null)
			{
				string text = obj.ToString();
				string sNewFolderName = ((!string.IsNullOrEmpty(text)) ? text : null);
				m_sNewFolderName = sNewFolderName;
				FireFolderRenameStateChanged();
			}
			UpdateRenameUI();
		}

		private void FireFolderRenameStateChanged()
		{
			RenameInfo renameInfo = default(RenameInfo);
			renameInfo.Name = null;
			RenameInfo renameInfo2 = renameInfo;
			if (!w_cbRenameFolder.Checked)
			{
				renameInfo2.Title = null;
			}
			else
			{
				renameInfo2.Title = NewFolderName;
			}
			SendMessage("TopLevelNodeRenamed", renameInfo2);
		}

		private void FolderRenamingChanged(string sNewName, string sNewTitle)
		{
			if (sNewName != null || sNewTitle == null)
			{
				m_sNewFolderName = sNewName;
			}
			else
			{
				m_sNewFolderName = sNewTitle;
			}
			UpdateRenameUI();
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
			List<SPContentType> list = new List<SPContentType>();
			foreach (Node targetNode in TargetNodes)
			{
				Node node2 = targetNode;
				SPWeb sPWeb;
				do
				{
					sPWeb = node2 as SPWeb;
					node2 = node2.Parent;
				}
				while (node2 != null && sPWeb == null);
				if (sPWeb == null)
				{
					continue;
				}
				foreach (SPContentType contentType in sPWeb.ContentTypes)
				{
					if (!list.Contains(contentType))
					{
						list.Add(contentType);
					}
				}
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
				FolderRenamingChanged(renameInfo.Name, renameInfo.Title);
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCFolderContentOptions));
			this.w_cbPreserveIds = new DevExpress.XtraEditors.CheckEdit();
			this.w_plMaxVersions = new DevExpress.XtraEditors.PanelControl();
			this.w_nudVersions = new DevExpress.XtraEditors.SpinEdit();
			this.w_lblMaxVersions = new DevExpress.XtraEditors.LabelControl();
			this.w_rbMaxVerisons = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbCopyAllVersions = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbVersions = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnEditRename = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbRenameFolder = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopySubfolders = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPreserveDocumentIDs = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbReattachPageLayouts = new DevExpress.XtraEditors.CheckEdit();
			this.w_pnlRename = new DevExpress.XtraEditors.PanelControl();
			this.w_plApplyNewContentTypes = new DevExpress.XtraEditors.PanelControl();
			this.w_cbApplyContentTypes = new DevExpress.XtraEditors.CheckEdit();
			this.w_bEditApplyContentTypes = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbDisableParsing = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbApplyDocumentSets = new DevExpress.XtraEditors.CheckEdit();
			this.w_bEditAndApplyDocumentSets = new DevExpress.XtraEditors.SimpleButton();
			this._cbUseAzureUpload = new DevExpress.XtraEditors.CheckEdit();
			this._helpAzureUpload = new TooltipsTest.HelpTipButton();
			this.cbEncryptAzureMigrationJobs = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveIds.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMaxVersions).BeginInit();
			this.w_plMaxVersions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_nudVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbMaxVerisons.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyAllVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbRenameFolder.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubfolders.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveDocumentIDs.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbReattachPageLayouts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlRename).BeginInit();
			this.w_pnlRename.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plApplyNewContentTypes).BeginInit();
			this.w_plApplyNewContentTypes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyContentTypes.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbDisableParsing.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyDocumentSets.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbUseAzureUpload.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._helpAzureUpload).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbEncryptAzureMigrationJobs.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbPreserveIds, "w_cbPreserveIds");
			this.w_cbPreserveIds.Name = "w_cbPreserveIds";
			this.w_cbPreserveIds.Properties.AutoWidth = true;
			this.w_cbPreserveIds.Properties.Caption = resources.GetString("w_cbPreserveIds.Properties.Caption");
			this.w_cbPreserveIds.CheckedChanged += new System.EventHandler(On_cbPreserveIds_CheckedChanged);
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
			int[] bits = new int[4] { 1000, 0, 0, 0 };
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
			resources.ApplyResources(this.w_rbCopyAllVersions, "w_rbCopyAllVersions");
			this.w_rbCopyAllVersions.Name = "w_rbCopyAllVersions";
			this.w_rbCopyAllVersions.Properties.AutoWidth = true;
			this.w_rbCopyAllVersions.Properties.Caption = resources.GetString("w_rbCopyAllVersions.Properties.Caption");
			this.w_rbCopyAllVersions.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyAllVersions.Properties.RadioGroupIndex = 1;
			this.w_rbCopyAllVersions.TabStop = false;
			resources.ApplyResources(this.w_cbVersions, "w_cbVersions");
			this.w_cbVersions.Name = "w_cbVersions";
			this.w_cbVersions.Properties.AutoWidth = true;
			this.w_cbVersions.Properties.Caption = resources.GetString("w_cbVersions.Properties.Caption");
			this.w_cbVersions.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			resources.ApplyResources(this.w_btnEditRename, "w_btnEditRename");
			this.w_btnEditRename.Name = "w_btnEditRename";
			this.w_btnEditRename.Click += new System.EventHandler(On_EditRename_Clicked);
			resources.ApplyResources(this.w_cbRenameFolder, "w_cbRenameFolder");
			this.w_cbRenameFolder.Name = "w_cbRenameFolder";
			this.w_cbRenameFolder.Properties.AutoWidth = true;
			this.w_cbRenameFolder.Properties.Caption = resources.GetString("w_cbRenameFolder.Properties.Caption");
			this.w_cbRenameFolder.CheckedChanged += new System.EventHandler(On_RenameFolder_CheckedChanged);
			resources.ApplyResources(this.w_cbCopyItems, "w_cbCopyItems");
			this.w_cbCopyItems.Name = "w_cbCopyItems";
			this.w_cbCopyItems.Properties.AutoWidth = true;
			this.w_cbCopyItems.Properties.Caption = resources.GetString("w_cbCopyItems.Properties.Caption");
			this.w_cbCopyItems.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			resources.ApplyResources(this.w_cbCopySubfolders, "w_cbCopySubfolders");
			this.w_cbCopySubfolders.Name = "w_cbCopySubfolders";
			this.w_cbCopySubfolders.Properties.AutoWidth = true;
			this.w_cbCopySubfolders.Properties.Caption = resources.GetString("w_cbCopySubfolders.Properties.Caption");
			resources.ApplyResources(this.w_cbPreserveDocumentIDs, "w_cbPreserveDocumentIDs");
			this.w_cbPreserveDocumentIDs.Name = "w_cbPreserveDocumentIDs";
			this.w_cbPreserveDocumentIDs.Properties.Caption = resources.GetString("w_cbPreserveDocumentIDs.Properties.Caption");
			this.w_cbPreserveDocumentIDs.CheckedChanged += new System.EventHandler(On_PreserveDocumentIDs_Checked);
			resources.ApplyResources(this.w_cbReattachPageLayouts, "w_cbReattachPageLayouts");
			this.w_cbReattachPageLayouts.Name = "w_cbReattachPageLayouts";
			this.w_cbReattachPageLayouts.Properties.AutoWidth = true;
			this.w_cbReattachPageLayouts.Properties.Caption = resources.GetString("w_cbReattachPageLayouts.Properties.Caption");
			this.w_pnlRename.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_pnlRename.Controls.Add(this.w_cbRenameFolder);
			this.w_pnlRename.Controls.Add(this.w_btnEditRename);
			resources.ApplyResources(this.w_pnlRename, "w_pnlRename");
			this.w_pnlRename.Name = "w_pnlRename";
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
			resources.ApplyResources(this.w_cbDisableParsing, "w_cbDisableParsing");
			this.w_cbDisableParsing.Name = "w_cbDisableParsing";
			this.w_cbDisableParsing.Properties.AutoWidth = true;
			this.w_cbDisableParsing.Properties.Caption = resources.GetString("w_cbDisableParsing.Properties.Caption");
			resources.ApplyResources(this.w_cbApplyDocumentSets, "w_cbApplyDocumentSets");
			this.w_cbApplyDocumentSets.Name = "w_cbApplyDocumentSets";
			this.w_cbApplyDocumentSets.Properties.AutoWidth = true;
			this.w_cbApplyDocumentSets.Properties.Caption = resources.GetString("w_cbApplyDocumentSets.Properties.Caption");
			this.w_cbApplyDocumentSets.CheckedChanged += new System.EventHandler(w_cbApplyDocumentSets_CheckedChanged);
			resources.ApplyResources(this.w_bEditAndApplyDocumentSets, "w_bEditAndApplyDocumentSets");
			this.w_bEditAndApplyDocumentSets.Name = "w_bEditAndApplyDocumentSets";
			this.w_bEditAndApplyDocumentSets.Click += new System.EventHandler(w_bEditAndApplyDocumentSets_Click);
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
			base.Controls.Add(this.w_cbApplyDocumentSets);
			base.Controls.Add(this.w_bEditAndApplyDocumentSets);
			base.Controls.Add(this.w_cbDisableParsing);
			base.Controls.Add(this.w_plApplyNewContentTypes);
			base.Controls.Add(this.w_pnlRename);
			base.Controls.Add(this.w_cbReattachPageLayouts);
			base.Controls.Add(this.w_cbPreserveDocumentIDs);
			base.Controls.Add(this.w_cbCopyItems);
			base.Controls.Add(this.w_cbCopySubfolders);
			base.Controls.Add(this.w_cbPreserveIds);
			base.Controls.Add(this.w_plMaxVersions);
			base.Controls.Add(this.w_cbVersions);
			base.Name = "TCFolderContentOptions";
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveIds.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plMaxVersions).EndInit();
			this.w_plMaxVersions.ResumeLayout(false);
			this.w_plMaxVersions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_nudVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbMaxVerisons.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyAllVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVersions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbRenameFolder.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubfolders.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPreserveDocumentIDs.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbReattachPageLayouts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlRename).EndInit();
			this.w_pnlRename.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plApplyNewContentTypes).EndInit();
			this.w_plApplyNewContentTypes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyContentTypes.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbDisableParsing.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbApplyDocumentSets.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbUseAzureUpload.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._helpAzureUpload).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbEncryptAzureMigrationJobs.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbCopyItems.Checked = Options.CopyListItems;
			w_cbCopySubfolders.Checked = Options.CopySubFolders;
			w_rbMaxVerisons.Checked = Options.CopyMaxVersions;
			w_rbCopyAllVersions.Checked = !m_options.CopyMaxVersions;
			w_cbVersions.Checked = Options.CopyVersions;
			w_nudVersions.Value = Options.MaximumVersionCount;
			m_sNewFolderName = Options.NewFolderName;
			w_cbRenameFolder.Checked = Options.RenameFolder;
			w_cbReattachPageLayouts.Checked = Options.ReattachPageLayouts;
			w_cbApplyContentTypes.Checked = Options.ApplyNewContentTypes;
			w_cbApplyDocumentSets.Checked = Options.ApplyNewDocumentSets;
			w_cbPreserveDocumentIDs.Checked = Options.PreserveDocumentIDs && SharePointConfigurationVariables.AllowDBWriting;
			m_bOriginalPreserveItemIdsValue = Options.PreserveItemIDs;
			w_cbPreserveIds.Checked = Options.PreserveItemIDs;
			CommonSerializableList<ContentTypeApplicationOptionsCollection> contentTypeApplicationOptions = ((Options.ContentTypeApplicationObjects != null) ? ((CommonSerializableList<ContentTypeApplicationOptionsCollection>)Options.ContentTypeApplicationObjects.Clone()) : null);
			m_contentTypeApplicationOptions = contentTypeApplicationOptions;
			CommonSerializableList<DocumentSetApplicationOptionsCollection> documentSetApplicationOptions = ((Options.DocumentSetApplicationObjects != null) ? ((CommonSerializableList<DocumentSetApplicationOptionsCollection>)Options.DocumentSetApplicationObjects.Clone()) : null);
			m_documentSetApplicationOptions = documentSetApplicationOptions;
			CommonSerializableList<DocumentSetFolderOptions> folderToDocumentSetApplicationOptions = ((Options.FolderToDocumentSetApplicationObjects != null) ? ((CommonSerializableList<DocumentSetFolderOptions>)Options.FolderToDocumentSetApplicationObjects.Clone()) : null);
			m_folderToDocumentSetApplicationOptions = folderToDocumentSetApplicationOptions;
			w_cbDisableParsing.Checked = Options.DisableDocumentParsing;
			_cbUseAzureUpload.Enabled = SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes);
			_cbUseAzureUpload.Checked = _cbUseAzureUpload.Enabled && Options.UseAzureOffice365Upload;
			if (_cbUseAzureUpload.Enabled && _cbUseAzureUpload.Checked)
			{
				cbEncryptAzureMigrationJobs.Enabled = true;
				cbEncryptAzureMigrationJobs.Checked = Options.EncryptAzureMigrationJobs;
			}
			UpdateEnabledState();
			UpdateRenameUI();
			UpdateAllAvailabilities();
		}

		private void MigrationModeChanged(MigrationMode newMigrationMode, bool bOverwritingOrUpdatingItems, bool bPropagatingItemDeletions)
		{
			if (TargetIsOMAdapter && !TargetIsClientOM)
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
				HideControl(w_pnlRename);
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

		private void On_RenameFolder_CheckedChanged(object sender, EventArgs e)
		{
			w_btnEditRename.Enabled = w_cbRenameFolder.Checked;
			if (!m_bSuspendRenameEvents)
			{
				if (w_cbRenameFolder.Checked)
				{
					EditRename();
				}
				else
				{
					FireFolderRenameStateChanged();
				}
			}
		}

		public override bool SaveUI()
		{
			Options.CopySubFolders = w_cbCopySubfolders.Checked;
			Options.CopyListItems = w_cbCopyItems.Checked;
			Options.CopyMaxVersions = w_rbMaxVerisons.Checked;
			Options.CopyVersions = w_cbVersions.Checked;
			Options.MaximumVersionCount = (int)w_nudVersions.Value;
			Options.NewFolderName = m_sNewFolderName;
			Options.RenameFolder = w_cbRenameFolder.Checked;
			Options.ReattachPageLayouts = w_cbReattachPageLayouts.Checked;
			Options.ApplyNewContentTypes = w_cbApplyContentTypes.Checked;
			Options.ApplyNewDocumentSets = w_cbApplyDocumentSets.Checked;
			Options.PreserveItemIDs = TargetIsOMAdapter && w_cbPreserveIds.Checked;
			Options.PreserveDocumentIDs = w_cbPreserveDocumentIDs.Checked && w_cbPreserveDocumentIDs.Enabled;
			Options.ContentTypeApplicationObjects = m_contentTypeApplicationOptions;
			Options.DocumentSetApplicationObjects = m_documentSetApplicationOptions;
			Options.FolderToDocumentSetApplicationObjects = m_folderToDocumentSetApplicationOptions;
			Options.ContentTypeApplicationObjects = m_contentTypeApplicationOptions;
			Options.DisableDocumentParsing = w_cbDisableParsing.Checked;
			Options.UseAzureOffice365Upload = _cbUseAzureUpload.Checked;
			Options.EncryptAzureMigrationJobs = cbEncryptAzureMigrationJobs.Checked;
			bool flag = false;
			if (!base.IsModeSwitched && Options.UseAzureOffice365Upload && SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
			{
				flag = SPUIUtils.IsAzureConnectionStringEmpty((SPNode)SourceNodes[0], (SPNode)TargetNodes[0], Options.EncryptAzureMigrationJobs, ref isAzureUserMappingWarningMessageRepeating);
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
			FireAvailabilityChanged(new AvailabilityChangedEventArgs(SharePointObjectScope.Item, w_cbCopyItems.Checked));
		}

		protected override void UpdateEnabledState()
		{
			w_btnEditRename.Enabled = w_cbRenameFolder.Enabled && w_cbRenameFolder.Checked;
			bool enabled = w_cbCopyItems.Checked && w_cbCopyItems.Enabled;
			w_cbVersions.Enabled = enabled;
			w_plMaxVersions.Enabled = w_cbVersions.Checked && w_cbVersions.Enabled;
			w_bEditApplyContentTypes.Enabled = w_cbApplyContentTypes.Checked && w_cbApplyContentTypes.Enabled;
			w_cbApplyDocumentSets.Enabled = false;
			w_bEditAndApplyDocumentSets.Enabled = false;
			if (TargetIs2010 && (!(TargetNodes[0] is SPFolder sPFolder) || !sPFolder.IsDocumentSet))
			{
				w_cbApplyDocumentSets.Enabled = true;
				w_bEditAndApplyDocumentSets.Enabled = w_cbApplyDocumentSets.Checked && w_cbApplyDocumentSets.Enabled;
			}
			if (TargetNodes != null && TargetNodes.Count != 0 && TargetNodes[0] is SPNode)
			{
				bool canWriteCreatedModifiedMetaInfo = ((SPNode)TargetNodes[0]).CanWriteCreatedModifiedMetaInfo;
			}
			w_cbPreserveIds.Enabled = TargetIsOMAdapter && !TargetIsClientOM;
			w_cbPreserveIds.Checked = w_cbPreserveIds.Checked && w_cbPreserveIds.Enabled;
			CheckEdit checkEdit = w_cbPreserveDocumentIDs;
			bool enabled2 = SharePointConfigurationVariables.AllowDBWriting && TargetIsOMAdapter && !TargetIsClientOM;
			checkEdit.Enabled = enabled2;
			w_cbPreserveDocumentIDs.Checked = w_cbPreserveDocumentIDs.Checked && w_cbPreserveDocumentIDs.Enabled;
			bool flag = false;
			bool flag2 = false;
			if (SourceFolder != null && TargetNodes != null && TargetNodes.Count > 0)
			{
				flag = SourceFolder.Adapter.SharePointVersion.MajorVersion == ((SPNode)TargetNodes[0]).Adapter.SharePointVersion.MajorVersion;
				if (((SPNode)SourceNodes[0]).Adapter.ExternalizationSupport == ExternalizationSupport.Supported)
				{
					ExternalizationSupport externalizationSupport = ((SPNode)TargetNodes[0]).Adapter.ExternalizationSupport;
				}
				ExternalizationSupport externalizationSupport2 = ((SPNode)TargetNodes[0]).Adapter.ExternalizationSupport;
				flag2 = ((SPNode)TargetNodes[0]).Adapter.IsNws;
			}
			w_cbReattachPageLayouts.Enabled = flag;
			if (!flag)
			{
				w_cbReattachPageLayouts.Checked = true;
			}
			w_cbDisableParsing.Enabled = !flag2;
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
			w_cbRenameFolder.Checked = NewFolderName != null;
			m_bSuspendRenameEvents = false;
		}

		protected override void UpdateScope()
		{
		}

		private void w_bEditAndApplyDocumentSets_Click(object sender, EventArgs e)
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
				FolderOptions = commonSerializableList2,
				TreatListAsFolder = true
			};
			if (documentSetApplicationOptionsSiteLevelConfigDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			CommonSerializableList<DocumentSetApplicationOptionsCollection> documentSetApplicationOptions = ((commonSerializableList.Count != 0) ? commonSerializableList : null);
			m_documentSetApplicationOptions = documentSetApplicationOptions;
			CommonSerializableList<DocumentSetFolderOptions> folderToDocumentSetApplicationOptions = ((commonSerializableList2.Count != 0) ? commonSerializableList2 : null);
			m_folderToDocumentSetApplicationOptions = folderToDocumentSetApplicationOptions;
			if (m_folderToDocumentSetApplicationOptions == null)
			{
				return;
			}
			foreach (DocumentSetFolderOptions folderToDocumentSetApplicationOption in m_folderToDocumentSetApplicationOptions)
			{
				if (folderToDocumentSetApplicationOption.FolderFilter.AppliesToTypes.Contains("Metalogix.SharePoint.SPListItem") && !folderToDocumentSetApplicationOption.FolderFilter.AppliesToTypes.Contains("Metalogix.SharePoint.SPFolder"))
				{
					folderToDocumentSetApplicationOption.FolderFilter.AppliesToTypes.Add("Metalogix.SharePoint.SPFolder");
				}
			}
		}

		private void w_bEditApplyContentTypes_Click(object sender, EventArgs e)
		{
			CommonSerializableList<ContentTypeApplicationOptionsCollection> commonSerializableList = m_contentTypeApplicationOptions ?? new CommonSerializableList<ContentTypeApplicationOptionsCollection>();
			ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = ((commonSerializableList.Count == 0) ? new ContentTypeApplicationOptionsCollection() : ((ContentTypeApplicationOptionsCollection)commonSerializableList[0]));
			SPList sPList = ((SourceFolder != null) ? SourceFolder.ParentList : null);
			SPList sPList2 = sPList;
			if (sPList2 == null)
			{
				if (SourceNodes == null || SourceNodes.Count == 0 || !(SourceNodes[0] is SPListItem))
				{
					return;
				}
				sPList2 = ((SPListItem)SourceNodes[0]).ParentList;
			}
			ContentTypeApplicationOptionsConfigDialog contentTypeApplicationOptionsConfigDialog = new ContentTypeApplicationOptionsConfigDialog(sPList2, GetTargetContentTypes())
			{
				Options = contentTypeApplicationOptionsCollection,
				HideColumnMappingOption = PasteActionUtils.CollectionContainsMultipleLists(SourceNodes)
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
