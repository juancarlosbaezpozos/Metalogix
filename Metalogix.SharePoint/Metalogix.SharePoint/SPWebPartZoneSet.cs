using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public class SPWebPartZoneSet : Set<string>
	{
		public SPWebPartZoneSet()
		{
		}

		public SPWebPartZoneSet(string[] zones)
		{
			this.AddZones(zones);
		}

		public SPWebPartZoneSet(Set<string> zones)
		{
			this.AddZones(zones.ToArray());
		}

		public void AddZone(string sZone)
		{
			if (!this.ContainsIgnoreCase(sZone))
			{
				base.AddToCollection(sZone);
			}
		}

		public void AddZones(string[] zones)
		{
			string[] strArrays = zones;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				this.AddZone(strArrays[i]);
			}
		}

		public void AddZones(List<string> zones)
		{
			this.AddZones(zones.ToArray());
		}

		public bool Contains(string[] zones)
		{
			bool flag;
			string[] strArrays = zones;
			int num = 0;
			while (true)
			{
				if (num >= (int)strArrays.Length)
				{
					flag = true;
					break;
				}
				else if (this.ContainsIgnoreCase(strArrays[num]))
				{
					num++;
				}
				else
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public bool Contains(List<string> zones)
		{
			return this.Contains(zones.ToArray());
		}

		public bool ContainsIgnoreCase(string sZone)
		{
			bool flag;
			foreach (string str in this)
			{
				if (str.Equals(sZone, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		private string GetZoneIgnoreCase(string sZone)
		{
			string str;
			foreach (string str1 in this)
			{
				if (str1.Equals(sZone, StringComparison.OrdinalIgnoreCase))
				{
					str = str1;
					return str;
				}
			}
			str = null;
			return str;
		}

		public static SPWebPartZoneSet Intersection(SPWebPartZoneSet s1, SPWebPartZoneSet s2)
		{
			return new SPWebPartZoneSet(Set<string>.Intersection(s1, s2));
		}

		public static SPWebPartZoneSet operator +(SPWebPartZoneSet s1, SPWebPartZoneSet s2)
		{
			return s1 + s2;
		}

		public static SPWebPartZoneSet operator -(SPWebPartZoneSet s1, SPWebPartZoneSet s2)
		{
			return s1 - s2;
		}

		public bool RemoveZone(string sZone)
		{
			bool flag;
			string zoneIgnoreCase = this.GetZoneIgnoreCase(sZone);
			flag = (zoneIgnoreCase == null ? false : base.RemoveFromCollection(zoneIgnoreCase));
			return flag;
		}

		public void RemoveZones(string[] zones)
		{
			string[] strArrays = zones;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				this.RemoveZone(strArrays[i]);
			}
		}

		public void RemoveZones(List<string> zones)
		{
			this.RemoveZones(zones.ToArray());
		}

		public override string ToString()
		{
			string str = "{";
			if (base.Count > 0)
			{
				foreach (string str1 in this)
				{
					str = string.Concat(str, str1, ", ");
				}
				str = str.Substring(0, str.Length - 2);
			}
			str = string.Concat(str, "}");
			return str;
		}

		public static SPWebPartZoneSet Union(SPWebPartZoneSet s1, SPWebPartZoneSet s2)
		{
			return new SPWebPartZoneSet(Set<string>.Union(s1, s2));
		}
	}
}