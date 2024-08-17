using Metalogix.Data.Mapping;
using System;

namespace Metalogix.UI.WinForms.Data.Mapping
{
	public class ListSummaryItemEventArgs : EventArgs
	{
		private ListSummaryItem m_tag;

		public ListSummaryItem Tag
		{
			get
			{
				return this.m_tag;
			}
		}

		public ListSummaryItemEventArgs(ListSummaryItem item)
		{
			this.m_tag = item;
		}
	}
}