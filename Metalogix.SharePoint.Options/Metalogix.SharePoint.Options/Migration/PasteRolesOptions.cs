using Metalogix.Actions;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteRolesOptions : SharePointActionOptions
	{
		private bool m_bRecursivelyCopyPermissionLevels;

		private bool m_bCopyRootPermissionLevels;

		public bool CopyRootPermissions
		{
			get
			{
				return this.m_bCopyRootPermissionLevels;
			}
			set
			{
				this.m_bCopyRootPermissionLevels = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool PersistMappings
		{
			get
			{
				return base.PersistMappings;
			}
			set
			{
				base.PersistMappings = value;
			}
		}

		[CmdletEnabledParameter(true)]
		public bool RecursivelyCopyPermissionLevels
		{
			get
			{
				return this.m_bRecursivelyCopyPermissionLevels;
			}
			set
			{
				this.m_bRecursivelyCopyPermissionLevels = value;
			}
		}

		public PasteRolesOptions()
		{
		}
	}
}