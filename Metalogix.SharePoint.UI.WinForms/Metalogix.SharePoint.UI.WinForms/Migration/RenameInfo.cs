using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public struct RenameInfo
	{
		private string m_sName;

		private string m_sTitle;

		public string Name
		{
			get
			{
				return this.m_sName;
			}
			set
			{
				this.m_sName = value;
			}
		}

		public string Title
		{
			get
			{
				return this.m_sTitle;
			}
			set
			{
				this.m_sTitle = value;
			}
		}
	}
}