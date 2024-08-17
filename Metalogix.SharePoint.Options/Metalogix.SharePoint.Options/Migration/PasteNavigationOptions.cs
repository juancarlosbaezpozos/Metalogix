using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteNavigationOptions : SharePointActionOptions
	{
		private bool m_bRecursive;

		private bool m_bCopyGlobalNavigation = true;

		private bool m_bCopyCurrentNavigation = true;

		private bool m_bRenameSpecificNodes;

		private TransformationTaskCollection m_ttcTaskCollection = new TransformationTaskCollection();

		private bool m_bUsingComprehensiveLinkCorrection;

		[CmdletEnabledParameter(false)]
		public override bool AllowDBUserWriting
		{
			get
			{
				return base.AllowDBUserWriting;
			}
			set
			{
				base.AllowDBUserWriting = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool CheckResults
		{
			get
			{
				return base.CheckResults;
			}
			set
			{
				base.CheckResults = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override ComparisonOptions CompareOptions
		{
			get
			{
				return base.CompareOptions;
			}
			set
			{
				base.CompareOptions = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override string ComparisonLevel
		{
			get
			{
				return base.ComparisonLevel;
			}
		}

		[CmdletParameterAlias("CopyQuickLaunch")]
		public bool CopyCurrentNavigation
		{
			get
			{
				return this.m_bCopyCurrentNavigation;
			}
			set
			{
				this.m_bCopyCurrentNavigation = value;
			}
		}

		[CmdletParameterAlias("CopyTopNavigationBar")]
		public bool CopyGlobalNavigation
		{
			get
			{
				return this.m_bCopyGlobalNavigation;
			}
			set
			{
				this.m_bCopyGlobalNavigation = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool CorrectingLinks
		{
			get
			{
				return base.CorrectingLinks;
			}
			set
			{
				base.CorrectingLinks = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override Metalogix.SharePoint.Migration.LinkCorrectionScope LinkCorrectionScope
		{
			get
			{
				return base.LinkCorrectionScope;
			}
			set
			{
				base.LinkCorrectionScope = value;
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
		public override bool LogSkippedItems
		{
			get
			{
				return base.LogSkippedItems;
			}
			set
			{
				base.LogSkippedItems = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool MapGroupsByName
		{
			get
			{
				return base.MapGroupsByName;
			}
			set
			{
				base.MapGroupsByName = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override bool MapMissingUsers
		{
			get
			{
				return base.MapMissingUsers;
			}
			set
			{
				base.MapMissingUsers = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public override string MapMissingUsersToLoginName
		{
			get
			{
				return base.MapMissingUsersToLoginName;
			}
			set
			{
				base.MapMissingUsersToLoginName = value;
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
		public override bool OverwriteGroups
		{
			get
			{
				return base.OverwriteGroups;
			}
			set
			{
				base.OverwriteGroups = value;
			}
		}

		[CmdletParameterAlias("CopySubSiteNavigation")]
		public bool Recursive
		{
			get
			{
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		[CmdletEnabledParameter(false)]
		[UsesStickySettings(false)]
		public bool RenameSpecificNodes
		{
			get
			{
				return this.m_bRenameSpecificNodes;
			}
			set
			{
				this.m_bRenameSpecificNodes = value;
			}
		}

		[UsesStickySettings(false)]
		public TransformationTaskCollection TaskCollection
		{
			get
			{
				return this.m_ttcTaskCollection;
			}
			set
			{
				this.m_ttcTaskCollection = value;
			}
		}

		[CmdletParameterAlias("UseComprehensiveLinkCorrection")]
		public bool UsingComprehensiveLinkCorrection
		{
			get
			{
				return this.m_bUsingComprehensiveLinkCorrection;
			}
			set
			{
				this.m_bUsingComprehensiveLinkCorrection = value;
			}
		}

		public PasteNavigationOptions()
		{
		}
	}
}