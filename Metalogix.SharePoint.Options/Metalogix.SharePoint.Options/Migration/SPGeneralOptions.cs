using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Migration;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPGeneralOptions : OptionsBase
	{
		private bool m_bVerbose;

		private bool m_bLogSkippedItems;

		private bool m_bCheckResults;

		private ComparisonOptions m_compareOptions = new ComparisonOptions();

		private bool m_bForceRefresh = true;

		private bool m_bOverrideCheckouts;

		private string m_sEmailServer;

		private string m_sEmailUserName;

		private string m_sEmailPassword;

		private string m_sToEmailAddress;

		private string m_sFromEmailAddress;

		private string m_sCCEmailAddress;

		private string m_sBCCEmailAddress;

		private string m_sEmailSuccessTemplateFilePath;

		private string m_sEmailFailureTemplateFilePath;

		private string m_sSubject;

		private bool m_bSendEmail;

		private bool m_bCorrectLinks = true;

		private bool m_bLinkCorrectTextFields;

		private Metalogix.SharePoint.Migration.LinkCorrectionScope m_bLinkCorrectionScope;

		private bool m_bUseComprehensiveLinkCorrection;

		public string BCCEmailAddress
		{
			get
			{
				return this.m_sBCCEmailAddress;
			}
			set
			{
				this.m_sBCCEmailAddress = value;
			}
		}

		public string CCEmailAddress
		{
			get
			{
				return this.m_sCCEmailAddress;
			}
			set
			{
				this.m_sCCEmailAddress = value;
			}
		}

		public virtual bool CheckResults
		{
			get
			{
				return this.m_bCheckResults;
			}
			set
			{
				this.m_bCheckResults = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public virtual ComparisonOptions CompareOptions
		{
			get
			{
				return this.m_compareOptions;
			}
			set
			{
				this.m_compareOptions = value;
			}
		}

		public string ComparisonLevel
		{
			get
			{
				return this.CompareOptions.Level.ToString();
			}
		}

		public bool CorrectingLinks
		{
			get
			{
				return this.m_bCorrectLinks;
			}
			set
			{
				this.m_bCorrectLinks = value;
			}
		}

		public string EmailFailureTemplateFilePath
		{
			get
			{
				return this.m_sEmailFailureTemplateFilePath;
			}
			set
			{
				this.m_sEmailFailureTemplateFilePath = value;
			}
		}

		public string EmailPassword
		{
			get
			{
				return this.m_sEmailPassword;
			}
			set
			{
				this.m_sEmailPassword = value;
			}
		}

		public string EmailServer
		{
			get
			{
				return this.m_sEmailServer;
			}
			set
			{
				this.m_sEmailServer = value;
			}
		}

		public string EmailSubject
		{
			get
			{
				return this.m_sSubject;
			}
			set
			{
				this.m_sSubject = value;
			}
		}

		public string EmailSuccessTemplateFilePath
		{
			get
			{
				return this.m_sEmailSuccessTemplateFilePath;
			}
			set
			{
				this.m_sEmailSuccessTemplateFilePath = value;
			}
		}

		public string EmailUserName
		{
			get
			{
				return this.m_sEmailUserName;
			}
			set
			{
				this.m_sEmailUserName = value;
			}
		}

		public bool EnableSslForEmail
		{
			get;
			set;
		}

		[CmdletEnabledParameter(false)]
		public bool ForceRefresh
		{
			get
			{
				return this.m_bForceRefresh;
			}
			set
			{
				this.m_bForceRefresh = value;
			}
		}

		public string FromEmailAddress
		{
			get
			{
				return this.m_sFromEmailAddress;
			}
			set
			{
				this.m_sFromEmailAddress = value;
			}
		}

		public Metalogix.SharePoint.Migration.LinkCorrectionScope LinkCorrectionScope
		{
			get
			{
				return this.m_bLinkCorrectionScope;
			}
			set
			{
				this.m_bLinkCorrectionScope = value;
			}
		}

		public bool LinkCorrectTextFields
		{
			get
			{
				return this.m_bLinkCorrectTextFields;
			}
			set
			{
				this.m_bLinkCorrectTextFields = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public virtual bool LogSkippedItems
		{
			get
			{
				return this.m_bLogSkippedItems;
			}
			set
			{
				this.m_bLogSkippedItems = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool OverrideCheckouts
		{
			get
			{
				return this.m_bOverrideCheckouts;
			}
			set
			{
				this.m_bOverrideCheckouts = value;
			}
		}

		public virtual bool SendEmail
		{
			get
			{
				return this.m_bSendEmail;
			}
			set
			{
				this.m_bSendEmail = value;
			}
		}

		public string ToEmailAddress
		{
			get
			{
				return this.m_sToEmailAddress;
			}
			set
			{
				this.m_sToEmailAddress = value;
			}
		}

		public bool UseComprehensiveLinkCorrection
		{
			get
			{
				return this.m_bUseComprehensiveLinkCorrection;
			}
			set
			{
				this.m_bUseComprehensiveLinkCorrection = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public virtual bool Verbose
		{
			get
			{
				return this.m_bVerbose;
			}
			set
			{
				this.m_bVerbose = value;
			}
		}

		public SPGeneralOptions()
		{
		}
	}
}