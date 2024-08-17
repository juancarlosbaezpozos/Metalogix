using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class WebPartOptions : SharePointActionOptions
	{
		private bool m_bCopySiteWebParts = true;

		private bool m_bCopyDocumentWebParts = true;

		private bool m_bCopyViewWebParts = true;

		private bool m_bCopyClosedWebParts;

		private bool m_bCopyContentZoneContent = true;

		private bool m_bCopyWebPartsRecursive;

		private ExistingWebPartsProtocol m_ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;

		private TransformationTaskCollection m_ttcTaskCollection = new TransformationTaskCollection();

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

		public bool CopyClosedWebParts
		{
			get
			{
				return this.m_bCopyClosedWebParts;
			}
			set
			{
				this.m_bCopyClosedWebParts = value;
			}
		}

		public bool CopyContentZoneContent
		{
			get
			{
				return this.m_bCopyContentZoneContent;
			}
			set
			{
				this.m_bCopyContentZoneContent = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopyDocumentWebParts
		{
			get
			{
				return this.m_bCopyDocumentWebParts;
			}
			set
			{
				this.m_bCopyDocumentWebParts = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopySiteWebParts
		{
			get
			{
				return this.m_bCopySiteWebParts;
			}
			set
			{
				this.m_bCopySiteWebParts = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool CopyViewWebParts
		{
			get
			{
				return this.m_bCopyViewWebParts;
			}
			set
			{
				this.m_bCopyViewWebParts = value;
			}
		}

		public bool CopyWebPartsRecursive
		{
			get
			{
				return this.m_bCopyWebPartsRecursive;
			}
			set
			{
				this.m_bCopyWebPartsRecursive = value;
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

		public ExistingWebPartsProtocol ExistingWebPartsAction
		{
			get
			{
				return this.m_ExistingWebPartsAction;
			}
			set
			{
				this.m_ExistingWebPartsAction = value;
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

		public bool OverwriteSites
		{
			get;
			set;
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

		[CmdletEnabledParameter("ContainsTransformationTasks", true)]
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

		[CmdletEnabledParameter(false)]
		public override bool Verbose
		{
			get
			{
				return base.Verbose;
			}
			set
			{
				base.Verbose = value;
			}
		}

		public WebPartOptions()
		{
		}
	}
}