using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.Audiences32.ico")]
	[ControlName("Audience Options")]
	public class TCAudienceOptions : ScopableTabbableControl
	{
		private SPAudienceOptions m_options;

		private IContainer components;

		private LabelControl w_lbAudienceQuestion;

		private CheckEdit w_rbDeleteAll;

		private CheckEdit w_rbOverwrite;

		private CheckEdit w_rbPreserve;

		private CheckEdit w_cbStartAudienceComiplation;

		public SPAudienceOptions Options
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

		public TCAudienceOptions()
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

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCAudienceOptions));
			this.w_lbAudienceQuestion = new DevExpress.XtraEditors.LabelControl();
			this.w_rbDeleteAll = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbOverwrite = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbPreserve = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbStartAudienceComiplation = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_rbDeleteAll.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwrite.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserve.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbStartAudienceComiplation.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lbAudienceQuestion, "w_lbAudienceQuestion");
			this.w_lbAudienceQuestion.Name = "w_lbAudienceQuestion";
			resources.ApplyResources(this.w_rbDeleteAll, "w_rbDeleteAll");
			this.w_rbDeleteAll.Name = "w_rbDeleteAll";
			this.w_rbDeleteAll.Properties.AutoWidth = true;
			this.w_rbDeleteAll.Properties.Caption = resources.GetString("w_rbDeleteAll.Properties.Caption");
			this.w_rbDeleteAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbDeleteAll.Properties.RadioGroupIndex = 1;
			this.w_rbDeleteAll.TabStop = false;
			resources.ApplyResources(this.w_rbOverwrite, "w_rbOverwrite");
			this.w_rbOverwrite.Name = "w_rbOverwrite";
			this.w_rbOverwrite.Properties.AutoWidth = true;
			this.w_rbOverwrite.Properties.Caption = resources.GetString("w_rbOverwrite.Properties.Caption");
			this.w_rbOverwrite.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbOverwrite.Properties.RadioGroupIndex = 1;
			this.w_rbOverwrite.TabStop = false;
			resources.ApplyResources(this.w_rbPreserve, "w_rbPreserve");
			this.w_rbPreserve.Name = "w_rbPreserve";
			this.w_rbPreserve.Properties.AutoWidth = true;
			this.w_rbPreserve.Properties.Caption = resources.GetString("w_rbPreserve.Properties.Caption");
			this.w_rbPreserve.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbPreserve.Properties.RadioGroupIndex = 1;
			this.w_rbPreserve.TabStop = false;
			resources.ApplyResources(this.w_cbStartAudienceComiplation, "w_cbStartAudienceComiplation");
			this.w_cbStartAudienceComiplation.Name = "w_cbStartAudienceComiplation";
			this.w_cbStartAudienceComiplation.Properties.AutoWidth = true;
			this.w_cbStartAudienceComiplation.Properties.Caption = resources.GetString("w_cbStartAudienceComiplation.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_cbStartAudienceComiplation);
			base.Controls.Add(this.w_rbPreserve);
			base.Controls.Add(this.w_rbOverwrite);
			base.Controls.Add(this.w_rbDeleteAll);
			base.Controls.Add(this.w_lbAudienceQuestion);
			base.Name = "TCAudienceOptions";
			((System.ComponentModel.ISupportInitialize)this.w_rbDeleteAll.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwrite.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserve.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbStartAudienceComiplation.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void LoadUI()
		{
			if (Options != null)
			{
				if (Options.PasteStyle == CopyAudiencesOptions.PasteAudienceStyles.DeleteExisting)
				{
					w_rbDeleteAll.Checked = true;
				}
				else if (Options.PasteStyle != CopyAudiencesOptions.PasteAudienceStyles.OverwriteExisting)
				{
					w_rbPreserve.Checked = true;
				}
				else
				{
					w_rbOverwrite.Checked = true;
				}
				w_cbStartAudienceComiplation.Checked = Options.StartAudienceCompilation;
			}
		}

		public override bool SaveUI()
		{
			if (w_rbDeleteAll.Checked)
			{
				Options.PasteStyle = CopyAudiencesOptions.PasteAudienceStyles.DeleteExisting;
			}
			else if (!w_rbOverwrite.Checked)
			{
				Options.PasteStyle = CopyAudiencesOptions.PasteAudienceStyles.PreserveExisting;
			}
			else
			{
				Options.PasteStyle = CopyAudiencesOptions.PasteAudienceStyles.OverwriteExisting;
			}
			Options.StartAudienceCompilation = w_cbStartAudienceComiplation.Checked;
			return true;
		}
	}
}
