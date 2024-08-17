using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Migration.Nintex.Mappings;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Nintex;
using Metalogix.SharePoint.Nintex.Enums;
using Metalogix.SharePoint.Nintex.NintexWorkflowArgs;
using Metalogix.SharePoint.Nintex.NintexWorkflowResults;
using Metalogix.SharePoint.Options;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	[ActionConfigRequired(false)]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Workflows.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("1:Paste Nintex Workflow... {0-Paste}")]
	[Name("Paste Nintex Workflow")]
	[Shortcut(ShortcutAction.Paste)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[TargetCardinality(Cardinality.One)]
	public abstract class PasteNintexWorkflow : PasteAction<PasteNintexWorkflowOptions>
	{
		protected readonly static string NintexWorkflowsTempStorageLocation;

		static PasteNintexWorkflow()
		{
			PasteNintexWorkflow.NintexWorkflowsTempStorageLocation = SharePointConfigurationVariables.NintexWorkflowsTempStorage;
		}

		protected PasteNintexWorkflow()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}
        
	    private void CleanUpNintexWorkflowsTempStorage(LogItem nintexWorkflowoperation, ExportNintexWorkflowResults exportResult, string mappedNwpFile)
	    {
	        nintexWorkflowoperation.Information = "Cleaning up the workflow files...";
	        base.FireOperationUpdated(nintexWorkflowoperation);
	        System.Action<string> action = delegate (string file)
	        {
	            if (File.Exists(file))
	            {
	                File.Delete(file);
	            }
	        };
	        action(exportResult.NwfFile);
	        action(exportResult.NwpFile);
	        action(mappedNwpFile);
	    }

        private void CreateTempStorageIfNotExist(LogItem nintexWorkflowoperation)
		{
			if (!Directory.Exists(PasteNintexWorkflow.NintexWorkflowsTempStorageLocation))
			{
				nintexWorkflowoperation.Information = string.Format("Creating output folder {0} because it does not exist.", PasteNintexWorkflow.NintexWorkflowsTempStorageLocation);
				base.FireOperationUpdated(nintexWorkflowoperation);
				try
				{
					Directory.CreateDirectory(PasteNintexWorkflow.NintexWorkflowsTempStorageLocation);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.PrepareOperationLog("Failed to create directory.", string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, exception, Environment.NewLine), ActionOperationStatus.Failed, string.Empty, ref nintexWorkflowoperation);
				}
			}
		}

		private ExportNintexWorkflowResults ExportNintexWorkflowOperationLogging(SPNintexWorkflow source, LogItem nintexWorkflowoperation)
		{
			nintexWorkflowoperation.Information = "Exporting workflow...";
			base.FireOperationUpdated(nintexWorkflowoperation);
			ExportNintexWorkflowResults exportNintexWorkflowResult = this.ExportWorkflow(source);
			if (string.IsNullOrEmpty(exportNintexWorkflowResult.Errors))
			{
				nintexWorkflowoperation.Information = "Exported workflow";
				nintexWorkflowoperation.Details = string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, exportNintexWorkflowResult.Log, Environment.NewLine);
				return exportNintexWorkflowResult;
			}
			nintexWorkflowoperation.Information = "Failed to export workflow.";
			nintexWorkflowoperation.Details = string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, exportNintexWorkflowResult.Errors, Environment.NewLine);
			nintexWorkflowoperation.Status = ActionOperationStatus.Failed;
			return null;
		}

		private ExportNintexWorkflowResults ExportWorkflow(SPNintexWorkflow workflow)
		{
			string str = Guid.NewGuid().ToString("N");
			string workflowListTitle = this.GetWorkflowListTitle(workflow);
			ExportNintexWorkflowArgs exportNintexWorkflowArg = new ExportNintexWorkflowArgs()
			{
				WorkflowType = workflow.WorkflowType.ToString(),
				SiteUrl = workflow.ParentList.ParentWeb.Url,
				ListTitle = workflowListTitle,
				WorkflowName = workflow.DisplayName,
				UserName = workflow.Adapter.Credentials.UserName,
				Password = workflow.Adapter.Credentials.Password.ToInsecureString(),
				NwfFile = Path.Combine(PasteNintexWorkflow.NintexWorkflowsTempStorageLocation, string.Concat(str, ".nwf")),
				NwpFile = Path.Combine(PasteNintexWorkflow.NintexWorkflowsTempStorageLocation, string.Concat(str, ".nwp")),
				EndpointUrl = SharePointConfigurationVariables.NintexEndpointUrl
			};
			ExportNintexWorkflowArgs exportNintexWorkflowArg1 = exportNintexWorkflowArg;
			string str1 = NintexWorkflowUtils.ExportWorkflow(exportNintexWorkflowArg1);
			ExportNintexWorkflowResults exportNintexWorkflowResult = new ExportNintexWorkflowResults()
			{
				NwpFile = exportNintexWorkflowArg1.NwpFile,
				NwfFile = exportNintexWorkflowArg1.NwfFile,
				Log = str1
			};
			return exportNintexWorkflowResult;
		}

		private string GetWorkflowListTitle(SPNintexWorkflow workflow)
		{
			string empty = string.Empty;
			try
			{
				if (workflow.AssociatedListID != Guid.Empty)
				{
					SPList listByGuid = workflow.ParentList.ParentWeb.Lists.GetListByGuid(workflow.AssociatedListID.ToString());
					if (listByGuid != null)
					{
						empty = listByGuid.Title;
					}
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = workflow.ParentList.Title;
				}
			}
			catch (Exception exception)
			{
				empty = workflow.ParentList.Title;
			}
			return empty;
		}

		private ImportNintexWorkflowResults ImportWorkflow(NintexWorkflowType type, SPWeb web, SPList list, string nwpFile)
		{
			ImportNintexWorkflowArgs importNintexWorkflowArg = new ImportNintexWorkflowArgs()
			{
				WorkflowType = type.ToString(),
				SiteUrl = web.Url,
				ListTitle = (list != null ? list.Title : string.Empty),
				UserName = web.Adapter.Credentials.UserName,
				Password = web.Adapter.Credentials.Password.ToInsecureString(),
				NwpFile = nwpFile
			};
			string str = NintexWorkflowUtils.ImportWorkflow(importNintexWorkflowArg);
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			return new ImportNintexWorkflowResults()
			{
				WorkflowId = new Guid(str)
			};
		}

		private ImportNintexWorkflowResults ImportWorkflowOperationLogging(SPNintexWorkflow source, SPWeb web, SPList list, LogItem nintexWorkflowoperation, string mappedNwpFile)
		{
			nintexWorkflowoperation.Information = "Importing the mapped workflow to the target...";
			base.FireOperationUpdated(nintexWorkflowoperation);
			ImportNintexWorkflowResults importNintexWorkflowResult = this.ImportWorkflow(source.WorkflowType, web, list, mappedNwpFile);
			if (importNintexWorkflowResult != null && !string.IsNullOrEmpty(importNintexWorkflowResult.Errors))
			{
				nintexWorkflowoperation.Information = "Failed to import workflow.";
				nintexWorkflowoperation.Details = string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, importNintexWorkflowResult.Errors, Environment.NewLine);
				nintexWorkflowoperation.Status = ActionOperationStatus.Failed;
				return null;
			}
			nintexWorkflowoperation.Information = "Imported workflow";
			nintexWorkflowoperation.Details = string.Format("{0} Workflow id: {1} {2}", nintexWorkflowoperation.Details, (importNintexWorkflowResult != null ? importNintexWorkflowResult.WorkflowId.ToString("D") : "N/A"), Environment.NewLine);
			base.FireOperationUpdated(nintexWorkflowoperation);
			return importNintexWorkflowResult;
		}

		protected void MigrateWorkflow(SPNintexWorkflow source, SPWeb web, SPList list)
		{
			SPNode sPNode;
			if (list != null)
			{
				sPNode = list;
			}
			else
			{
				sPNode = web;
			}
			SPNode sPNode1 = sPNode;
			string str = string.Format("Copy Nintex {0} Workflow", source.WorkflowType);
			LogItem logItem = new LogItem(str, source.Name, source.Url, sPNode1.Url, ActionOperationStatus.Running)
			{
				Information = "Exporting working...",
				Details = string.Empty
			};
			LogItem xML = logItem;
			base.FireOperationStarted(xML);
			try
			{
				try
				{
					this.CreateTempStorageIfNotExist(xML);
					ExportNintexWorkflowResults exportNintexWorkflowResult = this.ExportNintexWorkflowOperationLogging(source, xML);
					if (exportNintexWorkflowResult != null)
					{
						string str1 = this.UpdateWorkflowMappingsOperationLogging(source, xML, web, exportNintexWorkflowResult);
						ImportNintexWorkflowResults importNintexWorkflowResult = this.ImportWorkflowOperationLogging(source, web, list, xML, str1);
						if (importNintexWorkflowResult == null)
						{
							return;
						}
						else if (this.PublishWorkflowOpearationLogging(web, xML, importNintexWorkflowResult) != null)
						{
							if (SharePointConfigurationVariables.CleanupNintexWorkflowsTempStorage)
							{
								this.CleanUpNintexWorkflowsTempStorage(xML, exportNintexWorkflowResult, str1);
							}
							xML.Status = SPUtils.EvaluateLog(xML);
							base.LogTelemetryForWorkflows("Nintex_Workflows");
						}
						else
						{
							return;
						}
					}
					else
					{
						return;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (exception.Message.Contains("The specified package is not valid") || exception.Message.Contains("The application has encountered an unknown error."))
					{
						this.PrepareOperationLog(Resources.NintexWorkflowAppWarning, string.Format("{0} {1} {2}", xML.Details, Environment.NewLine, exception), ActionOperationStatus.Warning, source.XML, ref xML);
					}
					else
					{
						this.PrepareOperationLog("Failed to migrate workflow.", string.Format("{0} {1} {2}", xML.Details, Environment.NewLine, exception), ActionOperationStatus.Failed, source.XML, ref xML);
					}
				}
			}
			finally
			{
				if (base.SharePointOptions.Verbose)
				{
					xML.SourceContent = source.XML;
					xML.TargetContent = sPNode1.XML;
				}
				if (xML.Status == ActionOperationStatus.Completed)
				{
					xML.Information = "Successfully migrated workflow";
				}
				base.FireOperationFinished(xML);
			}
		}

		private void PrepareOperationLog(string info, string details, ActionOperationStatus status, string sourceContent, ref LogItem nintexWorkflowOperation)
		{
			nintexWorkflowOperation.Information = info;
			nintexWorkflowOperation.Details = details;
			nintexWorkflowOperation.Status = status;
			if (!string.IsNullOrEmpty(sourceContent))
			{
				nintexWorkflowOperation.SourceContent = sourceContent;
			}
		}

		private PublishNintexWorkflowResults PublishWorkflow(SPWeb web, Guid workflowId)
		{
			PublishNintexWorkflowArgs publishNintexWorkflowArg = new PublishNintexWorkflowArgs()
			{
				SiteUrl = web.Url,
				UserName = web.Adapter.Credentials.UserName,
				Password = web.Adapter.Credentials.Password.ToInsecureString(),
				WorkflowId = workflowId
			};
			return new PublishNintexWorkflowResults()
			{
				Log = NintexWorkflowUtils.PublishWorkflow(publishNintexWorkflowArg)
			};
		}

		private PublishNintexWorkflowResults PublishWorkflowOpearationLogging(SPWeb web, LogItem nintexWorkflowoperation, ImportNintexWorkflowResults importResult)
		{
			nintexWorkflowoperation.Information = "Publishing the workflow...";
			base.FireOperationUpdated(nintexWorkflowoperation);
			PublishNintexWorkflowResults publishNintexWorkflowResult = this.PublishWorkflow(web, importResult.WorkflowId);
			if (!string.IsNullOrEmpty(publishNintexWorkflowResult.Errors))
			{
				nintexWorkflowoperation.Information = "Failed to publish workflow.";
				nintexWorkflowoperation.Details = string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, publishNintexWorkflowResult.Errors, Environment.NewLine);
				nintexWorkflowoperation.Status = ActionOperationStatus.Failed;
				return null;
			}
			nintexWorkflowoperation.Information = "Successfully published workflow";
			nintexWorkflowoperation.Details = string.Format("{0} {1} {2}", nintexWorkflowoperation.Details, publishNintexWorkflowResult.Log, Environment.NewLine);
			base.FireOperationUpdated(nintexWorkflowoperation);
			return publishNintexWorkflowResult;
		}

		private string UpdateMappingsInWorkflow(SPNintexWorkflow source, string nwpFile, SPWeb targetWeb, LogItem nintexWorkflowoperation)
		{
			NintexWorkflowMapper nintexWorkflowMapper = new NintexWorkflowMapper(nwpFile);
			IMapper[] mapperArray = new IMapper[2];
			TitleMapper titleMapper = new TitleMapper()
			{
				WorkflowTitle = source.Name
			};
			mapperArray[0] = titleMapper;
			WorkflowDataMapper workflowDataMapper = new WorkflowDataMapper()
			{
				GuidMappings = base.GuidMappings,
				LinkCorrector = base.LinkCorrector,
				TargetWeb = targetWeb
			};
			mapperArray[1] = workflowDataMapper;
			nintexWorkflowMapper.Mappers = mapperArray;
			NintexWorkflowMapper nintexWorkflowMapper1 = nintexWorkflowMapper;
			string str = Path.Combine(PasteNintexWorkflow.NintexWorkflowsTempStorageLocation, string.Concat(Path.GetFileNameWithoutExtension(nwpFile), "_mapped.nwp"));
			OperationReportingResult operationReportingResult = new OperationReportingResult(nintexWorkflowMapper1.Save(str));
			if (operationReportingResult.ErrorOccured)
			{
				nintexWorkflowoperation.Status = ActionOperationStatus.Failed;
				nintexWorkflowoperation.Information = operationReportingResult.GetMessageOfFirstErrorElement;
				nintexWorkflowoperation.Details = operationReportingResult.GetAllErrorsAsString;
			}
			else if (operationReportingResult.WarningOccured || operationReportingResult.HasInformation)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (operationReportingResult.WarningOccured)
				{
					stringBuilder.AppendLine("Following activities may not function properly at target since their mapping is not found during migration:");
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(operationReportingResult.GetAllWarningsAsString);
					stringBuilder.AppendLine();
					nintexWorkflowoperation.Status = ActionOperationStatus.Warning;
				}
				if (operationReportingResult.HasInformation)
				{
					stringBuilder.AppendLine("While performing Mapping operation for Nintex Workflows, the below Guids were missing and could not be mapped:");
					stringBuilder.AppendLine();
					stringBuilder.AppendLine(operationReportingResult.AllInformationAsString);
					stringBuilder.AppendLine();
				}
				nintexWorkflowoperation.Information = "Please review details";
				nintexWorkflowoperation.Details = stringBuilder.ToString();
			}
			return str;
		}

		private string UpdateWorkflowMappingsOperationLogging(SPNintexWorkflow source, LogItem nintexWorkflowoperation, SPWeb taregtWeb, ExportNintexWorkflowResults exportResult)
		{
			nintexWorkflowoperation.Information = "Mapping metadata of the workflow...";
			base.FireOperationUpdated(nintexWorkflowoperation);
			string str = this.UpdateMappingsInWorkflow(source, exportResult.NwpFile, taregtWeb, nintexWorkflowoperation);
			nintexWorkflowoperation.Information = "Mapped metadata in workflow.";
			return str;
		}
	}
}