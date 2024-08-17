using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebApplicationCollection : NodeCollection
	{
		private SPBaseServer m_parentServer = null;

		public SPWebApplication Default
		{
			get
			{
				SPWebApplication sPWebApplication;
				foreach (SPWebApplication mCollection in this.m_collection)
				{
					if (mCollection.IsDefault)
					{
						sPWebApplication = mCollection;
						return sPWebApplication;
					}
				}
				sPWebApplication = null;
				return sPWebApplication;
			}
		}

		public SPBaseServer Parent
		{
			get
			{
				return this.m_parentServer;
			}
		}

		public SPWebApplicationCollection(SPBaseServer parent)
		{
			this.m_parentServer = parent;
		}

		public void FetchData()
		{
			string webApplications = this.Parent.Adapter.Reader.GetWebApplications();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(webApplications);
			this.FromXml(xmlDocument);
		}

		private void FromXml(XmlNode xmlNode)
		{
			this.ClearCollection();
			foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//WebApplicationCollection/WebApplication"))
			{
				base.AddToCollection(new SPWebApplication(xmlNodes, this.Parent.Adapter, this.Parent));
			}
		}

		public SPWebApplication GetWebApplicationByUrl(string sUrl)
		{
			SPWebApplication sPWebApplication;
			char[] chrArray = new char[] { '/' };
			foreach (SPWebApplication sPWebApplication1 in this)
			{
				if (sPWebApplication1.Url.TrimEnd(chrArray) == sUrl)
				{
					sPWebApplication = sPWebApplication1;
					return sPWebApplication;
				}
			}
			sPWebApplication = null;
			return sPWebApplication;
		}
	}
}