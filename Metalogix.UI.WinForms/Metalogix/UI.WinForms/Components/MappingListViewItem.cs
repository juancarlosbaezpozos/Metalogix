using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class MappingListViewItem : ListViewItem
	{
		private object m_oSource;

		private object m_oTarget;

		public object Source
		{
			get
			{
				return this.m_oSource;
			}
			set
			{
				this.m_oSource = value;
				this.UpdateUI();
			}
		}

		public object Target
		{
			get
			{
				return this.m_oTarget;
			}
			set
			{
				this.m_oTarget = value;
				this.UpdateUI();
			}
		}

		public MappingListViewItem(object source, object target) : base(new string[] { source.ToString(), target.ToString() })
		{
			this.Source = source;
			this.Target = target;
		}

		private void UpdateUI()
		{
			base.SubItems.Clear();
			base.Text = (this.m_oSource != null ? this.m_oSource.ToString() : "");
			base.SubItems.Add((this.m_oTarget != null ? this.m_oTarget.ToString() : ""));
		}
	}
}