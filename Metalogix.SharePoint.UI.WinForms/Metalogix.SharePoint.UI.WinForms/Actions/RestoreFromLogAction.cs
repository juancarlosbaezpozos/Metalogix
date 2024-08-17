using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Database;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Database;
using Metalogix.UI.WinForms.Actions;
using System;
using System.Collections;

namespace Metalogix.SharePoint.UI.WinForms.Actions
{
	[Image("Metalogix.SharePoint.UI.WinForms.Icons.Actions.RestoreFromDatabase.ico")]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(LogItemListControl))]
	[TargetType(typeof(LogItem))]
	public class RestoreFromLogAction : RestoreAction
	{
		public RestoreFromLogAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			bool flag1 = base.AppliesTo(sourceSelections, targetSelections);
			if (flag1)
			{
				LogItemCollection dataSource = ((LogItemListControl)sourceSelections[0]).DataSource;
				IEnumerator enumerator = targetSelections.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LogItem current = (LogItem)enumerator.Current;
						if (!flag1)
						{
							continue;
						}
						if (current.Status == ActionOperationStatus.MissingOnSource)
						{
							SPNode nodeByUrl = (SPNode)Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(current.Target);
							if (nodeByUrl == null || typeof(SPWeb).IsAssignableFrom(nodeByUrl.GetType()))
							{
								flag = false;
								return flag;
							}
							else
							{
								int num = 0;
								LogItem item = (LogItem)dataSource[num];
								string source = null;
								while (current != item)
								{
									if (item.Source != null && item.Source.Length > 0)
									{
										source = item.Source;
									}
									num++;
									item = (LogItem)dataSource[num];
								}
								SPNode sPNode = (SPNode)Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(source);
								if (sPNode != null)
								{
									if (sPNode.Adapter.Writer != null)
									{
										continue;
									}
									flag1 = false;
								}
								else
								{
									flag1 = false;
								}
							}
						}
						else if (current.Status != ActionOperationStatus.MissingOnTarget)
						{
							flag1 = false;
						}
						else
						{
							SPNode nodeByUrl1 = (SPNode)Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(current.Source);
							if (nodeByUrl1 == null || typeof(SPWeb).IsAssignableFrom(nodeByUrl1.GetType()))
							{
								flag = false;
								return flag;
							}
							else
							{
								int num1 = 0;
								LogItem logItem = (LogItem)dataSource[num1];
								string target = null;
								while (current != logItem)
								{
									if (logItem.Target != null && logItem.Target.Length > 0)
									{
										target = logItem.Target;
									}
									num1++;
									logItem = (LogItem)dataSource[num1];
								}
								SPNode sPNode1 = (SPNode)Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(target);
								if (sPNode1 != null)
								{
									if (sPNode1.Adapter.Writer != null)
									{
										continue;
									}
									flag1 = false;
								}
								else
								{
									flag = false;
									return flag;
								}
							}
						}
					}
					return flag1;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return flag;
			}
			return flag1;
		}

		public override void Run(IXMLAbleList source, IXMLAbleList target)
		{
			int num = 0;
			foreach (SPNode sPNode in source)
			{
				SPNode item = (SPNode)target[num];
				this.ActionOptions.LegalMatchedLocation = sPNode.Parent.Location;
				this.ActionOptions.AlgorithmMatchedLocation = this.ActionOptions.LegalMatchedLocation;
				IXMLAbleList nodeCollection = null;
				if (!(sPNode is SPListItem))
				{
					nodeCollection = new NodeCollection(new Node[] { sPNode });
				}
				else
				{
					SPList parentList = ((SPListItem)sPNode).ParentList;
					SPFolder parentFolder = ((SPListItem)sPNode).ParentFolder;
					Node[] nodeArray = new Node[] { sPNode };
					nodeCollection = new SPListItemCollection(parentList, parentFolder, nodeArray);
				}
				Node[] nodeArray1 = new Node[] { item };
				base.Run(nodeCollection, new NodeCollection(nodeArray1));
				num++;
			}
		}
	}
}