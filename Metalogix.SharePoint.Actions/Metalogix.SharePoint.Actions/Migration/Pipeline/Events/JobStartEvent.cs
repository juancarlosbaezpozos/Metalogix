using Metalogix.SharePoint.Actions.Migration.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline.Events
{
	public class JobStartEvent : IEvent
	{
		public string CorrelationId
		{
			get;
			set;
		}

		public string DBId
		{
			get;
			set;
		}

		public string Event
		{
			get;
			set;
		}

		public string FarmId
		{
			get;
			set;
		}

		public string JobId
		{
			get;
			set;
		}

		public string MigrationDirection
		{
			get;
			set;
		}

		public string MigrationType
		{
			get;
			set;
		}

		public string ServerId
		{
			get;
			set;
		}

		public string SiteId
		{
			get;
			set;
		}

		public string SubscriptionId
		{
			get;
			set;
		}

		public string Time
		{
			get;
			set;
		}

		public string TotalRetryCount
		{
			get;
			set;
		}

		public string WebId
		{
			get;
			set;
		}

		public JobStartEvent()
		{
		}
	}
}