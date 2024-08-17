using Metalogix;
using Metalogix.Data.Filters;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.SharePoint.Options.Transform
{
	public class FieldToManagedMetadataOption : ManagedMetadataOption
	{
		private IFilterExpression m_listFilterExpression;

		private IFilterExpression m_listFieldFilterExpression;

		[LocalizedDisplayName("FMMOFieldFilter")]
		public string FieldFilter
		{
			get
			{
				if (this.m_listFieldFilterExpression == null)
				{
					return string.Empty;
				}
				return this.m_listFieldFilterExpression.GetLogicString();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public IFilterExpression ListFieldFilterExpression
		{
			get
			{
				return this.m_listFieldFilterExpression;
			}
			set
			{
				this.m_listFieldFilterExpression = value;
			}
		}

		[LocalizedDisplayName("FMMOListFilter")]
		public string ListFilter
		{
			get
			{
				if (this.m_listFilterExpression == null)
				{
					return string.Empty;
				}
				return this.m_listFilterExpression.GetLogicString();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		public IFilterExpression ListFilterExpression
		{
			get
			{
				return this.m_listFilterExpression;
			}
			set
			{
				this.m_listFilterExpression = value;
			}
		}

		public FieldToManagedMetadataOption()
		{
			this.m_listFilterExpression = null;
			this.m_listFieldFilterExpression = null;
		}

		public FieldToManagedMetadataOption(XmlNode node) : this()
		{
			this.FromXML(node);
		}

		public FieldToManagedMetadataOption Copy()
		{
			return new FieldToManagedMetadataOption(XmlUtility.StringToXmlNode(base.ToXML()));
		}
	}
}