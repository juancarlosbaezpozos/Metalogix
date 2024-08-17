using Metalogix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public static class LinkUtils
	{
		public static Type[] LinkCorrectors;

		static LinkUtils()
		{
			LinkUtils.LinkCorrectors = LinkUtils.GetLinkCorrectors();
		}

		private static Type[] GetLinkCorrectors()
		{
			List<Type> types = new List<Type>();
			DirectoryInfo parent = Directory.GetParent(ApplicationData.MainAssembly.Location);
			ArrayList arrayLists = new ArrayList();
			arrayLists.AddRange(parent.GetFiles("*.dll"));
			Type type = typeof(ILinkCorrector);
			foreach (FileInfo arrayList in arrayLists)
			{
				Assembly assembly = null;
				try
				{
					assembly = Assembly.LoadFrom(arrayList.FullName);
					if (assembly != null)
					{
						Type[] exportedTypes = assembly.GetExportedTypes();
						for (int i = 0; i < (int)exportedTypes.Length; i++)
						{
							Type type1 = exportedTypes[i];
							if ((type1.GetInterface("ILinkCorrector") != type ? false : !type1.IsAbstract))
							{
								types.Add(type1);
							}
						}
					}
					else
					{
						continue;
					}
				}
				catch (Exception exception)
				{
				}
			}
			return types.ToArray();
		}
	}
}