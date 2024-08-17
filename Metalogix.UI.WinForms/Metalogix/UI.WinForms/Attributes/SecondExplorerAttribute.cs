using System;

namespace Metalogix.UI.WinForms.Attributes
{
	public class SecondExplorerAttribute : UIAttribute
	{
		private readonly bool m_bExplorerEnabled;

		public bool ExplorerEnabled
		{
			get
			{
				return this.m_bExplorerEnabled;
			}
		}

		public SecondExplorerAttribute(bool bExplorerEnabled)
		{
			this.m_bExplorerEnabled = bExplorerEnabled;
		}
	}
}