using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Nintex.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Nintex
{
	public class SPNintexWorkflow : SPFolder
	{
		private Guid _associatedListID;

		public Guid AssociatedListID
		{
			get
			{
				Guid guid;
				if (this._associatedListID == Guid.Empty)
				{
					Node node = base.Items.FirstOrDefault<Node>((Node item) => item.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase));
					if (node != null)
					{
						string str = node["AssociatedListID"];
						if (string.IsNullOrEmpty(str))
						{
							this._associatedListID = Guid.Empty;
							guid = this._associatedListID;
							return guid;
						}
						this._associatedListID = new Guid(str);
					}
				}
				guid = this._associatedListID;
				return guid;
			}
		}

		public override string ImageName
		{
			get
			{
				return "Metalogix.SharePoint.Icons.Workflows.ico";
			}
		}

		public NintexWorkflowType WorkflowType
		{
			get
			{
				NintexWorkflowType nintexWorkflowType = NintexWorkflowType.Unknown;
				string empty = string.Empty;
				if (base.Items.ParentFolder != null)
				{
					empty = ((SPFolder)base.Items.ParentFolder).DisplayName;
				}
				if (!empty.Equals("__globallyreusable", StringComparison.InvariantCultureIgnoreCase))
				{
					foreach (SPListItem sPListItem in base.Items.OfType<SPListItem>())
					{
						if (sPListItem.Name.EndsWith(".xoml.wfconfig.xml", StringComparison.OrdinalIgnoreCase))
						{
							nintexWorkflowType = (!string.IsNullOrEmpty(sPListItem["WorkflowCategory"]) || !base.Adapter.SharePointVersion.IsSharePoint2007 ? (NintexWorkflowType)Enum.Parse(typeof(NintexWorkflowType), sPListItem["WorkflowCategory"]) : NintexWorkflowType.List);
							break;
						}
					}
				}
				else
				{
					nintexWorkflowType = NintexWorkflowType.GloballyReusable;
				}
				return nintexWorkflowType;
			}
		}

		public SPNintexWorkflow(SPList parentList, SPFolder parentFolder, XmlNode xmlNode) : base(parentList, parentFolder, xmlNode)
		{
		}

		public SPNintexWorkflow(SharePointAdapter adapter, SPNode parent) : base(adapter, parent)
		{
		}
	}
}