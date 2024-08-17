using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class ADGraphUserResponse : Response
	{
		public bool BadRequestStatus
		{
			get;
			set;
		}

		public bool UserNotFoundStatus
		{
			get;
			set;
		}

		public ADGraphUserResponse()
		{
		}
	}
}