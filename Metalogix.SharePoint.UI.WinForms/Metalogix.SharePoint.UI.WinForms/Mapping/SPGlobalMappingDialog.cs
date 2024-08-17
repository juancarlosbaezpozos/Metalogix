using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraEditors;
using Metalogix.Data.Mapping;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Mapping
{
	public class SPGlobalMappingDialog : ScopableLeftNavigableTabsForm
	{
		private TCGlobalUserMappings _tcGlobalUsermappings;

		private TCGlobalDomainMappings _tcGlobalDomainMappings;

		private TCGlobalUrlMappings _tcGlobalUrlMappings;

		private TCGlobalGuidMappings _tcGlobalGuidMappings;

		private IContainer components;

		private SimpleButton bImport;

		public string SelectedSource
		{
			get
			{
				return _tcGlobalUsermappings.SelectedSource;
			}
			set
			{
				_tcGlobalUsermappings.SelectedSource = value;
			}
		}

		public string SelectedTarget
		{
			get
			{
				return _tcGlobalUsermappings.SelectedTarget;
			}
			set
			{
				_tcGlobalUsermappings.SelectedTarget = value;
			}
		}

		public SPGlobalMappingDialog(bool isBasicMode = false)
		{
			InitializeControls(isBasicMode);
			InitializeComponent();
			Initialize(isBasicMode);
		}

		public SPGlobalMappingDialog(string sSource, string sTarget, bool isBasicMode = false)
		{
			InitializeControls(isBasicMode);
			InitializeComponent();
			Initialize(isBasicMode);
			SelectedSource = sSource;
			SelectedTarget = sTarget;
		}

		private List<TabbableControl> AddMappingTabs(bool isBasicMode)
		{
			List<TabbableControl> list = new List<TabbableControl> { _tcGlobalUsermappings };
			if (!isBasicMode)
			{
				list.Add(_tcGlobalDomainMappings);
				list.Add(_tcGlobalUrlMappings);
				list.Add(_tcGlobalGuidMappings);
			}
			return list;
		}

		private void ApplyBasicModeSkin()
		{
			base.Size = new Size(1005, 680);
			bImport.Width += 15;
			bImport.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			bImport.LookAndFeel.UseDefaultLookAndFeel = false;
			w_btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			w_btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
			w_btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			w_btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
		}

		private void bImport_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Title = "Select xml file to import",
				Filter = "Xml files (*.xml)|*.xml",
				InitialDirectory = ApplicationData.ApplicationPath
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				LoadGlobalMappingsFromXml(openFileDialog.FileName);
			}
		}

		public void CreateDomainMapping(string source, string target)
		{
			ListPickerItem listPickerItem = new ListPickerItem();
			ListPickerItem listPickerItem2 = new ListPickerItem();
			ListSummaryItem listSummaryItem = new ListSummaryItem();
			listPickerItem.Target = source;
			listPickerItem.Tag = source;
			listPickerItem.TargetType = "string";
			listPickerItem2.Target = target;
			listPickerItem2.Tag = target;
			listPickerItem2.TargetType = "string";
			listSummaryItem.Source = listPickerItem;
			listSummaryItem.Target = listPickerItem2;
			bool flag = false;
			ListSummaryItem[] items = _tcGlobalDomainMappings.domainMappingSummaryControl.Items;
			foreach (ListSummaryItem listSummaryItem2 in items)
			{
				if (!(listSummaryItem.Source.Target != listSummaryItem2.Source.Target))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				_tcGlobalDomainMappings.domainMappingSummaryControl.Add(listSummaryItem);
			}
		}

		public void CreateUserMapping(string source, string target)
		{
			ListPickerItem listPickerItem = new ListPickerItem();
			ListPickerItem listPickerItem2 = new ListPickerItem();
			XmlElement xmlElement = new XmlDocument().CreateElement("SPUser");
			xmlElement.SetAttribute("LoginName", source);
			xmlElement.SetAttribute("Name", "");
			xmlElement.SetAttribute("Email", "");
			xmlElement.SetAttribute("Notes", "");
			listPickerItem.Target = source;
			listPickerItem.Tag = new SPUser(xmlElement);
			listPickerItem.TargetType = "SPUser";
			xmlElement.SetAttribute("LoginName", target);
			listPickerItem2.Target = target;
			listPickerItem2.Tag = new SPUser(xmlElement);
			listPickerItem2.TargetType = "SPUser";
			_tcGlobalUsermappings.globalMapperControl.Map(listPickerItem, listPickerItem2);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Initialize(bool isBasicMode)
		{
			base.ThreeStateConfiguration = false;
			base.Tabs = AddMappingTabs(isBasicMode);
			_tcGlobalUsermappings.Dock = DockStyle.Fill;
			bool flag = false;
			int num = base.Size.Height;
			int num2 = base.Size.Width;
			if (SystemInformation.PrimaryMonitorMaximizedWindowSize.Height < base.Size.Height)
			{
				num = SystemInformation.PrimaryMonitorMaximizedWindowSize.Height - 20;
				flag = true;
			}
			Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
			Size size = base.Size;
			if (primaryMonitorMaximizedWindowSize.Width - 10 < size.Width)
			{
				num2 = SystemInformation.PrimaryMonitorMaximizedWindowSize.Width - 20;
				flag = true;
			}
			if (flag)
			{
				base.Size = new Size(num2, num);
			}
			if (isBasicMode)
			{
				ApplyBasicModeSkin();
			}
		}

		private void InitializeComponent()
		{
			this.bImport = new DevExpress.XtraEditors.SimpleButton();
			base.SuspendLayout();
			base.w_btnCancel.Location = new System.Drawing.Point(921, 567);
			base.w_btnOK.Location = new System.Drawing.Point(831, 567);
			base.w_btnOK.Click += new System.EventHandler(w_btnOK_Click);
			this.bImport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.bImport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.bImport.Location = new System.Drawing.Point(12, 567);
			this.bImport.Name = "bImport";
			this.bImport.Size = new System.Drawing.Size(157, 23);
			this.bImport.TabIndex = 3;
			this.bImport.Text = "Import mappings from Xml";
			this.bImport.Click += new System.EventHandler(bImport_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(1008, 602);
			base.Controls.Add(this.bImport);
			this.MinimumSize = new System.Drawing.Size(640, 420);
			base.Name = "SPGlobalMappingDialog";
			this.Text = "Specify Global Mappings";
			base.Controls.SetChildIndex(this.bImport, 0);
			base.Controls.SetChildIndex(base.w_btnOK, 0);
			base.Controls.SetChildIndex(base.w_btnCancel, 0);
			base.ResumeLayout(false);
		}

		private void InitializeControls(bool isBasicMode)
		{
			_tcGlobalUsermappings = new TCGlobalUserMappings(isBasicMode);
			_tcGlobalDomainMappings = new TCGlobalDomainMappings();
			_tcGlobalUrlMappings = new TCGlobalUrlMappings();
			_tcGlobalGuidMappings = new TCGlobalGuidMappings();
		}

		private void LoadGlobalMappingsFromXml(string sFilePath)
		{
			try
			{
				if (!File.Exists(sFilePath))
				{
					return;
				}
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(sFilePath);
				XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes(".//Mapping");
				_tcGlobalUsermappings.globalMapperControl.DisableSort();
				foreach (XmlNode item in xmlNodeList)
				{
					try
					{
						CreateUserMapping(item.Attributes["Source"].Value, item.Attributes["Target"].Value);
					}
					catch (Exception)
					{
					}
				}
				_tcGlobalUsermappings.globalMapperControl.EnableSort();
				xmlNodeList = xmlDocument.DocumentElement.SelectNodes(".//UserMapping");
				foreach (XmlNode item2 in xmlNodeList)
				{
					try
					{
						CreateUserMapping(item2.Attributes["Source"].Value, item2.Attributes["Target"].Value);
					}
					catch (Exception)
					{
					}
				}
				xmlNodeList = xmlDocument.DocumentElement.SelectNodes(".//DomainMapping");
				foreach (XmlNode item3 in xmlNodeList)
				{
					try
					{
						CreateDomainMapping(item3.Attributes["Source"].Value, item3.Attributes["Target"].Value);
					}
					catch (Exception)
					{
					}
				}
				xmlNodeList = xmlDocument.DocumentElement.SelectNodes(".//UrlMapping");
				try
				{
					List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(xmlNodeList.Count);
					foreach (XmlNode item4 in xmlNodeList)
					{
						list.Add(new KeyValuePair<string, string>(item4.Attributes["Source"].Value, item4.Attributes["Target"].Value));
					}
					_tcGlobalUrlMappings.AppendMappings(list);
				}
				catch (Exception)
				{
				}
				xmlNodeList = xmlDocument.DocumentElement.SelectNodes(".//GuidMapping");
				try
				{
					List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>(xmlNodeList.Count);
					foreach (XmlNode item5 in xmlNodeList)
					{
						list2.Add(new KeyValuePair<string, string>(item5.Attributes["Source"].Value, item5.Attributes["Target"].Value));
					}
					_tcGlobalGuidMappings.AppendMappings(list2);
				}
				catch (Exception)
				{
				}
			}
			catch (Exception ex6)
			{
				Exception ex7 = ex6;
				GlobalServices.ErrorHandler.HandleException("Adding the user mappings from xml failed with error: " + ex7.Message, ex7);
			}
		}

		public static void ShowMappingDialog(object sender, EventArgs e)
		{
			MethodInfo method = typeof(SPGlobalMappingDialog).GetMethod("ShowDialog", new Type[0], new ParameterModifier[0]);
			if (method != null)
			{
				object obj = Activator.CreateInstance(typeof(SPGlobalMappingDialog));
				method.Invoke(obj, new object[0]);
			}
		}

		private void w_btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				SPGlobalMappings.GlobalUserMappings = (MappingsCollection)_tcGlobalUsermappings.localMappings.Clone();
				SPGlobalMappings.GlobalDomainMappings = (MappingsCollection)_tcGlobalDomainMappings.LocalMappings.Clone();
				SPGlobalMappings.Save();
			}
			catch (Exception exc)
			{
				GlobalServices.ErrorHandler.HandleException(exc);
			}
		}
	}
}
