using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Administration.LinkManagement;
using Metalogix.SharePoint.Options.Administration.LinkManagement;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.LinkManagement
{
	public class LinkCorrectionDialog : Form
	{
		private Button w_btnCancel;

		private ImageList w_imgList;

		private CheckBox w_cbRecursive;

		private Button w_btnCorrectLinks;

		private Button w_btnBuildReport;

		private CheckBox w_checkBoxIgnoreQueryStrings;

		private CheckedListBox w_clbLinkCorrectors;

		private Label w_lLinkCorrectorsHeading;

		private GroupBox w_gbLinkTargets;

		private RadioButton w_rbLinkTargetUseAlternative;

		private RadioButton w_rbLinkTargetUseActionTarget;

		private Button w_bLinkTargetBrowse;

		private TextBox w_tbLinkTargetPath;

		private IContainer components;

		private LinkCorrectionOptions m_options;

		private IXMLAbleList m_targets;

		private SPNode m_selectedLinkTarget;

		public LinkCorrectionOptions Options
		{
			get
			{
				return this.m_options;
			}
			set
			{
				this.m_options = value;
				this.LoadUI();
			}
		}

		public IXMLAbleList Targets
		{
			get
			{
				return this.m_targets;
			}
			set
			{
				this.m_targets = value;
			}
		}

		public LinkCorrectionDialog(IXMLAbleList targets)
		{
			this.InitializeComponent();
			this.m_targets = targets;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinkCorrectionDialog));
			this.w_btnCorrectLinks = new Button();
			this.w_btnCancel = new Button();
			this.w_imgList = new ImageList(this.components);
			this.w_cbRecursive = new CheckBox();
			this.w_btnBuildReport = new Button();
			this.w_checkBoxIgnoreQueryStrings = new CheckBox();
			this.w_lLinkCorrectorsHeading = new Label();
			this.w_clbLinkCorrectors = new CheckedListBox();
			this.w_gbLinkTargets = new GroupBox();
			this.w_bLinkTargetBrowse = new Button();
			this.w_tbLinkTargetPath = new TextBox();
			this.w_rbLinkTargetUseAlternative = new RadioButton();
			this.w_rbLinkTargetUseActionTarget = new RadioButton();
			this.w_gbLinkTargets.SuspendLayout();
			base.SuspendLayout();
			this.w_btnCorrectLinks.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnCorrectLinks.Location = new Point(306, 216);
			this.w_btnCorrectLinks.Name = "w_btnCorrectLinks";
			this.w_btnCorrectLinks.Size = new System.Drawing.Size(79, 23);
			this.w_btnCorrectLinks.TabIndex = 6;
			this.w_btnCorrectLinks.Text = "Correct Links";
			this.w_btnCorrectLinks.Click += new EventHandler(this.On_btnRunUpdate_Click);
			this.w_btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Location = new Point(391, 216);
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Size = new System.Drawing.Size(75, 23);
			this.w_btnCancel.TabIndex = 7;
			this.w_btnCancel.Text = "Cancel";
			this.w_imgList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("w_imgList.ImageStream");
			this.w_imgList.TransparentColor = Color.Transparent;
			this.w_imgList.Images.SetKeyName(0, "");
			this.w_imgList.Images.SetKeyName(1, "");
			this.w_cbRecursive.Location = new Point(204, 134);
			this.w_cbRecursive.Name = "w_cbRecursive";
			this.w_cbRecursive.Size = new System.Drawing.Size(87, 17);
			this.w_cbRecursive.TabIndex = 3;
			this.w_cbRecursive.Text = "Recursive";
			this.w_btnBuildReport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_btnBuildReport.Location = new Point(212, 216);
			this.w_btnBuildReport.Name = "w_btnBuildReport";
			this.w_btnBuildReport.Size = new System.Drawing.Size(88, 23);
			this.w_btnBuildReport.TabIndex = 5;
			this.w_btnBuildReport.Text = "Build Report";
			this.w_btnBuildReport.Click += new EventHandler(this.On_btnBuildReport_Click);
			this.w_checkBoxIgnoreQueryStrings.Location = new Point(204, 157);
			this.w_checkBoxIgnoreQueryStrings.Name = "w_checkBoxIgnoreQueryStrings";
			this.w_checkBoxIgnoreQueryStrings.Size = new System.Drawing.Size(173, 17);
			this.w_checkBoxIgnoreQueryStrings.TabIndex = 4;
			this.w_checkBoxIgnoreQueryStrings.Text = "Ignore query strings in URLs";
			this.w_lLinkCorrectorsHeading.AutoSize = true;
			this.w_lLinkCorrectorsHeading.Location = new Point(12, 9);
			this.w_lLinkCorrectorsHeading.Name = "w_lLinkCorrectorsHeading";
			this.w_lLinkCorrectorsHeading.Size = new System.Drawing.Size(80, 13);
			this.w_lLinkCorrectorsHeading.TabIndex = 0;
			this.w_lLinkCorrectorsHeading.Text = "Update links in:";
			this.w_clbLinkCorrectors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			this.w_clbLinkCorrectors.CheckOnClick = true;
			this.w_clbLinkCorrectors.FormattingEnabled = true;
			this.w_clbLinkCorrectors.Location = new Point(12, 25);
			this.w_clbLinkCorrectors.Name = "w_clbLinkCorrectors";
			this.w_clbLinkCorrectors.Size = new System.Drawing.Size(180, 184);
			this.w_clbLinkCorrectors.TabIndex = 1;
			this.w_gbLinkTargets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_gbLinkTargets.Controls.Add(this.w_bLinkTargetBrowse);
			this.w_gbLinkTargets.Controls.Add(this.w_tbLinkTargetPath);
			this.w_gbLinkTargets.Controls.Add(this.w_rbLinkTargetUseAlternative);
			this.w_gbLinkTargets.Controls.Add(this.w_rbLinkTargetUseActionTarget);
			this.w_gbLinkTargets.Location = new Point(198, 25);
			this.w_gbLinkTargets.Name = "w_gbLinkTargets";
			this.w_gbLinkTargets.Size = new System.Drawing.Size(268, 103);
			this.w_gbLinkTargets.TabIndex = 2;
			this.w_gbLinkTargets.TabStop = false;
			this.w_gbLinkTargets.Text = "Link Target";
			this.w_bLinkTargetBrowse.Location = new Point(157, 65);
			this.w_bLinkTargetBrowse.Name = "w_bLinkTargetBrowse";
			this.w_bLinkTargetBrowse.Size = new System.Drawing.Size(75, 23);
			this.w_bLinkTargetBrowse.TabIndex = 3;
			this.w_bLinkTargetBrowse.Text = "Browse...";
			this.w_bLinkTargetBrowse.UseVisualStyleBackColor = true;
			this.w_bLinkTargetBrowse.Click += new EventHandler(this.On_bLinkTargetBrowse_Click);
			this.w_tbLinkTargetPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_tbLinkTargetPath.Location = new Point(6, 19);
			this.w_tbLinkTargetPath.Name = "w_tbLinkTargetPath";
			this.w_tbLinkTargetPath.ReadOnly = true;
			this.w_tbLinkTargetPath.Size = new System.Drawing.Size(256, 20);
			this.w_tbLinkTargetPath.TabIndex = 1;
			this.w_rbLinkTargetUseAlternative.AutoSize = true;
			this.w_rbLinkTargetUseAlternative.Location = new Point(6, 68);
			this.w_rbLinkTargetUseAlternative.Name = "w_rbLinkTargetUseAlternative";
			this.w_rbLinkTargetUseAlternative.Size = new System.Drawing.Size(145, 17);
			this.w_rbLinkTargetUseAlternative.TabIndex = 2;
			this.w_rbLinkTargetUseAlternative.TabStop = true;
			this.w_rbLinkTargetUseAlternative.Text = "Use alternative link target";
			this.w_rbLinkTargetUseAlternative.UseVisualStyleBackColor = true;
			this.w_rbLinkTargetUseAlternative.CheckedChanged += new EventHandler(this.On_rbLinkTargetUseAlternative_CheckedChanged);
			this.w_rbLinkTargetUseActionTarget.AutoSize = true;
			this.w_rbLinkTargetUseActionTarget.Location = new Point(6, 45);
			this.w_rbLinkTargetUseActionTarget.Name = "w_rbLinkTargetUseActionTarget";
			this.w_rbLinkTargetUseActionTarget.Size = new System.Drawing.Size(106, 17);
			this.w_rbLinkTargetUseActionTarget.TabIndex = 1;
			this.w_rbLinkTargetUseActionTarget.TabStop = true;
			this.w_rbLinkTargetUseActionTarget.Text = "Use action target";
			this.w_rbLinkTargetUseActionTarget.UseVisualStyleBackColor = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.ClientSize = new System.Drawing.Size(478, 251);
			base.Controls.Add(this.w_gbLinkTargets);
			base.Controls.Add(this.w_checkBoxIgnoreQueryStrings);
			base.Controls.Add(this.w_cbRecursive);
			base.Controls.Add(this.w_clbLinkCorrectors);
			base.Controls.Add(this.w_lLinkCorrectorsHeading);
			base.Controls.Add(this.w_btnBuildReport);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnCorrectLinks);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new System.Drawing.Size(486, 278);
			base.Name = "LinkCorrectionDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Correct Links in SharePoint Content";
			this.w_gbLinkTargets.ResumeLayout(false);
			this.w_gbLinkTargets.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			SPNode item;
			if (this.m_options == null)
			{
				return;
			}
			this.w_cbRecursive.Checked = this.m_options.Recursive;
			this.w_checkBoxIgnoreQueryStrings.Checked = this.m_options.IgnoreQueryStrings;
			this.w_rbLinkTargetUseActionTarget.Checked = this.m_options.UseActionTargetAsLinkDictionary;
			this.w_rbLinkTargetUseAlternative.Checked = !this.m_options.UseActionTargetAsLinkDictionary;
			if (this.m_options.LocationLinkDictionary == null || this.m_options.LocationLinkDictionary.Count <= 0)
			{
				item = null;
			}
			else
			{
				item = this.m_options.LocationLinkDictionary[0] as SPNode;
			}
			this.m_selectedLinkTarget = item;
			this.UpdateEnabling();
			this.UpdateLinkTargetDisplay();
			this.w_clbLinkCorrectors.Items.Clear();
			Type[] linkCorrectors = LinkUtils.LinkCorrectors;
			for (int i = 0; i < (int)linkCorrectors.Length; i++)
			{
				Type type = linkCorrectors[i];
				this.w_clbLinkCorrectors.Items.Add(new LinkCorrectorTypeWrapper(type), (this.m_options.SelectedLinkCorrectors.Count > 0 ? this.m_options.SelectedLinkCorrectors.Contains(type) : true));
			}
			this.UpdateEnabling();
		}

		private void On_bLinkTargetBrowse_Click(object sender, EventArgs e)
		{
			NodeCollection nodeCollection = new NodeCollection();
			foreach (Node activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
			{
				if (!(activeConnection is SPConnection))
				{
					continue;
				}
				nodeCollection.Add(activeConnection);
			}
			NodeChooserDialog nodeChooserDialog = new NodeChooserDialog()
			{
				DataSource = nodeCollection
			};
			if (nodeChooserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			this.m_selectedLinkTarget = nodeChooserDialog.SelectedNode as SPNode;
			this.UpdateEnabling();
			this.UpdateLinkTargetDisplay();
		}

		private void On_btnBuildReport_Click(object sender, EventArgs e)
		{
			if (!this.VerifyOptions())
			{
				return;
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_options.ReportOnly = true;
			this.SaveOptions();
			base.Close();
		}

		private void On_btnRunUpdate_Click(object sender, EventArgs e)
		{
			if (!this.VerifyOptions())
			{
				return;
			}
			if (FlatXtraMessageBox.Show("This operation will update SharePoint content.\n\nDo you wish to continue?", "Update Links", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_options.ReportOnly = false;
			this.SaveOptions();
			base.Close();
		}

		private void On_rbLinkTargetUseAlternative_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateEnabling();
			this.UpdateLinkTargetDisplay();
		}

		private void SaveOptions()
		{
			IXMLAbleList mTargets;
			this.m_options.Recursive = this.w_cbRecursive.Checked;
			this.m_options.IgnoreQueryStrings = this.w_checkBoxIgnoreQueryStrings.Checked;
			this.m_options.UseActionTargetAsLinkDictionary = this.w_rbLinkTargetUseActionTarget.Checked;
			LinkCorrectionOptions mOptions = this.m_options;
			if (this.w_rbLinkTargetUseActionTarget.Checked)
			{
				mTargets = this.m_targets;
			}
			else
			{
				mTargets = new NodeCollection(new Node[] { this.m_selectedLinkTarget });
			}
			mOptions.LocationLinkDictionary = mTargets;
			this.m_options.SelectedLinkCorrectors.Clear();
			foreach (object checkedItem in this.w_clbLinkCorrectors.CheckedItems)
			{
				LinkCorrectorTypeWrapper linkCorrectorTypeWrapper = checkedItem as LinkCorrectorTypeWrapper;
				if (linkCorrectorTypeWrapper == null)
				{
					continue;
				}
				this.m_options.SelectedLinkCorrectors.Add(linkCorrectorTypeWrapper.InnerType);
			}
		}

		private void UpdateEnabling()
		{
			bool flag;
			Button wBtnCorrectLinks = this.w_btnCorrectLinks;
			Button wBtnBuildReport = this.w_btnBuildReport;
			flag = (this.w_rbLinkTargetUseActionTarget.Checked ? true : this.m_selectedLinkTarget != null);
			bool flag1 = flag;
			wBtnBuildReport.Enabled = flag;
			wBtnCorrectLinks.Enabled = flag1;
			this.w_bLinkTargetBrowse.Enabled = this.w_rbLinkTargetUseAlternative.Checked;
		}

		private void UpdateLinkTargetDisplay()
		{
			if (this.w_rbLinkTargetUseActionTarget.Checked)
			{
				this.w_tbLinkTargetPath.Text = ((SPNode)this.m_targets[0]).DisplayUrl;
				return;
			}
			if (this.m_selectedLinkTarget == null)
			{
				this.w_tbLinkTargetPath.Text = "None Selected";
				return;
			}
			this.w_tbLinkTargetPath.Text = this.m_selectedLinkTarget.DisplayUrl;
		}

		private bool VerifyOptions()
		{
			if (this.w_rbLinkTargetUseAlternative.Checked && this.m_selectedLinkTarget == null)
			{
				FlatXtraMessageBox.Show("Please select a site or library containing the migrated pages to link to.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.w_bLinkTargetBrowse.Focus();
				return false;
			}
			if (this.w_clbLinkCorrectors.CheckedItems.Count != 0)
			{
				return true;
			}
			FlatXtraMessageBox.Show("Please select one or more document types to correct.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.w_clbLinkCorrectors.Focus();
			return false;
		}
	}
}