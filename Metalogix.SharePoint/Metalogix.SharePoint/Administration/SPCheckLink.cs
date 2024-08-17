using Metalogix.SharePoint;
using System;
using System.Xml;

namespace Metalogix.SharePoint.Administration
{
	public class SPCheckLink : SPLink
	{
		private bool m_bValidLink = false;

		private string m_sValidityExplanation = "";

		private bool m_bFlagged = false;

		private string m_sWebPartType = null;

		public bool IsFlagged
		{
			get
			{
				return this.m_bFlagged;
			}
			protected set
			{
				this.m_bFlagged = value;
			}
		}

		public bool IsValidLink
		{
			get
			{
				return this.m_bValidLink;
			}
			protected set
			{
				this.m_bValidLink = value;
			}
		}

		public string ValidityExplanation
		{
			get
			{
				return this.m_sValidityExplanation;
			}
			set
			{
				this.m_sValidityExplanation = value;
			}
		}

		public string WebPartType
		{
			get
			{
				return this.m_sWebPartType;
			}
			set
			{
				this.m_sWebPartType = value;
			}
		}

		public SPCheckLink(string sLinkValue, LinkType sourceOfLink, string sWebPartType, SPNode parentNode, string sPropertyWithinParent, bool bIsValidLink, string sValidityMessage, bool bIsFlagged) : base(sLinkValue, sourceOfLink, parentNode, sPropertyWithinParent)
		{
			this.WebPartType = sWebPartType;
			this.IsValidLink = bIsValidLink;
			this.ValidityExplanation = sValidityMessage;
			this.IsFlagged = bIsFlagged;
		}

		public SPCheckLink(string sLinkValue, LinkType linkSourceType, string sWebPartType, string sParentUrl, string sPropertyWithinParent, bool bIsValidLink, string sValidityMessage, bool bIsFlagged) : base(sLinkValue, linkSourceType, sParentUrl, sPropertyWithinParent)
		{
			this.WebPartType = sWebPartType;
			this.IsValidLink = bIsValidLink;
			this.ValidityExplanation = sValidityMessage;
			this.IsFlagged = bIsFlagged;
		}

		public void WriteToXml(XmlTextWriter writer, bool bVerbose)
		{
			writer.WriteStartElement("Link");
			writer.WriteAttributeString("SourceType", Enum.GetName(typeof(LinkType), base.LinkSourceType));
			if ((base.LinkSourceType != LinkType.WebPart || this.WebPartType == null ? false : bVerbose))
			{
				writer.WriteAttributeString("WebPartType", this.WebPartType);
			}
			if (!string.IsNullOrEmpty(base.PropertyNameWithinParent))
			{
				writer.WriteAttributeString("Property", base.PropertyNameWithinParent);
			}
			writer.WriteAttributeString("TargetURL", base.LinkValue);
			if ((!bVerbose ? false : !string.IsNullOrEmpty(base.LinkSourceUrl)))
			{
				writer.WriteAttributeString("SourceAddress", base.LinkSourceUrl);
			}
			writer.WriteAttributeString("Result", string.Concat((this.IsValidLink ? "Succeeded" : "Failed"), (!string.IsNullOrEmpty(this.ValidityExplanation) ? string.Concat(" (", this.ValidityExplanation, ")") : "")));
			writer.WriteEndElement();
		}
	}
}