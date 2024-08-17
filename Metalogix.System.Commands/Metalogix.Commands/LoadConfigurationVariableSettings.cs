using Metalogix;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.Commands
{
	[Cmdlet("Load", "MetalogixConfigurationVariableSettings")]
	public class LoadConfigurationVariableSettings : AssemblyBindingCmdlet
	{
		[Parameter(Mandatory=true, Position=0, HelpMessage="The full file path of the configuration variable settings file to load.")]
		public string FilePath
		{
			get;
			set;
		}

		[Parameter(Mandatory=false, Position=1, HelpMessage="The scope to load these settings into. (Default: Environment).")]
		public ResourceScope Scope
		{
			get;
			set;
		}

		public LoadConfigurationVariableSettings()
		{
			this.Scope = ResourceScope.Environment;
		}

		private IEnumerable<KeyValuePair<string, string>> LoadSettingsFromFile()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(this.FilePath);
			ThreadSafeSerializableTable<string, string> threadSafeSerializableTable = new ThreadSafeSerializableTable<string, string>();
			threadSafeSerializableTable.FromXML(xmlDocument.DocumentElement);
			return threadSafeSerializableTable;
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
			if (!File.Exists(this.FilePath))
			{
				throw new FileNotFoundException("The file path to the configuration variable settings file is invalid.", this.FilePath);
			}
			foreach (KeyValuePair<string, string> keyValuePair in this.LoadSettingsFromFile())
			{
				SetConfigurationVariable.SetConfigurationValue(this.Scope, keyValuePair.Key, keyValuePair.Value);
			}
		}
	}
}