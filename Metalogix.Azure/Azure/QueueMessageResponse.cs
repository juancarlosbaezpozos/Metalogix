using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class QueueMessageResponse : Response
	{
		public string Id
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public QueueMessageResponse()
		{
		}
	}
}