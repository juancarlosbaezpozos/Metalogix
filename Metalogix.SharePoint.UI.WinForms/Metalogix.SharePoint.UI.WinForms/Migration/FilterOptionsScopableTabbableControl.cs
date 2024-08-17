using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class FilterOptionsScopableTabbableControl : ScopableTabbableControl
	{
		private IContainer components;

		protected bool SourceIsServerNode
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count == 0)
				{
					return false;
				}
				if (this.SourceNodes[0] is SPBaseServer)
				{
					return true;
				}
				return false;
			}
		}

		protected SPList SourceList
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count == 0)
				{
					return null;
				}
				if (this.SourceNodes[0] is SPFolder)
				{
					return ((SPFolder)this.SourceNodes[0]).ParentList;
				}
				if (!(this.SourceNodes[0] is SPListItem))
				{
					return null;
				}
				return ((SPListItem)this.SourceNodes[0]).ParentList;
			}
		}

		protected SPWeb SourceWeb
		{
			get
			{
				if (this.SourceNodes == null || this.SourceNodes.Count == 0)
				{
					return null;
				}
				if (!(this.SourceNodes[0] is SPWeb))
				{
					return null;
				}
				return (SPWeb)this.SourceNodes[0];
			}
		}

		public FilterOptionsScopableTabbableControl()
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