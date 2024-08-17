using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public static class SPObjectSizes
	{
		private static Dictionary<Type, long> s_ValueTable;

		private static object s_oLockValueTable;

		static SPObjectSizes()
		{
			SPObjectSizes.s_ValueTable = null;
			SPObjectSizes.s_oLockValueTable = new object();
		}

		public static long GetObjectSize(object oValue)
		{
			long num;
			if (oValue != null)
			{
				if (SPObjectSizes.s_ValueTable == null)
				{
					SPObjectSizes.InitializeValueTable();
				}
				Type type = oValue.GetType();
				long item = (SPObjectSizes.s_ValueTable.ContainsKey(type) ? SPObjectSizes.s_ValueTable[type] : (long)0);
				if (typeof(SPList).IsAssignableFrom(type))
				{
					SPList sPList = oValue as SPList;
					int count = sPList.Fields.Count - 50;
					int count1 = sPList.Views.Count - 3;
					if (count > 0)
					{
						item = item + (long)count * SPObjectSizes.s_ValueTable[typeof(SPField)];
					}
					if (count1 > 0)
					{
						item = item + (long)count1 * SPObjectSizes.s_ValueTable[typeof(SPView)];
					}
				}
				else if (!(type == typeof(SPWeb) ? false : type != typeof(SPSite)))
				{
					if ((oValue as SPWeb).HasPublishingFeature)
					{
						item += (long)150000;
					}
				}
				else if (type == typeof(SPListItem))
				{
					item *= (long)(oValue as SPListItem).VersionHistory.Count;
				}
				else if (type == typeof(SPGroup))
				{
					item += (long)(25 * (oValue as SPGroup).UserCount);
				}
				num = item;
			}
			else
			{
				num = (long)0;
			}
			return num;
		}

		public static long GetObjectTypeSize(Type type)
		{
			if (SPObjectSizes.s_ValueTable == null)
			{
				SPObjectSizes.InitializeValueTable();
			}
			return (SPObjectSizes.s_ValueTable.ContainsKey(type) ? SPObjectSizes.s_ValueTable[type] : (long)0);
		}

		private static void InitializeValueTable()
		{
			lock (SPObjectSizes.s_oLockValueTable)
			{
				if (SPObjectSizes.s_ValueTable == null)
				{
					SPObjectSizes.s_ValueTable = new Dictionary<Type, long>()
					{
						{ typeof(SPListItem), (long)250 },
						{ typeof(SPDiscussionItem), (long)250 },
						{ typeof(SPFolder), (long)250 },
						{ typeof(SPList), (long)15000 },
						{ typeof(SPDiscussionList), (long)15000 },
						{ typeof(SPWeb), (long)20000 },
						{ typeof(SPSite), (long)20000 },
						{ typeof(SPField), (long)750 },
						{ typeof(SPView), (long)5000 },
						{ typeof(SPUser), (long)300 },
						{ typeof(SPGroup), (long)375 }
					};
				}
			}
		}
	}
}