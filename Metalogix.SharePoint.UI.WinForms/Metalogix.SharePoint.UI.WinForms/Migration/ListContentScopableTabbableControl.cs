using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class ListContentScopableTabbableControl : ScopableTabbableControl
	{
		private SPListContentOptions _options;

		private IContainer components;

		public bool IsSource2010
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count <= 0)
				{
					return false;
				}
				return ((SPNode)this.SourceNodes[0]).Adapter.SharePointVersion.IsSharePoint2010OrLater;
			}
		}

		public bool IsTarget2010
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count <= 0)
				{
					return false;
				}
				return ((SPNode)this.TargetNodes[0]).Adapter.SharePointVersion.IsSharePoint2010OrLater;
			}
		}

		public bool IsTargetClientOM
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count <= 0 || !((SPNode)this.TargetNodes[0]).Adapter.SharePointVersion.IsSharePoint2010OrLater)
				{
					return false;
				}
				return ((SPNode)this.TargetNodes[0]).Adapter.IsClientOM;
			}
		}

		public bool IsTargetOMAdapter
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count == 0 || !(this.TargetNodes[0] is SPNode))
				{
					return true;
				}
				if (((SPNode)this.TargetNodes[0]).Adapter.IsNws)
				{
					return false;
				}
				return !((SPNode)this.TargetNodes[0]).Adapter.IsClientOM;
			}
		}

		public SPListContentOptions Options
		{
			get
			{
				return this._options;
			}
			set
			{
				this._options = value;
				this.LoadUI();
			}
		}

		public SPList SourceList
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count == 0 || !(this.SourceNodes[0] is SPList))
				{
					return null;
				}
				return (SPList)this.SourceNodes[0];
			}
		}

		public ListContentScopableTabbableControl()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
	}
}