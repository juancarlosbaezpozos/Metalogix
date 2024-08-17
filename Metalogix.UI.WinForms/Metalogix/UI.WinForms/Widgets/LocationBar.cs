using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Explorer;

namespace Metalogix.UI.WinForms.Widgets
{
    public class LocationBar : UserControl
    {
        public delegate void SelectedNodeChangedHandler(Node node);

        private delegate void UpdateUIDelegate();

        private EventHandler m_locLink_TextChanged;

        private string m_sDescriptor = "Location:";

        private Node m_node;

        private int m_iCustomizedButtonWidth;

        private NodeStatusChangedHandler m_selectedStatusHandler;

        private IContainer components;

        private ToolStrip w_toolStripLocation;

        private ToolStripLabel w_lblLoc;

        private ToolStripLabel w_toolStripButtonURL;

        private ContextMenuStrip w_contextMenuURL;

        private ToolStripMenuItem w_toolStripMenuItemCopy;

        private ToolStripMenuItem w_toolStripMenuItemOpen;

        private ToolStripButton w_toolStripIcon;

        private ContextMenuStrip w_contextMenuIcon;

        private ToolStripMenuItem w_copyRootURLToClipboardToolStripMenuItem;

        private ToolStripMenuItem w_openRootURLInNewWindowToolStripMenuItem;

        public string Descriptor
        {
            get
		{
			return m_sDescriptor;
		}
            set
		{
			m_sDescriptor = value;
			UpdateUI();
		}
        }

        public Node Node
        {
            get
		{
			return m_node;
		}
            set
		{
			if (m_node != null)
			{
				m_node.StatusChanged -= m_selectedStatusHandler;
			}
			m_node = value;
			if (m_node != null)
			{
				m_node.StatusChanged += m_selectedStatusHandler;
			}
			UpdateUI();
			Fire_SelectedNodeChanged();
		}
        }

        private Node RootNode
        {
            get
		{
			Node node = Node;
			while (node.Parent != null)
			{
				node = node.Parent;
			}
			return node;
		}
        }

        public event SelectedNodeChangedHandler SelectedNodeChanged;

        public LocationBar()
	{
		Initialize();
	}

        public LocationBar(string sDescriptor)
	{
		Initialize();
		Descriptor = sDescriptor;
	}

        public LocationBar(Node node)
	{
		Initialize();
		Node = node;
	}

        public LocationBar(string sDescriptor, Node node)
	{
		Initialize();
		Descriptor = sDescriptor;
		Node = node;
	}

        public void AddToolStripItem(ToolStripItem item)
	{
		item.Alignment = ToolStripItemAlignment.Right;
		if (item.Overflow == ToolStripItemOverflow.Never)
		{
			m_iCustomizedButtonWidth += item.Width;
		}
		w_toolStripLocation.Items.Add(item);
	}

