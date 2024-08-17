using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Common.Workflow2013
{
	public class SP2013WorkflowSubscription
	{
		public SP2013WorkflowDefinition AssociatedSP2013WorkflowDefinition
		{
			get;
			set;
		}

		public Guid DefinitionId
		{
			get;
			set;
		}

		public bool Enabled
		{
			get;
			set;
		}

		public Guid EventSourceId
		{
			get;
			set;
		}

		public List<string> EventTypes
		{
			get;
			set;
		}

		public string HistoryListId
		{
			get;
			set;
		}

		public string HistoryListTitle
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public bool ManualStartBypassesActivationLimit
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string ParentContentTypeId
		{
			get;
			set;
		}

		public SP2013WorkflowProperty[] PropertyDefinitions
		{
			get;
			set;
		}

		public string StatusFieldName
		{
			get;
			set;
		}

		public string TaskListId
		{
			get;
			set;
		}

		public string TaskListTitle
		{
			get;
			set;
		}

		public SP2013WorkflowSubscription()
		{
			this.EventTypes = new List<string>();
		}

		public SP2013WorkflowSubscription Clone()
		{
			SP2013WorkflowSubscription sP2013WorkflowSubscription = new SP2013WorkflowSubscription()
			{
				Id = this.Id,
				DefinitionId = this.DefinitionId,
				Name = this.Name,
				EventSourceId = this.EventSourceId,
				Enabled = this.Enabled,
				EventTypes = this.EventTypes,
				ParentContentTypeId = this.ParentContentTypeId,
				ManualStartBypassesActivationLimit = this.ManualStartBypassesActivationLimit,
				StatusFieldName = this.StatusFieldName,
				PropertyDefinitions = this.PropertyDefinitions,
				AssociatedSP2013WorkflowDefinition = this.AssociatedSP2013WorkflowDefinition.Clone(),
				HistoryListId = this.HistoryListId,
				TaskListId = this.TaskListId
			};
			return sP2013WorkflowSubscription;
		}
	}
}