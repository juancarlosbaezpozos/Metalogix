using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.UI.WinForms;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class SiteContentOptionsScopableTabbableControl : ScopableTabbableControl
	{
		public Type ActionType
		{
			get;
			set;
		}

		public SPNode FirstSourceNode
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count == 0)
				{
					return null;
				}
				return this.SourceNodes[0] as SPNode;
			}
		}

		public SPNode FirstTargetNode
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count == 0)
				{
					return null;
				}
				return this.TargetNodes[0] as SPNode;
			}
		}

		public SharePointAdapter SourceAdapter
		{
			get
			{
				if (this.FirstSourceNode == null)
				{
					return null;
				}
				return this.FirstSourceNode.Adapter;
			}
		}

		public SPWeb SourceWeb
		{
			get
			{
				return this.FirstSourceNode as SPWeb;
			}
		}

		public SharePointAdapter TargetAdapter
		{
			get
			{
				if (this.FirstTargetNode == null)
				{
					return null;
				}
				return this.FirstTargetNode.Adapter;
			}
		}

		public bool TargetIsOMAdapter
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count == 0 || !(this.TargetNodes[0] is SPNode))
				{
					return true;
				}
				return !((SPNode)this.TargetNodes[0]).Adapter.IsNws;
			}
		}

		public SPWeb TargetWeb
		{
			get
			{
				return this.FirstTargetNode as SPWeb;
			}
		}

		public SiteContentOptionsScopableTabbableControl()
		{
		}
	}
}