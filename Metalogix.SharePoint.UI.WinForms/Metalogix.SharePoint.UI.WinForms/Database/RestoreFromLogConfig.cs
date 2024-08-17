using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options.Database;
using Metalogix.SharePoint.UI.WinForms.Actions;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	[ActionConfig(new Type[] { typeof(RestoreFromLogAction) })]
	public class RestoreFromLogConfig : IActionConfig
	{
		public RestoreFromLogConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			ConfigurationResult configurationResult;
			RestoreOptions actionOptions = context.GetActionOptions<RestoreOptions>();
			if (actionOptions.Configured)
			{
				throw new NotSupportedException();
			}
			LogItemCollection dataSource = ((LogItemListControl)context.ActionContext.Sources[0]).DataSource;
			if (dataSource.ParentJob != null)
			{
				List<Node> nodes = new List<Node>();
				List<Node> nodes1 = new List<Node>();
				IEnumerator enumerator = context.ActionContext.Targets.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LogItem current = (LogItem)enumerator.Current;
						Node nodeByUrl = null;
						Node node = null;
						if (current.Status == ActionOperationStatus.MissingOnTarget)
						{
							nodeByUrl = Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(current.Source);
							if (nodeByUrl != null)
							{
								string displayUrl = nodeByUrl.Parent.DisplayUrl;
								string target = null;
								bool flag = false;
								using (IEnumerator<LogItem> enumerator1 = dataSource.GetEnumerator())
								{
									while (enumerator1.MoveNext())
									{
										LogItem logItem = enumerator1.Current;
										if (logItem.Source == displayUrl)
										{
											target = logItem.Target;
										}
										if (logItem != current)
										{
											continue;
										}
										flag = true;
										break;
									}
								}
								if (!flag || target == null)
								{
									FlatXtraMessageBox.Show("Selective Restore Manager cannot identify the restoration target.", "Could Not Restore Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
									configurationResult = ConfigurationResult.Cancel;
									return configurationResult;
								}
								else
								{
									node = Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(target);
								}
							}
							else
							{
								FlatXtraMessageBox.Show("Selective Restore Manager is not currently connected to the item to be restored.", "Could Not Restore Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								configurationResult = ConfigurationResult.Cancel;
								return configurationResult;
							}
						}
						else if (current.Status == ActionOperationStatus.MissingOnSource)
						{
							nodeByUrl = Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(current.Target);
							if (nodeByUrl != null)
							{
								string str = nodeByUrl.Parent.DisplayUrl;
								string source = null;
								bool flag1 = false;
								using (IEnumerator<LogItem> enumerator2 = dataSource.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										LogItem current1 = enumerator2.Current;
										if (current1.Target == str)
										{
											source = current1.Source;
										}
										if (current1 != current)
										{
											continue;
										}
										flag1 = true;
										break;
									}
								}
								if (!flag1 || source == null)
								{
									FlatXtraMessageBox.Show("Selective Restore Manager cannot identify the restoration target.", "Could Not Restore Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
									configurationResult = ConfigurationResult.Cancel;
									return configurationResult;
								}
								else
								{
									node = Metalogix.Explorer.Settings.ActiveConnections.GetNodeByUrl(source);
								}
							}
							else
							{
								FlatXtraMessageBox.Show("Selective Restore Manager is not currently connected to the item to be restored.", "Could Not Restore Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
								configurationResult = ConfigurationResult.Cancel;
								return configurationResult;
							}
						}
						if (node != null)
						{
							nodes.Add(nodeByUrl);
							nodes1.Add(node);
						}
						else
						{
							FlatXtraMessageBox.Show("Selective Restore Manager is not currently connected to the restore location.", "Could Not Restore Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							configurationResult = ConfigurationResult.Cancel;
							return configurationResult;
						}
					}
					context.ActionContext.Sources = new NodeCollection(nodes.ToArray());
					context.ActionContext.Targets = new NodeCollection(nodes1.ToArray());
					actionOptions.IncludePath = false;
					actionOptions.Configured = true;
					return ConfigurationResult.Run;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				return configurationResult;
			}
			return ConfigurationResult.Run;
		}
	}
}