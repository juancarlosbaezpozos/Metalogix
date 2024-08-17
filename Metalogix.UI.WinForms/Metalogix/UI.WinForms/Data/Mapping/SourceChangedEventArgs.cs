using System;

namespace Metalogix.UI.WinForms.Data.Mapping
{
	public class SourceChangedEventArgs : EventArgs
	{
		private object m_tag;

		public object Tag
		{
			get
			{
				return this.m_tag;
			}
		}

		public SourceChangedEventArgs(object item)
		{
			this.m_tag = item;
		}
	}
}