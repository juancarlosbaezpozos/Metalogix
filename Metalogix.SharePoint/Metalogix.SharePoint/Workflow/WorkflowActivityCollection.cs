using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Workflow
{
	public class WorkflowActivityCollection
	{
		private string version = null;

		private List<WorkflowActivity> activities;

		public List<WorkflowActivity> Activities
		{
			get
			{
				return this.activities;
			}
			set
			{
				this.activities = value;
			}
		}

		[XmlIgnore]
		public IList<string> ActivityNames
		{
			get
			{
				IList<string> strs = this.activities.ConvertAll<string>((WorkflowActivity e) => e.Name).AsReadOnly();
				return strs;
			}
		}

		[XmlIgnore]
		public WorkflowActivity this[string activityName]
		{
			get
			{
				WorkflowActivity workflowActivity = this.activities.Find((WorkflowActivity e) => e.Name.Equals(activityName, StringComparison.OrdinalIgnoreCase));
				return workflowActivity;
			}
		}

		[XmlIgnore]
		public IList<string> NintexOnlineWorkflowActivityNames
		{
			get
			{
				return this.ActivityNamesByType(WorkflowActivityType.NintexOnline);
			}
		}

		[XmlIgnore]
		public IList<string> NintexWorkflowActivityNames
		{
			get
			{
				return this.ActivityNamesByType(WorkflowActivityType.Nintex);
			}
		}

		[XmlIgnore]
		public IList<WorkflowActivity> NintexWorkflowActivityOnly
		{
			get
			{
				return this.ActivitiesByType(WorkflowActivityType.Nintex);
			}
		}

		[XmlIgnore]
		public int NoOfSupportedDefaultActivities
		{
			get
			{
				int length = Enum.GetValues(typeof(SharePointWorkflowActivity)).Length + Enum.GetValues(typeof(NintexWorkflowActivity)).Length;
				return length;
			}
		}

		[XmlIgnore]
		public int NoOfSupportedNintexActivities
		{
			get
			{
				return Enum.GetValues(typeof(NintexWorkflowActivity)).Length;
			}
		}

		[XmlIgnore]
		public int NoOfSupportedSharePointActivities
		{
			get
			{
				return Enum.GetValues(typeof(SharePointWorkflowActivity)).Length;
			}
		}

		[XmlIgnore]
		public IList<string> SharePointWorkflowActivityNames
		{
			get
			{
				return this.ActivityNamesByType(WorkflowActivityType.SharePoint);
			}
		}

		[XmlIgnore]
		public IList<WorkflowActivity> SharePointWorkflowActivityOnly
		{
			get
			{
				return this.ActivitiesByType(WorkflowActivityType.SharePoint);
			}
		}

		[XmlAttribute]
		public string Version
		{
			get
			{
				if (string.IsNullOrEmpty(this.version))
				{
					this.version = this.GetType().Assembly.GetName().Version.ToString();
				}
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		internal WorkflowActivityCollection()
		{
			this.activities = new List<WorkflowActivity>();
		}

		public IList<WorkflowActivity> ActivitiesByType(WorkflowActivityType activityType)
		{
			IList<WorkflowActivity> workflowActivities = this.activities.FindAll((WorkflowActivity e) => e.ActivityType == activityType).AsReadOnly();
			return workflowActivities;
		}

		public IList<string> ActivityNamesByType(WorkflowActivityType activityType)
		{
			IList<string> strs = this.activities.FindAll((WorkflowActivity e) => e.ActivityType == activityType).ConvertAll<string>((WorkflowActivity e) => e.Name).AsReadOnly();
			return strs;
		}

		private void AddDefaultSupportedActivities()
		{
			WorkflowActivity workflowActivity;
			foreach (SharePointWorkflowActivity value in Enum.GetValues(typeof(SharePointWorkflowActivity)))
			{
				workflowActivity = new WorkflowActivity(value.ToString(), WorkflowActivityType.SharePoint);
				switch (value)
				{
					case SharePointWorkflowActivity.FindValueActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ExternalListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.UpdateItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.LookupActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.CheckInItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.TodoItemTask:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.AssignedTo.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case SharePointWorkflowActivity.AddToArrayListActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.Value.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case SharePointWorkflowActivity.GroupAssignedTask:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.AssignedTo.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case SharePointWorkflowActivity.SetFieldActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.__ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.CreateItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.CopyItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ToListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.CheckOutItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.DeleteItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.UndoCheckOutItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.CollectDataTask:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.AssignedTo.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case SharePointWorkflowActivity.FindActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ExternalListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case SharePointWorkflowActivity.LookUpManagerOfActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.UserValue.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case SharePointWorkflowActivity.ApprovalTaskProcess:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case SharePointWorkflowActivity.CollectFeedbackTaskProcess:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case SharePointWorkflowActivity.OfficeTask:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case SharePointWorkflowActivity.DocSetContentsTask:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case SharePointWorkflowActivity.BuildAssignmentsXmlActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.Value.ToString(), WorkflowActivityAttributeOperation.UserMapping, WorkflowActivityAttributes.AccountId.ToString()));
						break;
					}
				}
				this.activities.Add(workflowActivity);
			}
			foreach (NintexWorkflowActivity nintexWorkflowActivity in Enum.GetValues(typeof(NintexWorkflowActivity)))
			{
				workflowActivity = new WorkflowActivity(nintexWorkflowActivity.ToString(), WorkflowActivityType.Nintex);
				switch (nintexWorkflowActivity)
				{
					case NintexWorkflowActivity.CheckInItemWithKeyActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.SetItemPermissions:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.TargetListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.CreateItemWithContentTypesActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ContentType.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case NintexWorkflowActivity.RetrieveDataActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.Key.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.StoreDataActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.Key.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.InstanceId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case NintexWorkflowActivity.QueryListActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.CamlQuery.ToString(), WorkflowActivityAttributeOperation.CAML));
						break;
					}
					case NintexWorkflowActivity.QueryUserProfileStoreActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.QueryUsername.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case NintexWorkflowActivity.MultiOutcomeInternal:
					case NintexWorkflowActivity.ApprovalActivityInternal2:
					case NintexWorkflowActivity.SetupCollectDataActivity:
					case NintexWorkflowActivity.AssignTodoTaskInternal:
					case NintexWorkflowActivity.CollectDataTaskAndEscalation:
					case NintexWorkflowActivity.ReviewActivityInternal2:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.TaskContentTypeId.ToString(), WorkflowActivityAttributeOperation.Workflow));
						break;
					}
					case NintexWorkflowActivity.ApproverConfig:
					case NintexWorkflowActivity.UserConfig:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.User.ToString(), WorkflowActivityAttributeOperation.Global));
						break;
					}
					case NintexWorkflowActivity.CreateSiteSpecificItemActivity:
					case NintexWorkflowActivity.CreateList2Activity:
					case NintexWorkflowActivity.UpdateMultipleItemActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.SiteId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.WebId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.CopyToSharepointSite2Activity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.TargetFolderId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.TargetSiteId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.TargetWebId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.CreateWeb2Activity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.SiteId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ParentWeb.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.DeleteWeb2Activity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.SiteId.ToString(), WorkflowActivityAttributeOperation.Guid));
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.ParentWebId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
					case NintexWorkflowActivity.UpdateItemWhenReadyActivity:
					{
						workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.__ListId.ToString(), WorkflowActivityAttributeOperation.Guid));
						break;
					}
				}
				this.activities.Add(workflowActivity);
			}
			foreach (string activite in NintexOnlineWorkflowActivity.GetActivites())
			{
				workflowActivity = new WorkflowActivity(activite, WorkflowActivityType.NintexOnline);
				string str = activite;
				if (str != null)
				{
					switch (str)
					{
						case "Microsoft.SharePoint.WorkflowServices.Activities.CreateListItem":
						case "Microsoft.SharePoint.WorkflowServices.Activities.DeleteListItem":
						case "Microsoft.SharePoint.WorkflowServices.Activities.CheckOutItem":
						case "Microsoft.SharePoint.WorkflowServices.Activities.UndoCheckOutItem":
						case "#QuerySPList":
						case "#RunIf":
						case "Microsoft.SharePoint.WorkflowServices.Activities.SetField":
						case "#ConditionalBranch":
						case "#LoopCondition":
						case "Microsoft.SharePoint.WorkflowServices.Activities.DelayUntil":
						case "System.Activities.Statements.Assign":
						case "Microsoft.SharePoint.WorkflowServices.Activities.UpdateListItem":
						case "#Filter":
						case "Microsoft.SharePoint.WorkflowServices.Activities.Calc":
						case "Microsoft.SharePoint.WorkflowServices.Activities.WaitForFieldChange":
						{
							workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.SelectList.ToString(), WorkflowActivityAttributeOperation.Guid));
							goto Label0;
						}
						case "Microsoft.SharePoint.WorkflowServices.Activities.CompositeTask":
						case "#SendNotificationActivity":
						{
							workflowActivity.Attributes.Add(new WorkflowActivityAttribute(WorkflowActivityAttributes.Login.ToString(), WorkflowActivityAttributeOperation.UserMapping));
							goto Label0;
						}
					}
				}
			Label0:
				this.activities.Add(workflowActivity);
			}
		}

		public bool HaveDefaultSupportedActivitesChanged()
		{
			bool flag;
			bool version = false;
			version = this.Version != typeof(WorkflowActivityCollection).Assembly.GetName().Version.ToString();
			if (!version)
			{
				WorkflowActivityCollection workflowActivityCollection = new WorkflowActivityCollection();
				workflowActivityCollection.PopulateDefaultSupportedActivities();
				foreach (WorkflowActivity activity in workflowActivityCollection.activities)
				{
					int count = this.activities.FindAll((WorkflowActivity e) => e.Name.Equals(activity.Name, StringComparison.OrdinalIgnoreCase)).Count;
					if ((count == 0 ? false : count <= 1))
					{
						if (!version)
						{
							WorkflowActivity item = this[activity.Name];
							if (item.Attributes.Count < activity.Attributes.Count)
							{
								flag = true;
							}
							else
							{
								flag = (item.Name != activity.Name ? true : item.ActivityType != activity.ActivityType);
							}
							version = flag;
							if (!version)
							{
								foreach (WorkflowActivityAttribute attribute in activity.Attributes)
								{
									int num = item.Attributes.FindAll((WorkflowActivityAttribute e) => e.Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase)).Count;
									version = (num == 0 ? true : num > 1);
									if (!version)
									{
										WorkflowActivityAttribute workflowActivityAttribute = item.Attributes.Find((WorkflowActivityAttribute e) => e.Name.Equals(attribute.Name, StringComparison.OrdinalIgnoreCase));
										version = (workflowActivityAttribute.Name != attribute.Name ? true : workflowActivityAttribute.Operation != attribute.Operation);
									}
									if (version)
									{
										break;
									}
								}
							}
						}
						if (version)
						{
							break;
						}
					}
					else
					{
						version = true;
						break;
					}
				}
			}
			return version;
		}

		public void PopulateDefaultSupportedActivities()
		{
			if (this.activities.Count > 0)
			{
				this.activities.Clear();
			}
			this.AddDefaultSupportedActivities();
		}

		public void UpdateDefaultSupportedActivites()
		{
			this.version = null;
			WorkflowActivityCollection workflowActivityCollection = new WorkflowActivityCollection();
			workflowActivityCollection.PopulateDefaultSupportedActivities();
			foreach (WorkflowActivity activity in workflowActivityCollection.activities)
			{
				this.activities.RemoveAll((WorkflowActivity e) => e.Name.Equals(activity.Name, StringComparison.OrdinalIgnoreCase));
			}
			this.AddDefaultSupportedActivities();
		}
	}
}