using Metalogix.Data;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data
{
	public class ConditionalMappingListViewItem : ListViewItem
	{
		private ConditionalMapping m_mapping;

		public ConditionalMapping Mapping
		{
			get
			{
				return this.m_mapping;
			}
			set
			{
				this.m_mapping.PropertiesChanged -= new PropertyChangedEventHandler(this.m_mapping_PropertiesChanged);
			}
		}

		public ConditionalMappingListViewItem(ConditionalMapping mapping) : base(mapping.ToString())
		{
			this.m_mapping = mapping;
			this.m_mapping.PropertiesChanged += new PropertyChangedEventHandler(this.m_mapping_PropertiesChanged);
		}

		private void m_mapping_PropertiesChanged(object sender, PropertyChangedEventArgs e)
		{
			base.Text = this.Mapping.ToString();
		}
	}
}