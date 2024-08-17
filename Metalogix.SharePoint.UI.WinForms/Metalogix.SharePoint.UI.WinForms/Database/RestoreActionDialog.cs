using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Database;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Widgets;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	public class RestoreActionDialog : CollapsableForm
	{
		private RestoreOptions m_options;

		private bool m_bWillOverwrite;

		private Location m_RestoreLocation;

		private Location m_defaultRestoreLocation;

		private string m_sWebAppUrl;

		private Node m_MatchedNode;

		private Node m_SourceNode;

		private DummyNode m_sourceFullDummy;

		private List<Node> m_restoreChain;

		private List<TimeSpan> m_times = new List<TimeSpan>();

		private List<ExplorerTreeNode> m_changeList = new List<ExplorerTreeNode>();

		private Location m_locationBuffer;

		private bool m_bUpdatingWebApps;

		private IContainer components;

		private Panel w_plButtons;

		protected Button w_btnCancel;

		protected Button w_btnOK;

		private Panel w_plLocation;

		private Label w_lblSelect;

		private ExplorerControlWithLocation w_explorer;

		private Button w_btnRestoreToDefaultLocation;

		private ComboBox w_cmbWebApplications;

		private Label w_lblWebApp;

		private CheckBox w_cbVerbose;

		public Location DefaultRestoreLocation
		{
			get
			{
				return m_defaultRestoreLocation;
			}
			set
			{
				m_defaultRestoreLocation = value;
			}
		}

		public Node LegalMatchNode
		{
			get
			{
				List<Node> shortestLegalParentChain = GetShortestLegalParentChain(RestoreLocation.GetNode());
				if (shortestLegalParentChain == null || shortestLegalParentChain.Count <= 0)
				{
					return null;
				}
				return shortestLegalParentChain[0];
			}
		}

		public Node MatchedNode
		{
			get
			{
				return m_MatchedNode;
			}
			set
			{
				m_MatchedNode = value;
				m_restoreChain = null;
				UpdateUI();
			}
		}

		public RestoreOptions Options
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

		public Location RestoreLocation
		{
			get
			{
				return m_RestoreLocation;
			}
			set
			{
				m_RestoreLocation = value;
				if (value != null)
				{
					w_btnOK.Enabled = true;
					w_btnRestoreToDefaultLocation.Enabled = true;
					if (w_explorer.DataSource != null)
					{
						w_explorer.NavigateToLocation(value);
					}
				}
			}
		}

		private DummyNode SourceFullDummy
		{
			get
			{
				if (m_sourceFullDummy == null)
				{
					m_sourceFullDummy = CloneDummyHierarchy(m_SourceNode, Color.Green);
				}
				return m_sourceFullDummy;
			}
		}

		public Node SourceNode
		{
			get
			{
				return m_SourceNode;
			}
			set
			{
				m_SourceNode = value;
				m_restoreChain = null;
				UpdateFilters();
				UpdateUI();
			}
		}

		public string WebApplicationUrl
		{
			get
			{
				return m_sWebAppUrl;
			}
			set
			{
				m_sWebAppUrl = value;
			}
		}

		public RestoreActionDialog()
		{
			InitializeComponent();
			HideControl(w_lblSelect);
		}

		private void CleanDummies()
		{
			foreach (ExplorerTreeNode change in m_changeList)
			{
				if (!(change.Node is DummyNode))
				{
					change.ColorizeText(Color.Empty);
				}
				else
				{
					change.Remove();
				}
			}
			m_changeList.Clear();
		}

		private DummyNode CloneDummy(Node sourceNode, Color highlightColor)
		{
			DummyNode dummyNode = sourceNode.CloneDummy();
			dummyNode.SetColor(highlightColor);
			return dummyNode;
		}

		private DummyNode CloneDummyHierarchy(Node sourceNode, Color highlightColor)
		{
			DummyNode dummyNode = CloneDummy(sourceNode, highlightColor);
			if (sourceNode.Children != null)
			{
				foreach (SPNode child in sourceNode.Children)
				{
					dummyNode.AddDummyChild(CloneDummyHierarchy(child, highlightColor));
				}
			}
			return dummyNode;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void ExpandTreeNode(Node sourceNode, ExplorerTreeNode targetTree)
		{
			if (m_restoreChain.Contains(sourceNode) && !targetTree.IsExpanded)
			{
				targetTree.Expand();
			}
		}

		private List<Node> GetShortestLegalParentChain(Node fromNode)
		{
			List<Node> list = new List<Node> { SourceNode };
			for (Node node = SourceNode.Parent; node != null; node = node.Parent)
			{
				list.Insert(0, node);
				PasteActionUtils.IsMatchingContainer(node, fromNode, out var bIsMatch, out var bCouldMatch);
				if (bIsMatch)
				{
					return list;
				}
				if (!bCouldMatch)
				{
					return null;
				}
			}
			return null;
		}

		private void HighlightTreeMatches(Node sourceNode, ExplorerTreeNode targetTree, bool bAddAll)
		{
			bAddAll = bAddAll || sourceNode == SourceNode;
			targetTree.UpdateChildrenUI();
			foreach (Node child in sourceNode.Children)
			{
				bool flag = false;
				if (!(m_restoreChain.Contains(child) || bAddAll))
				{
					continue;
				}
				foreach (ExplorerTreeNode node2 in targetTree.Nodes)
				{
					bool flag2 = false;
					if (node2.Node is SPSite && child is SPSite)
					{
						flag2 = !(((SPSite)node2.Node).Adapter.Server != w_cmbWebApplications.SelectedItem.ToString()) && ((SPSite)node2.Node).ServerRelativeUrl == ((SPSite)child).ServerRelativeUrl;
					}
					if (child.Name == node2.Node.Name || flag2)
					{
						node2.ColorizeText(Color.Blue);
						m_changeList.Add(node2);
						if (child.Name == SourceNode.Name)
						{
							m_bWillOverwrite = true;
						}
						HighlightTreeMatches(child, node2, bAddAll);
						flag = true;
					}
				}
				if (!flag)
				{
					ExplorerTreeNode explorerTreeNode2 = null;
					Node node = null;
					if (typeof(SPFolder).IsAssignableFrom(child.GetType()))
					{
						if (typeof(SPWeb).IsAssignableFrom(targetTree.Node.GetType()))
						{
							foreach (SPList list in ((SPWeb)targetTree.Node).Lists)
							{
								if (list.Name != child.Name)
								{
									continue;
								}
								node = list;
								break;
							}
						}
						else if (targetTree.Node.Children != null)
						{
							foreach (Node child2 in targetTree.Node.Children)
							{
								if (child2.Name != child.Name)
								{
									continue;
								}
								node = child2;
								break;
							}
						}
					}
					if (node != null)
					{
						explorerTreeNode2 = w_explorer.CreateTreeNode(CloneDummyHierarchy(node, Color.Empty));
						explorerTreeNode2.ColorizeText(Color.Blue);
						targetTree.Nodes.Insert(0, explorerTreeNode2);
						if (node.Name == SourceNode.Name)
						{
							m_bWillOverwrite = true;
						}
						HighlightTreeMatches(child, explorerTreeNode2, bAddAll);
						if (bAddAll || child == SourceNode)
						{
							explorerTreeNode2.Collapse();
						}
						else
						{
							explorerTreeNode2.Expand();
						}
					}
					else if (bAddAll)
					{
						explorerTreeNode2 = w_explorer.CreateTreeNode(CloneDummyHierarchy(child, Color.Green));
						targetTree.Nodes.Add(explorerTreeNode2);
						explorerTreeNode2.UpdateChildrenUI();
					}
					else if (m_restoreChain.Contains(child))
					{
						if (child != SourceNode)
						{
							explorerTreeNode2 = w_explorer.CreateTreeNode(CloneDummy(child, Color.Green));
							targetTree.Nodes.Insert(0, explorerTreeNode2);
							HighlightTreeMatches(child, explorerTreeNode2, bAddAll);
						}
						else
						{
							explorerTreeNode2 = w_explorer.CreateTreeNode(SourceFullDummy);
							targetTree.Nodes.Insert(0, explorerTreeNode2);
						}
						m_changeList.Add(explorerTreeNode2);
						explorerTreeNode2.UpdateChildrenUI();
					}
					m_changeList.Add(explorerTreeNode2);
				}
				if (!bAddAll)
				{
					break;
				}
			}
			if (m_restoreChain.Contains(sourceNode))
			{
				targetTree.Expand();
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Database.RestoreActionDialog));
			this.w_plButtons = new System.Windows.Forms.Panel();
			this.w_cbVerbose = new System.Windows.Forms.CheckBox();
			this.w_cmbWebApplications = new System.Windows.Forms.ComboBox();
			this.w_lblWebApp = new System.Windows.Forms.Label();
			this.w_btnRestoreToDefaultLocation = new System.Windows.Forms.Button();
			this.w_btnCancel = new System.Windows.Forms.Button();
			this.w_btnOK = new System.Windows.Forms.Button();
			this.w_plLocation = new System.Windows.Forms.Panel();
			this.w_explorer = new Metalogix.UI.WinForms.Widgets.ExplorerControlWithLocation();
			this.w_lblSelect = new System.Windows.Forms.Label();
			this.w_plButtons.SuspendLayout();
			this.w_plLocation.SuspendLayout();
			base.SuspendLayout();
			this.w_plButtons.Controls.Add(this.w_cbVerbose);
			this.w_plButtons.Controls.Add(this.w_cmbWebApplications);
			this.w_plButtons.Controls.Add(this.w_lblWebApp);
			this.w_plButtons.Controls.Add(this.w_btnRestoreToDefaultLocation);
			this.w_plButtons.Controls.Add(this.w_btnCancel);
			this.w_plButtons.Controls.Add(this.w_btnOK);
			resources.ApplyResources(this.w_plButtons, "w_plButtons");
			this.w_plButtons.Name = "w_plButtons";
			resources.ApplyResources(this.w_cbVerbose, "w_cbVerbose");
			this.w_cbVerbose.Name = "w_cbVerbose";
			this.w_cbVerbose.UseVisualStyleBackColor = true;
			this.w_cmbWebApplications.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.w_cmbWebApplications.DropDownWidth = 396;
			this.w_cmbWebApplications.FormattingEnabled = true;
			resources.ApplyResources(this.w_cmbWebApplications, "w_cmbWebApplications");
			this.w_cmbWebApplications.Name = "w_cmbWebApplications";
			this.w_cmbWebApplications.SelectedIndexChanged += new System.EventHandler(WebApplicationUrl_SelectedIndexChanged);
			resources.ApplyResources(this.w_lblWebApp, "w_lblWebApp");
			this.w_lblWebApp.Name = "w_lblWebApp";
			resources.ApplyResources(this.w_btnRestoreToDefaultLocation, "w_btnRestoreToDefaultLocation");
			this.w_btnRestoreToDefaultLocation.Name = "w_btnRestoreToDefaultLocation";
			this.w_btnRestoreToDefaultLocation.UseVisualStyleBackColor = true;
			this.w_btnRestoreToDefaultLocation.Click += new System.EventHandler(On_DefaultLocation_Clicked);
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(w_btnOK_Click);
			this.w_plLocation.Controls.Add(this.w_explorer);
			this.w_plLocation.Controls.Add(this.w_lblSelect);
			resources.ApplyResources(this.w_plLocation, "w_plLocation");
			this.w_plLocation.Name = "w_plLocation";
			this.w_explorer.Actions = new Metalogix.SharePoint.Actions.SharePointAction[0];
			this.w_explorer.CheckBoxes = false;
			this.w_explorer.DataSource = null;
			resources.ApplyResources(this.w_explorer, "w_explorer");
			this.w_explorer.LocationDescriptor = "Location:";
			this.w_explorer.MultiSelectEnabled = false;
			this.w_explorer.MultiSelectLimitationMethod = null;
			this.w_explorer.Name = "w_explorer";
			this.w_explorer.SelectedNodeChanged += new Metalogix.UI.WinForms.Widgets.ExplorerControl.SelectedNodeChangedHandler(On_Explorer_SelectedNodeChanged);
			resources.ApplyResources(this.w_lblSelect, "w_lblSelect");
			this.w_lblSelect.Name = "w_lblSelect";
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.w_plLocation);
			base.Controls.Add(this.w_plButtons);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "RestoreActionDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.Load += new System.EventHandler(On_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(On_Closing);
			this.w_plButtons.ResumeLayout(false);
			this.w_plLocation.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			w_cbVerbose.Checked = m_options.Verbose;
		}

		private void On_Closing(object sender, FormClosingEventArgs e)
		{
			if (base.DialogResult != DialogResult.OK)
			{
				return;
			}
			if (w_cmbWebApplications.SelectedItem == null)
			{
				FlatXtraMessageBox.Show("Please select a web application for the restoration", "Configuration Not Valid", MessageBoxButtons.OK);
				w_cmbWebApplications.Focus();
				e.Cancel = true;
			}
			if (m_bWillOverwrite)
			{
				string name = SourceNode.GetType().Name;
				object[] customAttributes = SourceNode.GetType().GetCustomAttributes(typeof(NameAttribute), inherit: true);
				if (customAttributes.Length != 0)
				{
					name = ((NameAttribute)customAttributes[0]).Name;
				}
				if (FlatXtraMessageBox.Show($"The {name.ToLower()} to be restored currently exists.{Environment.NewLine}This operation will overwrite the existing data.{Environment.NewLine}Continue?", "Overwrite?", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
				{
					e.Cancel = true;
				}
			}
		}

		private void On_DefaultLocation_Clicked(object sender, EventArgs e)
		{
			if (w_explorer.DataSource != null && RestoreLocation != null)
			{
				w_explorer.NavigateToLocation(RestoreLocation);
			}
			w_cmbWebApplications.SelectedItem = m_sWebAppUrl;
		}

		private void On_Explorer_SelectedNodeChanged(ReadOnlyCollection<ExplorerTreeNode> selectedNodes)
		{
			if (selectedNodes.Count == 0)
			{
				return;
			}
			ExplorerTreeNode explorerTreeNode = selectedNodes[selectedNodes.Count - 1];
			if (explorerTreeNode.Node is DummyNode)
			{
				w_cmbWebApplications.Enabled = false;
				w_btnOK.Enabled = false;
				return;
			}
			m_locationBuffer = explorerTreeNode.Node.Location;
			w_btnOK.Enabled = RestoreLocation != null || m_locationBuffer != null;
			if (explorerTreeNode.Node is SPNode sPNode)
			{
				m_bUpdatingWebApps = true;
				string text = ((w_cmbWebApplications.SelectedItem != null) ? w_cmbWebApplications.SelectedItem.ToString() : m_sWebAppUrl);
				w_cmbWebApplications.Items.Clear();
				if (!(sPNode is SPServer))
				{
					w_cmbWebApplications.Items.Add(sPNode.Adapter.Server);
					w_cmbWebApplications.SelectedItem = sPNode.Adapter.Server;
					w_cmbWebApplications.Enabled = false;
				}
				else
				{
					bool flag = false;
					foreach (SPWebApplication webApplication in ((SPServer)sPNode).WebApplications)
					{
						w_cmbWebApplications.Items.Add(webApplication.Url);
						if (!(webApplication.Url != text))
						{
							flag = true;
						}
					}
					if (flag)
					{
						w_cmbWebApplications.SelectedItem = text;
					}
					w_cmbWebApplications.Enabled = true;
				}
				m_bUpdatingWebApps = false;
			}
			UpdateUI();
		}

		private void On_Load(object sender, EventArgs e)
		{
			Exception ex = null;
			List<SPNode> list = new List<SPNode>();
			foreach (SPConnection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
			{
				try
				{
					if (activeConnection.Status == ConnectionStatus.Valid && activeConnection.Adapter.Writer != null)
					{
						list.Add(activeConnection);
					}
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
			}
			if (ex != null)
			{
				GlobalServices.ErrorHandler.HandleException("Error Loading Connections", "Some connected sites may not appear in the explorer: " + ex.Message, ex, ErrorIcon.Warning);
			}
			w_explorer.DataSource = new NodeCollection(list.ToArray());
			if (RestoreLocation != null)
			{
				w_explorer.NavigateToLocation(RestoreLocation);
			}
		}

		private void SaveUI()
		{
			m_options.Verbose = w_cbVerbose.Checked;
			m_RestoreLocation = m_locationBuffer;
			m_sWebAppUrl = w_cmbWebApplications.SelectedItem.ToString();
		}

		private void UpdateFilters()
		{
			if (SourceNode == null || SourceNode is SPListItem || SourceNode.GetType() == typeof(SPFolder))
			{
				if (w_explorer.NodeTypeFilter.Count > 0)
				{
					w_explorer.NodeTypeFilter.Clear();
				}
			}
			else if (SourceNode is SPSite)
			{
				if (w_explorer.NodeTypeFilter.Count != 2)
				{
					w_explorer.NodeTypeFilter.Clear();
					w_explorer.NodeTypeFilter.Add(typeof(DummyNode));
					w_explorer.NodeTypeFilter.Add(typeof(SPServer));
				}
			}
			else if (w_explorer.NodeTypeFilter.Count != 4)
			{
				w_explorer.NodeTypeFilter.Clear();
				w_explorer.NodeTypeFilter.Add(typeof(DummyNode));
				w_explorer.NodeTypeFilter.Add(typeof(SPWeb));
				w_explorer.NodeTypeFilter.Add(typeof(SPSite));
				w_explorer.NodeTypeFilter.Add(typeof(SPServer));
			}
		}

		private void UpdateUI()
		{
			if (!base.Visible)
			{
				return;
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				SuspendLayout();
				if (SourceNode == null || w_explorer.SelectedNode == null)
				{
					return;
				}
				m_restoreChain = GetShortestLegalParentChain(w_explorer.SelectedNode.Node);
				List<Node> list = new List<Node>();
				Node sourceNode = SourceNode;
				while (!sourceNode.Location.Equals(MatchedNode.Location))
				{
					if (!typeof(SPListItem).IsAssignableFrom(sourceNode.GetType()))
					{
						m_restoreChain.Add(sourceNode);
					}
					sourceNode = sourceNode.Parent;
				}
				if (m_restoreChain != null && m_restoreChain.Count < list.Count)
				{
					m_restoreChain = list;
				}
				CleanDummies();
				m_bWillOverwrite = false;
				if (m_restoreChain == null || w_cmbWebApplications.SelectedItem == null)
				{
					w_btnOK.Enabled = false;
				}
				else
				{
					HighlightTreeMatches(m_restoreChain[0], w_explorer.SelectedNode, bAddAll: false);
					w_btnOK.Enabled = true;
				}
				if (w_explorer.SelectedNode != null)
				{
					w_explorer.SelectedNode.Expand();
				}
			}
			finally
			{
				ResumeLayout();
				Cursor = Cursors.Default;
			}
		}

		private void w_btnOK_Click(object sender, EventArgs e)
		{
			if (w_cmbWebApplications.SelectedItem != null)
			{
				SaveUI();
			}
		}

		private void WebApplicationUrl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_bUpdatingWebApps)
			{
				UpdateUI();
			}
		}
	}
}
