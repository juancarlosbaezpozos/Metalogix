using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class MigrationMessageBase
	{
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

		public string Time
		{
			get;
			set;
		}

		public MigrationMessageBase()
		{
		}
	}
}