        private void AdjustLink()
	{
		SuspendLayout();
		int width = w_toolStripLocation.Size.Width - w_lblLoc.Width - 10 - w_toolStripIcon.Width - m_iCustomizedButtonWidth;
		if (Node != null)
		{
			int num = w_toolStripButtonURL.Size.Width - width;
			if (num > 0)
			{
				while (num > -15 && w_toolStripButtonURL.Text.Length > 0)
				{
					w_toolStripButtonURL.Text = w_toolStripButtonURL.Text.Substring(0, w_toolStripButtonURL.Text.Length - 1);
					num = w_toolStripButtonURL.Size.Width - width;
				}
				w_toolStripButtonURL.Text += "...";
			}
			else if (w_toolStripButtonURL.Text != Node.LinkableUrl)
			{
				if (!string.IsNullOrEmpty(Node.LinkableUrl))
				{
					w_toolStripButtonURL.Text = Node.LinkableUrl;
				}
				else
				{
					w_toolStripButtonURL.Text = Node.DisplayUrl;
				}
				num = w_toolStripButtonURL.Size.Width - width;
				if (num > 0)
				{
					while (num > -15 && w_toolStripButtonURL.Text.Length > 0)
					{
						w_toolStripButtonURL.Text = w_toolStripButtonURL.Text.Substring(0, w_toolStripButtonURL.Text.Length - 1);
						num = w_toolStripButtonURL.Size.Width - width;
					}
					w_toolStripButtonURL.Text += "...";
				}
			}
		}
		ResumeLayout(performLayout: false);
		PerformLayout();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void Fire_SelectedNodeChanged()
	{
		if (this.SelectedNodeChanged != null)
		{
			this.SelectedNodeChanged(Node);
		}
	}

        private void Initialize()
	{
		InitializeComponent();
		base.BorderStyle = BorderStyle.None;
		w_toolStripLocation.Renderer = new DisableLineRenderer();
		m_locLink_TextChanged = On_toolStripButtonURL_TextChanged;
		w_toolStripButtonURL.TextChanged += m_locLink_TextChanged;
		m_selectedStatusHandler = On_SelectedItemStatusChanged;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Widgets.LocationBar));
		this.w_toolStripLocation = new System.Windows.Forms.ToolStrip();
		this.w_lblLoc = new System.Windows.Forms.ToolStripLabel();
		this.w_toolStripIcon = new System.Windows.Forms.ToolStripButton();
		this.w_toolStripButtonURL = new System.Windows.Forms.ToolStripLabel();
		this.w_contextMenuURL = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.w_toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.w_toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.w_contextMenuIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.w_copyRootURLToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.w_openRootURLInNewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.w_toolStripLocation.SuspendLayout();
		this.w_contextMenuURL.SuspendLayout();
		this.w_contextMenuIcon.SuspendLayout();
		base.SuspendLayout();
		this.w_toolStripLocation.BackColor = System.Drawing.Color.White;
		componentResourceManager.ApplyResources(this.w_toolStripLocation, "w_toolStripLocation");
		this.w_toolStripLocation.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.w_toolStripLocation.Items;
		System.Windows.Forms.ToolStripItem[] wLblLoc = new System.Windows.Forms.ToolStripItem[3] { this.w_lblLoc, this.w_toolStripIcon, this.w_toolStripButtonURL };
		items.AddRange(wLblLoc);
		this.w_toolStripLocation.Name = "w_toolStripLocation";
		this.w_toolStripLocation.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
		this.w_toolStripLocation.MouseUp += new System.Windows.Forms.MouseEventHandler(On_MouseUp);
		this.w_toolStripLocation.Resize += new System.EventHandler(On_LocationBar_Resized);
		this.w_lblLoc.BackColor = System.Drawing.Color.Transparent;
		this.w_lblLoc.Name = "w_lblLoc";
		this.w_lblLoc.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		componentResourceManager.ApplyResources(this.w_lblLoc, "w_lblLoc");
		this.w_lblLoc.MouseUp += new System.Windows.Forms.MouseEventHandler(On_MouseUp);
		this.w_toolStripIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		componentResourceManager.ApplyResources(this.w_toolStripIcon, "w_toolStripIcon");
		this.w_toolStripIcon.Name = "w_toolStripIcon";
		this.w_toolStripIcon.Click += new System.EventHandler(w_toolStripIcon_Click);
		this.w_toolStripIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(On_IconMouseUp);
		this.w_toolStripButtonURL.BackColor = System.Drawing.Color.Transparent;
		this.w_toolStripButtonURL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
		componentResourceManager.ApplyResources(this.w_toolStripButtonURL, "w_toolStripButtonURL");
		this.w_toolStripButtonURL.Name = "w_toolStripButtonURL";
		this.w_toolStripButtonURL.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
		this.w_toolStripButtonURL.Click += new System.EventHandler(On_toolStripMenuItemOpen_Click);
		this.w_toolStripButtonURL.MouseUp += new System.Windows.Forms.MouseEventHandler(On_MouseUp);
		System.Windows.Forms.ToolStripItemCollection toolStripItemCollections = this.w_contextMenuURL.Items;
		System.Windows.Forms.ToolStripItem[] wToolStripMenuItemCopy = new System.Windows.Forms.ToolStripItem[2] { this.w_toolStripMenuItemCopy, this.w_toolStripMenuItemOpen };
		toolStripItemCollections.AddRange(wToolStripMenuItemCopy);
		this.w_contextMenuURL.Name = "w_contextMenuURL";
		this.w_contextMenuURL.ShowImageMargin = false;
		componentResourceManager.ApplyResources(this.w_contextMenuURL, "w_contextMenuURL");
		this.w_toolStripMenuItemCopy.Name = "w_toolStripMenuItemCopy";
		componentResourceManager.ApplyResources(this.w_toolStripMenuItemCopy, "w_toolStripMenuItemCopy");
		this.w_toolStripMenuItemCopy.Click += new System.EventHandler(On_toolStripMenuItemCopy_Click);
		this.w_toolStripMenuItemOpen.Name = "w_toolStripMenuItemOpen";
		componentResourceManager.ApplyResources(this.w_toolStripMenuItemOpen, "w_toolStripMenuItemOpen");
		this.w_toolStripMenuItemOpen.Click += new System.EventHandler(On_toolStripMenuItemOpen_Click);
		System.Windows.Forms.ToolStripItemCollection items1 = this.w_contextMenuIcon.Items;
		System.Windows.Forms.ToolStripItem[] wCopyRootURLToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripItem[2] { this.w_copyRootURLToClipboardToolStripMenuItem, this.w_openRootURLInNewWindowToolStripMenuItem };
		items1.AddRange(wCopyRootURLToClipboardToolStripMenuItem);
		this.w_contextMenuIcon.Name = "w_contextMenuIcon";
		componentResourceManager.ApplyResources(this.w_contextMenuIcon, "w_contextMenuIcon");
		this.w_copyRootURLToClipboardToolStripMenuItem.Name = "w_copyRootURLToClipboardToolStripMenuItem";
		componentResourceManager.ApplyResources(this.w_copyRootURLToClipboardToolStripMenuItem, "w_copyRootURLToClipboardToolStripMenuItem");
		this.w_copyRootURLToClipboardToolStripMenuItem.Click += new System.EventHandler(On_copyRootURLToClipboardToolStripMenuItem_Click);
		this.w_openRootURLInNewWindowToolStripMenuItem.Name = "w_openRootURLInNewWindowToolStripMenuItem";
		componentResourceManager.ApplyResources(this.w_openRootURLInNewWindowToolStripMenuItem, "w_openRootURLInNewWindowToolStripMenuItem");
		this.w_openRootURLInNewWindowToolStripMenuItem.Click += new System.EventHandler(On_openRootURLInNewWindowToolStripMenuItem_Click);
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.w_toolStripLocation);
		base.Name = "LocationBar";
		base.Load += new System.EventHandler(On_Load);
		this.w_toolStripLocation.ResumeLayout(false);
		this.w_toolStripLocation.PerformLayout();
		this.w_contextMenuURL.ResumeLayout(false);
		this.w_contextMenuIcon.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void On_copyRootURLToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Node rootNode = RootNode;
		if (rootNode != null)
		{
			Clipboard.SetData(DataFormats.Text, (rootNode.LinkableUrl != null) ? rootNode.LinkableUrl : rootNode.DisplayUrl);
		}
	}

        private void On_IconMouseUp(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		w_contextMenuIcon.Items.Clear();
		if (Node == null)
		{
			return;
		}
		Node rootNode = RootNode;
		if (rootNode != null)
		{
			w_contextMenuIcon.Items.Add(w_copyRootURLToClipboardToolStripMenuItem);
			if (!string.IsNullOrEmpty(rootNode.LinkableUrl))
			{
				w_contextMenuIcon.Items.Add(w_openRootURLInNewWindowToolStripMenuItem);
			}
			Rectangle bounds = ((sender is ToolStripItem) ? ((ToolStripItem)sender).Bounds : ((!typeof(Control).IsAssignableFrom(sender.GetType())) ? base.Bounds : ((Control)sender).Bounds));
			w_contextMenuIcon.Show(this, bounds.X + e.X, bounds.Y + e.Y);
		}
	}

        private void On_Load(object sender, EventArgs e)
	{
		if (base.ParentForm != null)
		{
			base.ParentForm.FormClosed += On_Parent_Closed;
		}
	}

        private void On_LocationBar_Resized(object sender, EventArgs e)
	{
		UpdateUI();
	}

        private void On_MouseUp(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		w_contextMenuURL.Items.Clear();
		if (Node != null)
		{
			w_contextMenuURL.Items.Add(w_toolStripMenuItemCopy);
			if (w_toolStripButtonURL.IsLink)
			{
				w_contextMenuURL.Items.Add(w_toolStripMenuItemOpen);
			}
			Rectangle bounds = ((sender is ToolStripItem) ? ((ToolStripItem)sender).Bounds : ((!typeof(Control).IsAssignableFrom(sender.GetType())) ? base.Bounds : ((Control)sender).Bounds));
			w_contextMenuURL.Show(this, bounds.X + e.X, bounds.Y + e.Y);
		}
	}

        private void On_openRootURLInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Node rootNode = RootNode;
		if (rootNode != null && !string.IsNullOrEmpty(rootNode.LinkableUrl))
		{
			Process process = new Process();
			process.StartInfo.FileName = rootNode.LinkableUrl;
			process.StartInfo.UseShellExecute = true;
			process.Start();
		}
	}

        private void On_Parent_Closed(object sender, FormClosedEventArgs e)
	{
		if (m_node != null)
		{
			m_node.StatusChanged -= m_selectedStatusHandler;
		}
	}

        private void On_SelectedItemStatusChanged()
	{
		if (Node != null)
		{
			w_toolStripButtonURL.Text = ((Node.LinkableUrl != null) ? Node.LinkableUrl : Node.DisplayUrl);
		}
	}

        private void On_toolStripButtonURL_TextChanged(object sender, EventArgs e)
	{
		UpdateUI();
	}

        private void On_toolStripMenuItemCopy_Click(object sender, EventArgs e)
	{
		string str = null;
		Clipboard.SetData(data: (Node != null) ? (string.IsNullOrEmpty(Node.LinkableUrl) ? Node.DisplayUrl : Node.LinkableUrl) : "", format: DataFormats.Text);
	}

        private void On_toolStripMenuItemOpen_Click(object sender, EventArgs e)
	{
		if (!w_toolStripButtonURL.IsLink)
		{
			return;
		}
		try
		{
			if (Node != null && !string.IsNullOrEmpty(Node.LinkableUrl))
			{
				Process process = new Process();
				process.StartInfo.FileName = Node.LinkableUrl;
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
		}
		catch (Exception exception)
		{
			FlatXtraMessageBox.Show("Error: \n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

        public void RemoveToolSTripItem(ToolStripItem item)
	{
		m_iCustomizedButtonWidth -= item.Width;
		w_toolStripLocation.Items.Remove(item);
	}

        private void UpdateUI()
	{
		try
		{
			if (!base.InvokeRequired)
			{
				w_toolStripButtonURL.TextChanged -= m_locLink_TextChanged;
				w_lblLoc.Text = m_sDescriptor;
				if (Node == null)
				{
					w_toolStripIcon.Visible = false;
					w_toolStripButtonURL.Text = "";
				}
				else
				{
					Node rootNode = RootNode;
					if (rootNode == null)
					{
						w_toolStripIcon.Image = null;
						w_toolStripIcon.Visible = false;
					}
					else
					{
						w_toolStripIcon.Image = rootNode.Image;
						w_toolStripIcon.Visible = true;
						if (string.IsNullOrEmpty(rootNode.LinkableUrl))
						{
							w_toolStripIcon.ToolTipText = "To navigate to a database connection in a browser, you must specify a host name. This can be set in the node's properties";
						}
						else
						{
							w_toolStripIcon.ToolTipText = "Navigate to Root URL in new window: " + rootNode.LinkableUrl;
						}
					}
					w_toolStripButtonURL.Text = ((Node.LinkableUrl != null) ? Node.LinkableUrl : Node.DisplayUrl);
					if (Node == null || string.IsNullOrEmpty(w_toolStripButtonURL.Text))
					{
						w_toolStripButtonURL.Visible = false;
					}
					else
					{
						w_toolStripButtonURL.Visible = true;
						if (!string.IsNullOrEmpty(m_node.LinkableUrl))
						{
							w_toolStripButtonURL.ToolTipText = "Navigate to URL in new window: " + w_toolStripButtonURL.Text;
							w_toolStripButtonURL.IsLink = true;
						}
						else
						{
							w_toolStripButtonURL.ToolTipText = "To navigate to a database connection in a browser, you must specify a host name. This can be set in the node's properties";
							w_toolStripButtonURL.IsLink = false;
						}
						AdjustLink();
					}
				}
				w_toolStripButtonURL.TextChanged += m_locLink_TextChanged;
			}
			else
			{
				Invoke(new UpdateUIDelegate(UpdateUI));
			}
		}
		catch (Exception)
		{
		}
	}

        private void w_toolStripIcon_Click(object sender, EventArgs e)
	{
		if (Node == null)
		{
			return;
		}
		Node rootNode = RootNode;
		try
		{
			if (rootNode != null && !string.IsNullOrEmpty(rootNode.LinkableUrl))
			{
				Process process = new Process();
				process.StartInfo.FileName = rootNode.LinkableUrl;
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			FlatXtraMessageBox.Show("Error: \n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}
    }
}
