using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class CreateQueueResponse : Response
	{
		public string Name
		{
			get;
			set;
		}

		public string PrimaryUri
		{
			get;
			set;
		}

		public string SecondaryUri
		{
			get;
			set;
		}

		public string SharedAccessSignature
		{
			get;
			set;
		}

		public CreateQueueResponse()
		{
			this.Name = string.Empty;
			this.SharedAccessSignature = string.Empty;
			this.PrimaryUri = string.Empty;
			this.SecondaryUri = string.Empty;
		}
	}
}