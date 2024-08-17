using System;

namespace Metalogix.SharePoint.Interfaces
{
	public interface ITaxonomySiteConnection : ITaxonomyConnection
	{
		string RootSiteGUID
		{
			get;
		}
	}
}