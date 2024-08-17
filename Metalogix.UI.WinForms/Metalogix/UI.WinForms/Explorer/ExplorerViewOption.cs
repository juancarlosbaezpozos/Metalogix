using Metalogix;
using Metalogix.Core;
using System;
using System.Drawing;

namespace Metalogix.UI.WinForms.Explorer
{
	public abstract class ExplorerViewOption
	{
		protected ExplorerViewOption()
		{
		}

		public abstract Type GetApplicableType();

		public virtual Image GetImage()
		{
			return new Bitmap(16, 16);
		}

		public virtual Image GetLargeImage()
		{
			return new Bitmap(32, 32);
		}

		public abstract string GetName();

		public abstract string GetOptionName();

		public bool GetSetting()
		{
			return ConfigurationVariables.GetConfigurationValue<bool>(this.GetOptionName());
		}

		protected static void Initialize(string optionName)
		{
			ExplorerViewOption.Initialize(optionName, false);
		}

		protected static void Initialize(string optionName, bool defaultValue)
		{
			ConfigurationVariables.InitializeConfigurationVariable<bool>(ResourceScope.ApplicationAndUserSpecific, optionName, defaultValue);
		}

		public virtual void OnSettingsChanged(string sOptionName, bool bValue)
		{
		}

		public void StoreSetting(bool bValue)
		{
			string optionName = this.GetOptionName();
			ConfigurationVariables.SetConfigurationValue<bool>(this.GetOptionName(), bValue);
			this.OnSettingsChanged(optionName, bValue);
		}
	}
}