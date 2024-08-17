using Metalogix;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Workflow
{
	public static class WorkflowRespository
	{
		private const string C_CONFIGURATION_XML_FILENAME = "WorkflowConfiguration.xml";

		private const string C_FORMAT_CONFIGPATHNAME = "{0}\\{1}";

		private const string C_FORMAT_RENAME = "{0}_{1}_{2}";

		private const string C_FORMAT_COMMON_APPDATA = "{0}\\{1}\\{2}";

		private const string C_FORMAT_DATETIME = "yyyy-MM-dd-hh.mm.ss.fff";

		private const string C_RENAME_CHANGED = "ChangeDetected";

		private const string C_RENAME_ERROR = "ErrorOpening";

		private static volatile WorkflowActivityCollection workflowActivityCollection;

		private readonly static object xmlLock;

		private static string commonApplicationData;

		private static volatile bool initialised;

		public static WorkflowActivityCollection Activities
		{
			get
			{
				if (WorkflowRespository.workflowActivityCollection == null)
				{
					lock (WorkflowRespository.xmlLock)
					{
						if (WorkflowRespository.workflowActivityCollection == null)
						{
							bool flag = false;
							string str = string.Format("{0}\\{1}", WorkflowRespository.CommonApplicationData, "WorkflowConfiguration.xml");
							try
							{
								if (File.Exists(str))
								{
									XmlSerializer xmlSerializer = new XmlSerializer(typeof(WorkflowActivityCollection));
									try
									{
										FileStream fileStream = new FileStream(str, FileMode.Open);
										try
										{
											WorkflowRespository.workflowActivityCollection = (WorkflowActivityCollection)xmlSerializer.Deserialize(fileStream);
											flag = true;
										}
										finally
										{
											if (fileStream != null)
											{
												((IDisposable)fileStream).Dispose();
											}
										}
									}
									finally
									{
										if (!flag)
										{
											WorkflowRespository.RenameConfigFile(str, "ErrorOpening");
										}
									}
								}
							}
							finally
							{
								if (!flag)
								{
									WorkflowRespository.workflowActivityCollection = new WorkflowActivityCollection();
									WorkflowRespository.workflowActivityCollection.PopulateDefaultSupportedActivities();
									WorkflowRespository.SaveWorkflowActivityRespository();
								}
								else if (WorkflowRespository.workflowActivityCollection.HaveDefaultSupportedActivitesChanged())
								{
									string str1 = WorkflowRespository.RenameConfigFile(str, "ChangeDetected");
									WorkflowRespository.workflowActivityCollection.UpdateDefaultSupportedActivites();
									WorkflowRespository.SaveWorkflowActivityRespository();
									File.Delete(str1);
								}
								WorkflowRespository.initialised = true;
							}
						}
					}
				}
				return WorkflowRespository.workflowActivityCollection;
			}
		}

		private static string CommonApplicationData
		{
			get
			{
				bool flag;
				if (string.IsNullOrEmpty(WorkflowRespository.commonApplicationData))
				{
					WorkflowRespository.commonApplicationData = string.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ApplicationData.CompanyFolderName, typeof(WorkflowRespository).Namespace);
				}
				if (!Directory.Exists(WorkflowRespository.commonApplicationData))
				{
					DirectoryInfo directoryInfo = Directory.CreateDirectory(WorkflowRespository.commonApplicationData);
					SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
					DirectorySecurity accessControl = directoryInfo.GetAccessControl();
					FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow);
					accessControl.ModifyAccessRule(AccessControlModification.Add, fileSystemAccessRule, out flag);
					directoryInfo.SetAccessControl(accessControl);
				}
				return WorkflowRespository.commonApplicationData;
			}
		}

		public static bool Initialised
		{
			get
			{
				return WorkflowRespository.initialised;
			}
		}

		static WorkflowRespository()
		{
			WorkflowRespository.workflowActivityCollection = null;
			WorkflowRespository.xmlLock = new object();
			WorkflowRespository.commonApplicationData = null;
			WorkflowRespository.initialised = false;
		}

		private static string RenameConfigFile(string configFile, string renamedPrefix)
		{
			string commonApplicationData = WorkflowRespository.CommonApplicationData;
			DateTime now = DateTime.Now;
			string str = string.Format("{0}\\{1}", commonApplicationData, string.Format("{0}_{1}_{2}", renamedPrefix, now.ToString("yyyy-MM-dd-hh.mm.ss.fff"), "WorkflowConfiguration.xml"));
			File.Move(configFile, str);
			return str;
		}

		private static void SaveWorkflowActivityRespository()
		{
			lock (WorkflowRespository.xmlLock)
			{
				if (WorkflowRespository.workflowActivityCollection != null)
				{
					XmlSerializer xmlSerializer = new XmlSerializer(WorkflowRespository.workflowActivityCollection.GetType());
					string str = string.Format("{0}\\{1}", WorkflowRespository.CommonApplicationData, "WorkflowConfiguration.xml");
					StreamWriter streamWriter = new StreamWriter(str, false, Encoding.UTF8);
					try
					{
						xmlSerializer.Serialize(streamWriter, WorkflowRespository.workflowActivityCollection);
					}
					finally
					{
						if (streamWriter != null)
						{
							((IDisposable)streamWriter).Dispose();
						}
					}
				}
			}
		}
	}
}