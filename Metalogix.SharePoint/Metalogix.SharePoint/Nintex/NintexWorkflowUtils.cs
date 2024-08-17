using Metalogix.SharePoint.Nintex.NintexWorkflowArgs;
using System;
using System.Text;

namespace Metalogix.SharePoint.Nintex
{
	public static class NintexWorkflowUtils
	{
		public static string ExportWorkflow(ExportNintexWorkflowArgs args)
		{
			string str = (new StringBuilder()).Append("-a export ").AppendFormat("-t \"{0}\" ", args.WorkflowType).AppendFormat("-s \"{0}\" ", args.SiteUrl).AppendFormat("-l \"{0}\" ", args.ListTitle).AppendFormat("-w \"{0}\" ", args.WorkflowName).AppendFormat("-u \"{0}\" ", args.UserName).AppendFormat("-p \"{0}\" ", args.Password).AppendFormat("-m \"{0}\" ", args.NwfFile).AppendFormat("-n \"{0}\" ", args.NwpFile).AppendFormat("-e \"{0}\" ", args.EndpointUrl).ToString();
			return ConsoleUtils.LaunchNintexWorkflows(str);
		}

		public static string ImportWorkflow(ImportNintexWorkflowArgs args)
		{
			string str = (new StringBuilder()).Append("-a import ").AppendFormat("-t \"{0}\" ", args.WorkflowType).AppendFormat("-s \"{0}\" ", args.SiteUrl).AppendFormat("-l \"{0}\" ", args.ListTitle).AppendFormat("-u \"{0}\" ", args.UserName).AppendFormat("-p \"{0}\" ", args.Password).AppendFormat("-n \"{0}\" ", args.NwpFile).ToString();
			return ConsoleUtils.LaunchNintexWorkflows(str);
		}

		public static string PublishWorkflow(PublishNintexWorkflowArgs args)
		{
			StringBuilder stringBuilder = (new StringBuilder()).Append("-a publish ").AppendFormat("-s \"{0}\" ", args.SiteUrl).AppendFormat("-u \"{0}\" ", args.UserName).AppendFormat("-p \"{0}\" ", args.Password);
			Guid workflowId = args.WorkflowId;
			string str = stringBuilder.AppendFormat("-w \"{0}\" ", workflowId.ToString("D")).ToString();
			return ConsoleUtils.LaunchNintexWorkflows(str);
		}
	}
}