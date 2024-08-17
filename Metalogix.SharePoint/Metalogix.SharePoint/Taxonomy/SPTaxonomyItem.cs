using Metalogix.Data;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Taxonomy
{
	public abstract class SPTaxonomyItem : IXmlable
	{
		public abstract Guid Id
		{
			get;
		}

		public abstract string Name
		{
			get;
		}

		public abstract SPTermStore TermStore
		{
			get;
		}

		protected SPTaxonomyItem()
		{
		}

		public abstract string ToXML();

		public abstract void ToXML(XmlWriter xmlWriter);
	}
}