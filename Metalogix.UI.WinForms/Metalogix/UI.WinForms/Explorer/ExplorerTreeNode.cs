using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Widgets;
using Metalogix.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Explorer
{
	public class ExplorerTreeNode : TreeNode
	{
		private int m_iCreationThread = Thread.CurrentThread.ManagedThreadId;

		private System.Windows.Forms.TreeView m_parentTree;

		private NodeChildrenChangedHandler m_childrenChangedHandler;

		private NodeStatusChangedHandler m_statusChangedHandler;

		private DisplayNameChangedHandler m_displayNameChangedHandler;

		private SetBusyDisplayHandler m_setBusyDisplayHandler;

		private Metalogix.Explorer.Node m_node;

		private bool m_bChildrenUIUpToDate;

		private bool m_bTextColorized;

		public new bool Checked
		{
			get
			{
				EnhancedTreeView treeView = base.TreeView as EnhancedTreeView;
				if (treeView == null)
				{
					return this.Checked;
				}
				return treeView.SelectedNodes.Contains(this);
			}
			set
			{
				base.Checked = true;
			}
		}

		public bool ChildrenUIUpToDate
		{
			get
			{
				return this.m_bChildrenUIUpToDate;
			}
		}

		public Metalogix.Explorer.Node Node
		{
			get
			{
				return this.m_node;
			}
		}

		public ExplorerTreeNode RootNode
		{
			get
			{
				if (base.Parent == null)
				{
					return this;
				}
				return ((ExplorerTreeNode)base.Parent).RootNode;
			}
		}

		public ExplorerTreeNode(Metalogix.Explorer.Node spNode, System.Windows.Forms.TreeView parentTree)
		{
			this.Initialize(spNode, parentTree, true);
		}

		public ExplorerTreeNode(Metalogix.Explorer.Node spNode, System.Windows.Forms.TreeView parentTree, bool bUpdateUI)
		{
			this.Initialize(spNode, parentTree, bUpdateUI);
		}

		public void AddImage(string imageName, Image image)
		{
			this.m_parentTree.ImageList.Images.Add(this.Node.ImageName, this.Node.Image);
			Bitmap bitmap = new Bitmap(this.Node.Image);
			Image image1 = ImageCache.GetImage("Metalogix.UI.WinForms.Icons.BusyIcon.png", base.GetType().Assembly);
			using (Graphics graphic = Graphics.FromImage(bitmap))
			{
				graphic.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphic.DrawImage(image1, 0, 0);
				graphic.Save();
			}
			this.m_parentTree.ImageList.Images.Add(string.Concat(this.Node.ImageName, "BusyImage"), bitmap);
		}

		private void ClearNodes()
		{
			foreach (ExplorerTreeNode node in base.Nodes)
			{
				node.Dispose();
			}
			base.Nodes.Clear();
		}

		public void ColorizeText(Color color)
		{
			if (color == Color.Empty)
			{
				this.SetColor(Color.Black);
				this.m_bTextColorized = false;
			}
			else
			{
				this.SetColor(color);
				this.m_bTextColorized = true;
			}
			this.UpdateUI();
		}

		private bool Contains(Metalogix.Explorer.Node spNode)
		{
			bool flag;
			IEnumerator enumerator = base.Nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (((ExplorerTreeNode)enumerator.Current).Node != spNode)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public void Dispose()
		{
			foreach (ExplorerTreeNode node in base.Nodes)
			{
				node.Dispose();
			}
			this.m_node.ChildrenChanged -= this.m_childrenChangedHandler;
			this.m_node.StatusChanged -= this.m_statusChangedHandler;
			this.m_node.DisplayNameChanged -= this.m_displayNameChangedHandler;
			this.m_node = null;
		}

		private void Initialize(Metalogix.Explorer.Node spNode, System.Windows.Forms.TreeView parentTree, bool bUpdateUI)
		{
			this.m_node = spNode;
			this.m_parentTree = parentTree;
			this.m_childrenChangedHandler = new NodeChildrenChangedHandler(this.On_spNode_ChildrenChanged);
			this.m_statusChangedHandler = new NodeStatusChangedHandler(this.On_spNode_StatusChanged);
			this.m_displayNameChangedHandler = new DisplayNameChangedHandler(this.On_spNode_DisplayNameChanged);
			this.m_setBusyDisplayHandler = new SetBusyDisplayHandler(this.On_SetBusyDisplayChanged);
			this.m_node.ChildrenChanged += this.m_childrenChangedHandler;
			this.m_node.StatusChanged += this.m_statusChangedHandler;
			this.m_node.DisplayNameChanged += this.m_displayNameChangedHandler;
			this.m_node.SetBusyDisplay += this.m_setBusyDisplayHandler;
			if (bUpdateUI)
			{
				this.UpdateUI();
			}
		}

		private void On_SetBusyDisplayChanged(bool isBusy)
		{
			this.SetBusyIcon(isBusy);
		}

		private void On_spNode_ChildrenChanged()
		{
			this.m_bChildrenUIUpToDate = false;
			this.UpdateChildrenUI();
		}

		private void On_spNode_DisplayNameChanged()
		{
			this.UpdateText();
		}

		private void On_spNode_StatusChanged()
		{
			try
			{
				if (this.m_node.IsConnectionRoot || this.m_node.Status == ConnectionStatus.Invalid)
				{
					this.UpdateUI();
				}
			}
			catch (Exception exception)
			{
			}
		}

		public void Redraw()
		{
			this.UpdateText();
			foreach (ExplorerTreeNode node in base.Nodes)
			{
				node.Redraw();
			}
		}

		private void SetBusyIcon(bool isBusy)
		{
			if (base.TreeView != null && base.TreeView.InvokeRequired)
			{
				Delegate setBusyDelegate = new ExplorerTreeNode.SetBusyDelegate(this.SetBusyIcon);
				System.Windows.Forms.TreeView treeView = base.TreeView;
				object[] objArray = new object[] { isBusy };
				treeView.Invoke(setBusyDelegate, objArray);
				return;
			}
			if (this.Node == null)
			{
				return;
			}
			if (!isBusy)
			{
				base.ImageKey = this.Node.ImageName;
				base.SelectedImageKey = this.Node.ImageName;
				return;
			}
			base.ImageKey = string.Concat(this.Node.ImageName, "BusyImage");
			base.SelectedImageKey = string.Concat(this.Node.ImageName, "BusyImage");
		}

		public void SetColor(Color col)
		{
			if (base.ForeColor != col)
			{
				if (base.TreeView != null && base.TreeView.InvokeRequired)
				{
					Delegate setColorDelegate = new ExplorerTreeNode.SetColorDelegate(this.SetColor);
					System.Windows.Forms.TreeView treeView = base.TreeView;
					object[] objArray = new object[] { col };
					treeView.Invoke(setColorDelegate, objArray);
					return;
				}
				base.ForeColor = col;
			}
		}

		public void UpdateChildrenUI()
		{
			try
			{
				if (base.TreeView != null)
				{
					if (!this.ChildrenUIUpToDate)
					{
						if (this.Node.Status != ConnectionStatus.Checking)
						{
							if (this.Node.Status == ConnectionStatus.NotChecked)
							{
								this.Node.FetchChildren();
							}
							else if (this.Node.Status != ConnectionStatus.Invalid)
							{
								if (this.m_iCreationThread != Thread.CurrentThread.ManagedThreadId)
								{
									bool invokeRequired = base.TreeView.InvokeRequired;
								}
								if (!base.TreeView.InvokeRequired)
								{
									if (this.Node.Children.Count <= 0)
									{
										this.ClearNodes();
									}
									else
									{
										Dictionary<Metalogix.Explorer.Node, int> nodes = new Dictionary<Metalogix.Explorer.Node, int>(this.Node.Children.Count);
										int num = 0;
										foreach (Metalogix.Explorer.Node child in this.Node.Children)
										{
											int num1 = num;
											num = num1 + 1;
											nodes.Add(child, num1);
										}
										int count = base.Nodes.Count;
										for (int i = 0; i < count; i++)
										{
											ExplorerTreeNode item = base.Nodes[i] as ExplorerTreeNode;
											if (nodes.ContainsKey(item.Node))
											{
												nodes.Remove(item.Node);
											}
											else
											{
												base.Nodes.RemoveAt(i);
												item.Dispose();
												i--;
												count--;
											}
										}
										foreach (KeyValuePair<Metalogix.Explorer.Node, int> node in nodes)
										{
											Metalogix.Explorer.Node key = node.Key;
											int value = node.Value;
											List<Type> nodeTypeFilter = ((TreeViewX)base.TreeView).NodeTypeFilter;
											if (nodeTypeFilter != null && nodeTypeFilter.Count != 0 && !nodeTypeFilter.Contains(key.GetType()))
											{
												continue;
											}
											ExplorerTreeNode explorerTreeNode = new ExplorerTreeNode(key, this.m_parentTree);
											base.Nodes.Insert(value, explorerTreeNode);
										}
									}
									this.m_bChildrenUIUpToDate = true;
									foreach (ExplorerTreeNode node1 in base.Nodes)
									{
										if (!node1.IsExpanded)
										{
											continue;
										}
										node1.UpdateChildrenUIAsync();
									}
								}
								else
								{
									Delegate updateChildrenUIDelegate = new ExplorerTreeNode.UpdateChildrenUIDelegate(this.UpdateChildrenUI);
									base.TreeView.Invoke(updateChildrenUIDelegate);
								}
							}
							else if (base.Nodes.Count > 0)
							{
								base.TreeView.Invoke(new ExplorerTreeNode.UpdateChildrenUIDelegate(this.ClearNodes));
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.m_bChildrenUIUpToDate = true;
			}
		}

		public void UpdateChildrenUIAsync()
		{
			Thread thread = new Thread(new ThreadStart(this.UpdateChildrenUI));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		private void UpdateText()
		{
			try
			{
				if (this.m_parentTree == null || !this.m_parentTree.InvokeRequired)
				{
					if (base.Text != this.Node.DisplayName)
					{
						base.Text = this.Node.DisplayName;
					}
					if (this.Node.Status == ConnectionStatus.Invalid)
					{
						base.Text = string.Concat(base.Text, " - ", this.Node.ErrorDescription);
					}
				}
				else
				{
					Delegate updateUIDelegate = new ExplorerTreeNode.UpdateUIDelegate(this.UpdateText);
					this.m_parentTree.Invoke(updateUIDelegate);
				}
			}
			catch (Exception exception)
			{
			}
		}

		public void UpdateUI()
		{
			try
			{
				if (this.m_parentTree == null || !this.m_parentTree.InvokeRequired)
				{
					DateTime now = DateTime.Now;
					if (!this.m_parentTree.ImageList.Images.ContainsKey(this.Node.ImageName))
					{
						this.AddImage(this.Node.ImageName, this.Node.Image);
					}
					if (base.ImageKey != this.Node.ImageName)
					{
						base.ImageKey = this.Node.ImageName;
					}
					if (base.SelectedImageKey != this.Node.ImageName)
					{
						base.SelectedImageKey = this.Node.ImageName;
					}
					this.UpdateText();
					if (this.Node is DummyNode && !this.m_bTextColorized)
					{
						this.SetColor(((DummyNode)this.Node).FontColor);
					}
					TimeSpan timeSpan = DateTime.Now.Subtract(now);
					double totalMilliseconds = timeSpan.TotalMilliseconds;
				}
				else
				{
					Delegate updateUIDelegate = new ExplorerTreeNode.UpdateUIDelegate(this.UpdateUI);
					this.m_parentTree.Invoke(updateUIDelegate);
				}
			}
			catch (Exception exception)
			{
			}
		}

		private delegate void SetBusyDelegate(bool isBusy);

		private delegate void SetColorDelegate(Color col);

		private delegate void UpdateChildrenUIDelegate();

		private delegate void UpdateUIDelegate();
	}
}