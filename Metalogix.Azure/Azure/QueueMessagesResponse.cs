using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class QueueMessagesResponse : Response
	{
		public List<QueueMessageResponse> Messages
		{
			get;
			set;
		}

		public QueueMessagesResponse()
		{
			this.Messages = new List<QueueMessageResponse>();
		}
	}
}