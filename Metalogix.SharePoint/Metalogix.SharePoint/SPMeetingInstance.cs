using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPMeetingInstance
	{
		private XmlNode m_instanceNode;

		private int m_iInstanceID = -1;

		private bool m_bIsRecurringMeeting = false;

		private bool m_bIsRecurringMeetingChecked = false;

		public int InstanceID
		{
			get
			{
				if (this.m_iInstanceID < 0)
				{
					this.m_iInstanceID = int.Parse(this["InstanceID"]);
				}
				return this.m_iInstanceID;
			}
		}

		public bool IsRecurringMeeting
		{
			get
			{
				if (!this.m_bIsRecurringMeetingChecked)
				{
					this.m_bIsRecurringMeeting = !string.IsNullOrEmpty(this["RecurrenceData"]);
					this.m_bIsRecurringMeetingChecked = true;
				}
				return this.m_bIsRecurringMeeting;
			}
		}

		public string this[string sPropertyName]
		{
			get
			{
				string value;
				XmlAttribute itemOf = this.m_instanceNode.Attributes[sPropertyName];
				if (itemOf == null)
				{
					value = null;
				}
				else
				{
					value = itemOf.Value;
				}
				return value;
			}
		}

		public SPMeetingInstance(XmlNode instanceNode)
		{
			this.m_instanceNode = instanceNode;
		}

		public bool HasProperty(string sPropertyName)
		{
			return this.m_instanceNode.Attributes[sPropertyName] != null;
		}
	}
}