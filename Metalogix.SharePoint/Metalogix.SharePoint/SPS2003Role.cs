using Metalogix.Permissions;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPS2003Role : SPRole2003
	{
		public override string[] AvailableRights
		{
			get
			{
				return Enum.GetNames(typeof(SPS2003Rights));
			}
		}

		public SPS2003Role(XmlNode xml) : base(xml)
		{
		}

		public static SPS2003Rights[] DecodeSPSMask(int mask)
		{
			SPS2003Rights[] sPS2003RightsArray;
			if (mask == -1)
			{
				sPS2003RightsArray = new SPS2003Rights[] { SPS2003Rights.FullMask };
			}
			else
			{
				int[] numArray = new int[26];
				int num = 0;
				ArrayList arrayLists = new ArrayList();
				foreach (int value in Enum.GetValues(typeof(SPS2003Rights)))
				{
					numArray[num] = value;
					num++;
				}
				for (int i = 24; i > 0; i--)
				{
					if ((mask < numArray[i] ? false : numArray[i] > 0))
					{
						mask -= numArray[i];
						arrayLists.Add((SPS2003Rights)numArray[i]);
					}
				}
				SPS2003Rights[] item = new SPS2003Rights[arrayLists.Count];
				for (int j = 0; j < arrayLists.Count; j++)
				{
					item[j] = (SPS2003Rights)arrayLists[j];
				}
				sPS2003RightsArray = item;
			}
			return sPS2003RightsArray;
		}

		protected override string[] GetRightsDescription()
		{
			int num = int.Parse(this.m_XML.Attributes["PermMask"].Value);
			SPS2003Rights[] values = SPS2003Role.DecodeSPSMask(num);
			int length = (int)values.Length;
			if (((int)values.Length != 1 ? false : values[0] == SPS2003Rights.FullMask))
			{
				values = (SPS2003Rights[])Enum.GetValues(typeof(SPS2003Rights));
				length = (int)values.Length - 2;
			}
			string[] str = new string[length];
			int num1 = 0;
			SPS2003Rights[] sPS2003RightsArray = values;
			for (int i = 0; i < (int)sPS2003RightsArray.Length; i++)
			{
				SPS2003Rights sPS2003Right = sPS2003RightsArray[i];
				if ((sPS2003Right.ToString() == "FullMask" ? false : sPS2003Right.ToString() != "EmptyMask"))
				{
					str[num1] = sPS2003Right.ToString();
					num1++;
				}
			}
			return str;
		}
	}
}