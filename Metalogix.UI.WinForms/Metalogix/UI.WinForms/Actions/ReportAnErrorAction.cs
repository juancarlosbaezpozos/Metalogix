using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.UI.WinForms.Jobs;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace Metalogix.UI.WinForms.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("2:Report an Error {2-Interact}")]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[Shortcut(ShortcutAction.ReportAnError)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Job))]
	public class ReportAnErrorAction : Metalogix.Actions.Action
	{
		public ReportAnErrorAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			string str;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			return ReportAnErrorAction.GetRunLocation(out str);
		}

		private static bool GetRunLocation(out string runLocation)
		{
			FileInfo fileInfo;
			runLocation = string.Empty;
			Version version = ApplicationData.MainAssembly.GetName().Version;
			object value = Registry.GetValue(string.Concat("HKEY_CURRENT_USER\\SOFTWARE\\Metalogix\\BugReporting\\", version), "Location", null);
			if (value != null)
			{
				fileInfo = new FileInfo(value.ToString());
				if (fileInfo.Exists)
				{
					runLocation = fileInfo.FullName;
					return true;
				}
			}
			DirectoryInfo directory = (new FileInfo(Assembly.GetCallingAssembly().FullName)).Directory;
			if (directory != null)
			{
				fileInfo = new FileInfo(Path.Combine(directory.FullName, "Metalogix.BugReporting.exe"));
				if (fileInfo.Exists)
				{
					runLocation = fileInfo.FullName;
					return true;
				}
			}
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			string str;
			try
			{
				JobListControl jobListControl = JobUIHelper.GetJobListControl(source);
				if (jobListControl != null)
				{
					if (ReportAnErrorAction.GetRunLocation(out str))
					{
						jobListControl.RunErrorReportingTool(str);
					}
				}
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}