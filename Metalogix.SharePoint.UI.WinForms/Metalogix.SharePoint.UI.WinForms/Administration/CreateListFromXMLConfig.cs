using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Adapters;
using Metalogix.UI.WinForms;
using System;
using System.IO;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(CreateListFromXMLAction) })]
	public class CreateListFromXMLConfig : IActionConfig
	{
		public CreateListFromXMLConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SPWeb item = (SPWeb)context.ActionContext.Targets[0];
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				Multiselect = true,
				AddExtension = true,
				DefaultExt = ".xml",
				Filter = "XML Files (*.xml)|*.xml",
				Title = "Select the XML List File(s)"
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			string[] fileNames = openFileDialog.FileNames;
			for (int i = 0; i < (int)fileNames.Length; i++)
			{
				string str = fileNames[i];
				try
				{
					string str1 = "";
					using (StreamReader streamReader = new StreamReader(str))
					{
						while (true)
						{
							string str2 = streamReader.ReadLine();
							string str3 = str2;
							if (str2 == null)
							{
								break;
							}
							str1 = string.Concat(str1, str3);
						}
						streamReader.Close();
					}
					item.Lists.AddList(str1, new AddListOptions());
				}
				catch (Exception exception)
				{
					throw new Exception(string.Concat("Error importing new list. This could be due to invalid XML. Internal error message: ", exception.Message));
				}
			}
			FlatXtraMessageBox.Show("List(s) sucessfully imported.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			return ConfigurationResult.Run;
		}
	}
}