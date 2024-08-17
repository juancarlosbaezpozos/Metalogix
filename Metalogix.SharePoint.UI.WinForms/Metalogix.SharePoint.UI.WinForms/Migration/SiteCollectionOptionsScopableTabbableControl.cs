using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class SiteCollectionOptionsScopableTabbableControl : ScopableTabbableControl
	{
		private SPWebTemplateCollection _targetWebTemplates;

		private IContainer components;

		protected SPNode FirstSourceNode
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

		protected SharePointAdapter SourceAdapter
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

		public SPBaseServer Target
		{
			get
			{
				if (this.TargetNodes == null || this.TargetNodes.Count == 0)
				{
					return null;
				}
				return this.TargetNodes[0] as SPBaseServer;
			}
		}

		public override NodeCollection TargetNodes
		{
			get
			{
				return base.TargetNodes;
			}
			set
			{
				base.TargetNodes = value;
				string str = this.UpdateServerData();
				if (str.Length > 0)
				{
					throw new Exception(str);
				}
			}
		}

		public SPWebTemplateCollection TargetWebTemplates
		{
			get
			{
				return this._targetWebTemplates;
			}
			set
			{
				this._targetWebTemplates = value;
				if (value != null)
				{
					this.UpdateTemplateUI();
				}
			}
		}

		public SiteCollectionOptionsScopableTabbableControl()
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

		protected string GetExperinceVersionLabel(int value)
		{
			if (this.Target == null)
			{
				return value.ToString();
			}
			if (this.Target.Adapter.SharePointVersion.IsSharePoint2013OrLater)
			{
				if (value == 15)
				{
					return "2013";
				}
				if (value == 14)
				{
					return "2010";
				}
			}
			return value.ToString();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		protected virtual string UpdateServerData()
		{
			return string.Empty;
		}

		protected virtual void UpdateTemplateUI()
		{
		}
	}
}