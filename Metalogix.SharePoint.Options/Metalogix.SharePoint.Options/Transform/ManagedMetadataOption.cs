using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Options.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint.Options.Transform
{
	public class ManagedMetadataOption : ActionOptionsBase
	{
		private string m_TargetTermstore = string.Empty;

		private string m_TargetGroup = string.Empty;

		private string m_TargetTermSet = string.Empty;

		private string m_TargetAnchor = string.Empty;

		private string m_NewFieldName = string.Empty;

		private string m_NewFieldDisplayName = string.Empty;

		private bool m_CreateNewField;

		private CommonSerializableList<ItemFieldValueFilter> m_itemFieldValueFilterCollection;

		[Bindable(false)]
		[Browsable(false)]
		public bool CreateNewField
		{
			get
			{
				return this.m_CreateNewField;
			}
			set
			{
				this.m_CreateNewField = value;
			}
		}

		public CommonSerializableList<ItemFieldValueFilter> ItemFieldValueFilterCollection
		{
			get
			{
				return this.m_itemFieldValueFilterCollection;
			}
			set
			{
				this.m_itemFieldValueFilterCollection = value;
			}
		}

		[LocalizedDisplayName("FMMONewField")]
		public string NewField
		{
			get
			{
				if (!this.m_CreateNewField)
				{
					return string.Format("[{0}]", Resources.FMMONo);
				}
				return string.Format("{0} ({1})", this.m_NewFieldDisplayName, this.m_NewFieldName);
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public string NewFieldDisplayName
		{
			get
			{
				return this.m_NewFieldDisplayName;
			}
			set
			{
				this.m_NewFieldDisplayName = value;
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public string NewFieldName
		{
			get
			{
				return this.m_NewFieldName;
			}
			set
			{
				this.m_NewFieldName = value;
			}
		}

		[LocalizedDisplayName("FMMONoOfSubstitutes")]
		public string NoOfSubstitutes
		{
			get
			{
				if (this.m_itemFieldValueFilterCollection == null || this.m_itemFieldValueFilterCollection.Count <= 0)
				{
					return string.Format("[{0}]", Resources.FMMONo);
				}
				string fMMOYes = Resources.FMMOYes;
				int count = this.m_itemFieldValueFilterCollection.Count;
				return string.Format("{0} ({1})", fMMOYes, count.ToString());
			}
		}

		[LocalizedDisplayName("TargetAnchor")]
		public string TargetAnchor
		{
			get
			{
				return this.m_TargetAnchor;
			}
			set
			{
				this.m_TargetAnchor = TransformUtils.SanitiseTaxonomyAnchorConfiguration(value);
			}
		}

		[LocalizedDisplayName("TargetTermGroup")]
		public string TargetGroup
		{
			get
			{
				return this.m_TargetGroup;
			}
			set
			{
				this.m_TargetGroup = TransformUtils.SanitiseForTaxonomy(value);
			}
		}

		[LocalizedDisplayName("TargetTermSet")]
		public string TargetTermSet
		{
			get
			{
				return this.m_TargetTermSet;
			}
			set
			{
				this.m_TargetTermSet = TransformUtils.SanitiseForTaxonomy(value);
			}
		}

		[LocalizedDisplayName("TargetTermstore")]
		public string TargetTermstore
		{
			get
			{
				return this.m_TargetTermstore;
			}
			set
			{
				this.m_TargetTermstore = value;
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public bool UsingItemFieldValueFilter
		{
			get
			{
				if (this.m_itemFieldValueFilterCollection == null)
				{
					return false;
				}
				return this.m_itemFieldValueFilterCollection.Count > 0;
			}
		}

		public ManagedMetadataOption()
		{
			this.m_itemFieldValueFilterCollection = new CommonSerializableList<ItemFieldValueFilter>();
		}

		public ManagedMetadataOption(XmlNode node) : this()
		{
			this.FromXML(node);
		}

		public List<string> GetTargetAnchorTerms()
		{
			List<string> strs = new List<string>();
			strs.AddRange(this.TargetAnchor.Split(TransformUtils.ANCHOR_MARKER_CHAR, StringSplitOptions.RemoveEmptyEntries));
			return strs;
		}
	}
}