using Metalogix.SharePoint.Common.Enums;
using Metalogix.SharePoint.Common.Workflow2013;
using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Common
{
	public class SP2013Utils
	{
		public SP2013Utils()
		{
		}

		public static string CreateSp2013WorkflowConfigXml(Guid listId, SP2013WorkflowSubscription workflowSubscription = null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml("<ConfigXml />");
			XmlNode firstChild = xmlDocument.FirstChild;
			if (listId != Guid.Empty)
			{
				XmlAttribute str = firstChild.OwnerDocument.CreateAttribute(ConfigXMLAttributes.WorkflowListGuid.ToString());
				str.Value = listId.ToString();
				firstChild.Attributes.Append(str);
			}
			if (workflowSubscription != null)
			{
				XmlAttribute xmlAttribute = firstChild.OwnerDocument.CreateAttribute(ConfigXMLAttributes.WorkflowSubscriptionObjectXml.ToString());
				xmlAttribute.Value = SP2013Utils.Serialize<SP2013WorkflowSubscription>(workflowSubscription);
				firstChild.Attributes.Append(xmlAttribute);
			}
			return firstChild.OuterXml;
		}

		public static string Serialize<T>(T result)
		where T : class
		{
			string str;
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				NewLineHandling = NewLineHandling.Entitize,
				Indent = true,
				OmitXmlDeclaration = true
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
			{
				(new XmlSerializer(typeof(T))).Serialize(xmlWriter, result);
				str = stringBuilder.ToString();
			}
			return str;
		}
	}
}