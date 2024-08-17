using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.UI.WinForms;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	public class ExportItems
	{
		private bool m_includeMetadata;

		public ExportItems(bool includeMetadata)
		{
			this.m_includeMetadata = includeMetadata;
		}

		public void RunAction(IXMLAbleList target)
		{
			DialogResult dialogResult;
			string str = this.showBrowserDialog();
			if (str == null)
			{
				return;
			}
			FileStream fileStream = null;
			XmlTextWriter xmlTextWriter = null;
			List<string> strs = new List<string>();
			int num = 0;
			try
			{
				if (this.m_includeMetadata && target.Count > 0)
				{
					string str1 = string.Format("{0}\\{1}.xml", str, ((SPListItem)((ListItemCollection)target)[0]).ParentList.Name);
					fileStream = new FileStream(str1, FileMode.Create, FileAccess.ReadWrite);
					xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
					xmlTextWriter.WriteStartElement("ListItems");
				}
				string empty = string.Empty;
				bool flag = false;
				DialogResult dialogResult1 = DialogResult.Yes;
				foreach (SPListItem sPListItem in target)
				{
					if (sPListItem.ParentList.IsDocumentLibrary)
					{
						empty = string.Format("{0}\\{1}", str, sPListItem.Name);
						if (File.Exists(empty))
						{
							if (!flag)
							{
								FileAlreadyExistsDialog fileAlreadyExistsDialog = new FileAlreadyExistsDialog()
								{
									FileName = empty
								};
								dialogResult = fileAlreadyExistsDialog.ShowDialog();
								flag = (flag ? true : fileAlreadyExistsDialog.ApplyToAllItems);
								if (flag)
								{
									dialogResult1 = dialogResult;
								}
							}
							else
							{
								dialogResult = dialogResult1;
							}
							if (dialogResult == DialogResult.No)
							{
								continue;
							}
							if (dialogResult == DialogResult.Cancel)
							{
								break;
							}
						}
						byte[] binary = sPListItem.GetBinary();
						if (binary == null)
						{
							strs.Add(sPListItem.Name);
							continue;
						}
						else if ((int)binary.Length > 0)
						{
							using (FileStream fileStream1 = new FileStream(empty, FileMode.Create, FileAccess.ReadWrite))
							{
								fileStream1.Write(binary, 0, (int)binary.Length);
								fileStream1.Flush();
								fileStream1.Close();
							}
							num++;
						}
					}
					if (!this.m_includeMetadata)
					{
						continue;
					}
					xmlTextWriter.WriteNode(XmlUtility.StringToXmlNode(sPListItem.XMLWithVersions).CreateNavigator(), true);
				}
			}
			finally
			{
				if (this.m_includeMetadata)
				{
					if (xmlTextWriter != null)
					{
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.Flush();
						xmlTextWriter.Close();
					}
					if (fileStream != null)
					{
						fileStream.Close();
						fileStream.Dispose();
					}
				}
			}
			if (strs.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Couldn't retrieve the contents of the following files(they may be externalized):");
				stringBuilder.AppendLine();
				foreach (string str2 in strs)
				{
					stringBuilder.AppendLine(str2);
				}
				FlatXtraMessageBox.Show(stringBuilder.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			if (num > 0 && FlatXtraMessageBox.Show(string.Format("Files saved to {0}. View location?", str), "Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
			{
				Process.Start("explorer.exe", str);
			}
		}

		protected string showBrowserDialog()
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
			{
				Description = "Select folder to save to"
			};
			if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
			{
				return null;
			}
			if (FlatXtraMessageBox.Show(string.Concat("Save files to ", folderBrowserDialog.SelectedPath, "?"), "Confirm save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return null;
			}
			return folderBrowserDialog.SelectedPath;
		}
	}
}