using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPLanguageCollection : IEnumerable
	{
		private SPBaseServer m_server = null;

		private List<SPLanguage> m_data = null;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPLanguage this[int index]
		{
			get
			{
				return this.m_data[index];
			}
		}

		public SPLanguage this[string sIdentifier]
		{
			get
			{
				SPLanguage sPLanguage;
				List<SPLanguage>.Enumerator enumerator = this.m_data.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SPLanguage current = enumerator.Current;
						if (current.ToString().ToLower() == sIdentifier.ToLower())
						{
							sPLanguage = current;
							return sPLanguage;
						}
						else if (current.LCID.ToString().ToLower() == sIdentifier.ToLower())
						{
							sPLanguage = current;
							return sPLanguage;
						}
						else if (current.Name.ToLower() == sIdentifier.ToLower())
						{
							sPLanguage = current;
							return sPLanguage;
						}
					}
					sPLanguage = null;
					return sPLanguage;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return sPLanguage;
			}
		}

		public SPBaseServer Parent
		{
			get
			{
				return this.m_server;
			}
		}

		public SPLanguageCollection(SPBaseServer parentServer)
		{
			this.m_server = parentServer;
			this.m_data = new List<SPLanguage>();
		}

		public SPLanguageCollection(XmlNode node, SPBaseServer parentServer)
		{
			this.m_server = parentServer;
			this.FromXml(node);
		}

		public void FetchData()
		{
			string languagesAndWebTemplates = this.m_server.Adapter.Reader.GetLanguagesAndWebTemplates();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(languagesAndWebTemplates);
			this.FromXml(xmlDocument);
		}

		public void FromXml(XmlNode node)
		{
			this.m_data.Clear();
			foreach (XmlNode xmlNodes in node.SelectSingleNode("LanguageCollection").SelectNodes("Language"))
			{
				this.m_data.Add(new SPLanguage(xmlNodes, this.Parent));
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}
	}
}