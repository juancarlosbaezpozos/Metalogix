using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPFolderBasicCollection : IEnumerable<SPFolderBasic>, IEnumerable
	{
		private List<SPFolderBasic> m_innerList;

		public int Count
		{
			get
			{
				return this.m_innerList.Count;
			}
		}

		public SPFolderBasic this[int index]
		{
			get
			{
				return this.m_innerList[index];
			}
		}

		public SPFolderBasic this[string name]
		{
			get
			{
				SPFolderBasic sPFolderBasic;
				foreach (SPFolderBasic mInnerList in this.m_innerList)
				{
					if (string.Equals(mInnerList.Name, name, StringComparison.OrdinalIgnoreCase))
					{
						sPFolderBasic = mInnerList;
						return sPFolderBasic;
					}
				}
				sPFolderBasic = null;
				return sPFolderBasic;
			}
		}

		public SPWeb Web
		{
			get;
			private set;
		}

		public string WebRelativeUrl
		{
			get;
			private set;
		}

		public SPFolderBasicCollection(SPWeb web, SPFolderBasic folder) : this(web, folder.WebRelativeUrl)
		{
		}

		public SPFolderBasicCollection(SPWeb web, string webRelativeUrl)
		{
			this.m_innerList = new List<SPFolderBasic>();
			this.Web = web;
			this.WebRelativeUrl = webRelativeUrl;
		}

		public SPFolderBasic Add(string folderXml)
		{
			string folder = this.Web.Adapter.Writer.AddFolderToFolder(folderXml);
			SPFolderBasic sPFolderBasic = new SPFolderBasic(this.Web, XmlUtility.StringToXmlNode(folder));
			this.m_innerList.Add(sPFolderBasic);
			return sPFolderBasic;
		}

		public void FetchData()
		{
			List<SPFolderBasic> sPFolderBasics = new List<SPFolderBasic>();
			string files = this.Web.Adapter.Reader.GetFiles(this.WebRelativeUrl, ListItemQueryType.Folder);
			if (!string.IsNullOrEmpty(files))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(files);
				foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes("/FolderContent/Folders/Folder"))
				{
					sPFolderBasics.Add(new SPFolderBasic(this.Web, xmlNodes));
				}
			}
			this.m_innerList = sPFolderBasics;
		}

		public SPFolderBasic Find(Predicate<SPFolderBasic> match)
		{
			return this.m_innerList.Find(match);
		}

		public IEnumerator<SPFolderBasic> GetEnumerator()
		{
			return this.m_innerList.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.m_innerList.GetEnumerator();
		}
	}
}