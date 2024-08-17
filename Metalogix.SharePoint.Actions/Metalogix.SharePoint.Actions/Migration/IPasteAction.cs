using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Migration;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	internal interface IPasteAction
	{
		Dictionary<string, string> AudienceIDMappings
		{
			get;
			set;
		}

		Dictionary<Guid, Guid> GuidMappings
		{
			get;
			set;
		}

		Metalogix.SharePoint.Migration.LinkCorrector LinkCorrector
		{
			get;
			set;
		}

		ThreadSafeDictionary<string, string> PrincipalMappings
		{
			get;
			set;
		}

		Dictionary<string, string> WorkflowItemMappings
		{
			get;
			set;
		}

		Dictionary<string, string> WorkflowMappings
		{
			get;
			set;
		}

		Dictionary<string, Dictionary<string, string>> WorkflowViewMappings
		{
			get;
			set;
		}
	}
}