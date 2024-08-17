using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPMeetingInstanceCollection
	{
		private XmlNode m_instancesXML;

		private SPWeb m_parentWeb;

		private List<SPMeetingInstance> m_data;

		private bool m_bContainsRecurringMeeting = false;

		public bool ContainsRecurringMeeting
		{
			get
			{
				return this.m_bContainsRecurringMeeting;
			}
		}

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		private XmlNode FieldsXML
		{
			get
			{
				return this.m_instancesXML;
			}
		}

		public SPMeetingInstance this[int index]
		{
			get
			{
				return this.m_data[index];
			}
		}

		public string XML
		{
			get
			{
				return this.m_instancesXML.OuterXml;
			}
		}

		public SPMeetingInstanceCollection(SPWeb parentWeb, XmlNode instancesXML)
		{
			this.m_parentWeb = parentWeb;
			this.m_instancesXML = instancesXML;
			this.m_data = new List<SPMeetingInstance>();
			this.ResetInstances();
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		private void ResetInstances()
		{
			this.m_data.Clear();
			if (this.m_instancesXML != null)
			{
				foreach (XmlNode xmlNodes in this.m_instancesXML.SelectNodes("./MeetingInstance"))
				{
					SPMeetingInstance sPMeetingInstance = new SPMeetingInstance(xmlNodes);
					this.m_data.Add(sPMeetingInstance);
					if (sPMeetingInstance.IsRecurringMeeting)
					{
						this.m_bContainsRecurringMeeting = true;
					}
				}
			}
		}
	}
}