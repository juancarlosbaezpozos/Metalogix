using Metalogix;
using Metalogix.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;

namespace Metalogix.UI.WinForms
{
	public class UIApplicationSettings
	{
		private XmlDocument m_settingsXMLDoc;

		public int ExplorerHeight
		{
			get
			{
				string setting = this.GetSetting("ExplorerHeight");
				if (setting == null)
				{
					return -1;
				}
				return int.Parse(setting);
			}
			set
			{
				this.StoreSetting("ExplorerHeight", value.ToString());
			}
		}

		public bool IsMaximized
		{
			get
			{
				string setting = this.GetSetting("IsMaximized");
				if (setting == null)
				{
					return false;
				}
				return bool.Parse(setting);
			}
			set
			{
				this.StoreSetting("IsMaximized", value.ToString());
			}
		}

		public int LeftExplorerWidth
		{
			get
			{
				string setting = this.GetSetting("LeftExplorerWidth");
				if (setting == null)
				{
					return -1;
				}
				return int.Parse(setting);
			}
			set
			{
				this.StoreSetting("LeftExplorerWidth", value.ToString());
			}
		}

		private XmlDocument SettingsXMLDoc
		{
			get
			{
				if (this.m_settingsXMLDoc == null)
				{
					bool flag = false;
					this.m_settingsXMLDoc = new XmlDocument();
					if (File.Exists(UISettings.SettingsXMLFile))
					{
						try
						{
							this.m_settingsXMLDoc.Load(UISettings.SettingsXMLFile);
							flag = true;
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							if (exception.Message.Contains("denied"))
							{
								GlobalServices.ErrorHandler.HandleException("Error Loading Settings", string.Concat("Problem loading settings: ", exception.Message), exception, ErrorIcon.Error);
								Environment.Exit(0);
							}
							GlobalServices.ErrorHandler.HandleException("Error Loading Settings", string.Format("Problem loading settings: {0}{1}It will be overwritten and we will continue. :", exception.Message, Environment.NewLine), exception, ErrorIcon.Information);
						}
					}
					if (!flag)
					{
						this.m_settingsXMLDoc.LoadXml("<Settings/>");
					}
				}
				return this.m_settingsXMLDoc;
			}
		}

		public bool ShowExplorerCheckBoxes
		{
			get
			{
				string setting = this.GetSetting("ShowExplorerCheckBoxes");
				if (setting == null)
				{
					return true;
				}
				return bool.Parse(setting);
			}
			set
			{
				this.StoreSetting("ShowExplorerCheckBoxes", value.ToString());
			}
		}

		public bool ShowJobsList
		{
			get
			{
				string setting = this.GetSetting("ShowJobsList");
				if (setting == null)
				{
					return true;
				}
				return bool.Parse(setting);
			}
			set
			{
				this.StoreSetting("ShowJobsList", value.ToString());
			}
		}

		public bool ShowSecondExplorer
		{
			get
			{
				string setting = this.GetSetting("ShowSecondExplorer");
				if (setting == null)
				{
					return true;
				}
				return bool.Parse(setting);
			}
			set
			{
				this.StoreSetting("ShowSecondExplorer", value.ToString());
			}
		}

		public Size WindowSize
		{
			get
			{
				string setting = this.GetSetting("WindowWidth");
				string str = this.GetSetting("WindowHeight");
				if (setting == null || str == null)
				{
					return new Size();
				}
				return new Size(int.Parse(setting), int.Parse(str));
			}
			set
			{
				this.StoreSetting("WindowHeight", value.Height.ToString());
				this.StoreSetting("WindowWidth", value.Width.ToString());
			}
		}

		public UIApplicationSettings()
		{
		}

		public void FlushSettings()
		{
			try
			{
				this.SettingsXMLDoc.Save(UISettings.SettingsXMLFile);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException("Error Saving Settings", string.Concat("Problem saving settings: ", exception.Message), exception, ErrorIcon.Warning);
			}
		}

		private string GetSetting(string sSettingName)
		{
			XmlNode xmlNodes = this.SettingsXMLDoc.FirstChild.SelectSingleNode(string.Concat("./", sSettingName));
			if (xmlNodes == null)
			{
				return null;
			}
			return xmlNodes.InnerText;
		}

		private void StoreSetting(string sSettingName, string sValue)
		{
			XmlNode xmlNodes = this.SettingsXMLDoc.FirstChild.SelectSingleNode(string.Concat("./", sSettingName));
			if (xmlNodes == null)
			{
				xmlNodes = this.SettingsXMLDoc.CreateElement(sSettingName);
				this.SettingsXMLDoc.FirstChild.AppendChild(xmlNodes);
			}
			xmlNodes.InnerText = sValue;
			if (this.SettingsChanged != null)
			{
				this.SettingsChanged();
			}
		}

		public event UIApplicationSettings.SettingsChangedHander SettingsChanged;

		public delegate void SettingsChangedHander();
	}
}