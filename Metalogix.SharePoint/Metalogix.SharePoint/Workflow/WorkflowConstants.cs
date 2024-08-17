using System;

namespace Metalogix.SharePoint.Workflow
{
	public class WorkflowConstants
	{
		public const string C_REGEX_GUID_MATCH = "\\{[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}(-[0-9a-fA-F]{12})\\}";

		public const string C_REGEX_MATCH_ANY_ACTIVITY = "(<[^</\\x22]+?:[A-Za-z0-9]*[\\x20\\>])";

		public const string C_REGEX_FIND_ALL_NAMESPACE = "((xmlns:?)([^=]*)=([\"][^\"]*[\"]))";

		public const int C_IDX_NAMESPACE = 3;

		public const int C_IDX_NAMESPACE_URI = 4;

		public const string C_ACTIVITY_XML_REGEX = "<[^</]+?:{0}[\\x20\\>]";

		public const string C_SPLIT_CHARS_ALL_ACTIVITIES = "<: >";

		public const string C_SPLIT_CHARS = "<:";

		public const string C_TRIM_CHARS = "\" ";

		public const string C_FMT_XPATH_SELECT_NODE = ".//{0}:{1}";

		public const string C_FMT_UNMAPPED_GUID_IN_ACTIVITY = "{0}.{1}:{2}";

		public const string C_MS_WFA_2007 = "Assembly=Microsoft.SharePoint.WorkflowActions, Version=12.0.0.0,";

		public const string C_MS_WFD_2007 = "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=12.0.0.0,";

		public const string C_MS_WFAH_2007 = "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=12.0.0.0";

		public const string C_MS_WFA_2010 = "Assembly=Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0,";

		public const string C_MS_WFD_2010 = "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=14.0.0.0,";

		public const string C_MS_WFAH_2010 = "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=14.0.0.0";

		public const string C_MS_WFA_2013 = "Assembly=Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0,";

		public const string C_MS_WFD_2013 = "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=15.0.0.0,";

		public const string C_MS_WFAH_2013 = "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=15.0.0.0";

		public const string C_MS_WFA_SPO = "Assembly=Microsoft.SharePoint.WorkflowActions, Version=16.0.0.0,";

		public const string C_MS_WFD_SPO = "Type=\"Microsoft.SharePoint.Workflow.SPItemKey, Microsoft.SharePoint, Version=16.0.0.0,";

		public const string C_MS_WFAH_SPO = "QualifiedName=\"Microsoft.SharePoint.WorkflowActions.Helper, Microsoft.SharePoint.WorkflowActions, Version=16.0.0.0";

		public WorkflowConstants()
		{
		}
	}
}