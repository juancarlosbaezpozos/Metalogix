using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Data;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyRoleAssignmentsDialog : Form
	{
		private CopyRoleAssignmentsOptions m_options;

		private IContainer components;

		private ConditionalMappingControl w_conditionalMappingControl;

		protected Button w_btnCancel;

		protected Button w_btnOK;

		public FilterBuilderType[] AvailableTypes
		{
			get
			{
				return this.w_conditionalMappingControl.AvailableTypes;
			}
			set
			{
				this.w_conditionalMappingControl.AvailableTypes = value;
			}
		}

		public CopyRoleAssignmentsOptions Options
		{
			get
			{
				return this.m_options;
			}
			set
			{
				this.m_options = value;
				this.w_conditionalMappingControl.Mappings = this.m_options.RoleAssignmentMappings;
			}
		}

		public CopyRoleAssignmentsDialog()
		{
			this.InitializeComponent();
			base.Icon = UISettings.Icon;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CopyRoleAssignmentsDialog));
			ConditionalMappingCollection conditionalMappingCollection = new ConditionalMappingCollection();
			this.w_btnCancel = new Button();
			this.w_btnOK = new Button();
			this.w_conditionalMappingControl = new ConditionalMappingControl();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new EventHandler(this.w_btnOK_Click);
			componentResourceManager.ApplyResources(this.w_conditionalMappingControl, "w_conditionalMappingControl");
			this.w_conditionalMappingControl.AvailableTypes = null;
			this.w_conditionalMappingControl.Mappings = conditionalMappingCollection;
			this.w_conditionalMappingControl.Name = "w_conditionalMappingControl";
			base.AcceptButton = this.w_btnOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_conditionalMappingControl);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CopyRoleAssignmentsDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
		}

		private void w_btnOK_Click(object sender, EventArgs e)
		{
			this.w_conditionalMappingControl.SaveUI();
			this.m_options.RoleAssignmentMappings = this.w_conditionalMappingControl.Mappings;
		}
	}
}