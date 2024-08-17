using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;

namespace Metalogix.UI.WinForms.Components
{
    public class SerializableTableMapper : XtraForm
    {
        private SerializableList<object> m_mappingSource;

        private SerializableList<object> m_mappingTarget;

        private SerializableTable<object, object> m_mappings;

        private int m_iRequiredHeight;

        private IContainer components;

        private Label w_lblInstructions;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOK;

        private ListView w_lvMapping;

        private ColumnHeader w_sourceColumnHeader;

        private ColumnHeader w_targetColumnHeader;

        private ContextMenuStrip w_ctxMapping;

        private ToolStripMenuItem abcToolStripMenuItem;

        private ToolStripMenuItem defToolStripMenuItem;

        public SerializableTable<object, object> Mappings
        {
            get
		{
			return m_mappings;
		}
            set
		{
			m_mappings = value;
			UpdateUI();
		}
        }

        public SerializableList<object> MappingSource
        {
            get
		{
			return m_mappingSource;
		}
            set
		{
			m_mappingSource = value;
			UpdateUI();
		}
        }

        public SerializableList<object> MappingTarget
        {
            get
		{
			return m_mappingTarget;
		}
            set
		{
			m_mappingTarget = value;
			UpdateUI();
		}
        }

        public SerializableTableMapper()
	{
		InitializeComponent();
	}

        protected void ApplyBasicModeSkin()
	{
		w_btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
		w_btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
		w_btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
		w_btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
	}

        protected virtual void BuildMapToMenu(ToolStripMenuItem mnuItem, ListView.SelectedListViewItemCollection collection)
	{
		foreach (object mappingTarget in MappingTarget)
		{
			ToolStripMenuItem toolStripMenuItem = CreateNewToolStripMenuItem(mappingTarget);
			mnuItem.DropDownItems.Add(toolStripMenuItem);
		}
	}

