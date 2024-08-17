using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Options.Migration.Mapping;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPMappingOptions : OptionsBase
	{
		private bool m_bRenameSpecificNodes;

		private bool m_bMapWebTemplates;

		private bool m_bMapChildSiteTemplates;

		private bool m_bMapColumns;

		private CommonSerializableTable<string, string> m_stWebTemplateMapping = new CommonSerializableTable<string, string>();

		private TransformationTaskCollection m_ttcTaskCollection = new TransformationTaskCollection();

		private Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings m_columnMappings = new Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings();

		private bool m_bAllowDBUserWriting;

		private bool m_bMapGroupsByName = true;

		private bool m_bOverwriteGroups = true;

		private bool m_sMapMissingUsers;

		private string m_sMapMissingUsersToLoginName;

		public bool AllowDBUserWriting
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

		[CmdletEnabledParameter(false)]
		public Metalogix.SharePoint.Options.Migration.Mapping.ColumnMappings ColumnMappings
		{
			get
			{
				return this.m_columnMappings;
			}
			set
			{
				this.m_columnMappings = value;
			}
		}

		public bool MapAudiences
		{
			get;
			set;
		}

		[CmdletEnabledParameter(false)]
		public bool MapChildWebTemplates
		{
			get
			{
				return this.m_bMapChildSiteTemplates;
			}
			set
			{
				this.m_bMapChildSiteTemplates = value;
			}
		}

		public bool MapColumns
		{
			get
			{
				return this.m_bMapColumns;
			}
			set
			{
				this.m_bMapColumns = value;
			}
		}

		public bool MapGroupsByName
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
		public bool MapMissingUsers
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

		[CmdletEnabledParameter(false)]
		public string MapMissingUsersToLoginName
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

		public bool MapWebTemplates
		{
			get
			{
				return this.m_bMapWebTemplates;
			}
			set
			{
				this.m_bMapWebTemplates = value;
			}
		}

		public bool OverwriteGroups
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

		[CmdletEnabledParameter(false)]
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
		public CommonSerializableTable<string, string> WebTemplateMappingTable
		{
			get
			{
				return this.m_stWebTemplateMapping;
			}
			set
			{
				this.m_stWebTemplateMapping = value;
			}
		}

		public SPMappingOptions()
		{
		}
	}
}