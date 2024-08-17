using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Filters;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Options.Transform
{
	public class ItemFieldValueFilter : ActionOptionsBase
	{
		private const byte C_MAX_DISPLAY_LENGTH = 50;

		private string _substitute;

		private IFilterExpression _filter;

		[Bindable(false)]
		[Browsable(false)]
		public IFilterExpression Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
			}
		}

		[LocalizedDisplayName("FMMOSubstitutionValue")]
		public string Substitute
		{
			get
			{
				return this._substitute;
			}
			set
			{
				this._substitute = TransformUtils.SanitiseForTaxonomy(value);
			}
		}

		[LocalizedDisplayName("FMMOItemFieldValueFilter")]
		public string ValueFilter
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this._filter != null)
				{
					stringBuilder.Append(this._filter.GetLogicString());
					if (stringBuilder.Length > 50)
					{
						stringBuilder.Length = 50;
						stringBuilder.Append("...");
					}
				}
				return stringBuilder.ToString();
			}
		}

		public ItemFieldValueFilter()
		{
			this._substitute = string.Empty;
			this._filter = null;
		}

		public ItemFieldValueFilter(XmlNode node) : this()
		{
			this.FromXML(node);
		}
	}
}