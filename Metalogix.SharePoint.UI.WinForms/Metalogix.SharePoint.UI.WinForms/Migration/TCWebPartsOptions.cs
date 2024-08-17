using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.WebPartsOptions.png")]
	[ControlName("Web Parts Options")]
	public class TCWebPartsOptions : MigrationElementsScopabbleTabbableControl
	{
		private SPWebPartOptions m_options;

		private bool _showCopyWebPartsOnLandingPages = true;

		private bool _showCopyWebPartsOnWebPartPages = true;

		private bool _showCopyWebPartsOnViewPages = true;

		public bool _showCopyFormPageWebParts = true;

		private bool _showCopyWebPartsRecursive = true;

		private bool m_bCopyingItems = true;

		private bool _copyingCustomizedFormPages;

		private IContainer components;

		private PanelControl w_plSite;

		private CheckEdit w_cbSiteWebParts;

		private CheckEdit w_cbItemWebParts;

		private PanelControl w_plExistingWebParts;

		private CheckEdit w_rbCloseExistingWebParts;

		private CheckEdit w_rbPreserveExistingWebParts;

		private CheckEdit w_rbDeleteExistingWebParts;

		private LabelControl w_lblExistingProtocol;

		private PanelControl w_plWebPartPages;

		private PanelControl w_plClosedPanel;

		private CheckEdit w_cbClosedWebParts;

		private PanelControl w_plViewPanel;

		private CheckEdit w_cbViewWebParts;

		private PanelControl w_plCopyContentZoneContent;

		private CheckEdit w_cbCopyContentZoneContent;

		private CheckEdit w_cbCopyWebPartsRecursive;

		private PanelControl w_plCopyWebPartsRecursive;

		private PanelControl w_plFormPanel;

		private CheckEdit w_cbFormWebParts;

		private HelpTipButton w_helpCopyContentZoneContent;

		private HelpTipButton w_helpFormWebParts;

		public new Type ActionType { get; set; }

		private bool CopyingCustomizedFormPages
		{
			get
			{
				return _copyingCustomizedFormPages;
			}
			set
			{
				_copyingCustomizedFormPages = value;
				UpdateEnabledState();
			}
		}

		public bool CopyingItems
		{
			get
			{
				return m_bCopyingItems;
			}
			set
			{
				m_bCopyingItems = value;
				UpdateEnabledState();
			}
		}

		public SPWebPartOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public bool ShowCopyFormPageWebParts
		{
			get
			{
				return _showCopyFormPageWebParts;
			}
			set
			{
				if (ShowCopyWebPartsOnViewPages != value)
				{
					if (!value)
					{
						HideControl(w_plFormPanel);
					}
					else
					{
						ShowControl(w_plFormPanel, this);
					}
					_showCopyFormPageWebParts = value;
				}
			}
		}

		public bool ShowCopyWebPartsOnLandingPages
		{
			get
			{
				return _showCopyWebPartsOnLandingPages;
			}
			set
			{
				if (ShowCopyWebPartsOnLandingPages != value)
				{
					if (!value)
					{
						HideControl(w_plSite);
					}
					else
					{
						ShowControl(w_plSite, this);
					}
					_showCopyWebPartsOnLandingPages = value;
				}
			}
		}

		public bool ShowCopyWebPartsOnViewPages
		{
			get
			{
				return _showCopyWebPartsOnViewPages;
			}
			set
			{
				if (ShowCopyWebPartsOnViewPages != value)
				{
					if (!value)
					{
						HideControl(w_plViewPanel);
					}
					else
					{
						ShowControl(w_plViewPanel, this);
					}
					_showCopyWebPartsOnViewPages = value;
				}
			}
		}

		public bool ShowCopyWebPartsOnWebPartPages
		{
			get
			{
				return _showCopyWebPartsOnWebPartPages;
			}
			set
			{
				if (ShowCopyWebPartsOnWebPartPages != value)
				{
					if (!value)
					{
						HideControl(w_plWebPartPages);
					}
					else
					{
						ShowControl(w_plWebPartPages, this);
					}
					_showCopyWebPartsOnWebPartPages = value;
				}
			}
		}

		public bool ShowCopyWebPartsRecursive
		{
			get
			{
				return _showCopyWebPartsRecursive;
			}
			set
			{
				if (ShowCopyWebPartsRecursive != value)
				{
					if (!value)
					{
						HideControl(w_plCopyWebPartsRecursive);
					}
					else
					{
						ShowControl(w_plCopyWebPartsRecursive, this);
					}
					_showCopyWebPartsRecursive = value;
				}
			}
		}

		private new SharePointVersion SourceSharePointVersion
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

		private new SharePointVersion TargetSharePointVersion
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

		public TCWebPartsOptions()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpCopyContentZoneContent.SetResourceString(type.FullName + w_cbCopyContentZoneContent.Name, type);
			w_helpFormWebParts.SetResourceString(type.FullName + w_cbFormWebParts.Name, type);
		}

		private void DisableWebpartOptions()
		{
			w_cbSiteWebParts.Checked = false;
			w_cbItemWebParts.Checked = false;
			w_cbViewWebParts.Checked = false;
			w_cbClosedWebParts.Checked = false;
			w_cbCopyContentZoneContent.Checked = false;
			w_cbCopyWebPartsRecursive.Checked = false;
			w_cbFormWebParts.Checked = false;
			w_cbSiteWebParts.Enabled = false;
			w_cbItemWebParts.Enabled = false;
			w_cbViewWebParts.Enabled = false;
			w_cbClosedWebParts.Enabled = false;
			w_cbCopyContentZoneContent.Enabled = false;
			w_cbCopyWebPartsRecursive.Enabled = false;
			w_cbFormWebParts.Enabled = false;
			w_rbDeleteExistingWebParts.Enabled = false;
			w_rbCloseExistingWebParts.Enabled = false;
			w_rbPreserveExistingWebParts.Enabled = false;
			w_rbCloseExistingWebParts.Checked = false;
			w_rbPreserveExistingWebParts.Checked = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			if (sMessage == "CopySPDCustomizedFormPages")
			{
				CopyingCustomizedFormPages = (bool)oValue;
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCWebPartsOptions));
			this.w_plFormPanel = new DevExpress.XtraEditors.PanelControl();
			this.w_plSite = new DevExpress.XtraEditors.PanelControl();
			this.w_cbSiteWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbItemWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_plExistingWebParts = new DevExpress.XtraEditors.PanelControl();
			this.w_lblExistingProtocol = new DevExpress.XtraEditors.LabelControl();
			this.w_rbCloseExistingWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbPreserveExistingWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbDeleteExistingWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_plWebPartPages = new DevExpress.XtraEditors.PanelControl();
			this.w_plClosedPanel = new DevExpress.XtraEditors.PanelControl();
			this.w_cbClosedWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_plViewPanel = new DevExpress.XtraEditors.PanelControl();
			this.w_cbViewWebParts = new DevExpress.XtraEditors.CheckEdit();
			this.w_plCopyContentZoneContent = new DevExpress.XtraEditors.PanelControl();
			this.w_helpCopyContentZoneContent = new TooltipsTest.HelpTipButton();
			this.w_cbCopyContentZoneContent = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyWebPartsRecursive = new DevExpress.XtraEditors.CheckEdit();
			this.w_plCopyWebPartsRecursive = new DevExpress.XtraEditors.PanelControl();
			this.w_helpFormWebParts = new TooltipsTest.HelpTipButton();
			this.w_cbFormWebParts = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_plFormPanel).BeginInit();
			this.w_plFormPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plSite).BeginInit();
			this.w_plSite.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbSiteWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbItemWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plExistingWebParts).BeginInit();
			this.w_plExistingWebParts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_rbCloseExistingWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveExistingWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbDeleteExistingWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebPartPages).BeginInit();
			this.w_plWebPartPages.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plClosedPanel).BeginInit();
			this.w_plClosedPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbClosedWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plViewPanel).BeginInit();
			this.w_plViewPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbViewWebParts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCopyContentZoneContent).BeginInit();
			this.w_plCopyContentZoneContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyContentZoneContent).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyContentZoneContent.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyWebPartsRecursive.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCopyWebPartsRecursive).BeginInit();
			this.w_plCopyWebPartsRecursive.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpFormWebParts).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFormWebParts.Properties).BeginInit();
			base.SuspendLayout();
			this.w_plFormPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plFormPanel.Controls.Add(this.w_helpFormWebParts);
			this.w_plFormPanel.Controls.Add(this.w_cbFormWebParts);
			resources.ApplyResources(this.w_plFormPanel, "w_plFormPanel");
			this.w_plFormPanel.Name = "w_plFormPanel";
			resources.ApplyResources(this.w_plSite, "w_plSite");
			this.w_plSite.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSite.Controls.Add(this.w_cbSiteWebParts);
			this.w_plSite.Name = "w_plSite";
			resources.ApplyResources(this.w_cbSiteWebParts, "w_cbSiteWebParts");
			this.w_cbSiteWebParts.Name = "w_cbSiteWebParts";
			this.w_cbSiteWebParts.Properties.AutoWidth = true;
			this.w_cbSiteWebParts.Properties.Caption = resources.GetString("w_cbSiteWebParts.Properties.Caption");
			this.w_cbSiteWebParts.CheckedChanged += new System.EventHandler(On_WebParts_CheckedChanged);
			resources.ApplyResources(this.w_cbItemWebParts, "w_cbItemWebParts");
			this.w_cbItemWebParts.Name = "w_cbItemWebParts";
			this.w_cbItemWebParts.Properties.AutoWidth = true;
			this.w_cbItemWebParts.Properties.Caption = resources.GetString("w_cbItemWebParts.Properties.Caption");
			this.w_cbItemWebParts.CheckedChanged += new System.EventHandler(On_WebParts_CheckedChanged);
			resources.ApplyResources(this.w_plExistingWebParts, "w_plExistingWebParts");
			this.w_plExistingWebParts.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plExistingWebParts.Controls.Add(this.w_lblExistingProtocol);
			this.w_plExistingWebParts.Controls.Add(this.w_rbCloseExistingWebParts);
			this.w_plExistingWebParts.Controls.Add(this.w_rbPreserveExistingWebParts);
			this.w_plExistingWebParts.Controls.Add(this.w_rbDeleteExistingWebParts);
			this.w_plExistingWebParts.Name = "w_plExistingWebParts";
			resources.ApplyResources(this.w_lblExistingProtocol, "w_lblExistingProtocol");
			this.w_lblExistingProtocol.Name = "w_lblExistingProtocol";
			resources.ApplyResources(this.w_rbCloseExistingWebParts, "w_rbCloseExistingWebParts");
			this.w_rbCloseExistingWebParts.Name = "w_rbCloseExistingWebParts";
			this.w_rbCloseExistingWebParts.Properties.Caption = resources.GetString("w_rbCloseExistingWebParts.Properties.Caption");
			this.w_rbCloseExistingWebParts.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCloseExistingWebParts.Properties.RadioGroupIndex = 1;
			this.w_rbCloseExistingWebParts.TabStop = false;
			resources.ApplyResources(this.w_rbPreserveExistingWebParts, "w_rbPreserveExistingWebParts");
			this.w_rbPreserveExistingWebParts.Name = "w_rbPreserveExistingWebParts";
			this.w_rbPreserveExistingWebParts.Properties.Caption = resources.GetString("w_rbPreserveExistingWebParts.Properties.Caption");
			this.w_rbPreserveExistingWebParts.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbPreserveExistingWebParts.Properties.RadioGroupIndex = 1;
			this.w_rbPreserveExistingWebParts.TabStop = false;
			resources.ApplyResources(this.w_rbDeleteExistingWebParts, "w_rbDeleteExistingWebParts");
			this.w_rbDeleteExistingWebParts.Name = "w_rbDeleteExistingWebParts";
			this.w_rbDeleteExistingWebParts.Properties.Caption = resources.GetString("w_rbDeleteExistingWebParts.Properties.Caption");
			this.w_rbDeleteExistingWebParts.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbDeleteExistingWebParts.Properties.RadioGroupIndex = 1;
			resources.ApplyResources(this.w_plWebPartPages, "w_plWebPartPages");
			this.w_plWebPartPages.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plWebPartPages.Controls.Add(this.w_cbItemWebParts);
			this.w_plWebPartPages.Name = "w_plWebPartPages";
			resources.ApplyResources(this.w_plClosedPanel, "w_plClosedPanel");
			this.w_plClosedPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plClosedPanel.Controls.Add(this.w_cbClosedWebParts);
			this.w_plClosedPanel.Name = "w_plClosedPanel";
			resources.ApplyResources(this.w_cbClosedWebParts, "w_cbClosedWebParts");
			this.w_cbClosedWebParts.Name = "w_cbClosedWebParts";
			this.w_cbClosedWebParts.Properties.AutoWidth = true;
			this.w_cbClosedWebParts.Properties.Caption = resources.GetString("w_cbClosedWebParts.Properties.Caption");
			this.w_plViewPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plViewPanel.Controls.Add(this.w_cbViewWebParts);
			resources.ApplyResources(this.w_plViewPanel, "w_plViewPanel");
			this.w_plViewPanel.Name = "w_plViewPanel";
			resources.ApplyResources(this.w_cbViewWebParts, "w_cbViewWebParts");
			this.w_cbViewWebParts.Name = "w_cbViewWebParts";
			this.w_cbViewWebParts.Properties.AutoWidth = true;
			this.w_cbViewWebParts.Properties.Caption = resources.GetString("w_cbViewWebParts.Properties.Caption");
			this.w_cbViewWebParts.CheckedChanged += new System.EventHandler(w_cbViewWebParts_CheckedChanged);
			this.w_plCopyContentZoneContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plCopyContentZoneContent.Controls.Add(this.w_helpCopyContentZoneContent);
			this.w_plCopyContentZoneContent.Controls.Add(this.w_cbCopyContentZoneContent);
			resources.ApplyResources(this.w_plCopyContentZoneContent, "w_plCopyContentZoneContent");
			this.w_plCopyContentZoneContent.Name = "w_plCopyContentZoneContent";
			this.w_helpCopyContentZoneContent.AnchoringControl = this.w_cbCopyContentZoneContent;
			this.w_helpCopyContentZoneContent.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCopyContentZoneContent.CommonParentControl = null;
			resources.ApplyResources(this.w_helpCopyContentZoneContent, "w_helpCopyContentZoneContent");
			this.w_helpCopyContentZoneContent.Name = "w_helpCopyContentZoneContent";
			this.w_helpCopyContentZoneContent.TabStop = false;
			resources.ApplyResources(this.w_cbCopyContentZoneContent, "w_cbCopyContentZoneContent");
			this.w_cbCopyContentZoneContent.Name = "w_cbCopyContentZoneContent";
			this.w_cbCopyContentZoneContent.Properties.AutoWidth = true;
			this.w_cbCopyContentZoneContent.Properties.Caption = resources.GetString("w_cbCopyContentZoneContent.Properties.Caption");
			this.w_cbCopyContentZoneContent.CheckedChanged += new System.EventHandler(w_cbCopyContentZoneContent_CheckedChanged);
			resources.ApplyResources(this.w_cbCopyWebPartsRecursive, "w_cbCopyWebPartsRecursive");
			this.w_cbCopyWebPartsRecursive.Name = "w_cbCopyWebPartsRecursive";
			this.w_cbCopyWebPartsRecursive.Properties.AutoWidth = true;
			this.w_cbCopyWebPartsRecursive.Properties.Caption = resources.GetString("w_cbCopyWebPartsRecursive.Properties.Caption");
			this.w_plCopyWebPartsRecursive.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plCopyWebPartsRecursive.Controls.Add(this.w_cbCopyWebPartsRecursive);
			resources.ApplyResources(this.w_plCopyWebPartsRecursive, "w_plCopyWebPartsRecursive");
			this.w_plCopyWebPartsRecursive.Name = "w_plCopyWebPartsRecursive";
			this.w_helpFormWebParts.AnchoringControl = this.w_cbFormWebParts;
			this.w_helpFormWebParts.BackColor = System.Drawing.Color.Transparent;
			this.w_helpFormWebParts.CommonParentControl = null;
			resources.ApplyResources(this.w_helpFormWebParts, "w_helpFormWebParts");
			this.w_helpFormWebParts.Name = "w_helpFormWebParts";
			this.w_helpFormWebParts.RealOffset = null;
			this.w_helpFormWebParts.RelativeOffset = null;
			this.w_helpFormWebParts.TabStop = false;
			resources.ApplyResources(this.w_cbFormWebParts, "w_cbFormWebParts");
			this.w_cbFormWebParts.Name = "w_cbFormWebParts";
			this.w_cbFormWebParts.Properties.AutoWidth = true;
			this.w_cbFormWebParts.Properties.Caption = resources.GetString("w_cbFormWebParts.Properties.Caption");
			base.Controls.Add(this.w_plFormPanel);
			base.Controls.Add(this.w_plCopyWebPartsRecursive);
			base.Controls.Add(this.w_plCopyContentZoneContent);
			base.Controls.Add(this.w_plViewPanel);
			base.Controls.Add(this.w_plClosedPanel);
			base.Controls.Add(this.w_plWebPartPages);
			base.Controls.Add(this.w_plExistingWebParts);
			base.Controls.Add(this.w_plSite);
			base.Name = "TCWebPartsOptions";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)this.w_plFormPanel).EndInit();
			this.w_plFormPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plSite).EndInit();
			this.w_plSite.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbSiteWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbItemWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plExistingWebParts).EndInit();
			this.w_plExistingWebParts.ResumeLayout(false);
			this.w_plExistingWebParts.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_rbCloseExistingWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveExistingWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbDeleteExistingWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebPartPages).EndInit();
			this.w_plWebPartPages.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plClosedPanel).EndInit();
			this.w_plClosedPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbClosedWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plViewPanel).EndInit();
			this.w_plViewPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbViewWebParts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCopyContentZoneContent).EndInit();
			this.w_plCopyContentZoneContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyContentZoneContent).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyContentZoneContent.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyWebPartsRecursive.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCopyWebPartsRecursive).EndInit();
			this.w_plCopyWebPartsRecursive.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpFormWebParts).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbFormWebParts.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private bool IsFormWebPartsOptionAvailable()
		{
			if (SourceSharePointVersion != null && TargetSharePointVersion != null && SourceSharePointVersion.MajorVersion == TargetSharePointVersion.MajorVersion)
			{
				return true;
			}
			return false;
		}

		protected override void LoadUI()
		{
			w_cbItemWebParts.Checked = Options.CopyDocumentWebParts;
			w_cbSiteWebParts.Checked = Options.CopySiteWebParts;
			w_cbViewWebParts.Checked = Options.CopyViewWebParts;
			if (!IsFormWebPartsOptionAvailable())
			{
				w_cbFormWebParts.Enabled = false;
				w_cbFormWebParts.Checked = false;
			}
			else
			{
				w_cbFormWebParts.Enabled = true;
				w_cbFormWebParts.Checked = Options.CopyFormWebParts;
			}
			w_cbClosedWebParts.Checked = Options.CopyClosedWebParts;
			w_cbCopyContentZoneContent.Checked = Options.CopyContentZoneContent;
			w_cbCopyWebPartsRecursive.Checked = Options.CopyWebPartsRecursive;
			switch (Options.ExistingWebPartsAction)
			{
			case ExistingWebPartsProtocol.Preserve:
				w_rbPreserveExistingWebParts.Checked = true;
				break;
			case ExistingWebPartsProtocol.Delete:
				w_rbDeleteExistingWebParts.Checked = true;
				break;
			case ExistingWebPartsProtocol.Close:
				w_rbCloseExistingWebParts.Checked = true;
				break;
			default:
				FlatXtraMessageBox.Show("A non-fatal internal error has occurred, this will not affect your current operation however. Please contact support@metalogix.net for assistance. (Internal Error: Existing web part setting not handled)", "Non-fatal Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				break;
			}
			UpdateEnabledState();
			bool flag = !(ActionType == null) && ActionType == typeof(PasteMySitesAction);
			if (SourceSharePointVersion.IsSharePointOnline && TargetSharePointVersion.IsSharePointOnline && flag)
			{
				DisableWebpartOptions();
			}
		}

		private void On_WebParts_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		public override bool SaveUI()
		{
			Options.CopyDocumentWebParts = w_cbItemWebParts.Checked;
			Options.CopySiteWebParts = w_cbSiteWebParts.Checked;
			Options.CopyViewWebParts = w_cbViewWebParts.Checked;
			Options.CopyFormWebParts = w_cbFormWebParts.Checked;
			Options.CopyClosedWebParts = w_cbClosedWebParts.Checked;
			Options.CopyContentZoneContent = w_cbCopyContentZoneContent.Checked;
			Options.CopyWebPartsRecursive = w_cbCopyWebPartsRecursive.Checked;
			if (w_rbDeleteExistingWebParts.Checked)
			{
				Options.ExistingWebPartsAction = ExistingWebPartsProtocol.Delete;
			}
			else if (w_rbPreserveExistingWebParts.Checked)
			{
				Options.ExistingWebPartsAction = ExistingWebPartsProtocol.Preserve;
			}
			else if (w_rbCloseExistingWebParts.Checked)
			{
				Options.ExistingWebPartsAction = ExistingWebPartsProtocol.Close;
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			w_cbSiteWebParts.Enabled = base.SitesAvailable;
			w_cbItemWebParts.Enabled = base.ItemsAvailable;
			w_cbViewWebParts.Enabled = base.ListsAvailable;
			w_cbFormWebParts.Enabled = IsFormWebPartsOptionAvailable() && base.ListsAvailable && !CopyingCustomizedFormPages;
			CheckEdit checkEdit = w_cbFormWebParts;
			bool @checked = IsFormWebPartsOptionAvailable() && (w_cbFormWebParts.Checked || CopyingCustomizedFormPages);
			checkEdit.Checked = @checked;
			w_cbClosedWebParts.Enabled = base.SitesAvailable || base.ListsAvailable || base.ItemsAvailable;
			bool flag = (w_cbItemWebParts.Checked && w_cbItemWebParts.Enabled && w_cbItemWebParts.Visible) || (w_cbViewWebParts.Checked && w_cbViewWebParts.Enabled && w_cbViewWebParts.Visible) || (w_cbSiteWebParts.Enabled && w_cbSiteWebParts.Checked && w_cbSiteWebParts.Visible && (base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site)) || (!w_cbItemWebParts.Visible && !w_cbViewWebParts.Visible && !w_cbSiteWebParts.Visible);
			bool flag2 = flag;
			bool flag3 = SourceSharePointVersion != null && TargetSharePointVersion != null && SourceSharePointVersion.IsSharePoint2010OrLater && TargetSharePointVersion.IsSharePoint2010OrLater;
			bool flag4 = flag3;
			w_cbClosedWebParts.Enabled = flag2;
			w_cbCopyContentZoneContent.Enabled = flag2 && flag4;
			w_cbCopyContentZoneContent.Checked = w_cbCopyContentZoneContent.Checked && flag4;
			if (w_cbCopyContentZoneContent.Checked)
			{
				w_rbDeleteExistingWebParts.Checked = true;
			}
			if ((w_cbCopyContentZoneContent.Checked && w_cbCopyContentZoneContent.Enabled) || !flag2)
			{
				w_plExistingWebParts.Enabled = false;
			}
			else
			{
				w_plExistingWebParts.Enabled = true;
			}
		}

		protected override void UpdateScope()
		{
			if (base.Scope != SharePointObjectScope.Site && base.Scope != SharePointObjectScope.SiteCollection)
			{
				HideControl(w_plSite);
			}
			if (base.Scope == SharePointObjectScope.Item || base.Scope == SharePointObjectScope.Folder || base.Scope == SharePointObjectScope.Permissions)
			{
				HideControl(w_plViewPanel);
				HideControl(w_plFormPanel);
			}
		}

		private void w_cbCopyContentZoneContent_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void w_cbViewWebParts_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
