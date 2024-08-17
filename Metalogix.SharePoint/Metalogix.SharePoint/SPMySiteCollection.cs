using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPMySiteCollection : SPSiteCollection
	{
		public SPMySiteCollection(SPTenant parentWeb) : base(parentWeb)
		{
		}

		public new void FetchData()
		{
			XmlDocument xmlDocument = new XmlDocument();
			string empty = string.Empty;
			empty = (!base.ParentServer.IsLimitedSiteCollectionConnection ? base.ParentServer.Adapter.Reader.GetSiteCollectionsOnWebApp(string.Empty) : base.FetchLimitSiteCollectionXml());
			xmlDocument.LoadXml(empty);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Site"))
			{
				base.AddSiteToCollection(base.ParentServer.Adapter, xmlNodes);
			}
		}
	}
}