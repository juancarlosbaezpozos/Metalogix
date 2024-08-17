using Metalogix.Permissions;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPRole2003 : SPRole
	{
		public override string[] AvailableRights
		{
			get
			{
				return Enum.GetNames(typeof(SPRights2003));
			}
		}

		public int PermissionMask
		{
			get
			{
				int num = int.Parse(this.m_XML.Attributes["PermMask"].Value);
				return num;
			}
		}

		public SPRole2003(XmlNode xml) : base(xml)
		{
		}

		public static SPRights2003[] DecodeMask(int mask)
		{
			SPRights2003[] sPRights2003Array;
			if (mask == -1)
			{
				sPRights2003Array = new SPRights2003[] { SPRights2003.FullMask };
			}
			else
			{
				int[] numArray = new int[25];
				int num = 0;
				ArrayList arrayLists = new ArrayList();
				foreach (int value in Enum.GetValues(typeof(SPRights2003)))
				{
					numArray[num] = value;
					num++;
				}
				for (int i = 24; i > 0; i--)
				{
					if ((mask < numArray[i] ? false : numArray[i] > 0))
					{
						mask -= numArray[i];
						arrayLists.Add((SPRights2003)numArray[i]);
					}
				}
				SPRights2003[] item = new SPRights2003[arrayLists.Count];
				for (int j = 0; j < arrayLists.Count; j++)
				{
					item[j] = (SPRights2003)arrayLists[j];
				}
				sPRights2003Array = item;
			}
			return sPRights2003Array;
		}

		protected override string[] GetRightsDescription()
		{
			int num = int.Parse(this.m_XML.Attributes["PermMask"].Value);
			SPRights2003[] values = SPRole2003.DecodeMask(num);
			int length = (int)values.Length;
			if (((int)values.Length != 1 ? false : values[0] == SPRights2003.FullMask))
			{
				values = (SPRights2003[])Enum.GetValues(typeof(SPRights2003));
				length = (int)values.Length - 2;
			}
			string[] str = new string[length];
			int num1 = 0;
			SPRights2003[] sPRights2003Array = values;
			for (int i = 0; i < (int)sPRights2003Array.Length; i++)
			{
				SPRights2003 sPRights2003 = sPRights2003Array[i];
				if ((sPRights2003.ToString() == "FullMask" ? false : sPRights2003.ToString() != "EmptyMask"))
				{
					str[num1] = sPRights2003.ToString();
					num1++;
				}
			}
			return str;
		}
	}
}