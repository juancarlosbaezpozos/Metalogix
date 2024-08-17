using Metalogix.DataStructures.Generic;
using Metalogix.Transformers;
using System;

namespace Metalogix.SharePoint.Options.Transform
{
	public class SiteColumnToManagedMetadataOptionCollection : TransformerOptions
	{
		private CommonSerializableList<SiteColumnToManagedMetadataOption> m_Items;

		public CommonSerializableList<SiteColumnToManagedMetadataOption> Items
		{
			get
			{
				return this.m_Items;
			}
			set
			{
				this.m_Items = value;
			}
		}

		public SiteColumnToManagedMetadataOptionCollection()
		{
			this.m_Items = new CommonSerializableList<SiteColumnToManagedMetadataOption>();
		}
	}
}