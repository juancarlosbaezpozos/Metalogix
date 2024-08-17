using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using Metalogix.Actions;
using Metalogix.Permissions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyUsersActionDialog : XtraForm
	{
		private static class TreeViewIcons
		{
			public static int Domain => 0;

			public static int User => 1;
		}

		private readonly string DOMAINLESS_DOMAIN = "DOMAINLESS USERS";

		private bool m_bConfiguring;

		private ConfigurationResult m_result = ConfigurationResult.Cancel;

		private IContainer components;

		private CheckEdit w_cbAllowSql;

		protected SimpleButton w_btnRun;

		protected SimpleButton w_btnSave;

		protected SimpleButton w_btnCancel;

		private XtraHeirarchicalCheckBoxTreeList _userSelectTree;

		private TreeListColumn _nameColumn;

		private ImageCollection _treeIcons;

		public bool AllowSql => w_cbAllowSql.Checked;

		public ConfigurationResult ConfigurationResult
		{
			get
			{
				return m_result;
			}
			set
			{
				m_result = value;
			}
		}

		public CopyUsersActionDialog()
		{
			InitializeComponent();
		}

		private void AddUserToTree(SPUser user, Dictionary<string, TreeListNode> domainMap, bool selected)
		{
			string userDomain = GetUserDomain(user);
			if (!domainMap.TryGetValue(userDomain, out var value))
			{
				value = _userSelectTree.AppendNode(null, null);
				value.ImageIndex = TreeViewIcons.Domain;
				value.SelectImageIndex = TreeViewIcons.Domain;
				value[0] = userDomain;
				domainMap.Add(userDomain, value);
			}
			TreeListNode treeListNode = _userSelectTree.AppendNode(null, value, selected ? CheckState.Checked : CheckState.Unchecked);
			treeListNode.Tag = user;
			treeListNode.ImageIndex = TreeViewIcons.User;
			treeListNode.SelectImageIndex = TreeViewIcons.User;
			treeListNode[0] = user.LoginName;
		}

		private void BuildTree(IEnumerable<SecurityPrincipal> selectedUsers, IEnumerable<SecurityPrincipal> otherUsers)
		{
			_userSelectTree.BeginUpdate();
			_userSelectTree.BeginUpdateCheckBoxes();
			try
			{
				Dictionary<string, TreeListNode> domainMap = new Dictionary<string, TreeListNode>();
				foreach (SPUser selectedUser in selectedUsers)
				{
					AddUserToTree(selectedUser, domainMap, selected: true);
				}
				foreach (SPUser otherUser in otherUsers)
				{
					AddUserToTree(otherUser, domainMap, selected: false);
				}
			}
			finally
			{
				_userSelectTree.EndUpdateCheckBoxes();
				_userSelectTree.EndUpdate();
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

		public List<SPUser> getSelectedLoginNames()
		{
			List<SPUser> sPUsers = new List<SPUser>();
			_userSelectTree.NodesIterator.DoOperation(delegate(TreeListNode node)
			{
				if (node.CheckState == CheckState.Checked && node.Tag != null)
				{
					sPUsers.Add((SPUser)node.Tag);
				}
			});
			return sPUsers;
		}

		private bool GetTargetIsOMAdapter(SPWeb targetWeb)
		{
			if (targetWeb.Adapter.IsNws)
			{
				return false;
			}
			return !targetWeb.Adapter.IsClientOM;
		}

		private string GetUserDomain(SPUser user)
		{
			int num = user.LoginName.IndexOf("\\", StringComparison.Ordinal);
			if (num < 0)
			{
				return DOMAINLESS_DOMAIN;
			}
			return user.LoginName.Substring(0, num);
		}

		public void Initialize(ActionConfigContext context, CopyUserOptions options)
		{
			m_bConfiguring = true;
			SPWeb targetWeb = context.ActionContext.Targets[0] as SPWeb;
			w_cbAllowSql.Enabled = SharePointConfigurationVariables.AllowDBWriting && GetTargetIsOMAdapter(targetWeb);
			w_cbAllowSql.Checked = w_cbAllowSql.Enabled && options.AllowDBUserWriting;
			if (context.ActionContext.Sources[0] is SPWeb sPWeb)
			{
				SecurityPrincipal[] array = sPWeb.SiteUsers.Where((SecurityPrincipal u) => options.SourceUsers.Any((SecurityPrincipal principal) => ((SPUser)principal).LoginName == ((SPUser)u).LoginName)).ToArray();
				SecurityPrincipal[] otherUsers = sPWeb.SiteUsers.Except(array).ToArray();
				BuildTree(array, otherUsers);
			}
			m_bConfiguring = false;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyUsersActionDialog));
			this.w_cbAllowSql = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnRun = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this._userSelectTree = new Metalogix.UI.WinForms.Components.XtraHeirarchicalCheckBoxTreeList();
			this._nameColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this._treeIcons = new DevExpress.Utils.ImageCollection(this.components);
			((System.ComponentModel.ISupportInitialize)this.w_cbAllowSql.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._userSelectTree).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._treeIcons).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbAllowSql, "w_cbAllowSql");
			this.w_cbAllowSql.Name = "w_cbAllowSql";
			this.w_cbAllowSql.Properties.AutoWidth = true;
			this.w_cbAllowSql.Properties.Caption = resources.GetString("w_cbAllowSql.Properties.Caption");
			this.w_cbAllowSql.CheckedChanged += new System.EventHandler(w_cbAllowSql_CheckedChanged);
			resources.ApplyResources(this.w_btnRun, "w_btnRun");
			this.w_btnRun.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnRun.Name = "w_btnRun";
			this.w_btnRun.Click += new System.EventHandler(On_Run_Clicked);
			resources.ApplyResources(this.w_btnSave, "w_btnSave");
			this.w_btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnSave.Name = "w_btnSave";
			this.w_btnSave.Click += new System.EventHandler(On_Save_Clicked);
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Click += new System.EventHandler(On_Cancel_Clicked);
			resources.ApplyResources(this._userSelectTree, "_userSelectTree");
			this._userSelectTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this._nameColumn });
			this._userSelectTree.Name = "_userSelectTree";
			this._userSelectTree.OptionsBehavior.AllowCopyToClipboard = false;
			this._userSelectTree.OptionsBehavior.AllowQuickHideColumns = false;
			this._userSelectTree.OptionsBehavior.AutoPopulateColumns = false;
			this._userSelectTree.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
			this._userSelectTree.OptionsBehavior.CopyToClipboardWithNodeHierarchy = false;
			this._userSelectTree.OptionsBehavior.Editable = false;
			this._userSelectTree.OptionsBehavior.ResizeNodes = false;
			this._userSelectTree.OptionsBehavior.ShowToolTips = false;
			this._userSelectTree.OptionsFilter.AllowColumnMRUFilterList = false;
			this._userSelectTree.OptionsFilter.AllowFilterEditor = false;
			this._userSelectTree.OptionsFilter.AllowMRUFilterList = false;
			this._userSelectTree.OptionsFind.ShowClearButton = false;
			this._userSelectTree.OptionsFind.ShowCloseButton = false;
			this._userSelectTree.OptionsFind.ShowFindButton = false;
			this._userSelectTree.OptionsMenu.EnableColumnMenu = false;
			this._userSelectTree.OptionsMenu.EnableFooterMenu = false;
			this._userSelectTree.OptionsMenu.ShowAutoFilterRowItem = false;
			this._userSelectTree.OptionsSelection.EnableAppearanceFocusedCell = false;
			this._userSelectTree.OptionsView.ShowCheckBoxes = true;
			this._userSelectTree.OptionsView.ShowColumns = false;
			this._userSelectTree.OptionsView.ShowFocusedFrame = false;
			this._userSelectTree.OptionsView.ShowHorzLines = false;
			this._userSelectTree.OptionsView.ShowIndicator = false;
			this._userSelectTree.OptionsView.ShowVertLines = false;
			this._userSelectTree.SelectImageList = this._treeIcons;
			resources.ApplyResources(this._nameColumn, "_nameColumn");
			this._nameColumn.FieldName = "Name";
			this._nameColumn.Name = "_nameColumn";
			this._nameColumn.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
			this._treeIcons.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("_treeIcons.ImageStream");
			this._treeIcons.Images.SetKeyName(0, "Globe16.png");
			this._treeIcons.Images.SetKeyName(1, "User16.png");
			base.AcceptButton = this.w_btnRun;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this._userSelectTree);
			base.Controls.Add(this.w_btnRun);
			base.Controls.Add(this.w_btnSave);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_cbAllowSql);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CopyUsersActionDialog";
			base.ShowInTaskbar = false;
			base.Shown += new System.EventHandler(On_Shown);
			((System.ComponentModel.ISupportInitialize)this.w_cbAllowSql.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._userSelectTree).EndInit();
			((System.ComponentModel.ISupportInitialize)this._treeIcons).EndInit();
			base.ResumeLayout(false);
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			m_result = ConfigurationResult.Cancel;
		}

		private void On_Run_Clicked(object sender, EventArgs e)
		{
			m_result = ConfigurationResult.Run;
		}

		private void On_Save_Clicked(object sender, EventArgs e)
		{
			m_result = ConfigurationResult.Save;
		}

		private void On_Shown(object sender, EventArgs e)
		{
			w_btnRun.Focus();
		}

		private void w_cbAllowSql_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_bConfiguring && w_cbAllowSql.Checked && FlatXtraMessageBox.Show(Resources.MsgUserDbWriteWarning, Resources.WarningString, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				w_cbAllowSql.Checked = false;
			}
		}
	}
}
