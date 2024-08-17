using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class CalendarSettingCollection : ReadOnlyCollection<ReferencedCalendar>
	{
		private XmlNode m_settingsXml;

		internal CalendarSettingCollection(XmlNode settingXml, IList<ReferencedCalendar> list) : base(list)
		{
			this.m_settingsXml = settingXml;
		}

		public static CalendarSettingCollection Create(XmlNode settingsXml)
		{
			CalendarSettingCollection calendarSettingCollection;
			List<ReferencedCalendar> referencedCalendars = new List<ReferencedCalendar>();
			if (settingsXml != null)
			{
				foreach (XmlNode xmlNodes in settingsXml.SelectNodes("AggregationCalendars/AggregationCalendar"))
				{
					referencedCalendars.Add(ReferencedCalendar.Create(xmlNodes));
				}
				calendarSettingCollection = new CalendarSettingCollection(settingsXml, referencedCalendars);
			}
			else
			{
				calendarSettingCollection = new CalendarSettingCollection(null, referencedCalendars);
			}
			return calendarSettingCollection;
		}

		public ReadOnlyCollection<T> GetTypes<T>()
		where T : ReferencedCalendar
		{
			List<T> ts = new List<T>();
			foreach (ReferencedCalendar referencedCalendar in this)
			{
				if (referencedCalendar is T)
				{
					ts.Add((T)referencedCalendar);
				}
			}
			return new ReadOnlyCollection<T>(ts);
		}
	}
}