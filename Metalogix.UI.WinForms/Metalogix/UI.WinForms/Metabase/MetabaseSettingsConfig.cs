using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.Actions;
using Metalogix.Metabase.Options;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	[ActionConfig(new Type[] { typeof(MetabaseSettingsAction) })]
	public class MetabaseSettingsConfig : IActionConfig
	{
		public MetabaseSettingsConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Connection item = context.ActionContext.Targets[0] as Connection;
			if (item == null)
			{
				throw new NullReferenceException("connection");
			}
			MetabaseSettingsOptions metabaseSettingsOption = new MetabaseSettingsOptions();
			if (item.MetabaseConnection == null)
			{
				metabaseSettingsOption.UseDefault = true;
			}
			else
			{
				metabaseSettingsOption.MetabaseType = item.MetabaseConnection.MetabaseType;
				metabaseSettingsOption.MetabaseContext = item.MetabaseConnection.MetabaseContext;
			}
			MetabaseSettingsDialog metabaseSettingsDialog = new MetabaseSettingsDialog()
			{
				Options = metabaseSettingsOption
			};
			if (metabaseSettingsDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = metabaseSettingsDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}