using DevExpress.XtraBars;
using Metalogix;
using Metalogix.UI.WinForms.Interfaces;
using System;
using System.Drawing;
using System.Reflection;

namespace Metalogix.UI.WinForms.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class ApplicationSettingAttribute : UIAttribute
	{
		private readonly Type m_settingType;

		private readonly int m_orderIndex;

		private IApplicationSetting m_setting;

		public string DisplayText
		{
			get
			{
				return this.Setting.DisplayText;
			}
		}

		public System.Drawing.Image Image
		{
			get
			{
				return this.GetImage(null);
			}
		}

		public string ImageName
		{
			get
			{
				return this.Setting.ImageName;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.m_settingType.CheckPreconditions();
			}
		}

		public System.Drawing.Image LargeImage
		{
			get
			{
				return this.GetLargeImage(null);
			}
		}

		public string LargeImageName
		{
			get
			{
				return this.Setting.LargeImageName;
			}
		}

		public int OrderIndex
		{
			get
			{
				return this.m_orderIndex;
			}
		}

		public IApplicationSetting Setting
		{
			get
			{
				if (this.m_setting == null)
				{
					this.m_setting = this.CreateSetting();
				}
				return this.m_setting;
			}
		}

		public Type SettingType
		{
			get
			{
				return this.m_settingType;
			}
		}

		public ApplicationSettingAttribute(Type settingType, int orderIndex = 2147483647)
		{
			this.m_settingType = settingType;
			this.m_orderIndex = orderIndex;
		}

		public IApplicationSetting CreateSetting()
		{
			if (this.SettingType == null)
			{
				return null;
			}
			return (IApplicationSetting)Activator.CreateInstance(this.SettingType);
		}

		public ItemClickEventHandler CreateSettingEventHandler()
		{
			IApplicationSetting setting = this.Setting;
			if (setting == null)
			{
				return null;
			}
			IApplicationSetting applicationSetting = setting;
			return new ItemClickEventHandler(applicationSetting.OnClick);
		}

		public System.Drawing.Image GetImage(Assembly assembly = null)
		{
			if (string.IsNullOrEmpty(this.ImageName))
			{
				return null;
			}
			return ImageCache.GetImage(this.ImageName, assembly ?? this.m_settingType.Assembly);
		}

		public System.Drawing.Image GetLargeImage(Assembly assembly = null)
		{
			if (string.IsNullOrEmpty(this.LargeImageName))
			{
				return null;
			}
			return ImageCache.GetImage(this.LargeImageName, assembly ?? this.m_settingType.Assembly);
		}
	}
}