        protected ToolStripMenuItem CreateNewToolStripMenuItem(object oTarget)
	{
		return new ToolStripMenuItem(oTarget.ToString(), null, On_contextMenuItemClick)
		{
			Tag = oTarget
		};
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Components.SerializableTableMapper));
		this.w_lblInstructions = new System.Windows.Forms.Label();
		this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
		this.w_lvMapping = new System.Windows.Forms.ListView();
		this.w_sourceColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.w_targetColumnHeader = new System.Windows.Forms.ColumnHeader();
		this.w_ctxMapping = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.abcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.defToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.w_ctxMapping.SuspendLayout();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_lblInstructions, "w_lblInstructions");
		this.w_lblInstructions.Name = "w_lblInstructions";
		componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
		this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.w_btnCancel.Name = "w_btnCancel";
		componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
		this.w_btnOK.Name = "w_btnOK";
		this.w_btnOK.Click += new System.EventHandler(On_Ok_Clicked);
		componentResourceManager.ApplyResources(this.w_lvMapping, "w_lvMapping");
		System.Windows.Forms.ListView.ColumnHeaderCollection columns = this.w_lvMapping.Columns;
		System.Windows.Forms.ColumnHeader[] wSourceColumnHeader = new System.Windows.Forms.ColumnHeader[2] { this.w_sourceColumnHeader, this.w_targetColumnHeader };
		columns.AddRange(wSourceColumnHeader);
		this.w_lvMapping.ContextMenuStrip = this.w_ctxMapping;
		this.w_lvMapping.FullRowSelect = true;
		this.w_lvMapping.Name = "w_lvMapping";
		this.w_lvMapping.UseCompatibleStateImageBehavior = false;
		this.w_lvMapping.View = System.Windows.Forms.View.Details;
		this.w_lvMapping.Resize += new System.EventHandler(On_Resize);
		componentResourceManager.ApplyResources(this.w_sourceColumnHeader, "w_sourceColumnHeader");
		componentResourceManager.ApplyResources(this.w_targetColumnHeader, "w_targetColumnHeader");
		this.w_ctxMapping.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.abcToolStripMenuItem });
		this.w_ctxMapping.Name = "w_ctxMapping";
		componentResourceManager.ApplyResources(this.w_ctxMapping, "w_ctxMapping");
		this.w_ctxMapping.Opening += new System.ComponentModel.CancelEventHandler(On_contextMenuRightClick_Opening);
		this.abcToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.defToolStripMenuItem });
		this.abcToolStripMenuItem.Name = "abcToolStripMenuItem";
		componentResourceManager.ApplyResources(this.abcToolStripMenuItem, "abcToolStripMenuItem");
		this.defToolStripMenuItem.Name = "defToolStripMenuItem";
		componentResourceManager.ApplyResources(this.defToolStripMenuItem, "defToolStripMenuItem");
		base.AcceptButton = this.w_btnOK;
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.w_btnCancel;
		base.Controls.Add(this.w_lvMapping);
		base.Controls.Add(this.w_btnCancel);
		base.Controls.Add(this.w_btnOK);
		base.Controls.Add(this.w_lblInstructions);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "SerializableTableMapper";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.w_ctxMapping.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private void On_contextMenuItemClick(object sender, EventArgs e)
	{
		ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
		foreach (int selectedIndex in w_lvMapping.SelectedIndices)
		{
			if (w_lvMapping.Items[selectedIndex] is MappingListViewItem item)
			{
				item.Target = toolStripMenuItem.Tag;
			}
		}
	}

        private void On_contextMenuRightClick_Opening(object sender, CancelEventArgs e)
	{
		if (w_lvMapping.SelectedIndices.Count < 1)
		{
			e.Cancel = true;
			return;
		}
		w_ctxMapping.Items.Clear();
		ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Map to");
		w_ctxMapping.Items.Add(toolStripMenuItem);
		BuildMapToMenu(toolStripMenuItem, w_lvMapping.SelectedItems);
	}

        protected virtual void On_Ok_Clicked(object sender, EventArgs e)
	{
		SaveUI();
		base.DialogResult = DialogResult.OK;
		Close();
	}

        private void On_Resize(object sender, EventArgs e)
	{
		int width = (w_lvMapping.Width - 4) / 2;
		if (m_iRequiredHeight > w_lvMapping.Height)
		{
			width -= 8;
		}
		w_sourceColumnHeader.Width = width;
		w_targetColumnHeader.Width = width;
	}

        protected void SaveUI()
	{
		Mappings.Clear();
		foreach (MappingListViewItem item in w_lvMapping.Items)
		{
			Mappings.Add(item.Source, item.Target);
		}
	}

        private void UpdateColumnWidths()
	{
		int width = (w_lvMapping.Width - 4) / 2;
		w_sourceColumnHeader.Width = width;
		w_targetColumnHeader.Width = width;
	}

        private void UpdateUI()
	{
		if (MappingSource == null || MappingSource.Count == 0 || MappingTarget == null || MappingTarget.Count == 0)
		{
			return;
		}
		string name = MappingSource.GetType().Name;
		object[] customAttributes = MappingSource.CollectionType.GetCustomAttributes(typeof(NameAttribute), inherit: true);
		if (customAttributes.Length != 0)
		{
			name = ((NameAttribute)customAttributes[0]).Name;
		}
		w_sourceColumnHeader.Text = "Source " + name;
		string str = MappingTarget.GetType().Name;
		customAttributes = MappingTarget.CollectionType.GetCustomAttributes(typeof(NameAttribute), inherit: true);
		if (customAttributes.Length != 0)
		{
			str = ((NameAttribute)customAttributes[0]).Name;
		}
		w_targetColumnHeader.Text = "Target " + str;
		if (name != str)
		{
			Text = "Map " + name + " to " + str;
		}
		else
		{
			string pluralName = MappingSource.GetType().Name + "s";
			customAttributes = MappingTarget.CollectionType.GetCustomAttributes(typeof(PluralNameAttribute), inherit: true);
			if (customAttributes.Length != 0)
			{
				pluralName = ((PluralNameAttribute)customAttributes[0]).PluralName;
			}
			Text = "Map " + pluralName;
		}
		if (Mappings == null)
		{
			foreach (object mappingSource in MappingSource)
			{
				w_lvMapping.Items.Add(new MappingListViewItem(mappingSource, MappingTarget[0]));
			}
		}
		else
		{
			foreach (object obj in MappingSource)
			{
				object obj1 = (Mappings.ContainsKey(obj) ? Mappings[obj] : MappingTarget[0]);
				w_lvMapping.Items.Add(new MappingListViewItem(obj, obj1));
			}
		}
		m_iRequiredHeight = w_lvMapping.Items.Count * 15;
		UpdateColumnWidths();
	}
    }
}
