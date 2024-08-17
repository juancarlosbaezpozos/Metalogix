using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.Widgets
{
	public class EnhancedTreeView : TreeView
	{
		private EnhancedTreeView.AllowSelectionDelegate m_allowSelectionMethod;

		private bool m_bMultiselectEnabled;

		private List<TreeNode> m_selectedNodes;

		private List<TreeNode> m_nodesToBeSelected;

		private TreeNode m_lastNode;

		private TreeNode m_firstShiftNode;

		private bool m_bSelecting;

		private TreeNode m_lastMousedOverNode;

		private List<IntPtr> m_nodesToInitialize;

		private bool m_bCheckBoxDisableInProgress;

		private Dictionary<TreeNode, EnhancedTreeView.NodeColorPair> m_selectedNodeColors = new Dictionary<TreeNode, EnhancedTreeView.NodeColorPair>();

		private Dictionary<TreeNode, Thread> m_blinkThreadDict;

		private BufferedGraphics m_buffGraphics;

		private BufferedGraphicsContext m_buffGraphicsContext;

		private bool m_bUseDoubleBuffering = true;

		private bool m_bSmudgeProtection;

		private bool m_bClearBackground;

		public bool CheckBoxesEnhanced
		{
			get
			{
				return base.CheckBoxes;
			}
			set
			{
				if (base.CheckBoxes != value)
				{
					if (value)
					{
						base.CheckBoxes = true;
						return;
					}
					this.m_bCheckBoxDisableInProgress = true;
					base.BeginUpdate();
					try
					{
						List<TreeNode> treeNodes = new List<TreeNode>();
						foreach (TreeNode node in base.Nodes)
						{
							this.GetExpandedNodes(node, ref treeNodes);
						}
						Point treeViewScrollPos = EnhancedTreeView.WinProcHelper.GetTreeViewScrollPos(this);
						base.CheckBoxes = false;
						foreach (TreeNode treeNode in treeNodes)
						{
							treeNode.Expand();
						}
						EnhancedTreeView.WinProcHelper.SetTreeViewScrollPos(this, treeViewScrollPos);
					}
					finally
					{
						base.EndUpdate();
						this.m_bCheckBoxDisableInProgress = false;
					}
				}
			}
		}

		private bool ClearBackground
		{
			get
			{
				if (!this.m_bClearBackground)
				{
					return false;
				}
				return this.SmudgeProtection;
			}
			set
			{
				this.m_bClearBackground = value;
			}
		}

		public bool MultiSelectEnabled
		{
			get
			{
				return this.m_bMultiselectEnabled;
			}
			set
			{
				this.m_bMultiselectEnabled = value;
			}
		}

		public EnhancedTreeView.AllowSelectionDelegate MultiSelectLimitationMethod
		{
			get
			{
				return this.m_allowSelectionMethod;
			}
			set
			{
				this.m_allowSelectionMethod = value;
			}
		}

		public new TreeNode SelectedNode
		{
			get
			{
				if (this.SelectedNodes.Count == 0)
				{
					return null;
				}
				return this.SelectedNodes[this.SelectedNodes.Count - 1];
			}
			set
			{
				try
				{
					this.BeginSelect();
					this.ClearSelection();
					if (value != null)
					{
						if (!object.ReferenceEquals(value.TreeView, this))
						{
							throw new Exception("Only nodes included in the tree view can be selected.");
						}
						this.SelectNode(value);
					}
				}
				finally
				{
					this.EndSelect();
				}
			}
		}

		public ReadOnlyCollection<TreeNode> SelectedNodes
		{
			get
			{
				return new ReadOnlyCollection<TreeNode>(this.m_selectedNodes);
			}
			set
			{
				try
				{
					this.BeginSelect();
					this.ClearSelection();
					if (value != null)
					{
						foreach (TreeNode treeNode in value)
						{
							if (treeNode == null)
							{
								throw new Exception("Cannot select null values");
							}
							if (object.ReferenceEquals(treeNode.TreeView, this))
							{
								continue;
							}
							throw new Exception("Only nodes included in the tree view can be selected.");
						}
						this.SelectNodes(value);
					}
				}
				finally
				{
					this.EndSelect();
				}
			}
		}

		public bool SmudgeProtection
		{
			get
			{
				return this.m_bSmudgeProtection;
			}
			set
			{
				this.m_bSmudgeProtection = value;
			}
		}

		public bool UseDoubleBuffering
		{
			get
			{
				return this.m_bUseDoubleBuffering;
			}
			set
			{
				if (this.m_bUseDoubleBuffering != value && value)
				{
					this.InitalizeBufferObjects();
				}
				this.m_bUseDoubleBuffering = value;
			}
		}

		public EnhancedTreeView()
		{
			if (!base.DesignMode)
			{
				this.m_buffGraphicsContext = BufferedGraphicsManager.Current;
				this.InitalizeBufferObjects();
			}
			this.m_selectedNodes = new List<TreeNode>();
			this.m_nodesToBeSelected = new List<TreeNode>();
			this.m_blinkThreadDict = new Dictionary<TreeNode, Thread>();
			this.m_bMultiselectEnabled = false;
			this.m_lastMousedOverNode = null;
			this.m_nodesToInitialize = new List<IntPtr>();
			this.m_bCheckBoxDisableInProgress = false;
			base.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}

		public void BeginSelect()
		{
			this.m_bSelecting = true;
		}

		private void BlinkNode(object oNode)
		{
			TreeNode treeNode = oNode as TreeNode;
			if (treeNode == null)
			{
				return;
			}
			this.HighlightNode(treeNode);
			Thread.Sleep(75);
			this.DeHighlightNode(treeNode);
			Thread.Sleep(75);
			this.HighlightNode(treeNode);
			Thread.Sleep(75);
			this.DeHighlightNode(treeNode);
			Thread.Sleep(75);
			this.HighlightNode(treeNode);
			Thread.Sleep(75);
			this.DeHighlightNode(treeNode);
			this.StopNodeBlinking(treeNode);
		}

		protected void BufferedPaint(Graphics g)
		{
			if (this.m_buffGraphics != null)
			{
				IntPtr hdc = this.m_buffGraphics.Graphics.GetHdc();
				Message message = Message.Create(base.Handle, 792, hdc, IntPtr.Zero);
				this.DefWndProc(ref message);
				this.m_buffGraphics.Graphics.ReleaseHdc(hdc);
				this.m_buffGraphics.Render(g);
			}
		}

		private void CaptureColorChangesInSelectedNodes()
		{
			EnhancedTreeView.NodeColorPair foreColor;
			foreach (TreeNode selectedNode in this.SelectedNodes)
			{
				if (!this.m_selectedNodeColors.TryGetValue(selectedNode, out foreColor) || selectedNode.ForeColor == SystemColors.HighlightText && selectedNode.BackColor == SystemColors.Highlight)
				{
					continue;
				}
				FieldInfo field = selectedNode.GetType().GetField("propBag", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field == null)
				{
					continue;
				}
				OwnerDrawPropertyBag value = field.GetValue(selectedNode) as OwnerDrawPropertyBag;
				if (value == null)
				{
					continue;
				}
				if (selectedNode.ForeColor != SystemColors.HighlightText)
				{
					foreColor.TextColor = value.ForeColor;
					value.ForeColor = SystemColors.HighlightText;
				}
				if (selectedNode.BackColor != SystemColors.Highlight)
				{
					foreColor.BackgroundColor = value.BackColor;
					value.BackColor = SystemColors.Highlight;
				}
				this.m_selectedNodeColors[selectedNode] = foreColor;
			}
		}

		private bool CheckSelectionAllowed(TreeNode nodeToSelect, ReadOnlyCollection<TreeNode> otherSelectedNodes)
		{
			if (!this.MultiSelectEnabled)
			{
				if (otherSelectedNodes.Count == 0)
				{
					return true;
				}
				return false;
			}
			if (this.MultiSelectLimitationMethod == null || otherSelectedNodes.Count <= 0)
			{
				return true;
			}
			return this.MultiSelectLimitationMethod(nodeToSelect, otherSelectedNodes);
		}

		public bool ClearSelection()
		{
			if (this.m_nodesToBeSelected.Count == 0)
			{
				return true;
			}
			this.m_nodesToBeSelected.Clear();
			if (this.m_bSelecting)
			{
				return true;
			}
			return this.CommitSelectionChanges();
		}

		private bool CommitSelectionChanges()
		{
			bool flag;
			TreeNode item;
			List<TreeNode> treeNodes = new List<TreeNode>();
			List<TreeNode> treeNodes1 = new List<TreeNode>();
			foreach (TreeNode mNodesToBeSelected in this.m_nodesToBeSelected)
			{
				if (this.m_selectedNodes.Contains(mNodesToBeSelected))
				{
					continue;
				}
				treeNodes.Add(mNodesToBeSelected);
			}
			foreach (TreeNode mSelectedNode in this.m_selectedNodes)
			{
				if (this.m_nodesToBeSelected.Contains(mSelectedNode))
				{
					continue;
				}
				treeNodes1.Add(mSelectedNode);
			}
			if (treeNodes.Count == 0 && treeNodes1.Count == 0)
			{
				return true;
			}
			if (this.MultiSelectLimitationMethod != null)
			{
				List<TreeNode> treeNodes2 = new List<TreeNode>(this.m_selectedNodes);
				foreach (TreeNode treeNode in treeNodes1)
				{
					treeNodes2.Remove(treeNode);
				}
				ReadOnlyCollection<TreeNode> treeNodes3 = new ReadOnlyCollection<TreeNode>(treeNodes2);
				List<TreeNode>.Enumerator enumerator = treeNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						if (this.CheckSelectionAllowed(enumerator.Current, treeNodes3))
						{
							continue;
						}
						this.m_nodesToBeSelected = new List<TreeNode>(this.m_selectedNodes);
						flag = false;
						return flag;
					}
					goto Label0;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
		Label0:
			base.OnBeforeSelect(new TreeViewCancelEventArgs(null, true, TreeViewAction.Unknown));
			if (this.FireBeforeMultiSelect(treeNodes.ToArray(), treeNodes1.ToArray()))
			{
				this.m_nodesToBeSelected = new List<TreeNode>(this.m_selectedNodes);
				return false;
			}
			base.BeginUpdate();
			try
			{
				this.m_selectedNodes = new List<TreeNode>(this.m_nodesToBeSelected);
				List<TreeNode>.Enumerator enumerator1 = treeNodes1.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						TreeNode current = enumerator1.Current;
						this.DeHighlightNode(current, false);
						this.UpdateNodeStateImage(current);
					}
				}
				finally
				{
					((IDisposable)enumerator1).Dispose();
				}
				List<TreeNode>.Enumerator enumerator2 = treeNodes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						TreeNode current1 = enumerator2.Current;
						this.StopNodeBlinking(current1);
						this.HighlightNode(current1, false);
						this.UpdateNodeStateImage(current1);
						if (current1.Parent == null || current1.Parent.IsExpanded)
						{
							continue;
						}
						current1.Parent.Expand();
					}
				}
				finally
				{
					((IDisposable)enumerator2).Dispose();
				}
				if (treeNodes.Count > 0)
				{
					TreeNode item1 = treeNodes[treeNodes.Count - 1];
					if (!item1.IsVisible)
					{
						item1.EnsureVisible();
					}
				}
			}
			finally
			{
				base.EndUpdate();
			}
			if (this.m_selectedNodes.Count == 0)
			{
				item = null;
			}
			else
			{
				item = this.m_selectedNodes[this.m_selectedNodes.Count - 1];
			}
			this.m_lastNode = item;
			this.m_firstShiftNode = null;
			base.OnAfterSelect(new TreeViewEventArgs(this.SelectedNode));
			this.FireAfterMultiSelect(treeNodes.ToArray(), treeNodes1.ToArray());
			return true;
		}

		private void DeHighlightNode(TreeNode node)
		{
			this.DeHighlightNode(node, true);
		}

		private void DeHighlightNode(TreeNode node, bool withUpdates)
		{
			if (!base.InvokeRequired)
			{
				if (withUpdates)
				{
					base.BeginUpdate();
				}
				this.UnstoreColors(node);
				if (withUpdates)
				{
					base.EndUpdate();
				}
				return;
			}
			EnhancedTreeView.ChangeHighlightsDelegate changeHighlightsDelegate = new EnhancedTreeView.ChangeHighlightsDelegate(this.DeHighlightNode);
			object[] objArray = new object[] { node, withUpdates };
			base.Invoke(changeHighlightsDelegate, objArray);
		}

		public bool DeselectNode(TreeNode node)
		{
			if (!this.m_nodesToBeSelected.Remove(node) || this.m_bSelecting)
			{
				return true;
			}
			return this.CommitSelectionChanges();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				try
				{
					if (this.m_buffGraphics != null)
					{
						this.m_buffGraphics.Dispose();
					}
					if (this.m_buffGraphicsContext != null)
					{
						this.m_buffGraphicsContext.Dispose();
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public bool EndSelect()
		{
			this.m_bSelecting = false;
			return this.CommitSelectionChanges();
		}

		private void FireAfterMultiSelect(TreeNode[] newlySelected, TreeNode[] newlyDeselected)
		{
			if (this.AfterMultiSelect != null)
			{
				EnhancedTreeView.MultiselectAfterEventArgs multiselectAfterEventArg = new EnhancedTreeView.MultiselectAfterEventArgs()
				{
					NewlySelectedNodes = newlySelected,
					NewlyDeselectedNodes = newlyDeselected
				};
				this.AfterMultiSelect(multiselectAfterEventArg);
			}
		}

		private bool FireBeforeMultiSelect(TreeNode[] newlySelected, TreeNode[] newlyDeselected)
		{
			if (this.BeforeMultiSelect == null)
			{
				return false;
			}
			EnhancedTreeView.MultiselectBeforeEventArgs multiselectBeforeEventArg = new EnhancedTreeView.MultiselectBeforeEventArgs()
			{
				NewlySelectedNodes = newlySelected,
				NewlyDeselectedNodes = newlyDeselected,
				Cancel = false
			};
			this.BeforeMultiSelect(multiselectBeforeEventArg);
			return multiselectBeforeEventArg.Cancel;
		}

		private void GetExpandedNodes(TreeNode node, ref List<TreeNode> expandedNodes)
		{
			if (node.IsExpanded)
			{
				expandedNodes.Add(node);
			}
			foreach (TreeNode treeNode in node.Nodes)
			{
				this.GetExpandedNodes(treeNode, ref expandedNodes);
			}
		}

		private int GetExpectedNodeStateImage(TreeNode node)
		{
			if (this.m_selectedNodes.Contains(node))
			{
				return 2;
			}
			if (object.ReferenceEquals(this.m_lastMousedOverNode, node))
			{
				return 1;
			}
			return 5;
		}

		private void HandleDoubleClickWinProc(Message m)
		{
			int num = EnhancedTreeView.WinProcHelper.SignedLOWORD(m.LParam);
			int num1 = EnhancedTreeView.WinProcHelper.SignedHIWORD(m.LParam);
			TreeNode nodeAt = base.GetNodeAt(num, num1);
			bool @checked = false;
			if (nodeAt != null)
			{
				@checked = nodeAt.Checked;
			}
			Message message = new Message()
			{
				Msg = 513,
				HWnd = m.HWnd,
				LParam = m.LParam,
				WParam = m.WParam
			};
			this.WndProc(ref message);
			if (nodeAt != null)
			{
				if (@checked == nodeAt.Checked && num >= nodeAt.Bounds.X && num <= nodeAt.Bounds.X + nodeAt.Bounds.Width)
				{
					nodeAt.Toggle();
				}
				this.OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(nodeAt, System.Windows.Forms.MouseButtons.Left, 2, num, num1));
			}
			this.OnDoubleClick(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, num, num1, 0));
			this.OnMouseDoubleClick(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, num, num1, 0));
		}

		private bool HandleNodeCheckBoxChangedWinProc(Message m)
		{
			if (this.m_nodesToInitialize != null && this.m_nodesToInitialize.Count > 0)
			{
				IntPtr[] array = this.m_nodesToInitialize.ToArray();
				this.m_nodesToInitialize.Clear();
				IntPtr[] intPtrArray = array;
				for (int i = 0; i < (int)intPtrArray.Length; i++)
				{
					EnhancedTreeView.SetNodeStateImageIndex(intPtrArray[i], base.Handle, 5);
				}
			}
			EnhancedTreeView.WinProcHelper.TV_ITEMEX structure = (EnhancedTreeView.WinProcHelper.TV_ITEMEX)Marshal.PtrToStructure(m.LParam, typeof(EnhancedTreeView.WinProcHelper.TV_ITEMEX));
			if (structure.mask == 24 && structure.stateMask == 61440)
			{
				MethodInfo getNodeFromHandle = EnhancedTreeView.WinProcHelper.GetNodeFromHandle;
				object[] objArray = new object[] { structure.hItem };
				TreeNode treeNode = (TreeNode)getNodeFromHandle.Invoke(this, objArray);
				if (treeNode != null && (structure.state == 8192 || structure.state == 4096))
				{
					if (this.m_bCheckBoxDisableInProgress)
					{
						return true;
					}
					if (structure.state == 8192)
					{
						if (!this.SelectNode(treeNode))
						{
							EnhancedTreeView.WinProcHelper.CheckedStateInternal.SetValue(treeNode, false, null);
							this.StartNodeBlinking(treeNode);
							return true;
						}
					}
					else if (structure.state == 4096 && !this.DeselectNode(treeNode))
					{
						EnhancedTreeView.WinProcHelper.CheckedStateInternal.SetValue(treeNode, true, null);
						return true;
					}
				}
			}
			return false;
		}

		private void HighlightNode(TreeNode node)
		{
			this.HighlightNode(node, true);
		}

		private void HighlightNode(TreeNode node, bool withUpdates)
		{
			if (base.InvokeRequired)
			{
				EnhancedTreeView.ChangeHighlightsDelegate changeHighlightsDelegate = new EnhancedTreeView.ChangeHighlightsDelegate(this.HighlightNode);
				object[] objArray = new object[] { node, withUpdates };
				base.Invoke(changeHighlightsDelegate, objArray);
				return;
			}
			if (withUpdates)
			{
				base.BeginUpdate();
			}
			base.BeginUpdate();
			this.StoreColors(node);
			node.BackColor = SystemColors.Highlight;
			node.ForeColor = SystemColors.HighlightText;
			base.EndUpdate();
			if (withUpdates)
			{
				base.EndUpdate();
			}
		}

		private void InitalizeBufferObjects()
		{
			this.m_buffGraphicsContext.MaximumBuffer = new System.Drawing.Size(base.Width + 1, base.Height + 1);
			if (this.m_buffGraphics != null)
			{
				this.m_buffGraphics.Dispose();
				this.m_buffGraphics = null;
			}
			if (base.Width > 0 && base.Height > 0)
			{
				this.m_buffGraphics = this.m_buffGraphicsContext.Allocate(base.CreateGraphics(), new Rectangle(0, 0, base.Width, base.Height));
			}
		}

		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			if (!base.DesignMode)
			{
				e.Cancel = true;
				if (e.Action == TreeViewAction.Unknown)
				{
					return;
				}
				if (e.Node == null)
				{
					this.ClearSelection();
					return;
				}
				if (this.MultiSelectEnabled && (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift) && this.SelectedNodes.Count != 0)
				{
					if (Control.ModifierKeys == Keys.Shift)
					{
						TreeNode mFirstShiftNode = this.m_firstShiftNode;
						if (this.m_firstShiftNode == null && this.m_lastNode != null)
						{
							this.m_firstShiftNode = this.m_lastNode;
						}
						this.BeginSelect();
						this.ToggleNodesBetween(this.m_firstShiftNode, this.m_lastNode, false, false);
						this.ToggleNodesBetween(this.m_firstShiftNode, e.Node, true, false);
						TreeNode treeNode = this.m_firstShiftNode;
						if (!this.EndSelect())
						{
							this.m_firstShiftNode = mFirstShiftNode;
							return;
						}
						this.m_firstShiftNode = treeNode;
						return;
					}
					if (Control.ModifierKeys == Keys.Control)
					{
						if (this.m_nodesToBeSelected.Contains(e.Node))
						{
							this.DeselectNode(e.Node);
							return;
						}
						if (!this.SelectNode(e.Node))
						{
							this.StartNodeBlinking(e.Node);
						}
					}
				}
				else if (!this.m_nodesToBeSelected.Contains(e.Node) || this.SelectedNodes.Count > 1)
				{
					this.BeginSelect();
					this.ClearSelection();
					this.SelectNode(e.Node);
					this.EndSelect();
					return;
				}
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this.UpdateHighlights();
			base.OnGotFocus(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			e.Handled = true;
			if (e.KeyCode == Keys.Up)
			{
				if (this.m_lastNode != null && this.m_lastNode.PrevVisibleNode != null)
				{
					this.OnBeforeSelect(new TreeViewCancelEventArgs(this.m_lastNode.PrevVisibleNode, false, TreeViewAction.ByKeyboard));
					return;
				}
			}
			else if (e.KeyCode == Keys.Down)
			{
				if (this.m_lastNode != null && this.m_lastNode.NextVisibleNode != null)
				{
					this.OnBeforeSelect(new TreeViewCancelEventArgs(this.m_lastNode.NextVisibleNode, false, TreeViewAction.ByKeyboard));
					return;
				}
			}
			else if (e.KeyCode == Keys.Left)
			{
				if (this.m_lastNode != null)
				{
					if (this.m_lastNode.IsExpanded)
					{
						this.m_lastNode.Collapse();
						return;
					}
					if (this.m_lastNode.Parent != null)
					{
						this.OnBeforeSelect(new TreeViewCancelEventArgs(this.m_lastNode.Parent, false, TreeViewAction.ByKeyboard));
						return;
					}
				}
			}
			else if (e.KeyCode == Keys.Right)
			{
				if (this.m_lastNode != null)
				{
					if (!this.m_lastNode.IsExpanded)
					{
						this.m_lastNode.Expand();
						return;
					}
					if (this.m_lastNode.Nodes.Count > 0)
					{
						this.OnBeforeSelect(new TreeViewCancelEventArgs(this.m_lastNode.NextVisibleNode, false, TreeViewAction.ByKeyboard));
						return;
					}
				}
			}
			else if (e.KeyCode != Keys.Space || !base.CheckBoxes)
			{
				e.Handled = false;
				base.OnKeyDown(e);
			}
			else
			{
				foreach (TreeNode selectedNode in this.SelectedNodes)
				{
					selectedNode.Checked = !selectedNode.Checked;
				}
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this.UpdateHighlights();
			base.OnLostFocus(e);
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			this.ClearBackground = false;
			base.OnMouseCaptureChanged(e);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			this.ClearBackground = false;
			base.OnMouseClick(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.ClearBackground = true;
			base.OnMouseDown(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.m_lastMousedOverNode != null)
			{
				TreeNode mLastMousedOverNode = this.m_lastMousedOverNode;
				this.m_lastMousedOverNode = null;
				this.UpdateNodeStateImage(mLastMousedOverNode);
			}
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.UpdateCheckBoxesForNewMousePosition(e.X, e.Y);
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.ClearBackground = false;
			base.OnMouseUp(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (this.UseDoubleBuffering && !base.DesignMode)
			{
				this.InitalizeBufferObjects();
			}
		}

		protected override void OnStyleChanged(EventArgs e)
		{
			base.OnStyleChanged(e);
			if (base.CheckBoxes && base.Nodes.Count > 0)
			{
				foreach (TreeNode node in base.Nodes)
				{
					this.UpdateNodeStateImageRecursively(node);
				}
			}
		}

		public bool SelectNode(TreeNode node)
		{
			if (!this.MultiSelectEnabled)
			{
				this.m_nodesToBeSelected.Clear();
				this.m_nodesToBeSelected.Add(node);
				if (this.m_bSelecting)
				{
					return true;
				}
				return this.CommitSelectionChanges();
			}
			if (this.m_nodesToBeSelected.Contains(node))
			{
				return true;
			}
			this.m_nodesToBeSelected.Add(node);
			if (this.m_bSelecting)
			{
				return true;
			}
			return this.CommitSelectionChanges();
		}

		public bool SelectNodes(IEnumerable<TreeNode> nodes)
		{
			bool mBSelecting = this.m_bSelecting;
			if (!mBSelecting)
			{
				this.BeginSelect();
			}
			foreach (TreeNode node in nodes)
			{
				this.SelectNode(node);
			}
			if (mBSelecting)
			{
				return true;
			}
			return this.EndSelect();
		}

		public void SetMultiselectSibblingsAndSameTreeOnly()
		{
			this.MultiSelectLimitationMethod = new EnhancedTreeView.AllowSelectionDelegate(EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.SiblingsAndSameTreeOnly);
		}

		public void SetMultiselectSiblingsOnly()
		{
			this.MultiSelectLimitationMethod = new EnhancedTreeView.AllowSelectionDelegate(EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.SiblingsOnly);
		}

		private static void SetNodeStateImageIndex(TreeNode node, int index)
		{
			if (node.TreeView != null)
			{
				EnhancedTreeView.SetNodeStateImageIndex(node.Handle, node.TreeView.Handle, index);
			}
		}

		private static void SetNodeStateImageIndex(IntPtr nodeHandle, IntPtr treeViewHandle, int index)
		{
			EnhancedTreeView.WinProcHelper.TV_ITEMEX tVITEMEX = new EnhancedTreeView.WinProcHelper.TV_ITEMEX()
			{
				mask = 24,
				hItem = nodeHandle,
				stateMask = 61440,
				state = EnhancedTreeView.WinProcHelper.IndexToStateIndexMask(index)
			};
			EnhancedTreeView.WinProcHelper.SendMessage(treeViewHandle, 4365, 0, ref tVITEMEX);
		}

		public void SetSingleSelectionOnly()
		{
			this.MultiSelectLimitationMethod = new EnhancedTreeView.AllowSelectionDelegate(EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.SingleSelectionOnly);
		}

		private void StartNodeBlinking(TreeNode node)
		{
			if (node == null)
			{
				return;
			}
			Thread thread = new Thread(new ParameterizedThreadStart(this.BlinkNode));
			lock (this.m_blinkThreadDict)
			{
				this.StopNodeBlinkingNoLock(node);
				this.m_blinkThreadDict.Add(node, thread);
				thread.Start(node);
			}
		}

		private void StopNodeBlinking(TreeNode node)
		{
			if (node == null)
			{
				return;
			}
			lock (this.m_blinkThreadDict)
			{
				this.StopNodeBlinkingNoLock(node);
			}
		}

		private void StopNodeBlinkingNoLock(TreeNode node)
		{
			Thread thread;
			try
			{
				if (this.m_blinkThreadDict.TryGetValue(node, out thread))
				{
					this.m_blinkThreadDict.Remove(node);
					thread.Abort();
				}
			}
			catch
			{
			}
		}

		private void StoreColors(TreeNode node)
		{
			if (this.m_selectedNodeColors.ContainsKey(node))
			{
				return;
			}
			EnhancedTreeView.NodeColorPair nodeColorPair = new EnhancedTreeView.NodeColorPair()
			{
				TextColor = node.ForeColor,
				BackgroundColor = node.BackColor
			};
			this.m_selectedNodeColors.Add(node, nodeColorPair);
		}

		private void ToggleNodesBetween(TreeNode startNode, TreeNode endNode, bool bSelect, bool bChangeStartNode)
		{
			bool y = startNode.Bounds.Y > endNode.Bounds.Y;
			TreeNode treeNode = startNode;
			if (!bChangeStartNode)
			{
				if (object.ReferenceEquals(startNode, endNode))
				{
					return;
				}
				treeNode = (!y ? treeNode.NextVisibleNode : treeNode.PrevVisibleNode);
			}
			while (!object.ReferenceEquals(treeNode, endNode) && treeNode != null)
			{
				if (!bSelect)
				{
					this.DeselectNode(treeNode);
				}
				else if (this.CheckSelectionAllowed(treeNode, this.SelectedNodes))
				{
					this.SelectNode(treeNode);
				}
				treeNode = (!y ? treeNode.NextVisibleNode : treeNode.PrevVisibleNode);
			}
			if (treeNode != null)
			{
				if (!bSelect)
				{
					this.DeselectNode(treeNode);
				}
				else if (this.CheckSelectionAllowed(treeNode, this.SelectedNodes))
				{
					this.SelectNode(treeNode);
					return;
				}
			}
		}

		private void UnstoreColors(TreeNode node)
		{
			EnhancedTreeView.NodeColorPair nodeColorPair;
			if (!this.m_selectedNodeColors.TryGetValue(node, out nodeColorPair))
			{
				return;
			}
			node.ForeColor = nodeColorPair.TextColor;
			node.BackColor = nodeColorPair.BackgroundColor;
			this.m_selectedNodeColors.Remove(node);
		}

		private void UpdateCheckBoxesForNewMousePosition(int xPosition, int yPosition)
		{
			TreeNode nodeAt = base.GetNodeAt(xPosition, yPosition);
			if (nodeAt != null)
			{
				if (!object.ReferenceEquals(nodeAt, this.m_lastMousedOverNode))
				{
					TreeNode mLastMousedOverNode = this.m_lastMousedOverNode;
					this.m_lastMousedOverNode = nodeAt;
					this.UpdateNodeStateImage(nodeAt);
					if (mLastMousedOverNode != null)
					{
						this.UpdateNodeStateImage(mLastMousedOverNode);
						return;
					}
				}
			}
			else if (this.m_lastMousedOverNode != null)
			{
				TreeNode treeNode = this.m_lastMousedOverNode;
				this.m_lastMousedOverNode = null;
				this.UpdateNodeStateImage(treeNode);
			}
		}

		private void UpdateDeletedNodesFromWinProc(Message m)
		{
			this.BeginSelect();
			foreach (TreeNode selectedNode in this.SelectedNodes)
			{
				try
				{
					bool flag = false;
					if (selectedNode.TreeView == null || selectedNode.Handle == m.LParam)
					{
						flag = true;
					}
					else
					{
						TreeNode parent = selectedNode.Parent;
						while (parent != null)
						{
							if (parent.Handle != m.LParam)
							{
								parent = parent.Parent;
							}
							else
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						this.DeselectNode(selectedNode);
					}
				}
				catch
				{
				}
			}
			this.EndSelect();
		}

		private void UpdateHighlights()
		{
			base.BeginUpdate();
			try
			{
				foreach (TreeNode selectedNode in this.SelectedNodes)
				{
					if (this.Focused || !base.HideSelection)
					{
						this.HighlightNode(selectedNode, false);
					}
					else
					{
						this.DeHighlightNode(selectedNode, false);
					}
				}
			}
			finally
			{
				base.EndUpdate();
			}
		}

		private void UpdateNodeStateImage(TreeNode node)
		{
			EnhancedTreeView.SetNodeStateImageIndex(node, this.GetExpectedNodeStateImage(node));
		}

		private void UpdateNodeStateImageRecursively(TreeNode node)
		{
			this.UpdateNodeStateImage(node);
			foreach (TreeNode treeNode in node.Nodes)
			{
				this.UpdateNodeStateImageRecursively(treeNode);
			}
		}

		private void UpdateUIAfterScroll()
		{
			Point client = base.PointToClient(System.Windows.Forms.Cursor.Position);
			this.UpdateCheckBoxesForNewMousePosition(client.X, client.Y);
		}

		protected override void WndProc(ref Message m)
		{
			if (!base.DesignMode)
			{
				if (m.Msg == 20 && !this.ClearBackground)
				{
					return;
				}
				if (m.Msg == 15 && this.UseDoubleBuffering && this.m_buffGraphics != null)
				{
					EnhancedTreeView.WinProcHelper.PAINTSTRUCT pAINTSTRUCT = new EnhancedTreeView.WinProcHelper.PAINTSTRUCT();
					IntPtr intPtr = EnhancedTreeView.WinProcHelper.BeginPaint(m.HWnd, ref pAINTSTRUCT);
					this.CaptureColorChangesInSelectedNodes();
					using (Graphics graphic = Graphics.FromHdc(intPtr))
					{
						this.BufferedPaint(graphic);
					}
					EnhancedTreeView.WinProcHelper.EndPaint(m.HWnd, ref pAINTSTRUCT);
					return;
				}
				if (m.Msg == 515)
				{
					this.HandleDoubleClickWinProc(m);
					return;
				}
				if ((m.Msg == 4415 || m.Msg == 4365) && this.HandleNodeCheckBoxChangedWinProc(m))
				{
					return;
				}
			}
			base.WndProc(ref m);
			if (!base.DesignMode)
			{
				if (m.Msg == 4353)
				{
					this.UpdateDeletedNodesFromWinProc(m);
					return;
				}
				if (!this.m_bCheckBoxDisableInProgress && (m.Msg == 4402 || m.Msg == 4352))
				{
					this.m_nodesToInitialize.Add(m.Result);
					return;
				}
				if (m.Msg == 522)
				{
					this.UpdateUIAfterScroll();
				}
			}
		}

		public event EnhancedTreeView.MultiselectAfterEventDelegate AfterMultiSelect;

		public event EnhancedTreeView.MultiselectBeforeEventDelegate BeforeMultiSelect;

		public delegate bool AllowSelectionDelegate(TreeNode nodeToBeSelected, ReadOnlyCollection<TreeNode> selectedNodes);

		private delegate void ChangeHighlightsDelegate(TreeNode node, bool bWithUpdates);

		public class MultiselectAfterEventArgs
		{
			public TreeNode[] NewlySelectedNodes;

			public TreeNode[] NewlyDeselectedNodes;

			public MultiselectAfterEventArgs()
			{
			}
		}

		public delegate void MultiselectAfterEventDelegate(EnhancedTreeView.MultiselectAfterEventArgs e);

		public class MultiselectBeforeEventArgs
		{
			public TreeNode[] NewlySelectedNodes;

			public TreeNode[] NewlyDeselectedNodes;

			public bool Cancel;

			public MultiselectBeforeEventArgs()
			{
			}
		}

		public delegate void MultiselectBeforeEventDelegate(EnhancedTreeView.MultiselectBeforeEventArgs e);

		public static class MultiselectLimitationStockMethodsAndHelpers
		{
			public static TreeNodeCollection GetContainingNodeCollection(TreeNode node)
			{
				if (node.Parent == null)
				{
					return node.TreeView.Nodes;
				}
				return node.Parent.Nodes;
			}

			public static bool OnSameLevel(TreeNode node1, TreeNode node2)
			{
				return EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.GetContainingNodeCollection(node1).Contains(node2);
			}

			public static bool SiblingsAndSameTreeOnly(TreeNode node, ReadOnlyCollection<TreeNode> selectedNodes)
			{
				if (selectedNodes == null || selectedNodes.Count == 0)
				{
					return true;
				}
				bool parent = EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.SiblingsOnly(node, selectedNodes);
				if (parent)
				{
					parent = selectedNodes[0].Parent != null;
				}
				return parent;
			}

			public static bool SiblingsOnly(TreeNode node, ReadOnlyCollection<TreeNode> selectedNodes)
			{
				if (selectedNodes == null || selectedNodes.Count == 0)
				{
					return true;
				}
				return EnhancedTreeView.MultiselectLimitationStockMethodsAndHelpers.OnSameLevel(node, selectedNodes[0]);
			}

			public static bool SingleSelectionOnly(TreeNode node, ReadOnlyCollection<TreeNode> selectedNodes)
			{
				if (selectedNodes != null && selectedNodes.Count != 0)
				{
					return false;
				}
				return true;
			}
		}

		private struct NodeColorPair
		{
			private Color m_textColor;

			private Color m_backgroundColor;

			public Color BackgroundColor
			{
				get
				{
					return this.m_backgroundColor;
				}
				set
				{
					this.m_backgroundColor = value;
				}
			}

			public Color TextColor
			{
				get
				{
					return this.m_textColor;
				}
				set
				{
					this.m_textColor = value;
				}
			}
		}

		private static class WinProcHelper
		{
			public const int TVM_GETITEM = 4364;

			public const int TVM_SETITEMA = 4365;

			public const int TVM_SETITEMW = 4415;

			public const int TVM_DELETEITEM = 4353;

			public const int TVM_INSERTITEMW = 4402;

			public const int TVM_INSERTITEMA = 4352;

			public const uint TVIF_STATE = 8;

			public const uint TVIF_HANDLE = 16;

			public const uint TVIF_STATEEX = 16;

			public const int TVIS_STATEIMAGEMASK = 61440;

			public const int TVIS_EX_DISABLED = 2;

			public const int WM_LBUTTONDOWN = 513;

			public const int WM_LBUTTONDBLCLK = 515;

			public const int WM_ERASEBKGND = 20;

			public const int WM_PAINT = 15;

			public const int WM_MOUSEWHEEL = 522;

			public const int CHECKBOX_UPDATE_MASK = 24;

			public const int CHECKBOX_STATUS_CHECKED = 8192;

			public const int CHECKBOX_STATUS_UNCHECKED = 4096;

			private const int SB_HORZ = 0;

			private const int SB_VERT = 1;

			public readonly static MethodInfo GetNodeFromHandle;

			public readonly static PropertyInfo CheckedStateInternal;

			static WinProcHelper()
			{
				EnhancedTreeView.WinProcHelper.GetNodeFromHandle = typeof(TreeView).GetMethod("NodeFromHandle", BindingFlags.Instance | BindingFlags.NonPublic);
				EnhancedTreeView.WinProcHelper.CheckedStateInternal = typeof(TreeNode).GetProperty("CheckedStateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
			}

			[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern IntPtr BeginPaint(IntPtr hWnd, ref EnhancedTreeView.WinProcHelper.PAINTSTRUCT paintStruct);

			[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern bool EndPaint(IntPtr hWnd, ref EnhancedTreeView.WinProcHelper.PAINTSTRUCT paintStruct);

			[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
			public static extern int GetScrollPos(int hWnd, int nBar);

			public static Point GetTreeViewScrollPos(TreeView treeView)
			{
				return new Point(EnhancedTreeView.WinProcHelper.GetScrollPos((int)treeView.Handle, 0), EnhancedTreeView.WinProcHelper.GetScrollPos((int)treeView.Handle, 1));
			}

			public static uint IndexToStateIndexMask(int index)
			{
				return (uint)(index << 12);
			}

			[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, ref EnhancedTreeView.WinProcHelper.TV_ITEMEX item);

			[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
			public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

			public static void SetTreeViewScrollPos(TreeView treeView, Point scrollPosition)
			{
				EnhancedTreeView.WinProcHelper.SetScrollPos(treeView.Handle, 0, scrollPosition.X, true);
				EnhancedTreeView.WinProcHelper.SetScrollPos(treeView.Handle, 1, scrollPosition.Y, true);
			}

			public static int SignedHIWORD(IntPtr n)
			{
				return EnhancedTreeView.WinProcHelper.SignedHIWORD((int)((long)n));
			}

			public static int SignedHIWORD(int n)
			{
				return (short)(n >> 16 & 65535);
			}

			public static int SignedLOWORD(IntPtr n)
			{
				return EnhancedTreeView.WinProcHelper.SignedLOWORD((int)((long)n));
			}

			public static int SignedLOWORD(int n)
			{
				return (short)(n & 65535);
			}

			public struct PAINTSTRUCT
			{
				public IntPtr hdc;

				public int fErase;

				public EnhancedTreeView.WinProcHelper.RECT rcPaint;

				public int fRestore;

				public int fIncUpdate;

				public int Reserved1;

				public int Reserved2;

				public int Reserved3;

				public int Reserved4;

				public int Reserved5;

				public int Reserved6;

				public int Reserved7;

				public int Reserved8;
			}

			public struct RECT
			{
				public int left;

				public int top;

				public int right;

				public int bottom;
			}

			public struct TV_ITEMEX
			{
				public uint mask;

				public IntPtr hItem;

				public uint state;

				public uint stateMask;

				public IntPtr pszText;

				public int cchTextMax;

				public int iImage;

				public int iSelectedImage;

				public int cChildren;

				public IntPtr lParam;

				public int iIntegral;

				public uint uStateEx;

				public IntPtr hwnd;

				public int iExpandedImage;
			}
		}
	}
}