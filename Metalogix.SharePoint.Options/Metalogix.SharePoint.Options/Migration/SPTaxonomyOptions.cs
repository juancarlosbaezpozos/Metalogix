using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPTaxonomyOptions : OptionsBase
	{
		private bool _resolveManagedMetadataByName = true;

		private bool m_bCopyReferencedManagedMetadata;

		private bool _transformListAndField;

		private bool _transformSiteColumns;

		private CommonSerializableTable<string, string> m_termstoreNameMappingTable = new CommonSerializableTable<string, string>();

		private bool m_bMapTermStores;

		public bool CopyReferencedManagedMetadata
		{
			get
			{
				return this.m_bCopyReferencedManagedMetadata;
			}
			set
			{
				this.m_bCopyReferencedManagedMetadata = value;
			}
		}

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

		[CmdletEnabledParameter(false)]
		public CommonSerializableTable<string, string> TermstoreNameMappingTable
		{
			get
			{
				return this.m_termstoreNameMappingTable;
			}
			set
			{
				this.m_termstoreNameMappingTable = value;
			}
		}

		public bool TransformListAndField
		{
			get
			{
				return this._transformListAndField;
			}
			set
			{
				this._transformListAndField = value;
			}
		}

		public bool TransformSiteColumns
		{
			get
			{
				return this._transformSiteColumns;
			}
			set
			{
				this._transformSiteColumns = value;
			}
		}

		public SPTaxonomyOptions()
		{
		}
	}
}