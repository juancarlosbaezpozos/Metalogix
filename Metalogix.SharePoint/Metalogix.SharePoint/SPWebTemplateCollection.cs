using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebTemplateCollection : IEnumerable<SPWebTemplate>, IEnumerable
	{
		private List<SPWebTemplate> m_data;

		private SPWeb m_spWeb = null;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPWebTemplate this[int index]
		{
			get
			{
				return this.m_data[index];
			}
		}

		public SPWebTemplate this[string sName]
		{
			get
			{
				SPWebTemplate sPWebTemplate = this.m_data.FirstOrDefault<SPWebTemplate>((SPWebTemplate w) => w.Name == sName);
				return sPWebTemplate;
			}
		}

		public SPWebTemplateCollection(SPWebTemplate[] data)
		{
			this.m_data = new List<SPWebTemplate>(data);
		}

		public SPWebTemplateCollection()
		{
			this.m_data = new List<SPWebTemplate>();
		}

		public SPWebTemplateCollection(SPWeb web)
		{
			this.m_spWeb = web;
			this.m_data = new List<SPWebTemplate>();
		}

		public static int CompareTemplateByName(SPWebTemplate x, SPWebTemplate y)
		{
			return x.Title.CompareTo(y.Title);
		}

		public void FetchData()
		{
			this.m_data.Clear();
			string webTemplates = this.m_spWeb.Adapter.Reader.GetWebTemplates();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(webTemplates);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes(".//WebTemplate"))
			{
				try
				{
					SPWebTemplate sPWebTemplate = new SPWebTemplate(xmlNodes);
					this.m_data.Add(sPWebTemplate);
				}
				catch
				{
				}
			}
		}

		public SPWebTemplate Find(int iID, int iConfig)
		{
			SPWebTemplate sPWebTemplate = this.m_data.FirstOrDefault<SPWebTemplate>((SPWebTemplate t) => (t.ID != iID ? false : (t.Config == iConfig ? true : iConfig < 0)));
			return sPWebTemplate;
		}

		public IEnumerator<SPWebTemplate> GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		public SPWebTemplate MapWebTemplate(SPWebTemplate sourceTemplate)
		{
			SPWebTemplate sPWebTemplate = this.Find(sourceTemplate.ID, sourceTemplate.Config);
			if (sPWebTemplate == null)
			{
				sPWebTemplate = this.Find(sourceTemplate.ID, -1);
			}
			return sPWebTemplate;
		}

		public void Sort()
		{
			this.m_data.Sort(new Comparison<SPWebTemplate>(SPWebTemplateCollection.CompareTemplateByName));
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}
	}
}