using Metalogix;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Options.Database
{
	public class RestoreOptions : PasteSiteCollectionOptions
	{
		private bool m_bIncludePath;

		private bool m_bConfigured;

		private string m_sWebAppUrl;

		private Location m_algorithmMatchLocation;

		private Location m_legalMatchLocation;

		public Location AlgorithmMatchedLocation
		{
			get
			{
				return this.m_algorithmMatchLocation;
			}
			set
			{
				this.m_algorithmMatchLocation = value;
			}
		}

		public bool Configured
		{
			get
			{
				return this.m_bConfigured;
			}
			set
			{
				this.m_bConfigured = value;
			}
		}

		public bool IncludePath
		{
			get
			{
				return this.m_bIncludePath;
			}
			set
			{
				this.m_bIncludePath = value;
			}
		}

		public Location LegalMatchedLocation
		{
			get
			{
				return this.m_legalMatchLocation;
			}
			set
			{
				this.m_legalMatchLocation = value;
			}
		}

		public string WebApplicationUrl
		{
			get
			{
				return this.m_sWebAppUrl;
			}
			set
			{
				this.m_sWebAppUrl = value;
			}
		}

		public RestoreOptions()
		{
		}

		public override void FromXML(XmlNode xmlNode)
		{
			base.FromXML(xmlNode);
			this.m_bConfigured = true;
			xmlNode = xmlNode.SelectSingleNode("./RestoreOptions");
			XmlAttribute itemOf = xmlNode.Attributes["IncludePath"];
			if (itemOf != null)
			{
				bool.TryParse(itemOf.Value, out this.m_bIncludePath);
			}
			XmlNode xmlNodes = xmlNode.SelectSingleNode("//MatchedLocation/Location");
			if (xmlNodes == null)
			{
				xmlNodes = xmlNode.SelectSingleNode("//AlgorithmMatchedLocation/Location");
				if (xmlNodes != null)
				{
					this.AlgorithmMatchedLocation = new Location(xmlNodes);
				}
				xmlNodes = xmlNode.SelectSingleNode("//LegalMatchedLocation/Location");
				if (xmlNodes != null)
				{
					this.LegalMatchedLocation = new Location(xmlNodes);
				}
			}
			else
			{
				this.AlgorithmMatchedLocation = new Location(xmlNodes);
				this.LegalMatchedLocation = this.AlgorithmMatchedLocation;
			}
			itemOf = xmlNode.Attributes["WebApplicationUrl"];
			if (itemOf != null)
			{
				this.m_sWebAppUrl = itemOf.Value;
				return;
			}
			SPNode node = this.AlgorithmMatchedLocation.GetNode() as SPNode;
			this.m_sWebAppUrl = (node != null ? node.Adapter.Url : "");
		}

		public override void ToXML(XmlWriter xmlTextWriter)
		{
			base.ToXML(xmlTextWriter);
			xmlTextWriter.WriteStartElement("RestoreOptions");
			xmlTextWriter.WriteAttributeString("IncludePath", this.m_bIncludePath.ToString());
			xmlTextWriter.WriteAttributeString("WebApplicationUrl", this.m_sWebAppUrl);
			if (this.AlgorithmMatchedLocation != null)
			{
				xmlTextWriter.WriteStartElement("AlgorithmMatchedLocation");
				this.AlgorithmMatchedLocation.ToXML(xmlTextWriter);
				xmlTextWriter.WriteEndElement();
			}
			if (this.LegalMatchedLocation != null)
			{
				xmlTextWriter.WriteStartElement("LegalMatchedLocation");
				this.LegalMatchedLocation.ToXML(xmlTextWriter);
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
		}
	}
}