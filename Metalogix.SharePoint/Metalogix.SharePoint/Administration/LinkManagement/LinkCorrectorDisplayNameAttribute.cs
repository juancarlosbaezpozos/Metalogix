using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	[AttributeUsage(AttributeTargets.Class)]
	public class LinkCorrectorDisplayNameAttribute : Attribute
	{
		private string m_sDisplayName;

		public string DisplayName
		{
			get
			{
				return this.m_sDisplayName;
			}
		}

		public LinkCorrectorDisplayNameAttribute(string sDisplayName)
		{
			this.m_sDisplayName = sDisplayName;
		}

		public override string ToString()
		{
			return this.m_sDisplayName;
		}
	}
}