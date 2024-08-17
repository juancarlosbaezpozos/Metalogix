using Metalogix;
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(PreviewDocumentAction) })]
	public class PreviewDocumentConfig : IActionConfig
	{
		private static List<string> s_fileList;

		private static FormClosedEventHandler s_onClose;

		private string m_sFileName;

		static PreviewDocumentConfig()
		{
			PreviewDocumentConfig.s_fileList = new List<string>();
			PreviewDocumentConfig.s_onClose = new FormClosedEventHandler(PreviewDocumentConfig.On_MainForm_Closed);
		}

		public PreviewDocumentConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			if (PreviewDocumentConfig.s_onClose.Target == null)
			{
				Form item = Application.OpenForms["MainForm"] ?? Application.OpenForms[0];
				item.FormClosed += PreviewDocumentConfig.s_onClose;
			}
			SPListItem sPListItem = (SPListItem)context.ActionContext.Targets[0];
			byte[] binary = sPListItem.GetBinary();
			DirectoryInfo directoryInfo = new DirectoryInfo(string.Concat(ApplicationData.ApplicationPath, "temp\\"));
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			this.m_sFileName = Path.Combine(Path.Combine(ApplicationData.ApplicationPath, "temp"), sPListItem.FileLeafRef);
			FileInfo fileInfo = new FileInfo(this.m_sFileName);
			int num = 1;
			while (fileInfo.Exists)
			{
				this.m_sFileName = Path.Combine(ApplicationData.ApplicationPath, string.Concat(num.ToString(), sPListItem.FileLeafRef));
				fileInfo = new FileInfo(this.m_sFileName);
				num++;
			}
			FileStream fileStream = new FileStream(this.m_sFileName, FileMode.Create, FileAccess.ReadWrite);
			fileStream.Write(binary, 0, (int)binary.Length);
			fileStream.Flush();
			fileStream.Close();
			Process process = new Process();
			process.Exited += new EventHandler(this.On_Proc_Exit);
			process.EnableRaisingEvents = true;
			process.StartInfo.FileName = this.m_sFileName;
			process.StartInfo.UseShellExecute = true;
			process.Start();
			PreviewDocumentConfig.s_fileList.Add(this.m_sFileName);
			return ConfigurationResult.Run;
		}

		private static void On_MainForm_Closed(object sender, FormClosedEventArgs e)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(ApplicationData.ApplicationPath, "temp"));
			if (directoryInfo.Exists)
			{
				FileInfo[] files = directoryInfo.GetFiles();
				for (int i = 0; i < (int)files.Length; i++)
				{
					FileInfo fileInfo = files[i];
					try
					{
						fileInfo.Delete();
					}
					catch
					{
					}
				}
			}
		}

		private void On_Proc_Exit(object sender, EventArgs e)
		{
			FileInfo fileInfo = new FileInfo(this.m_sFileName);
			string[] array = PreviewDocumentConfig.s_fileList.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				string str = array[i];
				FileInfo fileInfo1 = new FileInfo(str);
				if (fileInfo1.Exists && fileInfo1.Extension == fileInfo.Extension)
				{
					try
					{
						fileInfo1.Delete();
						PreviewDocumentConfig.s_fileList.Remove(str);
					}
					catch
					{
					}
				}
			}
		}
	}
}