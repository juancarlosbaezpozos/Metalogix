using Metalogix.SharePoint.Taxonomy;
using System;

namespace Metalogix.SharePoint.Taxonomy.Generic
{
	public abstract class SPTaxonomyItemCollection<T> : SPTaxonomyIndexedCollection<T>
	where T : SPTaxonomyItem
	{
		internal SPTaxonomyItemCollection() : base("Id")
		{
		}
	}
}