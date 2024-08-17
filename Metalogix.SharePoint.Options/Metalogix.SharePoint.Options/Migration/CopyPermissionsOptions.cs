using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyPermissionsOptions : SharePointActionOptions
	{
		private bool m_bCopyRootPermissions = true;

		private bool m_bMapRolesByName;

		private bool m_bClearRoleAssignments = true;

		private bool m_bOverrideRoleMappings;

		private ConditionalMappingCollection m_roleAssignmentMappings = new ConditionalMappingCollection();

		private bool m_bRecursiveSites = true;

		private bool m_bCopySitePermissions = true;

		private bool m_bCopyPermissionLevels;

		private bool m_bCopyListPermissions = true;

		private bool m_bRecursiveFolders = true;

		private bool m_bCopyFolderPermissions = true;

		private bool m_bCopyItemPermissions = true;

		public bool ClearRoleAssignments
		{
			get
			{
				return this.m_bClearRoleAssignments;
			}
			set
			{
				this.m_bClearRoleAssignments = value;
			}
		}

		public bool CopyFolderPermissions
		{
			get
			{
				return this.m_bCopyFolderPermissions;
			}
			set
			{
				this.m_bCopyFolderPermissions = value;
			}
		}

		public bool CopyItemPermissions
		{
			get
			{
				return this.m_bCopyItemPermissions;
			}
			set
			{
				this.m_bCopyItemPermissions = value;
			}
		}

		public bool CopyListPermissions
		{
			get
			{
				return this.m_bCopyListPermissions;
			}
			set
			{
				this.m_bCopyListPermissions = value;
			}
		}

		public bool CopyPermissionLevels
		{
			get
			{
				return this.m_bCopyPermissionLevels;
			}
			set
			{
				this.m_bCopyPermissionLevels = value;
			}
		}

		public bool CopyRootPermissions
		{
			get
			{
				return this.m_bCopyRootPermissions;
			}
			set
			{
				this.m_bCopyRootPermissions = value;
			}
		}

		public bool CopySitePermissions
		{
			get
			{
				return this.m_bCopySitePermissions;
			}
			set
			{
				this.m_bCopySitePermissions = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool CorrectingLinks
		{
			get
			{
				return false;
			}
			set
			{
				base.CorrectingLinks = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool LinkCorrectTextFields
		{
			get
			{
				return base.LinkCorrectTextFields;
			}
			set
			{
				base.LinkCorrectTextFields = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool MapAudiences
		{
			get
			{
				return base.MapAudiences;
			}
			set
			{
				base.MapAudiences = value;
			}
		}

		public bool MapRolesByName
		{
			get
			{
				return this.m_bMapRolesByName;
			}
			set
			{
				this.m_bMapRolesByName = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool OverrideCheckouts
		{
			get
			{
				return base.OverrideCheckouts;
			}
			set
			{
				base.OverrideCheckouts = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool OverrideRoleMappings
		{
			get
			{
				return this.m_bOverrideRoleMappings;
			}
			set
			{
				this.m_bOverrideRoleMappings = value;
			}
		}

		[CmdletParameterAlias("RecursivelyCopySubFolderPermissions")]
		public bool RecursiveFolders
		{
			get
			{
				return this.m_bRecursiveFolders;
			}
			set
			{
				this.m_bRecursiveFolders = value;
			}
		}

		[CmdletParameterAlias("RecursivelyCopySubSitePermissions")]
		public bool RecursiveSites
		{
			get
			{
				return this.m_bRecursiveSites;
			}
			set
			{
				this.m_bRecursiveSites = value;
			}
		}

		[CmdletEnabledParameter("OverrideRoleMappings", true)]
		[CmdletParameterEnumerate(true)]
		public ConditionalMappingCollection RoleAssignmentMappings
		{
			get
			{
				return this.m_roleAssignmentMappings;
			}
			set
			{
				this.m_roleAssignmentMappings = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool UseComprehensiveLinkCorrection
		{
			get
			{
				return base.UseComprehensiveLinkCorrection;
			}
			set
			{
				base.UseComprehensiveLinkCorrection = value;
			}
		}

		public CopyPermissionsOptions()
		{
		}
	}
}