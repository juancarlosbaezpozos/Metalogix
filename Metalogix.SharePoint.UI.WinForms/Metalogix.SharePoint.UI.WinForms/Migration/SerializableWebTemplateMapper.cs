using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class SerializableWebTemplateMapper : SerializableTableMapper
	{
		private SPWebTemplateCollection m_sourceTemplateCollection;

		private SPWebTemplateCollection m_targetTemplateCollection;

		private IContainer components;

		public SPWebTemplateCollection SourceTemplateCollection
		{
			get
			{
				return m_sourceTemplateCollection;
			}
			set
			{
				m_sourceTemplateCollection = value;
				CommonSerializableList<object> commonSerializableList = new CommonSerializableList<object>();
				if (value != null)
				{
					foreach (SPWebTemplate item in value)
					{
						if (!item.IsRootWebOnly && item.ID != -1)
						{
							commonSerializableList.Add(item);
						}
					}
				}
				base.MappingSource = commonSerializableList;
			}
		}

		public SPWebTemplateCollection TargetTemplateCollection
		{
			get
			{
				return m_targetTemplateCollection;
			}
			set
			{
				m_targetTemplateCollection = value;
				CommonSerializableList<object> commonSerializableList = new CommonSerializableList<object>();
				if (value != null)
				{
					foreach (SPWebTemplate item in value)
					{
						if (!item.IsHidden && !item.IsRootWebOnly)
						{
							commonSerializableList.Add(item);
						}
					}
				}
				base.MappingTarget = commonSerializableList;
			}
		}

		public SerializableWebTemplateMapper()
		{
			InitializeComponent();
		}

		protected override void BuildMapToMenu(ToolStripMenuItem mnuItem, ListView.SelectedListViewItemCollection collection)
		{
			SPWebTemplate sPWebTemplate = null;
			if (collection.Count == 1 && collection[0] is MappingListViewItem)
			{
				sPWebTemplate = ((MappingListViewItem)collection[0]).Source as SPWebTemplate;
				if (sPWebTemplate != null && !sPWebTemplate.IsHidden)
				{
					sPWebTemplate = null;
				}
			}
			if (sPWebTemplate != null && TargetTemplateCollection != null)
			{
				SPWebTemplate sPWebTemplate2 = TargetTemplateCollection.MapWebTemplate(sPWebTemplate);
				if (sPWebTemplate2 != null && sPWebTemplate2.IsHidden)
				{
					mnuItem.DropDownItems.Add(CreateNewToolStripMenuItem(sPWebTemplate2));
				}
			}
			base.BuildMapToMenu(mnuItem, collection);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.SerializableWebTemplateMapper));
			base.SuspendLayout();
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "SerializableWebTemplateMapper";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
