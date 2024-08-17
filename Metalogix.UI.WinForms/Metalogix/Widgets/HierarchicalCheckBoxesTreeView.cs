using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Metalogix.Widgets
{
	public class HierarchicalCheckBoxesTreeView : FlickerFreeTreeView
	{
		private object m_oNodeUpdateLock = new object();

		private bool m_bNodeUpdateInProgress;

		private HierarchicalCheckBoxesTreeView.ParentCheckboxBehavior m_parentCheckBoxCheckedWhen;

		public HierarchicalCheckBoxesTreeView.ParentCheckboxBehavior ParentCheckBoxCheckedWhen
		{
			get
			{
				return this.m_parentCheckBoxCheckedWhen;
			}
			set
			{
				this.m_parentCheckBoxCheckedWhen = value;
			}
		}

		public HierarchicalCheckBoxesTreeView()
		{
		}

		private void GetAllChildNodes(TreeNode node, List<TreeNode> nodes)
		{
			foreach (TreeNode treeNode in node.Nodes)
			{
				nodes.Add(treeNode);
				this.GetAllChildNodes(treeNode, nodes);
			}
		}

		private void HandleDoubleClickWinProc(Message m)
		{
			int num = HierarchicalCheckBoxesTreeView.WinProcHelper.SignedLOWORD(m.LParam);
			int num1 = HierarchicalCheckBoxesTreeView.WinProcHelper.SignedHIWORD(m.LParam);
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

		private void HandleNodeCheckBoxChangedWinProc(Message m)
		{
			HierarchicalCheckBoxesTreeView.WinProcHelper.TV_ITEMEX structure = (HierarchicalCheckBoxesTreeView.WinProcHelper.TV_ITEMEX)Marshal.PtrToStructure(m.LParam, typeof(HierarchicalCheckBoxesTreeView.WinProcHelper.TV_ITEMEX));
			if (structure.mask == 24 && structure.stateMask == 61440)
			{
				MethodInfo getNodeFromHandle = HierarchicalCheckBoxesTreeView.WinProcHelper.GetNodeFromHandle;
				object[] objArray = new object[] { structure.hItem };
				TreeNode treeNode = (TreeNode)getNodeFromHandle.Invoke(this, objArray);
				if (treeNode != null && (structure.state == 8192 || structure.state == 4096))
				{
					if (structure.state == 8192)
					{
						this.NodeCheckedStateChanged(treeNode, true);
						return;
					}
					if (structure.state == 4096)
					{
						this.NodeCheckedStateChanged(treeNode, false);
					}
				}
			}
		}

		private void NodeCheckedStateChanged(TreeNode node, bool bNewCheckedState)
		{
			if (this.m_bNodeUpdateInProgress)
			{
				return;
			}
			lock (this.m_oNodeUpdateLock)
			{
				this.m_bNodeUpdateInProgress = true;
				try
				{
					List<TreeNode> treeNodes = new List<TreeNode>();
					this.GetAllChildNodes(node, treeNodes);
					foreach (TreeNode treeNode in treeNodes)
					{
						treeNode.Checked = bNewCheckedState;
					}
					for (TreeNode i = node.Parent; i != null; i = i.Parent)
					{
						bool flag = true;
						bool flag1 = false;
						IEnumerator enumerator = i.Nodes.GetEnumerator();
						try
						{
							do
							{
								if (!enumerator.MoveNext())
								{
									break;
								}
								TreeNode current = (TreeNode)enumerator.Current;
								flag = (!flag ? false : current.Checked);
								flag1 = (flag1 ? true : current.Checked);
							}
							while (flag || !flag1);
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
						i.Checked = (this.ParentCheckBoxCheckedWhen == HierarchicalCheckBoxesTreeView.ParentCheckboxBehavior.AllChildrenChecked ? flag : flag1);
					}
				}
				finally
				{
					this.m_bNodeUpdateInProgress = false;
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (!base.DesignMode)
			{
				if (m.Msg == 515)
				{
					this.HandleDoubleClickWinProc(m);
					return;
				}
				if (m.Msg == 4415 || m.Msg == 4365)
				{
					this.HandleNodeCheckBoxChangedWinProc(m);
				}
			}
			base.WndProc(ref m);
		}

		public enum ParentCheckboxBehavior
		{
			AllChildrenChecked,
			AtLeastOneChildChecked
		}

		private static class WinProcHelper
		{
			public const int TVM_SETITEMA = 4365;

			public const int TVM_SETITEMW = 4415;

			public const int TVIS_STATEIMAGEMASK = 61440;

			public const int WM_LBUTTONDOWN = 513;

			public const int WM_LBUTTONDBLCLK = 515;

			public const int CHECKBOX_UPDATE_MASK = 24;

			public const int CHECKBOX_STATUS_CHECKED = 8192;

			public const int CHECKBOX_STATUS_UNCHECKED = 4096;

			public readonly static MethodInfo GetNodeFromHandle;

			public readonly static PropertyInfo CheckedStateInternal;

			static WinProcHelper()
			{
				HierarchicalCheckBoxesTreeView.WinProcHelper.GetNodeFromHandle = typeof(TreeView).GetMethod("NodeFromHandle", BindingFlags.Instance | BindingFlags.NonPublic);
				HierarchicalCheckBoxesTreeView.WinProcHelper.CheckedStateInternal = typeof(TreeNode).GetProperty("CheckedStateInternal", BindingFlags.Instance | BindingFlags.NonPublic);
			}

			public static int SignedHIWORD(IntPtr n)
			{
				return HierarchicalCheckBoxesTreeView.WinProcHelper.SignedHIWORD((int)((long)n));
			}

			public static int SignedHIWORD(int n)
			{
				return (short)(n >> 16 & 65535);
			}

			public static int SignedLOWORD(IntPtr n)
			{
				return HierarchicalCheckBoxesTreeView.WinProcHelper.SignedLOWORD((int)((long)n));
			}

			public static int SignedLOWORD(int n)
			{
				return (short)(n & 65535);
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