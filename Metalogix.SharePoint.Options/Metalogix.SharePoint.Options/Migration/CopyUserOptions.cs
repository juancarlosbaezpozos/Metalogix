using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyUserOptions : SharePointActionOptions
	{
		private SPUserCollection m_sourceUsers = new SPUserCollection();

		[CmdletParameterEnumerate(true)]
		[UsesStickySettings(false)]
		public SPUserCollection SourceUsers
		{
			get
			{
				return this.m_sourceUsers;
			}
			set
			{
				this.m_sourceUsers = value;
			}
		}

		public CopyUserOptions()
		{
		}
	}
}