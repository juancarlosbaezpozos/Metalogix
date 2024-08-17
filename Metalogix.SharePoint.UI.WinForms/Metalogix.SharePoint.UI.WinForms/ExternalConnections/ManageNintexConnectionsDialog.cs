using Metalogix;
using Metalogix.Data.Mapping;
using Metalogix.ExternalConnections;
using Metalogix.Interfaces;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Data.Mapping;
using Metalogix.UI.WinForms.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	public class ManageNintexConnectionsDialog : Form
	{
		private IContainer components;

		private ToolStrip toolStrip1;

		private Metalogix.UI.WinForms.Data.Mapping.ListControl w_listControl;

		private Button buttonOK;

		private ToolStripButton w_toolStripButtonNew;

		private ToolStripButton w_toolStripButtonEdit;

		private ToolStripButton w_toolStripButtonDelete;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton w_toolStripButtonRefresh;

		public ManageNintexConnectionsDialog()
		{
			this.InitializeComponent();
		}

		private void CheckConnection(SqlConnectionStringBuilder csbuilder)
		{
			(new NintexExternalConnection()).CheckConnection(csbuilder.ConnectionString);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ManageNintexConnectionsDialog));
			this.toolStrip1 = new ToolStrip();
			this.w_toolStripButtonNew = new ToolStripButton();
			this.w_toolStripButtonDelete = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.w_toolStripButtonEdit = new ToolStripButton();
			this.w_toolStripButtonRefresh = new ToolStripButton();
			this.buttonOK = new Button();
			this.w_listControl = new Metalogix.UI.WinForms.Data.Mapping.ListControl();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
			ToolStripItemCollection items = this.toolStrip1.Items;
			ToolStripItem[] wToolStripButtonNew = new ToolStripItem[] { this.w_toolStripButtonNew, this.w_toolStripButtonDelete, this.toolStripSeparator1, this.w_toolStripButtonEdit, this.w_toolStripButtonRefresh };
			items.AddRange(wToolStripButtonNew);
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(600, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.w_toolStripButtonNew.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Add16;
			this.w_toolStripButtonNew.ImageTransparentColor = Color.Magenta;
			this.w_toolStripButtonNew.Name = "w_toolStripButtonNew";
			this.w_toolStripButtonNew.Size = new System.Drawing.Size(48, 22);
			this.w_toolStripButtonNew.Text = "New";
			this.w_toolStripButtonNew.Click += new EventHandler(this.w_toolStripButtonNew_Click);
			this.w_toolStripButtonDelete.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Delete16;
			this.w_toolStripButtonDelete.ImageTransparentColor = Color.Magenta;
			this.w_toolStripButtonDelete.Name = "w_toolStripButtonDelete";
			this.w_toolStripButtonDelete.Size = new System.Drawing.Size(58, 22);
			this.w_toolStripButtonDelete.Text = "Delete";
			this.w_toolStripButtonDelete.Click += new EventHandler(this.w_toolStripButtonDelete_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.w_toolStripButtonEdit.Image = (Image)componentResourceManager.GetObject("w_toolStripButtonEdit.Image");
			this.w_toolStripButtonEdit.ImageTransparentColor = Color.Magenta;
			this.w_toolStripButtonEdit.Name = "w_toolStripButtonEdit";
			this.w_toolStripButtonEdit.Size = new System.Drawing.Size(74, 22);
			this.w_toolStripButtonEdit.Text = "Configure";
			this.w_toolStripButtonEdit.Click += new EventHandler(this.w_toolStripButtonEdit_Click);
			this.w_toolStripButtonRefresh.Alignment = ToolStripItemAlignment.Right;
			this.w_toolStripButtonRefresh.Image = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Refresh16;
			this.w_toolStripButtonRefresh.ImageTransparentColor = Color.Magenta;
			this.w_toolStripButtonRefresh.Name = "w_toolStripButtonRefresh";
			this.w_toolStripButtonRefresh.Size = new System.Drawing.Size(118, 22);
			this.w_toolStripButtonRefresh.Text = "Check Connections";
			this.w_toolStripButtonRefresh.Click += new EventHandler(this.w_toolStripButtonRefresh_Click);
			this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new Point(513, 462);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "Close";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.w_listControl.AllowEdit = false;
			this.w_listControl.AllowNewTagCreation = true;
			this.w_listControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_listControl.BorderStyle = BorderStyle.FixedSingle;
			this.w_listControl.CurrentFilter = null;
			this.w_listControl.CurrentView = null;
			this.w_listControl.ForeColor = SystemColors.ControlText;
			this.w_listControl.Items = null;
			this.w_listControl.Location = new Point(0, 28);
			this.w_listControl.MultiSelect = true;
			this.w_listControl.Name = "w_listControl";
			this.w_listControl.SelectedSource = null;
			this.w_listControl.ShowSource = false;
			this.w_listControl.Size = new System.Drawing.Size(600, 428);
			this.w_listControl.Sources = null;
			this.w_listControl.TabIndex = 1;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonOK;
			base.ClientSize = new System.Drawing.Size(600, 497);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.w_listControl);
			base.Controls.Add(this.toolStrip1);
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ManageNintexConnectionsDialog";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = Metalogix.SharePoint.Properties.Resources.Manage_Nintex_Workflow_Databases;
			base.Load += new EventHandler(this.ManageNintexConnectionsDialog_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void ManageNintexConnectionsDialog_Load(object sender, EventArgs e)
		{
			this.RefreshConnections();
		}

		private void RefreshConnections()
		{
			try
			{
				this.w_listControl.Items = null;
				this.w_listControl.AddCustomColumn("Status", "Status");
				this.w_listControl.CurrentView = new NintexExternalConnectionView();
				List<ExternalConnection> externalConnections = new List<ExternalConnection>();
				foreach (KeyValuePair<int, ExternalConnection> connectionsOfType in ExternalConnectionManager.INSTANCE.GetConnectionsOfType<NintexExternalConnection>())
				{
					externalConnections.Add(connectionsOfType.Value);
				}
				this.w_listControl.Items = externalConnections.ToArray();
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}

		private void w_toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			try
			{
				ListPickerItem[] selectedItems = this.w_listControl.GetSelectedItems();
				if ((int)selectedItems.Length == 0)
				{
					throw new Exception("Please select 1 or more items to edit");
				}
				if (FlatXtraMessageBox.Show("Do you wish to delete the selected connections?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.No)
				{
					Stack<ListPickerItem> listPickerItems = new Stack<ListPickerItem>(selectedItems);
					while (listPickerItems.Count != 0)
					{
						ListPickerItem listPickerItem = listPickerItems.Pop();
						(listPickerItem.Tag as NintexExternalConnection).Delete();
						this.w_listControl.DeleteItem(listPickerItem);
					}
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}

		private void w_toolStripButtonEdit_Click(object sender, EventArgs e)
		{
			try
			{
				ListPickerItem[] selectedItems = this.w_listControl.GetSelectedItems();
				if ((int)selectedItems.Length != 1)
				{
					throw new Exception("Please select only 1 item to configure");
				}
				ListPickerItem listPickerItem = selectedItems[0];
				NintexExternalConnection tag = listPickerItem.Tag as NintexExternalConnection;
				DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog();
				databaseConnectDialog.SetCheckDatabaseConnection(new Action<SqlConnectionStringBuilder>(this.CheckConnection));
				databaseConnectDialog.SqlServerName = tag.Server;
				databaseConnectDialog.SqlDatabaseName = tag.Database;
				databaseConnectDialog.Credentials = tag.Credentials;
				if (databaseConnectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					tag.Server = databaseConnectDialog.SqlServerName;
					tag.Database = databaseConnectDialog.SqlDatabaseName;
					tag.Credentials = databaseConnectDialog.Credentials;
					tag.Update();
					try
					{
						tag.CheckConnection();
					}
					catch
					{
					}
					this.w_listControl.UpdateItem(listPickerItem);
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}

		private void w_toolStripButtonNew_Click(object sender, EventArgs e)
		{
			try
			{
				DatabaseConnectDialog databaseConnectDialog = new DatabaseConnectDialog();
				databaseConnectDialog.SetCheckDatabaseConnection(new Action<SqlConnectionStringBuilder>(this.CheckConnection));
				if (databaseConnectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					NintexExternalConnection nintexExternalConnection = new NintexExternalConnection(databaseConnectDialog.SqlServerName, databaseConnectDialog.SqlDatabaseName, databaseConnectDialog.Credentials);
					nintexExternalConnection.Insert();
					try
					{
						nintexExternalConnection.CheckConnection();
					}
					catch
					{
					}
					ListPickerItem listPickerItem = new ListPickerItem()
					{
						Tag = nintexExternalConnection
					};
					this.w_listControl.AddItem(listPickerItem, true);
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}

		private void w_toolStripButtonRefresh_Click(object sender, EventArgs e)
		{
			try
			{
				ExternalConnectionManager.INSTANCE.CheckConnections<NintexExternalConnection>();
				this.RefreshConnections();
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}