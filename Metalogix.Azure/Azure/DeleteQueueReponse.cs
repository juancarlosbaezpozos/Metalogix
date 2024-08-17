using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class DeleteQueueReponse : Response
	{
		public bool Skipped
		{
			get;
			set;
		}

		public DeleteQueueReponse()
		{
		}
	}
}