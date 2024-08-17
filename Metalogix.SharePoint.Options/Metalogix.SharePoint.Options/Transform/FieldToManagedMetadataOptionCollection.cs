using Metalogix.DataStructures.Generic;
using Metalogix.Transformers;
using System;

namespace Metalogix.SharePoint.Options.Transform
{
	public class FieldToManagedMetadataOptionCollection : TransformerOptions
	{
		private CommonSerializableList<FieldToManagedMetadataOption> m_Items;

		public CommonSerializableList<FieldToManagedMetadataOption> Items
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

		public FieldToManagedMetadataOptionCollection()
		{
			this.m_Items = new CommonSerializableList<FieldToManagedMetadataOption>();
		}
	}
}