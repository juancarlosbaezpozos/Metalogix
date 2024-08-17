using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Reporting
{
	internal struct LogItem
	{
		private DateTime _finishedTime;

		internal int Day
		{
			get
			{
				return this.TimeStamp.Day;
			}
		}

		internal string Details
		{
			get;
			set;
		}

		internal TimeSpan Duration
		{
			get
			{
				return this.FinishedTime.Subtract(this.TimeStamp);
			}
		}

		internal DateTime FinishedTime
		{
			get
			{
				return this._finishedTime;
			}
			set
			{
				DateTime dateTime = new DateTime();
				this._finishedTime = (value == dateTime ? this.TimeStamp : value);
			}
		}

		internal int Hour
		{
			get
			{
				return this.TimeStamp.Hour;
			}
		}

		internal string Information
		{
			get;
			set;
		}

		internal string ItemName
		{
			get;
			set;
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

		internal string LogItemId
		{
			get;
			set;
		}

		internal int Minute
		{
			get
			{
				return this.TimeStamp.Minute;
			}
		}

		internal int Month
		{
			get
			{
				return this.TimeStamp.Month;
			}
		}

		internal string Operation
		{
			get;
			set;
		}

		internal string Source
		{
			get;
			set;
		}

		internal string SourceContent
		{
			get;
			set;
		}

		internal string Status
		{
			get;
			set;
		}

		internal string Target
		{
			get;
			set;
		}

		internal string TargetContent
		{
			get;
			set;
		}

		internal DateTime TimeStamp
		{
			get;
			set;
		}
	}
}