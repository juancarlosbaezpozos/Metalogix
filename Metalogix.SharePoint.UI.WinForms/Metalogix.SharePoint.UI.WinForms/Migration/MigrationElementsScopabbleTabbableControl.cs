using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.UI.WinForms.Mapping;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class MigrationElementsScopabbleTabbableControl : ScopableTabbableControl
	{
		private SPTermStoreCollection _sourceTermstores;

		private SPTermStoreCollection _targetTermstores;

		private IContainer components;

		public Type ActionType { get; set; }

		public virtual bool IsBasicMode => false;

		public SharePointVersion SourceSharePointVersion
		{
			get
			{
				if (SourceNodes == null || SourceNodes.Count == 0)
				{
					return null;
				}
				if (!(SourceNodes[0] is SPNode sPNode))
				{
					return null;
				}
				return sPNode.Adapter.SharePointVersion;
			}
		}

		public SPTermStoreCollection SourceTermstores
		{
			get
			{
				SPTermStoreCollection sPTermStoreCollection = _sourceTermstores;
				if (sPTermStoreCollection == null)
				{
					sPTermStoreCollection = (_sourceTermstores = GetTermStoresFromNode(SourceNodes[0]));
				}
				return sPTermStoreCollection;
			}
			set
			{
				_sourceTermstores = value;
			}
		}

		public SPWeb SourceWeb
		{
			get
			{
				if (SourceNodes == null || SourceNodes.Count == 0)
				{
					return null;
				}
				return SourceNodes[0] as SPWeb;
			}
		}

		public SharePointVersion TargetSharePointVersion
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count == 0)
				{
					return null;
				}
				if (!(TargetNodes[0] is SPNode sPNode))
				{
					return null;
				}
				return sPNode.Adapter.SharePointVersion;
			}
		}

		public SPTermStoreCollection TargetTermstores
		{
			get
			{
				SPTermStoreCollection sPTermStoreCollection = _targetTermstores;
				if (sPTermStoreCollection == null)
				{
					sPTermStoreCollection = (_targetTermstores = GetTermStoresFromNode(TargetNodes[0]));
				}
				return sPTermStoreCollection;
			}
			set
			{
				_targetTermstores = value;
			}
		}

		public MigrationElementsScopabbleTabbableControl()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public SPWeb GetHighestNonServerNode(Node node)
		{
			if (node.Parent == null || object.Equals(node.Parent, node) || node.Parent is SPServer)
			{
				return node as SPWeb;
			}
			return GetHighestNonServerNode(node.Parent);
		}

		private SPTermStoreCollection GetTermStoresFromNode(object node)
		{
			SPTermStoreCollection sPTermStoreCollection = null;
			if (node is SPBaseServer)
			{
				sPTermStoreCollection = (node as SPBaseServer).TermStores;
			}
			else if (node is SPWeb)
			{
				sPTermStoreCollection = (node as SPWeb).TermStores;
			}
			else if (node is SPList)
			{
				if ((node as SPList).ParentWeb != null)
				{
					sPTermStoreCollection = (node as SPList).ParentWeb.TermStores;
				}
			}
			else if (node is SPFolder)
			{
				if ((node as SPFolder).ParentList != null && (node as SPFolder).ParentList.ParentWeb != null)
				{
					sPTermStoreCollection = (node as SPFolder).ParentList.ParentWeb.TermStores;
				}
			}
			else if (node is SPListItem && (node as SPListItem).ParentList != null && (node as SPListItem).ParentList.ParentWeb != null)
			{
				sPTermStoreCollection = (node as SPListItem).ParentList.ParentWeb.TermStores;
			}
			if (sPTermStoreCollection == null)
			{
				throw new ArgumentNullException("Unable to obtain Target Termstores. termStores is null.");
			}
			return sPTermStoreCollection;
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "MigrationElementsScopabbleTabbableControl";
			base.ResumeLayout(false);
		}

		public bool IsTaxonomySupported()
		{
			SPNode sPNode = null;
			if (SourceNodes != null)
			{
				sPNode = SourceNodes[0] as SPNode;
			}
			SPNode sPNode2 = null;
			if (TargetNodes != null)
			{
				sPNode2 = TargetNodes[0] as SPNode;
			}
			if (sPNode == null || !sPNode.Adapter.SharePointVersion.IsSharePoint2010OrLater || !sPNode.Adapter.SupportsTaxonomy)
			{
				return false;
			}
			if (sPNode2 == null || !sPNode2.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				return false;
			}
			return sPNode2.Adapter.SupportsTaxonomy;
		}

		public void OpenUserMappingDialog()
		{
			string sSource = null;
			if (SourceNodes != null && SourceNodes.Count > 0)
			{
				Node node = (Node)SourceNodes[0];
				SPWeb highestNonServerNode = GetHighestNonServerNode(node);
				if (highestNonServerNode != null)
				{
					sSource = highestNonServerNode.DisplayUrl;
				}
			}
			string sTarget = null;
			if (TargetNodes != null && TargetNodes.Count > 0)
			{
				Node node2 = (Node)TargetNodes[0];
				SPWeb highestNonServerNode2 = GetHighestNonServerNode(node2);
				if (highestNonServerNode2 != null)
				{
					sTarget = highestNonServerNode2.DisplayUrl;
				}
			}
			SPGlobalMappingDialog sPGlobalMappingDialog = new SPGlobalMappingDialog(sSource, sTarget, IsBasicMode);
			sPGlobalMappingDialog.ShowDialog();
		}

		public bool SourceAndTargetAreTenant()
		{
			if (SourceNodes == null || TargetNodes == null)
			{
				return false;
			}
			SPNode sPNode = SourceNodes[0] as SPTenant;
			SPNode sPNode2 = TargetNodes[0] as SPTenant;
			if (sPNode == null)
			{
				return false;
			}
			return sPNode2 != null;
		}

		public bool TermstoresCanBeAutomaticallyMapped()
		{
			if (SourceTermstores == null || TargetTermstores == null || SourceTermstores.Count != 1)
			{
				return false;
			}
			return TargetTermstores.Count == 1;
		}
	}
}
