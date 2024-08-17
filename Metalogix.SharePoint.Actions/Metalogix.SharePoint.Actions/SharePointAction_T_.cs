using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions
{
	[ActionConfigRequired(true)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMWebComponents)]
	public abstract class SharePointAction<T> : Metalogix.Actions.Action<T>
	where T : Metalogix.Actions.ActionOptions
	{
		public override string DisplayName
		{
			get
			{
				string shortString = "";
				if (this.m_sources is NodeCollection)
				{
					shortString = ((NodeCollection)this.m_sources).ToShortString();
				}
				return string.Concat(this.Name, (shortString.Length > 0 ? string.Concat(": ", shortString) : ""));
			}
		}

		public T SharePointOptions
		{
			get
			{
				return this.ActionOptions;
			}
			set
			{
				this.ActionOptions.SetFromOptions(value);
				base.FireOptionsChanged();
			}
		}

		protected SharePointAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return SharePointAction<T>.SharePointActionAppliesTo(this, sourceSelections, targetSelections);
		}

		public override bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.EnabledOn(sourceSelections, targetSelections);
			if (!BCSHelper.SharePointActionEnabledOn(this, sourceSelections, targetSelections))
			{
				return false;
			}
			if (!flag || !base.RequiresWriteAccess)
			{
				return flag;
			}
			bool flag1 = true;
			bool flag2 = true;
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						break;
					}
					SPNode current = (SPNode)enumerator.Current;
					flag1 = (!flag1 ? false : current.Adapter.Writer != null);
					if (!flag1)
					{
						break;
					}
					flag2 = SharePointAction<T>.EnabledOnTarget(base.ActionType, current);
				}
				while (flag2);
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			if (flag1)
			{
				return flag2;
			}
			return false;
		}

		public static bool EnabledOnTarget(Type typeAction, SPNode nodeTarget)
		{
			if (nodeTarget == null)
			{
				return false;
			}
			bool flag = true;
			SharePointServerAdapterConfig serverAdapterConfiguration = nodeTarget.Adapter.ServerAdapterConfiguration;
			if (serverAdapterConfiguration != null && !serverAdapterConfiguration.IsEnabled(nodeTarget.Adapter.Credentials.UserName, typeAction.FullName))
			{
				flag = false;
			}
			return flag;
		}

		public static bool SharePointActionAppliesTo(Metalogix.Actions.Action action, IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!Metalogix.Actions.Action.AppliesToBase(action, sourceSelections, targetSelections))
			{
				return false;
			}
			bool flag1 = false;
			foreach (object sourceSelection in sourceSelections)
			{
				SPNode sPNode = sourceSelection as SPNode;
				if (sPNode == null)
				{
					continue;
				}
				if ((sPNode.Parent == null || sPNode is SPSite) && sPNode.Status != ConnectionStatus.Valid)
				{
					flag = false;
					return flag;
				}
				else
				{
					if (sPNode.Parent != null)
					{
						continue;
					}
					if (!flag1)
					{
						flag1 = true;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
			}
			flag1 = false;
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPNode current = enumerator.Current as SPNode;
					if (current == null)
					{
						continue;
					}
					if ((current.Parent == null || current is SPSite) && current.Status != ConnectionStatus.Valid)
					{
						flag = false;
						return flag;
					}
					else
					{
						if (current.Parent != null)
						{
							continue;
						}
						if (!flag1)
						{
							flag1 = true;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
				}
				return true;
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
	}
}