using Metalogix.SharePoint.Actions.Migration.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline.Events
{
	public class JobQueuedEvent : IEvent
	{
		public string CorrelationId
		{
			get;
			set;
		}

		public string DbId
		{
			get;
			set;
		}

		public string Event
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

		public string SiteId
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

		public JobQueuedEvent()
		{
		}
	}
}