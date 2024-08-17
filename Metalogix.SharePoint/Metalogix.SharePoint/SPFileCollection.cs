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
	public class SPFileCollection : IEnumerable<SPFile>, IEnumerable
	{
		private List<SPFile> m_innerList;

		public int Count
		{
			get
			{
				return this.m_innerList.Count;
			}
		}

		public SPFile this[int index]
		{
			get
			{
				return this.m_innerList[index];
			}
		}

		public SPFile this[string name]
		{
			get
			{
				SPFile sPFile;
				foreach (SPFile mInnerList in this.m_innerList)
				{
					if (string.Equals(mInnerList.Name, name, StringComparison.OrdinalIgnoreCase))
					{
						sPFile = mInnerList;
						return sPFile;
					}
				}
				sPFile = null;
				return sPFile;
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

		public SPFileCollection(SPWeb web, SPFolderBasic folder) : this(web, folder.WebRelativeUrl)
		{
		}

		public SPFileCollection(SPWeb web, string webRelativeUrl)
		{
			this.m_innerList = new List<SPFile>();
			this.Web = web;
			this.WebRelativeUrl = webRelativeUrl;
		}

		public SPFile Add(SPFile file)
		{
			return this.Add(file, file.GetContent());
		}

		public SPFile Add(SPFile file, byte[] content)
		{
			return this.Add(file.XML, content);
		}

		public SPFile Add(string fileXml, byte[] content)
		{
			fileXml = this.UpdateFileUrlsInXmlForAdd(fileXml);
			string folder = this.Web.Adapter.Writer.AddFileToFolder(fileXml, content, null);
			SPFile sPFile = new SPFile(this.Web, XmlUtility.StringToXmlNode(folder));
			if (this[sPFile.Name] != null)
			{
				this.m_innerList.Remove(this[sPFile.Name]);
			}
			this.m_innerList.Add(sPFile);
			return sPFile;
		}

		public void FetchData()
		{
			List<SPFile> sPFiles = new List<SPFile>();
			string files = this.Web.Adapter.Reader.GetFiles(this.WebRelativeUrl, ListItemQueryType.ListItem);
			if (!string.IsNullOrEmpty(files))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(files);
				foreach (XmlNode xmlNodes in xmlDocument.DocumentElement.SelectNodes("/FolderContent/Files/File"))
				{
					sPFiles.Add(new SPFile(this.Web, xmlNodes));
				}
			}
			this.m_innerList = sPFiles;
		}

		public SPFile Find(Predicate<SPFile> match)
		{
			return this.m_innerList.Find(match);
		}

		public IEnumerator<SPFile> GetEnumerator()
		{
			return this.m_innerList.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.m_innerList.GetEnumerator();
		}

		private string UpdateFileUrlsInXmlForAdd(string fileXml)
		{
			string[] webRelativeUrl;
			XmlNode xmlNode = XmlUtility.StringToXmlNode(fileXml);
			XmlAttribute itemOf = xmlNode.Attributes["ParentFolderPath"];
			if (itemOf == null)
			{
				itemOf = xmlNode.OwnerDocument.CreateAttribute("ParentFolderPath");
				xmlNode.Attributes.Append(itemOf);
			}
			itemOf.Value = this.WebRelativeUrl;
			XmlAttribute xmlAttribute = xmlNode.Attributes["Url"];
			if (xmlAttribute != null)
			{
				webRelativeUrl = new string[] { this.WebRelativeUrl, UrlUtils.GetAfterLastSlash(xmlAttribute.Value) };
				xmlAttribute.Value = UrlUtils.ConcatUrls(webRelativeUrl);
			}
			else
			{
				xmlAttribute = xmlNode.OwnerDocument.CreateAttribute("Url");
				xmlNode.Attributes.Append(xmlAttribute);
				XmlAttribute itemOf1 = xmlNode.Attributes["Name"];
				if (itemOf1 == null)
				{
					throw new Exception("Cannot add file. Xml does not contain a Name attribute.");
				}
				webRelativeUrl = new string[] { this.WebRelativeUrl, itemOf1.Value };
				xmlAttribute.Value = UrlUtils.ConcatUrls(webRelativeUrl);
			}
			return xmlNode.OuterXml;
		}
	}
}