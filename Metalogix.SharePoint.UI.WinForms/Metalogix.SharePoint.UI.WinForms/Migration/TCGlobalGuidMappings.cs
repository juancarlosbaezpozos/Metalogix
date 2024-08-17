using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Adapters;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Mapping;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.GuidMapping32.ico")]
	[ControlName("GUID Mappings")]
	public class TCGlobalGuidMappings : ScopableTabbableControl
	{
		private IContainer components;

		private BasicTextMappingControl w_btmcMappings;

		public TCGlobalGuidMappings()
		{
			InitializeComponent();
			w_btmcMappings.SourceInputValidator = IsValidGUID;
			w_btmcMappings.TargetInputValidator = IsValidGUID;
			w_btmcMappings.SourceUniquenessComparison = CompareGuidStrings;
			LoadUI();
		}

		public void AppendMappings(IEnumerable<KeyValuePair<string, string>> mappings)
		{
			w_btmcMappings.AppendMappings(mappings);
		}

		private int CompareGuidStrings(string sSource, string sTarget)
		{
			string text;
			if (sSource == null)
			{
				text = null;
			}
			else
			{
				char[] trimChars = new char[2] { '{', '}' };
				text = sSource.Trim(trimChars).ToLower();
			}
			sSource = text;
			string text2;
			if (sTarget == null)
			{
				text2 = null;
			}
			else
			{
				char[] trimChars2 = new char[2] { '{', '}' };
				text2 = sTarget.Trim(trimChars2).ToLower();
			}
			sTarget = text2;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGlobalGuidMappings));
			this.w_btmcMappings = new Metalogix.UI.WinForms.Data.Mapping.BasicTextMappingControl();
			base.SuspendLayout();
			this.w_btmcMappings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_btmcMappings.Location = new System.Drawing.Point(3, 3);
			this.w_btmcMappings.Name = "w_btmcMappings";
			this.w_btmcMappings.Size = new System.Drawing.Size(441, 316);
			this.w_btmcMappings.SourceColumnName = "Source GUID";
			this.w_btmcMappings.SourceInputValidator = null;
			this.w_btmcMappings.SourceUniquenessComparison = null;
			this.w_btmcMappings.TabIndex = 0;
			this.w_btmcMappings.TargetColumnName = "Target GUID";
			this.w_btmcMappings.TargetInputValidator = null;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_btmcMappings);
			base.Name = "TCGlobalGuidMappings";
			base.Size = new System.Drawing.Size(447, 322);
			base.ResumeLayout(false);
		}

		private bool IsValidGUID(string sInput, out string sErrorMessage)
		{
			sErrorMessage = null;
			if (string.IsNullOrEmpty(sInput))
			{
				sErrorMessage = "Cannot map an empty string.";
				return false;
			}
			if (Utils.IsGuid(sInput))
			{
				return true;
			}
			sErrorMessage = string.Concat("The input must be a valid GUID. (eg. ", Guid.NewGuid(), ")");
			return false;
		}

		protected override void LoadUI()
		{
			CommonSerializableTable<string, string> commonSerializableTable = new CommonSerializableTable<string, string>(SPGlobalMappings.GlobalGuidMappings.Count);
			foreach (KeyValuePair<Guid, Guid> globalGuidMapping in SPGlobalMappings.GlobalGuidMappings)
			{
				commonSerializableTable.Add(globalGuidMapping.Key.ToString("D"), globalGuidMapping.Value.ToString("D"));
			}
			w_btmcMappings.Mappings = commonSerializableTable;
		}

		public override bool SaveUI()
		{
			if (w_btmcMappings.ContainsInvalidMappings && FlatXtraMessageBox.Show("One or more GUID mappings will not be saved because the source or target value is missing or invalid.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				return false;
			}
			CommonSerializableTable<Guid, Guid> commonSerializableTable = new CommonSerializableTable<Guid, Guid>(w_btmcMappings.Mappings.Count);
			foreach (KeyValuePair<string, string> mapping in w_btmcMappings.Mappings)
			{
				commonSerializableTable.Add(new Guid(mapping.Key), new Guid(mapping.Value));
			}
			SPGlobalMappings.GlobalGuidMappings = commonSerializableTable;
			return true;
		}
	}
}
