using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraHeirarchicalCheckBoxTreeList : TreeList
	{
		private bool _updateInProgress;

		private int _beginUpdateCount;

		public XtraHeirarchicalCheckBoxTreeList()
		{
			this.InitializeSettings();
			this.InitializeEvents();
		}

		public override void BeginUpdate()
		{
			try
			{
				this._beginUpdateCount++;
				base.BeginUpdate();
			}
			catch
			{
				this._beginUpdateCount--;
				throw;
			}
		}

		public void BeginUpdateCheckBoxes()
		{
			this._updateInProgress = true;
		}

		public override void EndUpdate()
		{
			try
			{
				base.EndUpdate();
			}
			finally
			{
				this._beginUpdateCount--;
			}
		}

		public void EndUpdateCheckBoxes()
		{
			if (this._updateInProgress)
			{
				foreach (TreeListNode node in base.Nodes)
				{
					this.UpdateNodeStateRecusively(node);
				}
				this._updateInProgress = false;
			}
		}

		private void InitializeEvents()
		{
			base.AfterCheckNode += new NodeEventHandler(this.On_AfterCheckNode);
		}

		private void InitializeSettings()
		{
			base.OptionsBehavior.AllowCopyToClipboard = false;
			base.OptionsBehavior.AllowQuickHideColumns = false;
			base.OptionsBehavior.AutoPopulateColumns = false;
			base.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
			base.OptionsBehavior.CopyToClipboardWithNodeHierarchy = false;
			base.OptionsBehavior.Editable = false;
			base.OptionsBehavior.ResizeNodes = false;
			base.OptionsBehavior.ShowToolTips = false;
			base.OptionsFilter.AllowColumnMRUFilterList = false;
			base.OptionsFilter.AllowFilterEditor = false;
			base.OptionsFilter.AllowMRUFilterList = false;
			base.OptionsFind.ShowClearButton = false;
			base.OptionsFind.ShowCloseButton = false;
			base.OptionsFind.ShowFindButton = false;
			base.OptionsMenu.EnableColumnMenu = false;
			base.OptionsMenu.EnableFooterMenu = false;
			base.OptionsMenu.ShowAutoFilterRowItem = false;
			base.OptionsSelection.EnableAppearanceFocusedCell = false;
			base.OptionsView.ShowCheckBoxes = true;
			base.OptionsView.ShowColumns = false;
			base.OptionsView.ShowFocusedFrame = false;
			base.OptionsView.ShowHorzLines = false;
			base.OptionsView.ShowIndicator = false;
			base.OptionsView.ShowVertLines = false;
		}

		private void On_AfterCheckNode(object sender, NodeEventArgs e)
		{
			if (this._updateInProgress || e.Node.CheckState == CheckState.Indeterminate)
			{
				return;
			}
			this._updateInProgress = true;
			bool flag = this._beginUpdateCount == 0;
			if (flag)
			{
				this.BeginUpdate();
			}
			try
			{
				foreach (TreeListNode node in e.Node.Nodes)
				{
					this.PushCheckChangeDown(node, e.Node.CheckState);
				}
				if (e.Node.ParentNode != null)
				{
					this.PropagateCheckChangeUp(e.Node.ParentNode);
				}
			}
			finally
			{
				if (flag)
				{
					this.EndUpdate();
				}
				this._updateInProgress = false;
			}
		}

		private void PropagateCheckChangeUp(TreeListNode node)
		{
			this.UpdateCheckStateBasedOnChildren(node);
			if (node.ParentNode != null)
			{
				this.PropagateCheckChangeUp(node.ParentNode);
			}
		}

		private void PushCheckChangeDown(TreeListNode node, CheckState state)
		{
			node.CheckState = state;
			foreach (TreeListNode treeListNode in node.Nodes)
			{
				this.PushCheckChangeDown(treeListNode, state);
			}
		}

		private void UpdateCheckStateBasedOnChildren(TreeListNode node)
		{
			CheckState? nullable = null;
			IEnumerator enumerator = node.Nodes.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					TreeListNode current = (TreeListNode)enumerator.Current;
					if (nullable.HasValue)
					{
						if (nullable.Value == current.CheckState)
						{
							continue;
						}
						nullable = new CheckState?(CheckState.Indeterminate);
					}
					else
					{
						nullable = new CheckState?(current.CheckState);
					}
				}
				while (nullable.Value != CheckState.Indeterminate);
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			node.CheckState = nullable.Value;
		}

		private void UpdateNodeStateRecusively(TreeListNode node)
		{
			if (!node.HasChildren)
			{
				return;
			}
			foreach (TreeListNode treeListNode in node.Nodes)
			{
				this.UpdateNodeStateRecusively(treeListNode);
			}
			this.UpdateCheckStateBasedOnChildren(node);
		}
	}
}