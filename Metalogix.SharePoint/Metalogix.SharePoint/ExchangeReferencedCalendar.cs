using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class ExchangeReferencedCalendar : ReferencedCalendar
	{
		public ExchangeReferencedCalendar(XmlNode referenceXml) : base(referenceXml)
		{
		}
	}
}