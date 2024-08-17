using Metalogix.SharePoint.Actions.Migration.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline.Events
{
	public class JobLogFileCreateEvent : IEvent
	{
		public string CorrelationId
		{
			get;
			set;
		}

		public string Event
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public JobLogFileCreateEvent()
		{
		}
	}
}