using Metalogix.Actions;
using Metalogix.DataStructures;
using System;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAudience : Metalogix.DataStructures.IComparable
	{
		private XmlNode m_xml;

		private SPAudienceCollection m_collection;

		public string Description
		{
			get
			{
				return this.GetAttributeValue("Description");
			}
		}

		public string GroupOperation
		{
			get
			{
				return this.GetAttributeValue("GroupOperation");
			}
		}

		public Guid ID
		{
			get
			{
				return new Guid(this.GetAttributeValue("ID"));
			}
		}

		public int MemberShipCount
		{
			get
			{
				string attributeValue = this.GetAttributeValue("MemberShipCount");
				return (string.IsNullOrEmpty(attributeValue) ? -1 : int.Parse(attributeValue));
			}
		}

		public string Name
		{
			get
			{
				return this.GetAttributeValue("Name");
			}
		}

		public string Owner
		{
			get
			{
				return this.GetAttributeValue("Owner");
			}
		}

		public string Site
		{
			get
			{
				return this.GetAttributeValue("Site");
			}
		}

		public string XML
		{
			get
			{
				return this.m_xml.OuterXml;
			}
		}

		public SPAudience(SPAudienceCollection parentCollection, XmlNode node)
		{
			this.m_xml = node;
			this.m_collection = parentCollection;
		}

		private string GetAttributeValue(string sName)
		{
			string value;
			XmlAttribute itemOf = this.m_xml.Attributes[sName];
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

		public SPAudience.AudienceRule[] GetAudienceRules()
		{
			XmlNodeList xmlNodeLists = this.m_xml.SelectNodes(".//Audience/Rule");
			SPAudience.AudienceRule[] audienceRuleArray = new SPAudience.AudienceRule[xmlNodeLists.Count];
			for (int i = 0; i < xmlNodeLists.Count; i++)
			{
				SPAudience.AudienceRule audienceRule = new SPAudience.AudienceRule()
				{
					LeftContent = xmlNodeLists[i].Attributes["LeftContent"].Value,
					Operator = xmlNodeLists[i].Attributes["Operator"].Value,
					RightContent = xmlNodeLists[i].Attributes["RightContent"].Value
				};
				audienceRuleArray[i] = audienceRule;
			}
			return audienceRuleArray;
		}

		public bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool flag;
			if (targetComparable == null)
			{
				throw new Exception("Cannot compare an SPAudience to a null value");
			}
			if (!(targetComparable is SPAudience))
			{
				throw new Exception("SPAudience can only be compared to another SPAudience");
			}
			SPAudience sPAudience = (SPAudience)targetComparable;
			if (sPAudience.Name != this.Name)
			{
				differencesOutput.Write("The audience names do not match.", "Audience name");
				flag = false;
			}
			else if (!(sPAudience.Description != this.Description))
			{
				if (sPAudience.GroupOperation != this.GroupOperation)
				{
					differencesOutput.Write("The audience group operations do not match.", "Audience group operation");
					if ((!(sPAudience.GroupOperation != "AUDIENCE_AND_OPERATION") || !(sPAudience.GroupOperation != "AUDIENCE_OR_OPERATION") || !(this.GroupOperation != "AUDIENCE_AND_OPERATION") ? false : this.GroupOperation != "AUDIENCE_OR_OPERATION"))
					{
						flag = false;
						return flag;
					}
				}
				if (!(sPAudience.Owner != this.Owner))
				{
					if (sPAudience.ID != this.ID)
					{
						differencesOutput.Write("The audience IDs do not match. This is expected.", "Audience ID", DifferenceStatus.Difference, true);
					}
					if (sPAudience.MemberShipCount != this.MemberShipCount)
					{
						differencesOutput.Write("The audience membership counts do not match. This is expected.", "Audience membership count", DifferenceStatus.Difference, true);
					}
					if (sPAudience.Site != this.Site)
					{
						differencesOutput.Write("The audience sites do not match. This is expected.", "Audience site", DifferenceStatus.Difference, true);
					}
					SPAudience.AudienceRule[] audienceRules = sPAudience.GetAudienceRules();
					SPAudience.AudienceRule[] audienceRuleArray = this.GetAudienceRules();
					if ((int)audienceRules.Length == (int)audienceRuleArray.Length)
					{
						int num = 0;
						while (num < (int)audienceRuleArray.Length)
						{
							bool flag1 = false;
							int num1 = 0;
							while (num1 < (int)audienceRules.Length)
							{
								SPAudience.AudienceRule audienceRule = audienceRuleArray[num];
								SPAudience.AudienceRule audienceRule1 = audienceRules[num1];
								if ((!(audienceRule.LeftContent == audienceRule1.LeftContent) || !(audienceRule.Operator == audienceRule1.Operator) ? true : !(audienceRule.RightContent == audienceRule1.RightContent)))
								{
									num1++;
								}
								else
								{
									flag1 = true;
									break;
								}
							}
							if (flag1)
							{
								num++;
							}
							else
							{
								differencesOutput.Write("One or more rules do not match.", "Audience rule");
								flag = false;
								return flag;
							}
						}
						flag = true;
					}
					else
					{
						differencesOutput.Write("The audiences do not have the same number of rules.", "Number of audience rules");
						flag = false;
					}
				}
				else
				{
					differencesOutput.Write("The audience owners do not match.", "Audience owner");
					flag = false;
				}
			}
			else
			{
				differencesOutput.Write("The audience descriptions do not match.", "Audience description");
				flag = false;
			}
			return flag;
		}

		public struct AudienceRule
		{
			public string LeftContent;

			public string Operator;

			public string RightContent;
		}
	}
}