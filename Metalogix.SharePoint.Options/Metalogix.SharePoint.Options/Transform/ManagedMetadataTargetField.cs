using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Transform
{
	public class ManagedMetadataTargetField
	{
		public string FieldName
		{
			get;
			private set;
		}

		public string TermGroupName
		{
			get;
			private set;
		}

		public string TermSetName
		{
			get;
			private set;
		}

		public string TermstoreName
		{
			get;
			private set;
		}

		public ManagedMetadataTargetField(string fieldName, string termstoreName, string termGroupName, string termSetName)
		{
			this.FieldName = fieldName;
			this.TermstoreName = termstoreName;
			this.TermGroupName = termGroupName;
			this.TermSetName = termSetName;
		}
	}
}