using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Explorer;

namespace Metalogix.UI.WinForms.Widgets
{
    public class NodePropertyGrid : UserControl
    {
        private delegate void CollectionChangedDelegate(NodeCollectionChangeType changeType, Node changedNode);

        private delegate void UpdateSelectedObjectsDelegate();

        private NodeCollection m_DataSource;

        private NodeCollectionChangedHandler m_HandlerCollectionChanged;

        private List<Node> m_PropertyGridObjects = new List<Node>();

        private IContainer components;

        private PropertyGrid w_propertyGrid;

        public NodeCollection DataSource
        {
            get
		{
			return m_DataSource;
		}
            set
		{
			if (m_DataSource != null)
			{
				m_DataSource.OnNodeCollectionChanged -= m_HandlerCollectionChanged;
				m_HandlerCollectionChanged = null;
			}
			m_DataSource = value;
			if (m_DataSource != null)
			{
				m_HandlerCollectionChanged = On_DataSource_CollectionChanged;
				m_DataSource.OnNodeCollectionChanged += m_HandlerCollectionChanged;
			}
			UpdateUI();
		}
        }

        public NodePropertyGrid()
	{
		InitializeComponent();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Widgets.NodePropertyGrid));
		this.w_propertyGrid = new System.Windows.Forms.PropertyGrid();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_propertyGrid, "w_propertyGrid");
		this.w_propertyGrid.Name = "w_propertyGrid";
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_propertyGrid);
		base.Name = "NodePropertyGrid";
		base.ResumeLayout(false);
	}

        private void On_DataSource_CollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
	{
		if (base.InvokeRequired)
		{
			Delegate collectionChangedDelegate = new CollectionChangedDelegate(On_DataSource_CollectionChanged);
			object[] objArray = new object[2] { changeType, changedNode };
			Invoke(collectionChangedDelegate, objArray);
			return;
		}
		switch (changeType)
		{
		case NodeCollectionChangeType.FullReset:
			UpdateUI();
			break;
		case NodeCollectionChangeType.NodeAdded:
			UpdateUIAddItem(changedNode);
			break;
		case NodeCollectionChangeType.NodeRemoved:
			UpdateUIDeleteItem(changedNode);
			break;
		case NodeCollectionChangeType.NodeChanged:
			UpdateUIRefreshItem(changedNode);
			break;
		}
	}

        private void UpdateSelectedObjects()
	{
		if (w_propertyGrid.InvokeRequired)
		{
			Delegate updateSelectedObjectsDelegate = new UpdateSelectedObjectsDelegate(UpdateSelectedObjects);
			w_propertyGrid.Invoke(updateSelectedObjectsDelegate);
			return;
		}
		lock (m_PropertyGridObjects)
		{
			if (m_PropertyGridObjects.ToArray().Length == 0)
			{
				w_propertyGrid.SelectedObjects = null;
			}
		}
		try
		{
			w_propertyGrid.SelectedObjects = m_PropertyGridObjects.ToArray();
		}
		catch (Exception)
		{
		}
	}

        private void UpdateUI()
	{
		m_PropertyGridObjects.Clear();
		if (DataSource != null)
		{
			foreach (Node dataSource in DataSource)
			{
				m_PropertyGridObjects.Add(dataSource);
			}
		}
		UpdateSelectedObjects();
	}

        private void UpdateUIAddItem(Node node)
	{
		m_PropertyGridObjects.Add(node);
		UpdateSelectedObjects();
	}

        private void UpdateUIDeleteItem(Node deletedNode)
	{
		m_PropertyGridObjects.Remove(deletedNode);
		UpdateSelectedObjects();
	}

        private void UpdateUIRefreshItem(Node updatedNode)
	{
		UpdateSelectedObjects();
	}
    }
}
