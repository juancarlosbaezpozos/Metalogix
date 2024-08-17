using Metalogix.Actions;
using Metalogix.SharePoint.Administration.LinkManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Options.Administration.LinkManagement
{
	public class LinkCorrectionOptions : ActionOptions
	{
		private bool m_bReportOnly = true;

		private bool m_bRecursive = true;

		private bool m_bIgnoreQueryStrings = true;

		private bool m_bUseActionTargetAsLinkDictionary = true;

		private IXMLAbleList m_spLocationLinkDictionary;

		private string m_sFieldLookupProperty = "MigrationSourceURL";

		private List<Type> m_listSelectedLinkCorrectors = new List<Type>();

		public bool IgnoreQueryStrings
		{
			get
			{
				return this.m_bIgnoreQueryStrings;
			}
			set
			{
				this.m_bIgnoreQueryStrings = value;
			}
		}

		public IXMLAbleList LocationLinkDictionary
		{
			get
			{
				return this.m_spLocationLinkDictionary;
			}
			set
			{
				this.m_spLocationLinkDictionary = value;
			}
		}

		public string LookupProperty
		{
			get
			{
				return this.m_sFieldLookupProperty;
			}
			set
			{
				this.m_sFieldLookupProperty = value;
			}
		}

		public bool Recursive
		{
			get
			{
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		public bool ReportOnly
		{
			get
			{
				return this.m_bReportOnly;
			}
			set
			{
				this.m_bReportOnly = value;
			}
		}

		public List<Type> SelectedLinkCorrectors
		{
			get
			{
				return this.m_listSelectedLinkCorrectors;
			}
		}

		public bool UseActionTargetAsLinkDictionary
		{
			get
			{
				return this.m_bUseActionTargetAsLinkDictionary;
			}
			set
			{
				this.m_bUseActionTargetAsLinkDictionary = value;
			}
		}

		public LinkCorrectionOptions()
		{
		}

		public override void FromXML(XmlNode xmlNode)
		{
			if (xmlNode == null)
			{
				return;
			}
			XmlNode xmlNodes = xmlNode.SelectSingleNode("//LinkCorrectionOptions");
			if (xmlNodes == null)
			{
				return;
			}
			XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./ReportOnly");
			if (xmlNodes1 != null)
			{
				bool.TryParse(xmlNodes1.InnerText, out this.m_bReportOnly);
			}
			XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./Recursive");
			if (xmlNodes2 != null)
			{
				bool.TryParse(xmlNodes2.InnerText, out this.m_bRecursive);
			}
			XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./IgnoreQueryStrings");
			if (xmlNodes3 != null)
			{
				bool.TryParse(xmlNodes3.InnerText, out this.m_bIgnoreQueryStrings);
			}
			XmlNode xmlNodes4 = xmlNodes.SelectSingleNode("./ActionTargetAsLinkDictionary");
			if (xmlNodes4 != null)
			{
				bool.TryParse(xmlNodes4.InnerText, out this.m_bUseActionTargetAsLinkDictionary);
			}
			XmlNode xmlNodes5 = xmlNodes.SelectSingleNode("./LinkDictionaryLocation");
			if (xmlNodes5 != null && xmlNodes5.HasChildNodes)
			{
				int num = 0;
				int num1 = 10;
				while (num < num1)
				{
					try
					{
						this.m_spLocationLinkDictionary = XMLAbleList.CreateIXMLAbleList(xmlNodes5.FirstChild.OuterXml);
						num = num1;
					}
					catch
					{
						num++;
					}
				}
			}
			XmlNodeList xmlNodeLists = xmlNodes.SelectNodes("./SelectedLinkCorrectors/LinkCorrector");
			if (xmlNodeLists != null)
			{
				this.m_listSelectedLinkCorrectors.Clear();
				Type[] linkCorrectors = LinkUtils.LinkCorrectors;
			Label0:
				foreach (XmlNode xmlNodes6 in xmlNodeLists)
				{
					if (xmlNodes6 == null)
					{
						continue;
					}
					XmlAttribute itemOf = xmlNodes6.Attributes["ClassName"];
					if (itemOf == null)
					{
						continue;
					}
					Type[] typeArray = linkCorrectors;
					int num2 = 0;
					while (num2 < (int)typeArray.Length)
					{
						Type type = typeArray[num2];
						if (type.FullName != itemOf.Value)
						{
							num2++;
						}
						else
						{
							this.m_listSelectedLinkCorrectors.Add(type);
							goto Label0;
						}
					}
				}
			}
			XmlNode xmlNodes7 = xmlNodes.SelectSingleNode("./LookupProperty");
			if (xmlNodes7 != null)
			{
				this.m_sFieldLookupProperty = xmlNodes7.InnerText;
			}
		}

		public override void ToXML(XmlWriter xmlTextWriter)
		{
			xmlTextWriter.WriteStartElement("LinkCorrectionOptions");
			xmlTextWriter.WriteElementString("ReportOnly", this.m_bReportOnly.ToString());
			xmlTextWriter.WriteElementString("Recursive", this.m_bRecursive.ToString());
			xmlTextWriter.WriteElementString("IgnoreQueryStrings", this.m_bIgnoreQueryStrings.ToString());
			xmlTextWriter.WriteElementString("ActionTargetAsLinkDictionary", this.m_bUseActionTargetAsLinkDictionary.ToString());
			if (this.LocationLinkDictionary != null)
			{
				xmlTextWriter.WriteStartElement("LinkDictionaryLocation");
				xmlTextWriter.WriteRaw(XMLAbleList.SerializeXMLAbleList(this.LocationLinkDictionary));
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteStartElement("SelectedLinkCorrectors");
			foreach (Type mListSelectedLinkCorrector in this.m_listSelectedLinkCorrectors)
			{
				xmlTextWriter.WriteStartElement("LinkCorrector");
				xmlTextWriter.WriteAttributeString("ClassName", mListSelectedLinkCorrector.FullName);
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteElementString("LookupProperty", this.m_sFieldLookupProperty);
			xmlTextWriter.WriteEndElement();
		}

		private struct XmlNames
		{
			public const string ACTION_TARGET_AS_LINK_DICTIONARY = "ActionTargetAsLinkDictionary";

			public const string CLASS_NAME = "ClassName";

			public const string IGNORE_QUERY_STRINGS = "IgnoreQueryStrings";

			public const string LINK_CORRECTOR = "LinkCorrector";

			public const string LOOKUP_PROPERTY = "LookupProperty";

			public const string LINKDICTIONARY_LOCATION = "LinkDictionaryLocation";

			public const string MAPPING_LIST = "MappingList";

			public const string OPTIONS = "LinkCorrectionOptions";

			public const string RECURSIVE = "Recursive";

			public const string REPORT_ONLY = "ReportOnly";

			public const string SELECTED_LINK_CORRECTORS = "SelectedLinkCorrectors";
		}
	}
}