using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Batchable(false, "")]
	[Image("Metalogix.UI.WinForms.Resources.ExportJobsToXML16.png")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("5:Export selected job to xml {0-Jobs}")]
	[MenuTextPlural("5:Export selected jobs to xml {0-Jobs}", PluralCondition.MultipleTargets)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(Job))]
	public class ExportJobsToXml : Metalogix.Actions.Action
	{
		public ExportJobsToXml()
		{
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.EnabledOn(sourceSelections, targetSelections))
			{
				return false;
			}
			return !JobHelper.ContainsRunningActions(targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				if (JobHelper.ContainsRunningActions(target))
				{
					throw new ConditionalDetailException("Cannot export jobs while actions are being run");
				}
				Cursor.Current = Cursors.WaitCursor;
				string tempPath = Path.GetTempPath();
				Guid guid = Guid.NewGuid();
				string str = string.Concat(tempPath, "\\", guid.ToString(), ".xml");
				StreamWriter streamWriter = new StreamWriter(str);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(streamWriter)
				{
					Formatting = Formatting.Indented
				};
				xmlTextWriter.WriteStartElement("Jobs");
				foreach (Job job in target)
				{
					job.ToXML(xmlTextWriter, true);
				}
				xmlTextWriter.WriteEndElement();
				streamWriter.Close();
				Cursor.Current = Cursors.Default;
				Process process = new Process();
				process.StartInfo.FileName = str;
				process.StartInfo.UseShellExecute = true;
				process.Start();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Cursor.Current = Cursors.Default;
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}