using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class CopyAudiencesOptions : SharePointActionOptions
	{
		private CopyAudiencesOptions.PasteAudienceStyles m_pasteStyle = CopyAudiencesOptions.PasteAudienceStyles.PreserveExisting;

		private bool m_bStartAudienceCompilation;

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

		[CmdletParameterAlias("ExistingAudiencesAction")]
		public CopyAudiencesOptions.PasteAudienceStyles PasteStyle
		{
			get
			{
				return this.m_pasteStyle;
			}
			set
			{
				this.m_pasteStyle = value;
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

		public bool StartAudienceCompilation
		{
			get
			{
				return this.m_bStartAudienceCompilation;
			}
			set
			{
				this.m_bStartAudienceCompilation = value;
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

		public CopyAudiencesOptions()
		{
		}

		public enum PasteAudienceStyles
		{
			DeleteExisting,
			OverwriteExisting,
			PreserveExisting
		}
	}
}