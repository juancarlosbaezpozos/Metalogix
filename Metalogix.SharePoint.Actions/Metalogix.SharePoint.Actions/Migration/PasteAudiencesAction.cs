using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointAudiences", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Audiences.ico")]
	[MenuText("3:Paste Audiences...{0-Paste}")]
	[Name("Paste Audiences")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPServer))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class PasteAudiencesAction : PasteAction<CopyAudiencesOptions>
	{
		public PasteAudiencesAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!(sourceSelections is AudienceServerNodeCollection))
			{
				return false;
			}
			return base.AppliesTo(sourceSelections, targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, base.SharePointOptions.ForceRefresh);
			SPServer item = target[0] as SPServer;
			SPServer sPServer = source[0] as SPServer;
			if (item == null)
			{
				throw new ArgumentException("Target is not an SPServer");
			}
			if (sPServer.Audiences == null)
			{
				throw new Exception("Audiences do not exist on the source server.");
			}
			if (item.Audiences == null)
			{
				throw new Exception("Audiences do not exist on the target server.");
			}
			if (base.SharePointOptions.PasteStyle == CopyAudiencesOptions.PasteAudienceStyles.DeleteExisting)
			{
				LogItem logItem = new LogItem("Deleting all target Audiences", item.Name, sPServer.DisplayUrl, item.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				try
				{
					try
					{
						item.Audiences.DeleteAllAudiences();
						logItem.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception)
					{
						logItem.Exception = exception;
						return;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem);
				}
			}
			foreach (SPAudience audience in sPServer.Audiences)
			{
				if (audience.ID == Guid.Empty)
				{
					continue;
				}
				LogItem str = new LogItem(string.Concat("Copying Audience: ", audience.Name), item.Name, sPServer.DisplayUrl, item.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(str);
				try
				{
					try
					{
						if (base.SharePointOptions.PasteStyle != CopyAudiencesOptions.PasteAudienceStyles.PreserveExisting || item.Audiences[audience.Name] == null)
						{
							AddAudienceOptions addAudienceOption = new AddAudienceOptions()
							{
								Overwrite = base.SharePointOptions.PasteStyle == CopyAudiencesOptions.PasteAudienceStyles.OverwriteExisting
							};
							SPAudience sPAudience = item.Audiences.AddOrUpdateAudience(audience.XML, addAudienceOption);
							if (!base.SharePointOptions.CheckResults)
							{
								str.Status = ActionOperationStatus.Completed;
							}
							else
							{
								DifferenceLog differenceLogs = new DifferenceLog();
								if (!audience.IsEqual(sPAudience, differenceLogs, base.SharePointOptions.CompareOptions))
								{
									str.Status = ActionOperationStatus.Different;
								}
								else
								{
									str.Status = ActionOperationStatus.Completed;
								}
								if (differenceLogs.ToString() != "")
								{
									str.Information = differenceLogs.ToString();
								}
							}
							if (base.SharePointOptions.Verbose)
							{
								str.SourceContent = audience.XML;
								str.TargetContent = sPAudience.XML;
							}
						}
						else
						{
							str.Operation = string.Concat("Skipping Audience: ", audience.Name);
							str.Information = "Audience already exists on the target server.";
							str.Status = ActionOperationStatus.Skipped;
							base.FireOperationUpdated(str);
							continue;
						}
					}
					catch (Exception exception1)
					{
						str.Exception = exception1;
						if (base.SharePointOptions.Verbose)
						{
							str.SourceContent = audience.XML;
						}
					}
				}
				finally
				{
					base.FireOperationFinished(str);
				}
			}
			if (base.SharePointOptions.StartAudienceCompilation)
			{
				LogItem logItem1 = new LogItem("Starting audience compilation on target server", item.Name, sPServer.DisplayUrl, item.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem1);
				try
				{
					try
					{
						item.Adapter.Writer.BeginCompilingAllAudiences();
						logItem1.Status = ActionOperationStatus.Completed;
					}
					catch (Exception exception2)
					{
						logItem1.Exception = exception2;
						return;
					}
				}
				finally
				{
					base.FireOperationFinished(logItem1);
				}
			}
		}
	}
}