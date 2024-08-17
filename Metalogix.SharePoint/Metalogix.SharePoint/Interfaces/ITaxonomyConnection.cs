using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy;
using System;

namespace Metalogix.SharePoint.Interfaces
{
	public interface ITaxonomyConnection
	{
		SharePointAdapter Adapter
		{
			get;
		}

		string DisplayUrl
		{
			get;
		}

		Metalogix.Explorer.Node Node
		{
			get;
		}

		SPTermStoreCollection TermStores
		{
			get;
		}

		string Url
		{
			get;
		}
	}
}