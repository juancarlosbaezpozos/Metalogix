using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class Response
	{
		public string Details
		{
			get;
			set;
		}

		public bool Success
		{
			get;
			set;
		}

		public Response()
		{
			this.Details = string.Empty;
		}
	}
}