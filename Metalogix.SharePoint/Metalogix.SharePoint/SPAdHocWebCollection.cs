using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using System;

namespace Metalogix.SharePoint
{
	public class SPAdHocWebCollection : SPWebCollection
	{
		public SPAdHocWebCollection(SPWeb site) : base(site)
		{
			base.AddToCollection(site);
		}

		public SPAdHocWebCollection(SPWeb site, SPWeb[] webs) : base(site)
		{
			base.AddRangeToCollection(webs);
		}

		public override void Add(Node item)
		{
			throw new InvalidOperationException(Resources.AdHoc_Web_Collection_Is_Immutable);
		}

		public override SPWeb AddWeb(string sWebXML, AddWebOptions addOptions, LogItem logItem = null)
		{
			throw new InvalidOperationException(Resources.AdHoc_Web_Collection_Is_Immutable);
		}

		public override bool DeleteWeb(string sWebName)
		{
			throw new InvalidOperationException(Resources.AdHoc_Web_Collection_Is_Immutable);
		}

		public override void FetchData()
		{
		}
	}
}