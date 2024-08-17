using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPOptimizationNode
	{
		private DateTime m_dtModified = DateTime.MinValue;

		private string m_sName = null;

		private bool m_bHasUniqueValues = false;

		private SPOptimizationNodeCollection m_uniqueChildren = null;

		public SPOptimizationNodeCollection Children
		{
			get
			{
				return this.m_uniqueChildren;
			}
		}

		public bool HasUniqueValues
		{
			get
			{
				return this.m_bHasUniqueValues;
			}
			set
			{
				this.m_bHasUniqueValues = value;
			}
		}

		public DateTime Modified
		{
			get
			{
				return this.m_dtModified;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		internal SPOptimizationNode(string sName, DateTime dtModified, bool bHasUniquePermissions, SPOptimizationNodeCollection children)
		{
			this.m_sName = sName;
			this.m_dtModified = dtModified;
			this.m_bHasUniqueValues = bHasUniquePermissions;
			if (children != null)
			{
				this.m_uniqueChildren = children;
			}
			else
			{
				this.m_uniqueChildren = new SPOptimizationNodeCollection();
			}
		}

		public void AddByUrl(string sUrl)
		{
			string str = sUrl.Trim(new char[] { '/' });
			string str1 = str.Trim();
			string str2 = "";
			int num = str.IndexOf('/');
			if (num >= 0)
			{
				str1 = str.Substring(0, num);
				str2 = str.Substring(num);
			}
			SPOptimizationNode item = this.Children[str1];
			if (item == null)
			{
				item = new SPOptimizationNode(str1, this.m_dtModified, string.IsNullOrEmpty(str2), null);
				this.Children.Add(item);
			}
			if (!string.IsNullOrEmpty(str2))
			{
				item.AddByUrl(str2);
			}
			else
			{
				item.HasUniqueValues = true;
			}
		}

		public SPOptimizationNode Find(string sUrl)
		{
			string str = sUrl.Trim(new char[] { '/' });
			string str1 = str.Trim();
			string str2 = "";
			int num = str.IndexOf('/');
			if (num >= 0)
			{
				str1 = str.Substring(0, num);
				str2 = str.Substring(num);
			}
			SPOptimizationNode item = this.Children[str1];
			return ((item == null ? false : !(str2 == "")) ? item.Find(str2) : item);
		}

		public static SPOptimizationNode InstantiateFromXML(XmlNode xmlNode)
		{
			SPOptimizationNode sPOptimizationNode = null;
			if (xmlNode != null)
			{
				XmlNode xmlNodes = xmlNode.SelectSingleNode("//UniqueValueLocations");
				if (xmlNodes != null)
				{
					DateTime dateTime = Utils.ParseDateAsUtc(xmlNodes.Attributes["LastChange"].Value);
					sPOptimizationNode = new SPOptimizationNode("", dateTime, true, null);
					foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("./UniqueValueLocation"))
					{
						sPOptimizationNode.AddByUrl(xmlNodes1.Attributes["Url"].Value);
					}
				}
			}
			return sPOptimizationNode;
		}
	}
}