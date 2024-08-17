using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Workflow
{
	public static class NintexOnlineWorkflowActivity
	{
		public const string CreateListItem = "Microsoft.SharePoint.WorkflowServices.Activities.CreateListItem";

		public const string DeleteItem = "Microsoft.SharePoint.WorkflowServices.Activities.DeleteListItem";

		public const string Checkoutitem = "Microsoft.SharePoint.WorkflowServices.Activities.CheckOutItem";

		public const string DiscardCheckout = "Microsoft.SharePoint.WorkflowServices.Activities.UndoCheckOutItem";

		public const string Querylist = "#QuerySPList";

		public const string RunIf = "#RunIf";

		public const string SetFieldValue = "Microsoft.SharePoint.WorkflowServices.Activities.SetField";

		public const string SetCondition = "#ConditionalBranch";

		public const string Loop = "#LoopCondition";

		public const string PauseUntil = "Microsoft.SharePoint.WorkflowServices.Activities.DelayUntil";

		public const string SetVariable = "System.Activities.Statements.Assign";

		public const string UpdateListItem = "Microsoft.SharePoint.WorkflowServices.Activities.UpdateListItem";

		public const string Filter = "#Filter";

		public const string MathOperation = "Microsoft.SharePoint.WorkflowServices.Activities.Calc";

		public const string WaitForItemUpdate = "Microsoft.SharePoint.WorkflowServices.Activities.WaitForFieldChange";

		public const string AssignFlexiTask = "Microsoft.SharePoint.WorkflowServices.Activities.CompositeTask";

		public const string SendNotification = "#SendNotificationActivity";

		public static List<string> GetActivites()
		{
			List<string> strs = new List<string>()
			{
				"Microsoft.SharePoint.WorkflowServices.Activities.CreateListItem",
				"Microsoft.SharePoint.WorkflowServices.Activities.DeleteListItem",
				"Microsoft.SharePoint.WorkflowServices.Activities.CheckOutItem",
				"Microsoft.SharePoint.WorkflowServices.Activities.UndoCheckOutItem",
				"#QuerySPList",
				"#RunIf",
				"Microsoft.SharePoint.WorkflowServices.Activities.SetField",
				"#ConditionalBranch",
				"#LoopCondition",
				"Microsoft.SharePoint.WorkflowServices.Activities.DelayUntil",
				"System.Activities.Statements.Assign",
				"Microsoft.SharePoint.WorkflowServices.Activities.UpdateListItem",
				"#Filter",
				"Microsoft.SharePoint.WorkflowServices.Activities.Calc",
				"Microsoft.SharePoint.WorkflowServices.Activities.WaitForFieldChange",
				"Microsoft.SharePoint.WorkflowServices.Activities.CompositeTask",
				"#SendNotificationActivity"
			};
			return strs;
		}
	}
}