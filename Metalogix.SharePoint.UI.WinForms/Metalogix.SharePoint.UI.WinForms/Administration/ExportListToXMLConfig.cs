using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.UI.WinForms;
using Metalogix.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(ExportListToXMLAction) })]
	public class ExportListToXMLConfig : IActionConfig
	{
		public ExportListToXMLConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (context.ActionContext.Targets.Count <= 0)
			{
				FlatXtraMessageBox.Show("No item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return ConfigurationResult.Cancel;
			}
			SPList item = context.ActionContext.Targets[0] as SPList;
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				FileName = item.Name,
				RestoreDirectory = true,
				Filter = "XML (*.xml)|*.xml"
			};
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			string fileName = saveFileDialog.FileName;
			string directoryName = Path.GetDirectoryName(fileName);
			FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
			XmlNode xmlNode = XmlUtility.StringToXmlNode(item.XML);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
			xmlTextWriter.WriteNode(xmlNode.CreateNavigator(), false);
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			fileStream.Close();
			if (FlatXtraMessageBox.Show(string.Concat("File saved to ", directoryName, ". View location?"), "Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
			{
				return ConfigurationResult.Run;
			}
			Process.Start("explorer.exe", directoryName);
			return ConfigurationResult.Run;
		}
	}
}