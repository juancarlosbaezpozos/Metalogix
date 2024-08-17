using DevExpress.XtraEditors;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class SerializableTermstoreMapper : SerializableTableMapper
	{
		private const string C_UNABLE_TO_MAP = "The following Term Stores on the same server cannot be mapped to themselves:";

		private const string C_TS_TO_TS = "'{0}' to '{1}'";

		private const string C_PLEASE_SELECT_ALTERNATES = "Please select an alternate target Term Store for each of the above";

		private SPTermStoreCollection m_sourceTermstoreCollection;

		private SPTermStoreCollection m_targetTermstoreCollection;

		private IContainer components;

		public SPTermStoreCollection SourceTermstoreCollection
		{
			get
			{
				return this.m_sourceTermstoreCollection;
			}
			set
			{
				this.m_sourceTermstoreCollection = value;
				CommonSerializableList<object> commonSerializableList = new CommonSerializableList<object>();
				if (value != null)
				{
					foreach (SPTermStore sPTermStore in (IEnumerable<SPTermStore>)value)
					{
						commonSerializableList.Add(sPTermStore);
					}
				}
				if (commonSerializableList.Count == 0)
				{
					commonSerializableList.Add(new SPTermStore("<No Term Store Exists>"));
				}
				base.MappingSource = commonSerializableList;
			}
		}

		public SPTermStoreCollection TargetTermstoreCollection
		{
			get
			{
				return this.m_targetTermstoreCollection;
			}
			set
			{
				this.m_targetTermstoreCollection = value;
				CommonSerializableList<object> commonSerializableList = new CommonSerializableList<object>();
				if (value != null)
				{
					commonSerializableList.Add(new SPTermStore("<Exclude>"));
					foreach (SPTermStore sPTermStore in (IEnumerable<SPTermStore>)value)
					{
						commonSerializableList.Add(sPTermStore);
					}
				}
				base.MappingTarget = commonSerializableList;
			}
		}

		public SerializableTermstoreMapper(bool isBasicMode = false)
		{
			this.InitializeComponent();
			if (isBasicMode)
			{
				base.ApplyBasicModeSkin();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(458, 326);
			base.Name = "SerializableTermstoreMapper";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void On_Ok_Clicked(object sender, EventArgs e)
		{
			if (base.Mappings != null)
			{
				bool flag = false;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("The following Term Stores on the same server cannot be mapped to themselves:");
				stringBuilder.AppendLine(string.Empty);
				base.SaveUI();
				foreach (SPTermStore key in base.Mappings.Keys)
				{
					SPTermStore item = (SPTermStore)base.Mappings[key];
					if (!(key.Id != Guid.Empty) || !(key.Id.ToString() == item.Id.ToString()) || !string.Equals(key.Name, item.Name))
					{
						continue;
					}
					flag = true;
					stringBuilder.AppendLine(string.Format("'{0}' to '{1}'", key.Name, item.Name));
				}
				if (flag)
				{
					stringBuilder.AppendLine(string.Empty);
					stringBuilder.AppendLine("Please select an alternate target Term Store for each of the above");
					FlatXtraMessageBox.Show(stringBuilder.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			base.Close();
		}
	}
}