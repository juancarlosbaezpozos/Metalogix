using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteTaxonomyOptions : SharePointActionOptions
	{
		private bool m_taxonomyConfigured;

		private CommonSerializableTable<string, string> m_termstoreNameMappingTable;

		private bool m_bMapTermStores;

		private bool _resolveManagedMetadataByName = true;

		[CmdletEnabledParameter(true)]
		public bool MapTermStores
		{
			get
			{
				return this.m_bMapTermStores;
			}
			set
			{
				this.m_bMapTermStores = value;
			}
		}

		[CmdletEnabledParameter("ResolveManagedMetadataByName", true)]
		[CmdletParameterEnumerate(true)]
		public bool ResolveManagedMetadataByName
		{
			get
			{
				return this._resolveManagedMetadataByName;
			}
			set
			{
				this._resolveManagedMetadataByName = value;
			}
		}

		public bool TaxonomyConfigured
		{
			get
			{
				return this.m_taxonomyConfigured;
			}
			set
			{
				this.m_taxonomyConfigured = value;
			}
		}

		[CmdletEnabledParameter("MapTermStores", true)]
		public CommonSerializableTable<string, string> TermstoreNameMappingTable
		{
			get
			{
				if (this.m_termstoreNameMappingTable == null)
				{
					this.m_termstoreNameMappingTable = new CommonSerializableTable<string, string>();
				}
				return this.m_termstoreNameMappingTable;
			}
			set
			{
				this.m_termstoreNameMappingTable = value;
			}
		}

		public PasteTaxonomyOptions()
		{
		}
	}
}