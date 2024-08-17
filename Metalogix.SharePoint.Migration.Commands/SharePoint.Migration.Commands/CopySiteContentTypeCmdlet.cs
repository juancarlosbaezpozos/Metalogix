using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Commands;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointContentTypes")]
	public class CopySiteContentTypeCmdlet : SharePointActionCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopySiteContentTypesAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether to copy content type out of the box workflow associations.")]
		public SwitchParameter CopyContentTypeOOBWorkflowAssociations
		{
			get
			{
				return this.PasteContentTypesOptions.CopyContentTypeOOBWorkflowAssociations;
			}
			set
			{
				this.PasteContentTypesOptions.CopyContentTypeOOBWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether SPD or Nintex created CT wfa's will be copied.")]
		public SwitchParameter CopyContentTypeSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteContentTypesOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteContentTypesOptions.CopyContentTypeSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether SPD or Nintex created list workflow associations will be copied.")]
		public SwitchParameter CopyListSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteContentTypesOptions.CopyListSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteContentTypesOptions.CopyListSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to copy referenced taxonomy.")]
		public SwitchParameter CopyReferencedManagedMetadata
		{
			get
			{
				return this.PasteContentTypesOptions.CopyReferencedManagedMetadata;
			}
			set
			{
				this.PasteContentTypesOptions.CopyReferencedManagedMetadata = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether SPD or Nintex created web wfa's will be copied.")]
		public SwitchParameter CopyWebSharePointDesignerNintexWorkflowAssociations
		{
			get
			{
				return this.PasteContentTypesOptions.CopyWebSharePointDesignerNintexWorkflowAssociations;
			}
			set
			{
				this.PasteContentTypesOptions.CopyWebSharePointDesignerNintexWorkflowAssociations = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether Content Types are filtered or not.")]
		public SwitchParameter FilterCTs
		{
			get
			{
				return this.PasteContentTypesOptions.FilterCTs;
			}
			set
			{
				this.PasteContentTypesOptions.FilterCTs = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Contains the source CTs to include.")]
		public CommonSerializableList<string> FilteredCTCollection
		{
			get
			{
				return this.PasteContentTypesOptions.FilteredCTCollection;
			}
			set
			{
				this.PasteContentTypesOptions.FilteredCTCollection = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates whether or not to map term stores within the copy.")]
		public SwitchParameter MapTermStores
		{
			get
			{
				return this.PasteContentTypesOptions.MapTermStores;
			}
			set
			{
				this.PasteContentTypesOptions.MapTermStores = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteContentTypesOptions PasteContentTypesOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteContentTypesOptions;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Defines whether to copy content types recursively or not - as selected by user.")]
		public SwitchParameter Recursive
		{
			get
			{
				return this.PasteContentTypesOptions.Recursive;
			}
			set
			{
				this.PasteContentTypesOptions.Recursive = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates that mapping of terms should be done by name and hierarchy, rather than guid.")]
		public SwitchParameter ResolveManagedMetadataByName
		{
			get
			{
				return this.PasteContentTypesOptions.ResolveManagedMetadataByName;
			}
			set
			{
				this.PasteContentTypesOptions.ResolveManagedMetadataByName = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="The term store mapping table to be used in the copy mapping.")]
		public CommonSerializableTable<string, string> TermstoreNameMappingTable
		{
			get
			{
				return this.PasteContentTypesOptions.TermstoreNameMappingTable;
			}
			set
			{
				this.PasteContentTypesOptions.MapTermStores = true;
				this.PasteContentTypesOptions.TermstoreNameMappingTable = value;
			}
		}

		public CopySiteContentTypeCmdlet()
		{
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}