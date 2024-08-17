using Metalogix;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.UI.WinForms.Explorer
{
	public static class ApplicationControlHelper
	{
		private static object m_lockNodeItemsViewCache;

		private static volatile Dictionary<Type, ItemsViewType> m_nodeItemsViewCache;

		private static Dictionary<Type, ItemsViewType> NodeItemsViewCache
		{
			get
			{
				if (ApplicationControlHelper.m_nodeItemsViewCache != null)
				{
					return ApplicationControlHelper.m_nodeItemsViewCache;
				}
				lock (ApplicationControlHelper.m_lockNodeItemsViewCache)
				{
					if (ApplicationControlHelper.m_nodeItemsViewCache == null)
					{
						Assembly mainAssembly = ApplicationData.MainAssembly;
						Dictionary<Type, ItemsViewType> types = new Dictionary<Type, ItemsViewType>();
						if (mainAssembly != null)
						{
							object[] customAttributes = mainAssembly.GetCustomAttributes(typeof(ItemsViewAttribute), true);
							for (int i = 0; i < (int)customAttributes.Length; i++)
							{
								ItemsViewAttribute itemsViewAttribute = (ItemsViewAttribute)customAttributes[i];
								if (!types.ContainsKey(itemsViewAttribute.NodeType))
								{
									types.Add(itemsViewAttribute.NodeType, itemsViewAttribute.ViewType);
								}
							}
						}
						ApplicationControlHelper.m_nodeItemsViewCache = types;
					}
				}
				return ApplicationControlHelper.m_nodeItemsViewCache;
			}
		}

		static ApplicationControlHelper()
		{
			ApplicationControlHelper.m_lockNodeItemsViewCache = new object();
		}

		public static ItemsViewType GetItemsViewGivenNode(Node node)
		{
			if (node == null)
			{
				return ItemsViewType.Unspecified;
			}
			return ApplicationControlHelper.GetItemsViewGivenNodeType(node.GetType());
		}

		public static ItemsViewType GetItemsViewGivenNodes(params Node[] nodes)
		{
			if (nodes == null || (int)nodes.Length == 0)
			{
				return ItemsViewType.Unspecified;
			}
			Node[] nodeArray = nodes;
			int num = 0;
			while (num < (int)nodeArray.Length)
			{
				switch (ApplicationControlHelper.GetItemsViewGivenNode(nodeArray[num]))
				{
					case ItemsViewType.Standard:
					case ItemsViewType.Unspecified:
					{
						num++;
						continue;
					}
					case ItemsViewType.Metabase:
					{
						return ItemsViewType.Metabase;
					}
					default:
					{
						goto case ItemsViewType.Unspecified;
					}
				}
			}
			return ItemsViewType.Standard;
		}

		public static ItemsViewType GetItemsViewGivenNodeType(Type nodeType)
		{
			if (nodeType == null || nodeType.GetInterface(typeof(Node).FullName) == null)
			{
				return ItemsViewType.Unspecified;
			}
			if (ApplicationControlHelper.NodeItemsViewCache.ContainsKey(nodeType))
			{
				return ApplicationControlHelper.NodeItemsViewCache[nodeType];
			}
			return ApplicationControlHelper.GetItemsViewGivenNodeType(nodeType.BaseType);
		}
	}
}