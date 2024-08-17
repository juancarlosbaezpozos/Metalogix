using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.SharePoint.Adapters;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Mapping;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.UrlMapping32.ico")]
	[ControlName("Url Mappings")]
	public class TCGlobalUrlMappings : ScopableTabbableControl
	{
		private IContainer components;

		private BasicTextMappingControl w_btmcMappings;

		public TCGlobalUrlMappings()
		{
			InitializeComponent();
			w_btmcMappings.SourceInputValidator = IsValidUrl;
			w_btmcMappings.TargetInputValidator = IsValidUrl;
			w_btmcMappings.SourceUniquenessComparison = CompareUrls;
			LoadUI();
		}

		public void AppendMappings(IEnumerable<KeyValuePair<string, string>> mappings)
		{
			w_btmcMappings.AppendMappings(mappings);
		}

		private int CompareUrls(string sSource, string sTarget)
		{
			sSource = UrlUtils.StandardizeFormat(sSource);
			sTarget = UrlUtils.StandardizeFormat(sTarget);
			return string.Compare(sSource, sTarget);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGlobalUrlMappings));
			this.w_btmcMappings = new Metalogix.UI.WinForms.Data.Mapping.BasicTextMappingControl();
			base.SuspendLayout();
			this.w_btmcMappings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_btmcMappings.Location = new System.Drawing.Point(3, 3);
			this.w_btmcMappings.Name = "w_btmcMappings";
			this.w_btmcMappings.Size = new System.Drawing.Size(441, 316);
			this.w_btmcMappings.SourceColumnName = "Source Address";
			this.w_btmcMappings.SourceInputValidator = null;
			this.w_btmcMappings.SourceUniquenessComparison = null;
			this.w_btmcMappings.TabIndex = 0;
			this.w_btmcMappings.TargetColumnName = "Target Address";
			this.w_btmcMappings.TargetInputValidator = null;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_btmcMappings);
			base.Name = "TCGlobalUrlMappings";
			base.Size = new System.Drawing.Size(447, 322);
			base.ResumeLayout(false);
		}

		private bool IsValidUrl(string sInput, out string sErrorMessage)
		{
			sErrorMessage = null;
			if (string.IsNullOrEmpty(sInput))
			{
				sErrorMessage = "Cannot map an empty string.";
				return false;
			}
			if (Uri.IsWellFormedUriString(UrlUtils.StandardizeFormat(sInput), UriKind.Absolute))
			{
				return true;
			}
			sErrorMessage = "The input must be an absolute site address. (eg. http://localhost)";
			return false;
		}

		protected override void LoadUI()
		{
			w_btmcMappings.Mappings = SPGlobalMappings.GlobalUrlMappings;
		}

		public override bool SaveUI()
		{
			if (w_btmcMappings.ContainsInvalidMappings && FlatXtraMessageBox.Show("One or more link correction mappings will not be saved because the source or target value is missing or invalid.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return false;
			}
			SPGlobalMappings.GlobalUrlMappings = w_btmcMappings.Mappings;
			return true;
		}
	}
}
