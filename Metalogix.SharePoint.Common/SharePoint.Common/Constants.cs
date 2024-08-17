using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Common
{
	public class Constants
	{
		public const string WORKFLOW_TASK_SP2013_ID = "0x0108003365C4474CAE8C42BCE396314E88E51F";

		public const string OOB_WORKFLOWS = "OOB_Workflows";

		public const string SPD_WORKFLOWS = "SPD_Workflows";

		public const string NINTEX_WORKFLOWS = "Nintex_Workflows";

		public readonly static HashSet<string> NecessaryProperties;

		static Constants()
		{
			HashSet<string> strs = new HashSet<string>();
			strs.Add("SPDConfig.StartOnCreate");
			strs.Add("SPDConfig.StartOnChange");
			strs.Add("SPDConfig.StartManually");
			strs.Add("AutosetStatusToStageName");
			strs.Add("Definition.Description");
			strs.Add("isReusable");
			strs.Add("CreatedBySPD");
			strs.Add("IsProjectMode");
			strs.Add("RequiresInitiationForm");
			strs.Add("SubscriptionName");
			Constants.NecessaryProperties = strs;
		}

		public Constants()
		{
		}
	}
}