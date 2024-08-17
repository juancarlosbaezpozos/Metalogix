using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Migration;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Options
{
	public class SharePointActionOptions : ActionOptions
	{
		private Metalogix.SharePoint.Options.MigrationMode m_MigrationMode;

		private bool m_bMapAudiences;

		private bool m_bVerbose;

		private bool m_bLogSkippedItems;

		private bool m_bCheckResults;

		private ComparisonOptions m_compareOptions = new ComparisonOptions();

		private bool m_bForceRefresh = true;

		private bool m_bCorrectLinks = true;

		private bool m_bLinkCorrectTextFields;

		private Metalogix.SharePoint.Migration.LinkCorrectionScope m_bLinkCorrectionScope;

		private bool m_bUseComprehensiveLinkCorrection;

		private bool m_bAllowDBUserWriting;

		private bool m_bMapGroupsByName = true;

		private bool m_bOverwriteGroups = true;

		private bool m_sMapMissingUsers;

		private string m_sMapMissingUsersToLoginName;

		private bool m_bPersistMappings;

		private bool m_bOverrideCheckouts = true;

		public virtual bool AllowDBUserWriting
		{
			get
			{
				return this.m_bAllowDBUserWriting;
			}
			set
			{
				this.m_bAllowDBUserWriting = value;
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

		[CmdletEnabledParameter("CheckResults", true)]
		public virtual string ComparisonLevel
		{
			get
			{
				return this.CompareOptions.Level.ToString();
			}
		}

		public virtual bool CorrectingLinks
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

		public virtual bool ForceRefresh
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

		public bool? IsFromAdvancedMode
		{
			get;
			set;
		}

		[CmdletEnabledParameter("CorrectingLinks", true)]
		public virtual Metalogix.SharePoint.Migration.LinkCorrectionScope LinkCorrectionScope
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

		[CmdletEnabledParameter("CorrectingLinks", true)]
		public virtual bool LinkCorrectTextFields
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

		public virtual bool MapAudiences
		{
			get
			{
				return this.m_bMapAudiences;
			}
			set
			{
				this.m_bMapAudiences = value;
			}
		}

		public virtual bool MapGroupsByName
		{
			get
			{
				return this.m_bMapGroupsByName;
			}
			set
			{
				this.m_bMapGroupsByName = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public virtual bool MapMissingUsers
		{
			get
			{
				return this.m_sMapMissingUsers;
			}
			set
			{
				this.m_sMapMissingUsers = value;
			}
		}

		public virtual string MapMissingUsersToLoginName
		{
			get
			{
				return this.m_sMapMissingUsersToLoginName;
			}
			set
			{
				this.m_sMapMissingUsersToLoginName = value;
			}
		}

		public Metalogix.SharePoint.Options.MigrationMode MigrationMode
		{
			get
			{
				return this.m_MigrationMode;
			}
			set
			{
				this.m_MigrationMode = value;
			}
		}

		public virtual bool OverrideCheckouts
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

		[CmdletEnabledParameter("MapGroupsByName", true)]
		public virtual bool OverwriteGroups
		{
			get
			{
				return this.m_bOverwriteGroups;
			}
			set
			{
				this.m_bOverwriteGroups = value;
			}
		}

		public virtual bool PersistMappings
		{
			get
			{
				return this.m_bPersistMappings;
			}
			set
			{
				this.m_bPersistMappings = value;
			}
		}

		[CmdletEnabledParameter("CorrectingLinks", true)]
		public virtual bool UseComprehensiveLinkCorrection
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

		[CmdletParameterAlias("VerboseLog")]
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

		public SharePointActionOptions()
		{
		}

		public override void FromXML(XmlNode xmlNode)
		{
			base.FromXML(xmlNode);
			if (!SharePointConfigurationVariables.AllowCheckResults)
			{
				this.CheckResults = false;
			}
			this.SetMigrationMode(xmlNode);
		}

		public override void MakeOptionsIncremental(DateTime? incrementFromTime)
		{
			this.ForceRefresh = true;
			this.MigrationMode = Metalogix.SharePoint.Options.MigrationMode.Incremental;
		}

		private void SetMigrationMode(XmlNode xmlNode)
		{
			this.MigrationMode = Metalogix.SharePoint.Options.MigrationMode.Custom;
			string name = "MigrationMode";
			PropertyInfo[] properties = base.GetType().GetProperties();
			int num = 0;
			while (num < (int)properties.Length)
			{
				PropertyInfo propertyInfo = properties[num];
				if (!propertyInfo.PropertyType.Equals(this.MigrationMode.GetType()))
				{
					num++;
				}
				else
				{
					name = propertyInfo.Name;
					break;
				}
			}
			XmlNode xmlNodes = xmlNode.SelectSingleNode(string.Concat("./", name));
			if (xmlNodes != null)
			{
				try
				{
					this.MigrationMode = (Metalogix.SharePoint.Options.MigrationMode)Enum.Parse(typeof(Metalogix.SharePoint.Options.MigrationMode), xmlNodes.InnerText, true);
				}
				catch
				{
					this.MigrationMode = Metalogix.SharePoint.Options.MigrationMode.Custom;
				}
			}
		}
	}
}