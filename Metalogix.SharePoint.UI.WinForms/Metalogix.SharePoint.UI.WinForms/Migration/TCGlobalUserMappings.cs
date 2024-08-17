using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Metalogix.Data.Mapping;
using Metalogix.Explorer;
using Metalogix.SharePoint.UI.WinForms.Mapping;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Data.Mapping;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.UserMapping32.ico")]
	[ControlName("User Mappings")]
	public class TCGlobalUserMappings : ScopableTabbableControl
	{
		private SPWeb m_current;

		private bool _isBasicMode;

		public MappingsCollection localMappings;

		private string m_sLastSelectedSource;

		private string m_sLastSelectedTarget;

		private IContainer components;

		public ListMapperControl globalMapperControl;

		public string SelectedSource
		{
			get
			{
				return (string)globalMapperControl.SelectedSource;
			}
			set
			{
				globalMapperControl.SelectedSource = value;
			}
		}

		public string SelectedTarget
		{
			get
			{
				return (string)globalMapperControl.SelectedTarget;
			}
			set
			{
				globalMapperControl.SelectedTarget = value;
			}
		}

		public TCGlobalUserMappings(bool isBasicMode = false)
		{
			_isBasicMode = isBasicMode;
			InitializeComponent();
			globalMapperControl.UpdateUserIcon();
			Initialize();
		}

		public TCGlobalUserMappings(string sSource, string sTarget)
		{
			InitializeComponent();
			Initialize();
			SelectedSource = sSource;
			SelectedTarget = sTarget;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void globalMapperControl_OnClearClicked(object sender, EventArgs eventArgs)
		{
			localMappings.ClearCollection();
		}

		private void globalMapperControl_OnMapped(object sender, ListSummaryItemEventArgs e)
		{
			if (m_current != null && e.Tag != null)
			{
				if (localMappings[e.Tag] != null)
				{
					FlatXtraMessageBox.Show("Source Principal has already been mapped.  Please remove existing mapping then try again.");
					localMappings.Add(e.Tag);
					globalMapperControl.Unmap(e.Tag);
				}
				else
				{
					localMappings.Add(e.Tag);
				}
			}
		}

		private void globalMapperControl_OnNewButtonClicked(object sender, ListPickerItem[] e)
		{
			ListPickerItem listPickerItem = new ListPickerItem();
			if (m_current != null)
			{
				CreateUserMappingDialog createUserMappingDialog = new CreateUserMappingDialog(_isBasicMode);
				createUserMappingDialog.ShowDialog();
				if (createUserMappingDialog.DialogResult == DialogResult.OK)
				{
					XmlElement xmlElement = new XmlDocument().CreateElement("SPUser");
					xmlElement.SetAttribute("LoginName", createUserMappingDialog.LoginName);
					xmlElement.SetAttribute("Name", createUserMappingDialog.DisplayName);
					xmlElement.SetAttribute("Email", createUserMappingDialog.Email);
					xmlElement.SetAttribute("Notes", createUserMappingDialog.Notes);
					listPickerItem.Group = "User Mappings";
					listPickerItem.Target = createUserMappingDialog.LoginName;
					listPickerItem.Tag = new SPUser(xmlElement);
					globalMapperControl.AddTargetItem(listPickerItem, bReadOnly: true);
				}
			}
		}

		private void globalMapperControl_OnSourceChanged(object sender, SourceChangedEventArgs e)
		{
			try
			{
				globalMapperControl.OnMapped -= globalMapperControl_OnMapped;
				globalMapperControl.OnUnmapped -= globalMapperControl_OnUnmapped;
				globalMapperControl.SourceItems = null;
				if (e.Tag == null)
				{
					return;
				}
				Node node = null;
				foreach (Connection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
				{
					try
					{
						node = (activeConnection.Node.DisplayUrl.Equals(e.Tag.ToString()) ? activeConnection.Node : activeConnection.Node.GetNodeByUrl(e.Tag.ToString()));
					}
					catch
					{
						continue;
					}
					if (node != null && node is SPWeb)
					{
						break;
					}
				}
				if (node != null && node is SPWeb)
				{
					m_current = (SPWeb)node;
					globalMapperControl.SourceItems = m_current.SiteUsers.ToArray();
					globalMapperControl.SummaryItems = localMappings.ToArray();
				}
				m_sLastSelectedSource = e.Tag.ToString();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (m_sLastSelectedSource != null && e.Tag != null && m_sLastSelectedSource != e.Tag.ToString())
				{
					globalMapperControl.SelectedSource = m_sLastSelectedSource;
				}
				GlobalServices.ErrorHandler.HandleException("Failed to update source: " + ex2.Message, ex2);
			}
			finally
			{
				globalMapperControl.OnMapped += globalMapperControl_OnMapped;
				globalMapperControl.OnUnmapped += globalMapperControl_OnUnmapped;
			}
		}

		private void globalMapperControl_OnTargetChanged(object sender, SourceChangedEventArgs e)
		{
			try
			{
				globalMapperControl.OnMapped -= globalMapperControl_OnMapped;
				globalMapperControl.OnUnmapped -= globalMapperControl_OnUnmapped;
				globalMapperControl.TargetItems = null;
				if (e.Tag == null)
				{
					return;
				}
				Node node = null;
				foreach (Connection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
				{
					try
					{
						node = (activeConnection.Node.DisplayUrl.Equals(e.Tag.ToString()) ? activeConnection.Node : activeConnection.Node.GetNodeByUrl(e.Tag.ToString()));
					}
					catch
					{
						continue;
					}
					if (node != null)
					{
						break;
					}
				}
				if (node != null && node is SPWeb)
				{
					m_current = (SPWeb)node;
					globalMapperControl.TargetItems = m_current.SiteUsers.ToArray();
					globalMapperControl.SummaryItems = localMappings.ToArray();
				}
				m_sLastSelectedTarget = e.Tag.ToString();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				if (m_sLastSelectedTarget != null && e.Tag != null && m_sLastSelectedTarget != e.Tag.ToString())
				{
					globalMapperControl.SelectedTarget = m_sLastSelectedTarget;
				}
				GlobalServices.ErrorHandler.HandleException("Failed to update target: " + ex2.Message, ex2);
			}
			finally
			{
				globalMapperControl.OnMapped += globalMapperControl_OnMapped;
				globalMapperControl.OnUnmapped += globalMapperControl_OnUnmapped;
			}
		}

		private void globalMapperControl_OnUnmapped(object sender, ListSummaryItemEventArgs e)
		{
			if (m_current != null && e.Tag != null)
			{
				ListSummaryItem listSummaryItem = localMappings[e.Tag];
				if (listSummaryItem != null)
				{
					localMappings.Remove(listSummaryItem);
				}
			}
		}

		public void Initialize()
		{
			globalMapperControl.ShowGroups = false;
			globalMapperControl.AllowNewTagCreation = true;
			globalMapperControl.AllowAutomap = false;
			globalMapperControl.RemoveTargetOnMap = false;
			globalMapperControl.RemoveSourceOnMap = true;
			if (localMappings != null)
			{
				localMappings.ClearCollection();
			}
			localMappings = (MappingsCollection)SPGlobalMappings.GlobalUserMappings.Clone();
			try
			{
				List<string> list = new List<string>();
				foreach (Connection activeConnection in Metalogix.Explorer.Settings.ActiveConnections)
				{
					try
					{
						if (activeConnection is SPBaseServer)
						{
							foreach (SPWeb site in (activeConnection as SPBaseServer).Sites)
							{
								if (!list.Contains(site.DisplayUrl))
								{
									list.Add(site.DisplayUrl);
								}
							}
						}
						else
						{
							if (!(activeConnection is SPWeb))
							{
								continue;
							}
							try
							{
								if (!list.Contains((activeConnection as SPWeb).RootWeb.DisplayUrl))
								{
									list.Add((activeConnection as SPWeb).RootWeb.DisplayUrl);
								}
							}
							catch (Exception)
							{
							}
							continue;
						}
					}
					catch
					{
					}
				}
				globalMapperControl.OnNewSourceButtonClicked += globalMapperControl_OnNewButtonClicked;
				globalMapperControl.OnSourceChanged += globalMapperControl_OnSourceChanged;
				globalMapperControl.Sources = list.ToArray();
				globalMapperControl.OnTargetChanged += globalMapperControl_OnTargetChanged;
				globalMapperControl.Targets = list.ToArray();
				globalMapperControl.OnClearClicked += globalMapperControl_OnClearClicked;
			}
			catch (Exception)
			{
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGlobalUserMappings));
			this.globalMapperControl = new Metalogix.UI.WinForms.Data.Mapping.ListMapperControl();
			base.SuspendLayout();
			this.globalMapperControl.AllowAutomap = true;
			this.globalMapperControl.AllowDeletion = true;
			this.globalMapperControl.AllowNewSource = true;
			this.globalMapperControl.AllowNewTagCreation = false;
			this.globalMapperControl.AllowNewTarget = false;
			resources.ApplyResources(this.globalMapperControl, "globalMapperControl");
			this.globalMapperControl.Name = "globalMapperControl";
			this.globalMapperControl.NewSourceLabel = "Create New\nUser";
			this.globalMapperControl.NewTargetLabel = "Target";
			this.globalMapperControl.RemoveSourceOnMap = true;
			this.globalMapperControl.RemoveTargetOnMap = true;
			this.globalMapperControl.SelectedSource = null;
			this.globalMapperControl.SelectedTarget = null;
			this.globalMapperControl.ShowBottomToolStrip = false;
			this.globalMapperControl.ShowGroups = true;
			this.globalMapperControl.ShowSourceOnSource = true;
			this.globalMapperControl.ShowSourceOnTarget = true;
			this.globalMapperControl.SourceItems = null;
			this.globalMapperControl.Sources = null;
			this.globalMapperControl.SummaryItems = new Metalogix.Data.Mapping.ListSummaryItem[0];
			this.globalMapperControl.TargetItems = null;
			this.globalMapperControl.Targets = null;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.globalMapperControl);
			base.Name = "TCGlobalUserMappings";
			base.ResumeLayout(false);
		}
	}
}
