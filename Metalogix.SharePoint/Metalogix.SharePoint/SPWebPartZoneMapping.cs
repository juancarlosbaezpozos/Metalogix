using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.SharePoint
{
	public class SPWebPartZoneMapping
	{
		private static Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> s_ZoneAreaMappings;

		private static object s_oZoneAreaMappingsLock;

		private static Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> s_ZoneAreas;

		private static object s_oZoneAreasLock;

		private Dictionary<string, string> m_StoredMappings = new Dictionary<string, string>();

		private SPWebPartZoneSet m_TargetZones = new SPWebPartZoneSet();

		private Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> m_AreaMappings = null;

		private Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> m_ZoneAreas = null;

		private Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> AreaMappings
		{
			get
			{
				if (this.m_AreaMappings == null)
				{
					this.m_AreaMappings = SPWebPartZoneMapping.GetZoneAreaMappings();
				}
				return this.m_AreaMappings;
			}
		}

		public string this[string sSourceZone]
		{
			get
			{
				string str = sSourceZone;
				if (!this.m_StoredMappings.TryGetValue(sSourceZone, out str))
				{
					this.AddMappingForSourceZone(sSourceZone, out str);
				}
				return str;
			}
		}

		private Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> ZoneAreaListings
		{
			get
			{
				if (this.m_ZoneAreas == null)
				{
					this.m_ZoneAreas = SPWebPartZoneMapping.GetZoneAreas();
				}
				return this.m_ZoneAreas;
			}
		}

		static SPWebPartZoneMapping()
		{
			SPWebPartZoneMapping.s_ZoneAreaMappings = null;
			SPWebPartZoneMapping.s_oZoneAreaMappingsLock = new object();
			SPWebPartZoneMapping.s_ZoneAreas = null;
			SPWebPartZoneMapping.s_oZoneAreasLock = new object();
		}

		public SPWebPartZoneMapping(SPWebPartZoneSet availableTargetZones)
		{
			if (availableTargetZones == null)
			{
				throw new ArgumentNullException("availableTargetZones", "A web part zone mapping object was created without a list of available target zones.");
			}
			this.m_TargetZones = new SPWebPartZoneSet(availableTargetZones);
		}

		private void AddMappingForSourceZone(string sSourceZone, out string sTargetZone)
		{
			if (!this.m_TargetZones.Contains(sSourceZone))
			{
				SPWebPartZoneSet sPWebPartZoneSet = null;
				SPWebPartZoneMapping.ZoneArea zoneArea = SPWebPartZoneMapping.ZoneArea.Null;
				this.FindZoneSet(sSourceZone, out zoneArea, out sPWebPartZoneSet);
				SPWebPartZoneSet sPWebPartZoneSet1 = SPWebPartZoneSet.Intersection(sPWebPartZoneSet, this.m_TargetZones);
				if (sPWebPartZoneSet1.Count > 0)
				{
					this.m_StoredMappings.Add(sSourceZone, sPWebPartZoneSet1.ToArray()[0]);
				}
				else if (zoneArea != SPWebPartZoneMapping.ZoneArea.Null)
				{
					string str = this.FindMappingInAlternateZoneArea(zoneArea);
					if (str != null)
					{
						this.m_StoredMappings.Add(sSourceZone, str);
					}
				}
			}
			else
			{
				this.m_StoredMappings.Add(sSourceZone, sSourceZone);
			}
			if (!this.m_StoredMappings.ContainsKey(sSourceZone))
			{
				this.m_StoredMappings.Add(sSourceZone, sSourceZone);
			}
			sTargetZone = this.m_StoredMappings[sSourceZone];
		}

		public static string DetermineBestZone(string sZoneToMap, SPWebPartZoneSet availableZones)
		{
			if (availableZones == null)
			{
				throw new ArgumentNullException("availableZones", string.Concat("A web part zone mapping was attempted without a list of available target zones. The zone being mapped was '", sZoneToMap, "'."));
			}
			return (new SPWebPartZoneMapping(availableZones))[sZoneToMap];
		}

		public static Dictionary<string, string> DetermineBestZones(SPWebPartZoneSet zonesToMap, SPWebPartZoneSet availableZones)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>(zonesToMap.Count);
			SPWebPartZoneMapping sPWebPartZoneMapping = new SPWebPartZoneMapping(availableZones);
			foreach (string str in zonesToMap)
			{
				strs.Add(str, sPWebPartZoneMapping[str]);
			}
			return strs;
		}

		private string FindMappingInAlternateZoneArea(SPWebPartZoneMapping.ZoneArea sourceArea)
		{
			SPWebPartZoneMapping.ZoneArea[] zoneAreaArray;
			string array;
			if (this.AreaMappings.TryGetValue(sourceArea, out zoneAreaArray))
			{
				SPWebPartZoneMapping.ZoneArea[] zoneAreaArray1 = zoneAreaArray;
				int num = 0;
				while (num < (int)zoneAreaArray1.Length)
				{
					SPWebPartZoneMapping.ZoneArea zoneArea = zoneAreaArray1[num];
					SPWebPartZoneSet sPWebPartZoneSet = SPWebPartZoneSet.Intersection(this.m_TargetZones, this.ZoneAreaListings[zoneArea]);
					if (sPWebPartZoneSet.Count <= 0)
					{
						num++;
					}
					else
					{
						array = sPWebPartZoneSet.ToArray()[0];
						return array;
					}
				}
			}
			array = null;
			return array;
		}

		private void FindZoneSet(string queryZone, out SPWebPartZoneMapping.ZoneArea containingAreaName, out SPWebPartZoneSet containingArea)
		{
			containingAreaName = SPWebPartZoneMapping.ZoneArea.Null;
			containingArea = new SPWebPartZoneSet();
			foreach (SPWebPartZoneMapping.ZoneArea key in this.ZoneAreaListings.Keys)
			{
				if (this.ZoneAreaListings[key].Contains(queryZone))
				{
					containingAreaName = key;
					containingArea = this.ZoneAreaListings[key];
					break;
				}
			}
		}

		private static Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> GetZoneAreaMappings()
		{
			lock (SPWebPartZoneMapping.s_oZoneAreaMappingsLock)
			{
				if (SPWebPartZoneMapping.s_ZoneAreaMappings == null)
				{
					SPWebPartZoneMapping.s_ZoneAreaMappings = new Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]>();
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings = SPWebPartZoneMapping.s_ZoneAreaMappings;
					SPWebPartZoneMapping.ZoneArea[] zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.UpperCentre, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.TopLeft, SPWebPartZoneMapping.ZoneArea.Left };
					sZoneAreaMappings.Add(SPWebPartZoneMapping.ZoneArea.Top, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.Left, SPWebPartZoneMapping.ZoneArea.BottomRight };
					zoneAreas.Add(SPWebPartZoneMapping.ZoneArea.Bottom, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings1 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.UpperCentre, SPWebPartZoneMapping.ZoneArea.LowerCentre, SPWebPartZoneMapping.ZoneArea.Bottom, SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Left };
					sZoneAreaMappings1.Add(SPWebPartZoneMapping.ZoneArea.Centre, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas1 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.LowerCentre, SPWebPartZoneMapping.ZoneArea.Bottom, SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Left };
					zoneAreas1.Add(SPWebPartZoneMapping.ZoneArea.UpperCentre, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings2 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.UpperCentre, SPWebPartZoneMapping.ZoneArea.Bottom, SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Left };
					sZoneAreaMappings2.Add(SPWebPartZoneMapping.ZoneArea.LowerCentre, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas2 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Left, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Top };
					zoneAreas2.Add(SPWebPartZoneMapping.ZoneArea.TopLeft, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings3 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.TopLeft, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Top };
					sZoneAreaMappings3.Add(SPWebPartZoneMapping.ZoneArea.Left, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas3 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Left, SPWebPartZoneMapping.ZoneArea.TopLeft, SPWebPartZoneMapping.ZoneArea.BottomLeft, SPWebPartZoneMapping.ZoneArea.Top };
					zoneAreas3.Add(SPWebPartZoneMapping.ZoneArea.MiddleLeft, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings4 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Left, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Top };
					sZoneAreaMappings4.Add(SPWebPartZoneMapping.ZoneArea.BottomLeft, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas4 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Right, SPWebPartZoneMapping.ZoneArea.MiddleRight, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Bottom };
					zoneAreas4.Add(SPWebPartZoneMapping.ZoneArea.TopRight, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings5 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.TopRight, SPWebPartZoneMapping.ZoneArea.MiddleRight, SPWebPartZoneMapping.ZoneArea.BottomRight, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Bottom };
					sZoneAreaMappings5.Add(SPWebPartZoneMapping.ZoneArea.Right, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas5 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Right, SPWebPartZoneMapping.ZoneArea.TopRight, SPWebPartZoneMapping.ZoneArea.BottomRight, SPWebPartZoneMapping.ZoneArea.Bottom };
					zoneAreas5.Add(SPWebPartZoneMapping.ZoneArea.MiddleRight, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> sZoneAreaMappings6 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Right, SPWebPartZoneMapping.ZoneArea.MiddleRight, SPWebPartZoneMapping.ZoneArea.Centre, SPWebPartZoneMapping.ZoneArea.Bottom };
					sZoneAreaMappings6.Add(SPWebPartZoneMapping.ZoneArea.BottomRight, zoneAreaArray);
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneMapping.ZoneArea[]> zoneAreas6 = SPWebPartZoneMapping.s_ZoneAreaMappings;
					zoneAreaArray = new SPWebPartZoneMapping.ZoneArea[] { SPWebPartZoneMapping.ZoneArea.Left, SPWebPartZoneMapping.ZoneArea.TopLeft, SPWebPartZoneMapping.ZoneArea.MiddleLeft, SPWebPartZoneMapping.ZoneArea.Right, SPWebPartZoneMapping.ZoneArea.TopRight, SPWebPartZoneMapping.ZoneArea.MiddleRight };
					zoneAreas6.Add(SPWebPartZoneMapping.ZoneArea.LeftSideNavigation, zoneAreaArray);
				}
			}
			return SPWebPartZoneMapping.s_ZoneAreaMappings;
		}

		private static Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> GetZoneAreas()
		{
			lock (SPWebPartZoneMapping.s_oZoneAreasLock)
			{
				if (SPWebPartZoneMapping.s_ZoneAreas == null)
				{
					SPWebPartZoneMapping.s_ZoneAreas = new Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet>();
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas = SPWebPartZoneMapping.s_ZoneAreas;
					string[] strArrays = new string[] { "Header", "TitleBar", "Top", "TopColumnZone", "TopZone" };
					sZoneAreas.Add(SPWebPartZoneMapping.ZoneArea.Top, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Bottom", "BottomZone", "Footer" };
					zoneAreas.Add(SPWebPartZoneMapping.ZoneArea.Bottom, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas1 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Body", "Center", "FullPage", "MiddleColumn", "MiddleMiddleZone" };
					sZoneAreas1.Add(SPWebPartZoneMapping.ZoneArea.Centre, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas1 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Row1", "Row2", "TopRow" };
					zoneAreas1.Add(SPWebPartZoneMapping.ZoneArea.UpperCentre, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas2 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Row3", "Row4" };
					sZoneAreas2.Add(SPWebPartZoneMapping.ZoneArea.LowerCentre, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas2 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "TopLeftZone", "MiddleUpperLeftZone" };
					zoneAreas2.Add(SPWebPartZoneMapping.ZoneArea.TopLeft, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas3 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Left", "LeftColumn", "LeftColumnZone", "LeftZone" };
					sZoneAreas3.Add(SPWebPartZoneMapping.ZoneArea.Left, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas3 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "CenterLeft", "CenterLeftColumn", "MiddleLeftZone" };
					zoneAreas3.Add(SPWebPartZoneMapping.ZoneArea.MiddleLeft, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas4 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "BottomLeftZone", "MiddleLowerLeftZone" };
					sZoneAreas4.Add(SPWebPartZoneMapping.ZoneArea.BottomLeft, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas4 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "TopRightZone", "MiddleUpperRightZone" };
					zoneAreas4.Add(SPWebPartZoneMapping.ZoneArea.TopRight, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas5 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "Right", "RightColumn", "RightColumnZone", "RightZone" };
					sZoneAreas5.Add(SPWebPartZoneMapping.ZoneArea.Right, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas5 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "CenterRight", "CenterRightColumn", "MiddleRightZone" };
					zoneAreas5.Add(SPWebPartZoneMapping.ZoneArea.MiddleRight, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> sZoneAreas6 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "BottomRightZone", "MiddleLowerRightZone" };
					sZoneAreas6.Add(SPWebPartZoneMapping.ZoneArea.BottomRight, new SPWebPartZoneSet(strArrays));
					Dictionary<SPWebPartZoneMapping.ZoneArea, SPWebPartZoneSet> zoneAreas6 = SPWebPartZoneMapping.s_ZoneAreas;
					strArrays = new string[] { "BlogNavigator" };
					zoneAreas6.Add(SPWebPartZoneMapping.ZoneArea.LeftSideNavigation, new SPWebPartZoneSet(strArrays));
				}
			}
			return SPWebPartZoneMapping.s_ZoneAreas;
		}

		private enum ZoneArea
		{
			Null,
			Top,
			Bottom,
			Centre,
			UpperCentre,
			LowerCentre,
			TopLeft,
			Left,
			MiddleLeft,
			BottomLeft,
			TopRight,
			Right,
			MiddleRight,
			BottomRight,
			LeftSideNavigation
		}
	}
}