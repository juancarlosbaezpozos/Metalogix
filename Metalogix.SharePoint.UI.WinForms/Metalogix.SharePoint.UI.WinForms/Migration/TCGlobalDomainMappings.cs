using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Data.Mapping;
using Metalogix.SharePoint.UI.WinForms.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Mapping;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.DomainMapping32.ico")]
	[ControlName("Domain Mappings")]
	public class TCGlobalDomainMappings : ScopableTabbableControl
	{
		private MappingsCollection m_localMappings;

		private IContainer components;

		public ListSummaryControl domainMappingSummaryControl;

		private ToolStrip w_toolStripPicker;

		private ToolStripButton w_toolStripButtonNew;

		private ToolStripButton w_toolStripButtonUnmap;

		private ToolStripButton w_toolStripButtonClear;

		public MappingsCollection LocalMappings
		{
			get
			{
				m_localMappings = new MappingsCollection();
				ListSummaryItem[] items = domainMappingSummaryControl.Items;
				foreach (ListSummaryItem item in items)
				{
					m_localMappings.Add(item);
				}
				return m_localMappings;
			}
		}

		public TCGlobalDomainMappings()
		{
			InitializeComponent();
			Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Initialize()
		{
			domainMappingSummaryControl.ShowGroups = false;
			foreach (ListSummaryItem globalDomainMapping in SPGlobalMappings.GlobalDomainMappings)
			{
				bool flag = false;
				ListSummaryItem[] items = domainMappingSummaryControl.Items;
				foreach (ListSummaryItem listSummaryItem2 in items)
				{
					if (!(globalDomainMapping.Source.Target != listSummaryItem2.Source.Target))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					domainMappingSummaryControl.Add(globalDomainMapping);
				}
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGlobalDomainMappings));
			this.domainMappingSummaryControl = new Metalogix.UI.WinForms.Data.Mapping.ListSummaryControl();
			this.w_toolStripPicker = new System.Windows.Forms.ToolStrip();
			this.w_toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
			this.w_toolStripButtonUnmap = new System.Windows.Forms.ToolStripButton();
			this.w_toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
			this.w_toolStripPicker.SuspendLayout();
			base.SuspendLayout();
			resources.ApplyResources(this.domainMappingSummaryControl, "domainMappingSummaryControl");
			this.domainMappingSummaryControl.BackColor = System.Drawing.Color.White;
			this.domainMappingSummaryControl.Items = new Metalogix.Data.Mapping.ListSummaryItem[0];
			this.domainMappingSummaryControl.MultiSelect = false;
			this.domainMappingSummaryControl.Name = "domainMappingSummaryControl";
			this.domainMappingSummaryControl.ShowBottomToolStrip = false;
			this.domainMappingSummaryControl.ShowGroups = true;
			resources.ApplyResources(this.w_toolStripPicker, "w_toolStripPicker");
			this.w_toolStripPicker.BackColor = System.Drawing.Color.White;
			this.w_toolStripPicker.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			System.Windows.Forms.ToolStripItemCollection items = this.w_toolStripPicker.Items;
			System.Windows.Forms.ToolStripItem[] toolStripItems = new System.Windows.Forms.ToolStripItem[3] { this.w_toolStripButtonNew, this.w_toolStripButtonUnmap, this.w_toolStripButtonClear };
			items.AddRange(toolStripItems);
			this.w_toolStripPicker.Name = "w_toolStripPicker";
			this.w_toolStripPicker.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.w_toolStripButtonNew.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Map16;
			resources.ApplyResources(this.w_toolStripButtonNew, "w_toolStripButtonNew");
			this.w_toolStripButtonNew.Margin = new System.Windows.Forms.Padding(0, 8, 0, 2);
			this.w_toolStripButtonNew.Name = "w_toolStripButtonNew";
			this.w_toolStripButtonNew.Click += new System.EventHandler(w_toolStripButtonNew_Click);
			this.w_toolStripButtonUnmap.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.MappingOptions;
			resources.ApplyResources(this.w_toolStripButtonUnmap, "w_toolStripButtonUnmap");
			this.w_toolStripButtonUnmap.Name = "w_toolStripButtonUnmap";
			this.w_toolStripButtonUnmap.Click += new System.EventHandler(w_toolStripButtonUnmap_Click);
			this.w_toolStripButtonClear.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Delete16;
			resources.ApplyResources(this.w_toolStripButtonClear, "w_toolStripButtonClear");
			this.w_toolStripButtonClear.Name = "w_toolStripButtonClear";
			this.w_toolStripButtonClear.Click += new System.EventHandler(w_toolStripButtonClear_Click);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_toolStripPicker);
			base.Controls.Add(this.domainMappingSummaryControl);
			base.Name = "TCGlobalDomainMappings";
			this.w_toolStripPicker.ResumeLayout(false);
			this.w_toolStripPicker.PerformLayout();
			base.ResumeLayout(false);
		}

		private void w_toolStripButtonClear_Click(object sender, EventArgs e)
		{
			if (FlatXtraMessageBox.Show("Are you sure you want to clear all mappings?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				ListSummaryItem[] items = domainMappingSummaryControl.Items;
				foreach (ListSummaryItem item in items)
				{
					domainMappingSummaryControl.Remove(item);
				}
			}
		}

		private void w_toolStripButtonNew_Click(object sender, EventArgs e)
		{
			CreateDomainMappingDialog createDomainMappingDialog = new CreateDomainMappingDialog();
			createDomainMappingDialog.ShowDialog();
			if (createDomainMappingDialog.DialogResult != DialogResult.OK)
			{
				return;
			}
			ListPickerItem listPickerItem = new ListPickerItem();
			ListPickerItem listPickerItem2 = new ListPickerItem();
			ListSummaryItem listSummaryItem = new ListSummaryItem();
			listPickerItem.Target = createDomainMappingDialog.SourceDomain;
			listPickerItem.Tag = createDomainMappingDialog.SourceDomain;
			listPickerItem.TargetType = "string";
			listPickerItem2.Target = createDomainMappingDialog.TargetDomain;
			listPickerItem2.Tag = createDomainMappingDialog.TargetDomain;
			listPickerItem2.TargetType = "string";
			listSummaryItem.Source = listPickerItem;
			listSummaryItem.Target = listPickerItem2;
			bool flag = false;
			ListSummaryItem[] items = domainMappingSummaryControl.Items;
			foreach (ListSummaryItem listSummaryItem2 in items)
			{
				if (!(listSummaryItem.Source.Target != listSummaryItem2.Source.Target))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				FlatXtraMessageBox.Show("A mapping with this source already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				domainMappingSummaryControl.Add(listSummaryItem);
			}
		}

		private void w_toolStripButtonUnmap_Click(object sender, EventArgs e)
		{
			ListSummaryItem[] selectedItems = domainMappingSummaryControl.GetSelectedItems();
			if (selectedItems.Length >= 1)
			{
				ListSummaryItem[] array = selectedItems;
				foreach (ListSummaryItem item in array)
				{
					domainMappingSummaryControl.Remove(item);
				}
			}
		}
	}
}
