using Metalogix.Core.OperationLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration.Nintex.Mappings
{
	public class TitleMapper : IMapper
	{
		public string WorkflowTitle
		{
			get;
			set;
		}

		public TitleMapper()
		{
		}

		public HashSet<Guid> UpdateFile(string file, OperationReporting opReport)
		{
			HashSet<Guid> guids;
			try
			{
				if (string.IsNullOrEmpty(this.WorkflowTitle))
				{
					guids = null;
				}
				else if (string.Equals("Settings.xml", Path.GetFileName(file), StringComparison.OrdinalIgnoreCase))
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(file);
					if (xmlDocument.DocumentElement != null)
					{
						XmlNode workflowTitle = xmlDocument.DocumentElement.SelectSingleNode("//Title");
						if (workflowTitle != null)
						{
							workflowTitle.InnerText = this.WorkflowTitle;
						}
					}
					xmlDocument.Save(file);
					return null;
				}
				else
				{
					guids = null;
				}
			}
			catch (Exception exception)
			{
				opReport.LogError(exception, "An error occurred while mapping Title in nwp file.");
				return null;
			}
			return guids;
		}
	}
}