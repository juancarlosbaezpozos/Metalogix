using Metalogix.SharePoint.Actions.Migration.Pipeline;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline.Events
{
	public class JobProgressEvent : IEvent
	{
		public string BytesProcessed
		{
			get;
			set;
		}

		public string CorrelationId
		{
			get;
			set;
		}

		public string CreatedOrUpdatedFileStatsBySize
		{
			get;
			set;
		}

		public string Event
		{
			get;
			set;
		}

		public string FilesCreated
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

		public string ObjectsProcessed
		{
			get;
			set;
		}

		public string ObjectsStatsByType
		{
			get;
			set;
		}

		public string Time
		{
			get;
			set;
		}

		public string TotalErrors
		{
			get;
			set;
		}

		public string TotalExpectedSPObjects
		{
			get;
			set;
		}

		public string TotalRetryCount
		{
			get;
			set;
		}

		public string TotalWarnings
		{
			get;
			set;
		}

		public JobProgressEvent()
		{
		}
	}
}