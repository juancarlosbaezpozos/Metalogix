using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Administration.CheckLinks
{
	public class CheckLinksCompletionDetailOrderProvider : CompletionDetailsOrderProvider
	{
		public CheckLinksCompletionDetailOrderProvider()
		{
			List<string> orderingList = this.OrderingList;
			string[] checkLinksDetailSucceeded = new string[] { Resources.CheckLinks_Detail_Succeeded, Resources.CheckLinks_Detail_Failed, Resources.CheckLinks_Detail_Flagged };
			orderingList.AddRange(checkLinksDetailSucceeded);
		}
	}
}