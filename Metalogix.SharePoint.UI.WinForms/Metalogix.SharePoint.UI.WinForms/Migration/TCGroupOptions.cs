using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using Metalogix.Utilities;
using Metalogix.Widgets;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.Groups32.ico")]
	[ControlName("Group Options")]
	public class TCGroupOptions : ScopableTabbableControl
	{
		private SPGroupOptions m_options;

		private IContainer components;

		private PanelControl w_plFilterTree;

		internal CheckEdit w_cbOverwriteGroups;

		internal CheckEdit w_cbMapGroupsByName;

		private HierarchicalCheckBoxesTreeView w_tvGroupSelector;

		private ImageList w_lTreeViewIcons;

		public SPGroupOptions Options
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

		public override NodeCollection SourceNodes
		{
			get
			{
				return base.SourceNodes;
			}
			set
			{
				base.SourceNodes = value;
				UpdateGroupTree();
			}
		}

		public TCGroupOptions()
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGroupOptions));
			this.w_plFilterTree = new DevExpress.XtraEditors.PanelControl();
			this.w_tvGroupSelector = new Metalogix.Widgets.HierarchicalCheckBoxesTreeView();
			this.w_lTreeViewIcons = new System.Windows.Forms.ImageList(this.components);
			this.w_cbOverwriteGroups = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbMapGroupsByName = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_plFilterTree).BeginInit();
			this.w_plFilterTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteGroups.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapGroupsByName.Properties).BeginInit();
			base.SuspendLayout();
			this.w_plFilterTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plFilterTree.Controls.Add(this.w_tvGroupSelector);
			this.w_plFilterTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.w_plFilterTree.Location = new System.Drawing.Point(0, 0);
			this.w_plFilterTree.Name = "w_plFilterTree";
			this.w_plFilterTree.Size = new System.Drawing.Size(344, 184);
			this.w_plFilterTree.TabIndex = 0;
			this.w_tvGroupSelector.CheckBoxes = true;
			this.w_tvGroupSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.w_tvGroupSelector.ImageIndex = 0;
			this.w_tvGroupSelector.ImageList = this.w_lTreeViewIcons;
			this.w_tvGroupSelector.Location = new System.Drawing.Point(0, 0);
			this.w_tvGroupSelector.Name = "w_tvGroupSelector";
			this.w_tvGroupSelector.ParentCheckBoxCheckedWhen = Metalogix.Widgets.HierarchicalCheckBoxesTreeView.ParentCheckboxBehavior.AtLeastOneChildChecked;
			this.w_tvGroupSelector.SelectedImageIndex = 0;
			this.w_tvGroupSelector.Size = new System.Drawing.Size(344, 184);
			this.w_tvGroupSelector.SmudgeProtection = false;
			this.w_tvGroupSelector.TabIndex = 1;
			this.w_tvGroupSelector.UseDoubleBuffering = true;
			this.w_lTreeViewIcons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("w_lTreeViewIcons.ImageStream");
			this.w_lTreeViewIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.w_lTreeViewIcons.Images.SetKeyName(0, "Group.ico");
			this.w_lTreeViewIcons.Images.SetKeyName(1, "User.ico");
			this.w_cbOverwriteGroups.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbOverwriteGroups.Location = new System.Drawing.Point(24, 213);
			this.w_cbOverwriteGroups.Name = "w_cbOverwriteGroups";
			this.w_cbOverwriteGroups.Properties.AutoWidth = true;
			this.w_cbOverwriteGroups.Properties.Caption = "Overwrite existing groups";
			this.w_cbOverwriteGroups.Size = new System.Drawing.Size(147, 19);
			this.w_cbOverwriteGroups.TabIndex = 99;
			this.w_cbMapGroupsByName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbMapGroupsByName.Location = new System.Drawing.Point(6, 190);
			this.w_cbMapGroupsByName.Name = "w_cbMapGroupsByName";
			this.w_cbMapGroupsByName.Properties.AutoWidth = true;
			this.w_cbMapGroupsByName.Properties.Caption = "Map groups by name";
			this.w_cbMapGroupsByName.Size = new System.Drawing.Size(123, 19);
			this.w_cbMapGroupsByName.TabIndex = 98;
			this.w_cbMapGroupsByName.CheckedChanged += new System.EventHandler(On_Checked_Changed);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_cbOverwriteGroups);
			base.Controls.Add(this.w_cbMapGroupsByName);
			base.Controls.Add(this.w_plFilterTree);
			base.Name = "TCGroupOptions";
			base.Size = new System.Drawing.Size(344, 245);
			((System.ComponentModel.ISupportInitialize)this.w_plFilterTree).EndInit();
			this.w_plFilterTree.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteGroups.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapGroupsByName.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbMapGroupsByName.Checked = m_options.MapGroupsByName;
			w_cbOverwriteGroups.Checked = m_options.OverwriteGroups;
			UpdateGroupTree();
		}

		private void On_Checked_Changed(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		public override bool SaveUI()
		{
			m_options.MapGroupsByName = w_cbMapGroupsByName.Checked;
			m_options.OverwriteGroups = w_cbOverwriteGroups.Enabled && w_cbOverwriteGroups.Checked;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder));
			xmlWriter.WriteStartElement("GroupExclusions");
			foreach (TreeNode node in w_tvGroupSelector.Nodes)
			{
				xmlWriter.WriteStartElement("Group");
				xmlWriter.WriteAttributeString("Name", node.Text);
				if (!node.Checked)
				{
					xmlWriter.WriteAttributeString("Excluded", true.ToString());
					flag = true;
				}
				else
				{
					xmlWriter.WriteAttributeString("Excluded", false.ToString());
					foreach (TreeNode node2 in node.Nodes)
					{
						if (!node2.Checked)
						{
							xmlWriter.WriteStartElement("User");
							xmlWriter.WriteAttributeString("Name", node2.Text);
							xmlWriter.WriteAttributeString("Excluded", true.ToString());
							xmlWriter.WriteEndElement();
							flag = true;
						}
					}
				}
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			if (!flag)
			{
				Options.GroupExclusions = null;
			}
			else
			{
				Options.GroupExclusions = XmlUtility.StringToXmlNode(stringBuilder.ToString());
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			w_cbOverwriteGroups.Enabled = w_cbMapGroupsByName.Enabled && w_cbMapGroupsByName.Checked;
			w_cbOverwriteGroups.Checked = w_cbOverwriteGroups.Checked && w_cbOverwriteGroups.Enabled;
		}

		private void UpdateGroupTree()
		{
			w_tvGroupSelector.SuspendLayout();
			w_tvGroupSelector.Nodes.Clear();
			if (SourceNodes != null && SourceNodes.Count > 0 && typeof(SPWeb).IsAssignableFrom(SourceNodes[0].GetType()))
			{
				foreach (SPGroup item in (IEnumerable<SecurityPrincipal>)(SourceNodes[0] as SPWeb).Groups)
				{
					TreeNode treeNode = new TreeNode(item.Name, 0, 0);
					if (m_options == null || m_options.GroupExclusions == null)
					{
						treeNode.Checked = true;
					}
					else
					{
						bool @checked = true;
						XmlNode xmlNode = m_options.GroupExclusions.SelectSingleNode($"./Group[@Name='{item.Name}']");
						if (xmlNode != null && xmlNode.Attributes["Excluded"] != null)
						{
							@checked = !bool.Parse(xmlNode.Attributes["Excluded"].Value);
						}
						treeNode.Checked = @checked;
					}
					bool flag = false;
					foreach (SPUser item2 in (IEnumerable<SecurityPrincipal>)item.Users)
					{
						TreeNode treeNode2 = new TreeNode(item2.LoginName, 1, 1);
						if (m_options == null || m_options.GroupExclusions == null)
						{
							treeNode2.Checked = treeNode.Checked;
						}
						else
						{
							bool flag2 = treeNode.Checked;
							XmlNode xmlNode2 = m_options.GroupExclusions.SelectSingleNode($"./Group[@Name='{item.Name}']/User[@Name='{item2.LoginName}']");
							if (xmlNode2 != null && xmlNode2.Attributes["Excluded"] != null)
							{
								flag2 = flag2 && !bool.Parse(xmlNode2.Attributes["Excluded"].Value);
							}
							treeNode2.Checked = flag2;
							flag = flag || !flag2;
						}
						treeNode.Nodes.Add(treeNode2);
					}
					if (treeNode.Checked && flag)
					{
						treeNode.Expand();
					}
					w_tvGroupSelector.Nodes.Add(treeNode);
				}
			}
			w_tvGroupSelector.ResumeLayout();
		}
	}
}
