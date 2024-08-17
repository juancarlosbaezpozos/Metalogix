using Metalogix.Commands;
using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Clear", "GlobalMappings")]
	public class ClearGlobalMappings : AssemblyBindingCmdlet
	{
		private bool _clearGuids;

		private bool _clearUrls;

		private bool _clearUsers;

		private bool _clearDomains;

		[Parameter(Mandatory=false, HelpMessage="Indicates that global domain mappings should be cleared.")]
		public SwitchParameter ClearDomainMappingss
		{
			get
			{
				return this._clearDomains;
			}
			set
			{
				this._clearDomains = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that global GUID mappings should be cleared.")]
		public SwitchParameter ClearGuidMappings
		{
			get
			{
				return this._clearGuids;
			}
			set
			{
				this._clearGuids = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that global URL mappings should be cleared.")]
		public SwitchParameter ClearUrlMappings
		{
			get
			{
				return this._clearUrls;
			}
			set
			{
				this._clearUrls = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that global user mappings should be cleared.")]
		public SwitchParameter ClearUserMappings
		{
			get
			{
				return this._clearUsers;
			}
			set
			{
				this._clearUsers = value;
			}
		}

		public ClearGlobalMappings()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
		}

		protected override void EndProcessing()
		{
			base.EndProcessing();
		}

		protected override void ProcessRecord()
		{
			if (this._clearUsers)
			{
				SPGlobalMappings.GlobalUserMappings.Clear();
			}
			if (this._clearDomains)
			{
				SPGlobalMappings.GlobalDomainMappings.Clear();
			}
			if (this._clearGuids)
			{
				SPGlobalMappings.GlobalGuidMappings.Clear();
			}
			if (this._clearUrls)
			{
				SPGlobalMappings.GlobalUrlMappings.Clear();
			}
			SPGlobalMappings.Save();
		}
	}
}