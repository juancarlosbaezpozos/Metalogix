using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebPartCollection : IEnumerable<SPWebPart>, IXMLAbleList, IXmlable, IEnumerable
	{
		private SPWebPartPage m_parentPage = null;

		private List<SPWebPart> m_data = new List<SPWebPart>();

		public Type CollectionType
		{
			get
			{
				return typeof(SPWebPart);
			}
		}

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public object this[int iIndex]
		{
			get
			{
				return this.m_data[iIndex];
			}
		}

		public SPWebPartPage Parent
		{
			get
			{
				return this.m_parentPage;
			}
		}

		public string Xml
		{
			get
			{
				return this.ToXML();
			}
		}

		public SPWebPartCollection(SPWebPartPage parent)
		{
			this.m_parentPage = parent;
		}

		public SPWebPartCollection(SPWebPartPage parent, List<SPWebPart> webParts)
		{
			this.m_parentPage = parent;
			this.m_data.AddRange(webParts.ToArray());
		}

		public SPWebPartCollection(SPWebPartPage parent, XmlNode webPartsXml)
		{
			this.m_parentPage = parent;
			this.FromXML(webPartsXml);
		}

		public SPWebPartCollection(SPWebPartPage parent, string sXml)
		{
			this.m_parentPage = parent;
			this.FromXML(XmlUtility.StringToXmlNode(sXml));
		}

		public SPWebPartCollection(XmlNodeList webPartsListXml)
		{
			XmlNode xmlNodes = (new XmlDocument()).CreateElement("WebParts");
			foreach (XmlNode xmlNodes1 in webPartsListXml)
			{
				xmlNodes.AppendChild(xmlNodes1);
			}
			this.FromXML(xmlNodes);
		}

		public void FromXML(XmlNode xmlNode)
		{
			if (xmlNode != null)
			{
				foreach (XmlNode childNode in xmlNode.ChildNodes)
				{
					this.m_data.Add(SPWebPart.CreateWebPart(childNode));
				}
			}
		}

		public IEnumerator<SPWebPart> GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		public List<SPWebPart> GetWebPartsInZone(string sZone)
		{
			List<SPWebPart> sPWebParts = new List<SPWebPart>();
			foreach (SPWebPart sPWebPart in this)
			{
				if (sPWebPart.Zone.Equals(sZone, StringComparison.OrdinalIgnoreCase))
				{
					sPWebParts.Add(sPWebPart.Clone());
				}
			}
			return sPWebParts;
		}

		public static Dictionary<string, List<SPWebPart>> GetWebPartsSortedByZoneAndPartOrder(IEnumerable<SPWebPart> webParts, bool bSortAscending)
		{
			Dictionary<string, List<SPWebPart>> strs = SPWebPartCollection.SortWebPartsByZone(webParts);
			foreach (string key in strs.Keys)
			{
				SPWebPartCollection.SortWebPartsByPartOrder(strs[key], bSortAscending);
			}
			return strs;
		}

		private List<SPWebPart> MapWebPartsToTargetPage(SPWebPartPage sourceWebPartPage, SPWebPartPage targetWebPartPage)
		{
			List<SPWebPart> sPWebParts = new List<SPWebPart>();
			SPWebPartZoneSet sPWebPartZoneSet = new SPWebPartZoneSet();
			if (targetWebPartPage.HasEmbeddedWikiField)
			{
				sPWebPartZoneSet.AddZones(new string[] { "Right", "Left" });
			}
			SPWebPartZoneMapping sPWebPartZoneMapping = new SPWebPartZoneMapping(SPWebPartZoneSet.Union(targetWebPartPage.Zones, sPWebPartZoneSet));
			Dictionary<string, int> nextAvailablePartOrders = targetWebPartPage.GetNextAvailablePartOrders();
			SPWebPartZoneSet zonesInUse = targetWebPartPage.ZonesInUse;
			List<SPWebPart> sPWebParts1 = new List<SPWebPart>();
			Dictionary<string, List<SPWebPart>> webPartsSortedByZoneAndPartOrder = SPWebPartCollection.GetWebPartsSortedByZoneAndPartOrder(sourceWebPartPage.WebParts, true);
			foreach (string key in webPartsSortedByZoneAndPartOrder.Keys)
			{
				string item = sPWebPartZoneMapping[key];
				int partOrder = (nextAvailablePartOrders.ContainsKey(item) ? nextAvailablePartOrders[item] : 0);
				bool flag = (partOrder > 0 ? false : !zonesInUse.Contains(item));
				foreach (SPWebPart sPWebPart in webPartsSortedByZoneAndPartOrder[key])
				{
					if (!sPWebPart.IsClosed)
					{
						sPWebPart.Zone = item;
						if (flag)
						{
							partOrder = sPWebPart.PartOrder + 1;
						}
						else
						{
							sPWebPart.PartOrder = partOrder;
							partOrder++;
						}
						sPWebParts.Add(sPWebPart);
					}
					else
					{
						sPWebParts1.Add(sPWebPart);
					}
				}
				nextAvailablePartOrders[item] = partOrder;
			}
			if (sPWebParts1.Count > 0)
			{
				foreach (SPWebPart lower in sPWebParts1)
				{
					lower.Zone = sPWebPartZoneMapping[lower.Zone].ToLower();
					lower.PartOrder = (nextAvailablePartOrders.ContainsKey(lower.Zone) ? nextAvailablePartOrders[lower.Zone] : 0);
					nextAvailablePartOrders[lower.Zone] = lower.PartOrder + 1;
					sPWebParts.Add(lower);
				}
			}
			return sPWebParts;
		}

		private static int PartOrderSortAscending(SPWebPart wp1, SPWebPart wp2)
		{
			return wp1.PartOrder.CompareTo(wp2.PartOrder);
		}

		private static int PartOrderSortDescending(SPWebPart wp1, SPWebPart wp2)
		{
			return -SPWebPartCollection.PartOrderSortAscending(wp1, wp2);
		}

		public void ResetWebParts(XmlNode xmlNode)
		{
			this.m_data.Clear();
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				this.m_data.Add(SPWebPart.CreateWebPart(childNode));
			}
		}

		public static void SortWebPartsByPartOrder(List<SPWebPart> webParts, bool bSortAscending)
		{
			if (!bSortAscending)
			{
				webParts.Sort(new Comparison<SPWebPart>(SPWebPartCollection.PartOrderSortDescending));
			}
			else
			{
				webParts.Sort(new Comparison<SPWebPart>(SPWebPartCollection.PartOrderSortAscending));
			}
		}

		public static Dictionary<string, List<SPWebPart>> SortWebPartsByZone(IEnumerable<SPWebPart> webParts)
		{
			Dictionary<string, List<SPWebPart>> strs = new Dictionary<string, List<SPWebPart>>();
			foreach (SPWebPart webPart in webParts)
			{
				string zone = webPart.Zone;
				if (!strs.ContainsKey(zone))
				{
					strs.Add(zone, new List<SPWebPart>());
				}
				strs[zone].Add(webPart);
			}
			return strs;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		public string ToXML()
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
			this.ToXML(new XmlTextWriter(stringWriter));
			return stringWriter.ToString();
		}

		public void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("WebParts");
			foreach (SPWebPart sPWebPart in this)
			{
				xmlWriter.WriteRaw(sPWebPart.Xml);
			}
			xmlWriter.WriteEndElement();
		}
	}
}