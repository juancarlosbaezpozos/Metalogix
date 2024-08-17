using Metalogix;
using Metalogix.Actions;
using Metalogix.Data.Mapping;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.Data.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Metalogix.SharePoint.ExternalConnections;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	public class ChooseNintexConnectionsDialog : Form
	{
		private IContainer components;

		private ToolStrip toolStrip1;

		private ListPickerControl w_listPickerControl;

		private Button buttonCancel;

		private Button buttonOK;

		private ToolStripLabel toolStripLabel1;

		private ToolStripComboBox w_toolStripComboBoxViews;

		private ToolStripLabel toolStripLabel2;

		public ActionContext Context
		{
			get;
			set;
		}

		public ChooseNintexConnectionsDialog()
		{
			this.InitializeComponent();
		}

		private void ChooseNintexConnectionsDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.w_toolStripComboBoxViews.SelectedIndex = 1;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public Dictionary<int, ExternalConnection> GetSelectedConnections()
		{
			Dictionary<int, ExternalConnection> nums = new Dictionary<int, ExternalConnection>();
			ListPickerItem[] selectedItems = this.w_listPickerControl.SelectedItems;
			for (int i = 0; i < (int)selectedItems.Length; i++)
			{
				ExternalConnection tag = selectedItems[i].Tag as ExternalConnection;
				nums.Add(tag.ExternalConnectionID, tag);
			}
			return nums;
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChooseNintexConnectionsDialog));
			this.toolStrip1 = new ToolStrip();
			this.toolStripLabel1 = new ToolStripLabel();
			this.w_toolStripComboBoxViews = new ToolStripComboBox();
			this.toolStripLabel2 = new ToolStripLabel();
			this.w_listPickerControl = new ListPickerControl();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
			ToolStripItemCollection items = this.toolStrip1.Items;
			ToolStripItem[] wToolStripComboBoxViews = new ToolStripItem[] { this.toolStripLabel1, this.w_toolStripComboBoxViews, this.toolStripLabel2 };
			items.AddRange(wToolStripComboBoxViews);
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(783, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStripLabel1.ForeColor = SystemColors.ControlDark;
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(459, 22);
			this.toolStripLabel1.Text = "NOTE: Available databases are accessible by this node but might not be attached to it.";
			this.w_toolStripComboBoxViews.Alignment = ToolStripItemAlignment.Right;
			this.w_toolStripComboBoxViews.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_toolStripComboBoxViews.Items.AddRange(new object[] { "Available Databases (Read-Only)", "Attached Databases" });
			this.w_toolStripComboBoxViews.Name = "w_toolStripComboBoxViews";
			this.w_toolStripComboBoxViews.Size = new System.Drawing.Size(182, 25);
			this.w_toolStripComboBoxViews.SelectedIndexChanged += new EventHandler(this.w_toolStripComboBoxViews_SelectedIndexChanged);
			this.toolStripLabel2.Alignment = ToolStripItemAlignment.Right;
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(86, 22);
			this.toolStripLabel2.Text = "Database View:";
			this.w_listPickerControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_listPickerControl.Items = null;
			this.w_listPickerControl.Location = new Point(0, 28);
			this.w_listPickerControl.Name = "w_listPickerControl";
			this.w_listPickerControl.SelectedItems = new ListPickerItem[0];
			this.w_listPickerControl.SelectedSource = null;
			this.w_listPickerControl.ShowSource = false;
			this.w_listPickerControl.Size = new System.Drawing.Size(783, 458);
			this.w_listPickerControl.Sources = null;
			this.w_listPickerControl.TabIndex = 1;
			this.buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(696, 492);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new Point(615, 492);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(783, 527);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.w_listPickerControl);
			base.Controls.Add(this.toolStrip1);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "ChooseNintexConnectionsDialog";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Nintex Database Picker";
			base.Load += new EventHandler(this.NintexConnectionPickerDialog_Load);
			base.FormClosing += new FormClosingEventHandler(this.ChooseNintexConnectionsDialog_FormClosing);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void NintexConnectionPickerDialog_Load(object sender, EventArgs e)
		{
			this.w_toolStripComboBoxViews.SelectedIndex = 0;
		}

		private void w_toolStripComboBoxViews_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.Context != null && this.Context.Targets.Count != 0)
				{
					Node item = (Node)this.Context.Targets[0];
					Dictionary<int, ExternalConnection> externalConnectionsOfType = null;
					switch (this.w_toolStripComboBoxViews.SelectedIndex)
					{
						case 0:
						{
							externalConnectionsOfType = item.GetExternalConnectionsOfType<NintexExternalConnection>(true);
							this.w_listPickerControl.Enabled = false;
							break;
						}
						case 1:
						{
							externalConnectionsOfType = item.GetExternalConnectionsOfType<NintexExternalConnection>(false);
							this.w_listPickerControl.Enabled = true;
							break;
						}
					}
					Dictionary<int, ExternalConnection> connectionsOfType = ExternalConnectionManager.INSTANCE.GetConnectionsOfType<NintexExternalConnection>();
					ExternalConnection[] externalConnectionArray = new ExternalConnection[connectionsOfType.Count];
					connectionsOfType.Values.CopyTo(externalConnectionArray, 0);
					this.w_listPickerControl.Items = externalConnectionArray;
					List<ListPickerItem> listPickerItems = new List<ListPickerItem>();
					foreach (KeyValuePair<int, ExternalConnection> keyValuePair in externalConnectionsOfType)
					{
						listPickerItems.Add(new ListPickerItem()
						{
							Tag = keyValuePair.Value
						});
					}
					this.w_listPickerControl.SelectedItems = listPickerItems.ToArray();
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}