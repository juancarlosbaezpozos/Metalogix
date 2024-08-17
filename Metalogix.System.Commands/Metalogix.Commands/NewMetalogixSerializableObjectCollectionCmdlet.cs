using Metalogix.Actions;
using System;
using System.Management.Automation;

namespace Metalogix.Commands
{
	public class NewMetalogixSerializableObjectCollectionCmdlet
	{
		public NewMetalogixSerializableObjectCollectionCmdlet()
		{
		}

		[Cmdlet("New", "MetalogixSerializableObjectCollection")]
		public class NewMetalogixSerializableObjectCollection : AssemblyBindingCmdlet
		{
			private string m_sXML;

			[Parameter(Mandatory=true, Position=0, HelpMessage="The XML representation of a Metalogix object collection. Generally this will be obtained by requesting the PowerShell command for a job configured in the GUI. This cmdlet is not recommended for general use.")]
			public string SerializedValue
			{
				get
				{
					return this.m_sXML;
				}
				set
				{
					this.m_sXML = value;
				}
			}

			public NewMetalogixSerializableObjectCollection()
			{
			}

			protected override void BeginProcessing()
			{
				base.BeginProcessing();
				base.WriteObject(XMLAbleList.CreateIXMLAbleList(this.m_sXML));
			}

			protected override void EndProcessing()
			{
				base.EndProcessing();
			}
		}
	}
}