using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.Actions;
using Metalogix.Metabase.Interfaces;
using Metalogix.Metabase.Options;
using Metalogix.UI.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	[ActionConfig(new Type[] { typeof(DisconnectAction) })]
	public class DisconnectConfig : IActionConfig
	{
		public DisconnectConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult = ConfigurationResult.Run;
			foreach (object target in context.ActionContext.Targets)
			{
				Connection connection = target as Connection;
				if (target == null || connection.MetabaseConnection == null)
				{
					continue;
				}
				MetabaseConnection metabaseConnection = connection.MetabaseConnection;
				DisconnectOptions actionOptions = context.GetActionOptions<DisconnectOptions>();
				string adapterType = metabaseConnection.Adapter.AdapterType;
				if (adapterType == null || !(adapterType == "SqlCe"))
				{
					continue;
				}
				string adapterContext = metabaseConnection.Adapter.AdapterContext;
				if (FlatXtraMessageBox.Show(string.Format("Do you wish to save the metabase database for {0}?", connection.Node.DisplayUrl), "Save Metabase Database", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					continue;
				}
				SaveFileDialog saveFileDialog = new SaveFileDialog()
				{
					Title = "Save Metabase Sql Server Compact database...",
					AddExtension = true,
					DefaultExt = ".sdf",
					Filter = "Sql Server Compact database (*.sdf) | *.sdf",
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
					CheckFileExists = false,
					OverwritePrompt = true
				};
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					continue;
				}
				string fileName = saveFileDialog.FileName;
				actionOptions.SaveFileMappings.Add(adapterContext, fileName);
			}
			return configurationResult;
		}
	}
}