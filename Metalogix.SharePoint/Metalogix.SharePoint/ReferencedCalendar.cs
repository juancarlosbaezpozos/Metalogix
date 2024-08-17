using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class ReferencedCalendar
	{
		protected XmlNode m_referenceXml;

		public ReferencedCalendarType Type
		{
			get
			{
				return ReferencedCalendar.ParseType(this.m_referenceXml);
			}
		}

		protected ReferencedCalendar(XmlNode referenceXml)
		{
			this.m_referenceXml = referenceXml;
		}

		public static ReferencedCalendar Create(XmlNode node)
		{
			ReferencedCalendar sharePointReferencedCalendar;
			ReferencedCalendarType referencedCalendarType = ReferencedCalendar.ParseType(node);
			if (referencedCalendarType != ReferencedCalendarType.Exchange)
			{
				if (referencedCalendarType != ReferencedCalendarType.SharePoint)
				{
					throw new Exception("Ïncorrect calendar reference type.");
				}
				sharePointReferencedCalendar = new SharePointReferencedCalendar(node);
			}
			else
			{
				sharePointReferencedCalendar = new ExchangeReferencedCalendar(node);
			}
			return sharePointReferencedCalendar;
		}

		private static ReferencedCalendarType ParseType(XmlNode node)
		{
			ReferencedCalendarType referencedCalendarType;
			if (node.Attributes["Type"] == null)
			{
				throw new Exception("Ïncorrect calendar reference: Type property doesn't exist.");
			}
			string value = node.Attributes["Type"].Value;
			if (!value.Equals("SharePoint", StringComparison.OrdinalIgnoreCase))
			{
				if (!value.Equals("Exchange", StringComparison.OrdinalIgnoreCase))
				{
					throw new Exception(string.Concat("Ïncorrect calendar reference: Type property value '", value, "' is incorrect."));
				}
				referencedCalendarType = ReferencedCalendarType.Exchange;
			}
			else
			{
				referencedCalendarType = ReferencedCalendarType.SharePoint;
			}
			return referencedCalendarType;
		}
	}
}