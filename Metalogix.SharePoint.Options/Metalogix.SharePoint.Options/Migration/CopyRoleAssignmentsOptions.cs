using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyRoleAssignmentsOptions : SharePointActionOptions
	{
		private bool m_bMapRolesByName;

		private bool m_bClearRoleAssignments = true;

		private bool m_bOverrideRoleMappings;

		private ConditionalMappingCollection m_roleAssignmentMappings = new ConditionalMappingCollection();

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

		public CopyRoleAssignmentsOptions()
		{
		}
	}
}