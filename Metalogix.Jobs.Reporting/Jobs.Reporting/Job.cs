using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Reporting
{
	internal struct Job
	{
		private DateTime _created;

		private DateTime _finished;

		internal string Action
		{
			get;
			set;
		}

		internal DateTime Created
		{
			get
			{
				return this._created;
			}
			set
			{
				DateTime dateTime = new DateTime();
				this._created = (value == dateTime ? this.Started : value);
			}
		}

		internal string CreatedBy
		{
			get;
			set;
		}

		internal TimeSpan Duration
		{
			get
			{
				return this.Finished.Subtract(this.Started);
			}
		}

		internal DateTime Finished
		{
			get
			{
				return this._finished;
			}
			set
			{
				DateTime dateTime = new DateTime();
				this._finished = (value == dateTime ? this.Started : value);
			}
		}

		internal string JobId
		{
			get;
			set;
		}

		internal long LicensedDataUsed
		{
			get;
			set;
		}

		internal string MachineName
		{
			get;
			set;
		}

		internal string ResultsSummary
		{
			get;
			set;
		}

		internal string Source
		{
			get;
			set;
		}

		internal string SourceXml
		{
			get;
			set;
		}

		internal DateTime Started
		{
			get;
			set;
		}

		internal string Status
		{
			get;
			set;
		}

		internal string StatusMessage
		{
			get;
			set;
		}

		internal string Target
		{
			get;
			set;
		}

		internal string TargetXml
		{
			get;
			set;
		}

		internal string Title
		{
			get;
			set;
		}

		internal string UserName
		{
			get;
			set;
		}
	}
}