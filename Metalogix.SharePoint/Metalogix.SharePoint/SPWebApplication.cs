using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebApplication : SPNode
	{
		private string m_sXml;

		private int m_iMaxFileSize = 0;

		private string m_sName;

		private string m_sURL;

		private SPBaseServer m_parentServer;

		private List<SPPath> m_lPaths;

		private List<SPContentDatabase> m_lContentDatabases;

		private bool m_bDefault = false;

		private bool m_bIsMySitePortal = false;

		public List<SPContentDatabase> ContentDatabases
		{
			get
			{
				return this.m_lContentDatabases;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.m_bDefault;
			}
		}

		public bool IsMySitePortal
		{
			get
			{
				return this.m_bIsMySitePortal;
			}
		}

		public int MaximumFileSize
		{
			get
			{
				return this.m_iMaxFileSize;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public SPBaseServer ParentServer
		{
			get
			{
				return this.m_parentServer;
			}
		}

		public List<SPPath> Paths
		{
			get
			{
				return this.m_lPaths;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				return this.Url;
			}
		}

		public override string Url
		{
			get
			{
				return this.m_sURL;
			}
		}

		public override string XML
		{
			get
			{
				return this.m_sXml;
			}
		}

		public SPWebApplication(XmlNode node, SharePointAdapter adapter, SPBaseServer parent) : base(adapter, parent)
		{
			this.m_parentServer = parent;
			this.m_lPaths = new List<SPPath>();
			this.m_lContentDatabases = new List<SPContentDatabase>();
			this.m_sXml = node.OuterXml;
			this.FromXml(node);
		}

		protected override void ClearChildNodes()
		{
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj != null)
			{
				if (obj.GetType() == typeof(SPWebApplication))
				{
					SPWebApplication sPWebApplication = (SPWebApplication)obj;
					object.ReferenceEquals(this, sPWebApplication);
					flag = (sPWebApplication.Name != this.Name ? false : sPWebApplication.Url == this.Url);
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public bool Equals(SPWebApplication webApp)
		{
			bool flag;
			if (!(webApp == null))
			{
				flag = (webApp.Name != this.Name ? false : webApp.Url == this.Url);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		protected override Node[] FetchChildNodes()
		{
			return new SPWebApplication[0];
		}

		public void FromXml(XmlNode node)
		{
			if (node.Name != "WebApplication")
			{
				throw new ArgumentException("Node is not a WebApplication node");
			}
			this.m_sName = node.Attributes["Name"].Value;
			string value = node.Attributes["URL"].Value;
			char[] chrArray = new char[] { '/' };
			this.m_sURL = value.TrimEnd(chrArray);
			if (node.Attributes["IsDefault"] != null)
			{
				this.m_bDefault = bool.Parse(node.Attributes["IsDefault"].Value);
			}
			if (node.Attributes["IsMySitePortal"] != null)
			{
				this.m_bIsMySitePortal = bool.Parse(node.Attributes["IsMySitePortal"].Value);
			}
			if (node.Attributes["MaximumFileSize"] != null)
			{
				this.m_iMaxFileSize = int.Parse(node.Attributes["MaximumFileSize"].Value);
			}
			foreach (XmlNode xmlNodes in node.SelectNodes("./Path"))
			{
				this.Paths.Add(new SPPath(xmlNodes.InnerText, bool.Parse(xmlNodes.Attributes["IsWildcard"].Value)));
			}
			foreach (XmlNode xmlNodes1 in node.SelectNodes("./ContentDatabase"))
			{
				this.ContentDatabases.Add(new SPContentDatabase(xmlNodes1));
			}
		}

		public override int GetHashCode()
		{
			string str = string.Concat(this.Name, "{", this.Url, "}");
			return str.GetHashCode();
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			SPWebApplication sPWebApplication = targetComparable as SPWebApplication;
			flag = (!(sPWebApplication == null) ? this == sPWebApplication : false);
			return flag;
		}

		public static bool operator ==(SPWebApplication a, SPWebApplication b)
		{
			bool flag;
			if (object.ReferenceEquals(a, b))
			{
				flag = true;
			}
			else if ((a == null ? false : b != null))
			{
				flag = (a.Name != b.Name ? false : a.Url == b.Url);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public static bool operator !=(SPWebApplication a, SPWebApplication b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}