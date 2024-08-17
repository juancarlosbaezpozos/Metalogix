using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.ContentTypes.png")]
	[ControlName("Content Type Options")]
	public class TCContentTypeOptions : ScopableTabbableControl
	{
		private PasteContentTypesOptions m_options;

		private ConfigurationResult m_result = ConfigurationResult.Cancel;

		private IContainer components;

		private CheckBox w_cbRecursive;

		private LeftRightSelectionControl w_lrscContentTypeSelector;

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

		public PasteContentTypesOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				if (value != null)
				{
					InitializeUI();
					LoadUI();
				}
			}
		}

		public TCContentTypeOptions()
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

		private string GetContentTypeName(object obj)
		{
			return (((ListViewItem)obj).Tag as SPContentType).Name;
		}

		private CommonSerializableList<string> GetContentTypes()
		{
			ListViewItem[] selectedItems = w_lrscContentTypeSelector.GetSelectedItems<ListViewItem>();
			CommonSerializableList<string> commonSerializableList = new CommonSerializableList<string>();
			if (selectedItems.Length < 1)
			{
				return null;
			}
			ListViewItem[] array = selectedItems;
			for (int i = 0; i < array.Length; i++)
			{
				commonSerializableList.Add(((SPContentType)array[i].Tag).Name);
			}
			return commonSerializableList;
		}

		private void InitializeComponent()
		{
			this.w_cbRecursive = new System.Windows.Forms.CheckBox();
			this.w_lrscContentTypeSelector = new Metalogix.UI.WinForms.Components.LeftRightSelectionControl();
			base.SuspendLayout();
			this.w_cbRecursive.Location = new System.Drawing.Point(6, 0);
			this.w_cbRecursive.Name = "w_cbRecursive";
			this.w_cbRecursive.Size = new System.Drawing.Size(198, 24);
			this.w_cbRecursive.TabIndex = 1;
			this.w_cbRecursive.Text = "Copy Content Types Recursively";
			this.w_cbRecursive.UseVisualStyleBackColor = true;
			this.w_cbRecursive.CheckedChanged += new System.EventHandler(w_cbRecursive_CheckedChanged);
			this.w_lrscContentTypeSelector.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_lrscContentTypeSelector.DeselectedItemsLabel = "Content Types on source site";
			this.w_lrscContentTypeSelector.ItemSorting = Metalogix.UI.WinForms.Components.SortingType.None;
			this.w_lrscContentTypeSelector.Location = new System.Drawing.Point(6, 30);
			this.w_lrscContentTypeSelector.Name = "w_lrscContentTypeSelector";
			this.w_lrscContentTypeSelector.SelectedItemsLabel = "Content Types to copy";
			this.w_lrscContentTypeSelector.Size = new System.Drawing.Size(436, 379);
			this.w_lrscContentTypeSelector.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_lrscContentTypeSelector);
			base.Controls.Add(this.w_cbRecursive);
			base.Name = "TCContentTypeOptions";
			base.Size = new System.Drawing.Size(457, 416);
			base.ResumeLayout(false);
		}

		private void InitializeUI()
		{
			w_lrscContentTypeSelector.ItemNamingMethod = GetContentTypeName;
			w_lrscContentTypeSelector.ItemSortValueMethod = GetContentTypeName;
			w_lrscContentTypeSelector.ItemSorting = SortingType.Ascending;
			List<ListViewItem> list = new List<ListViewItem>();
			List<string> list2 = new List<string>();
			foreach (SPNode sourceNode in SourceNodes)
			{
				foreach (SPContentType contentType in (sourceNode as SPWeb).ContentTypes)
				{
					string name = contentType.Name;
					if (!list2.Contains(name))
					{
						list.Add(new ListViewItem(name)
						{
							Tag = contentType
						});
						list2.Add(name);
					}
				}
			}
			w_lrscContentTypeSelector.SetAvailableItems(list);
		}

		protected override void LoadUI()
		{
			w_cbRecursive.Checked = Options.Recursive;
			w_lrscContentTypeSelector.Enabled = !Options.Recursive;
			if (Options.FilteredCTCollection != null)
			{
				SetContentTypes(Options.FilteredCTCollection);
			}
		}

		public override bool SaveUI()
		{
			Options.Recursive = w_cbRecursive.Checked;
			CommonSerializableList<string> contentTypes = GetContentTypes();
			Options.FilterCTs = !Options.Recursive;
			Options.FilteredCTCollection = contentTypes;
			return true;
		}

		private void SetContentTypes(IEnumerable<string> contentTypes)
		{
			IEnumerable<ListViewItem> selectedItems = from i in w_lrscContentTypeSelector.GetAvailableItems<ListViewItem>()
				where contentTypes.Contains(((SPContentType)i.Tag).Name)
				select i;
			w_lrscContentTypeSelector.SetSelectedItems(selectedItems);
		}

		private void w_cbRecursive_CheckedChanged(object sender, EventArgs e)
		{
			w_lrscContentTypeSelector.Enabled = !w_cbRecursive.Checked;
		}
	}
}
