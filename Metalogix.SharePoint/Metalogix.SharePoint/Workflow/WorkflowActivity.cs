using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Workflow
{
	public class WorkflowActivity
	{
		private string name;

		private List<WorkflowActivityAttribute> attributes;

		private WorkflowActivityType activityType;

		[XmlAttribute]
		public WorkflowActivityType ActivityType
		{
			get
			{
				return this.activityType;
			}
			set
			{
				this.activityType = value;
			}
		}

		public List<WorkflowActivityAttribute> Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal WorkflowActivity(string activityName, WorkflowActivityType type)
		{
			this.name = activityName;
			this.activityType = type;
			this.attributes = new List<WorkflowActivityAttribute>();
		}

		internal WorkflowActivity()
		{
			this.name = string.Empty;
			this.activityType = WorkflowActivityType.SharePoint;
			this.attributes = new List<WorkflowActivityAttribute>();
		}
	}
}