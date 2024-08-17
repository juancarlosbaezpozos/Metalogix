using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPLanguage
	{
		private SPNode m_nodeParent = null;

		private string m_sName = null;

		private int m_iLCID = -1;

		private int m_iDefaultExperienceVersion = -1;

		private Dictionary<int, SPWebTemplate[]> m_experienceVersionToTemplateMap = null;

		private readonly object m_oExperienceVersionLock = new object();

		public ReadOnlyCollection<int> ExperienceVersions
		{
			get
			{
				ReadOnlyCollection<int> nums;
				if (this.m_experienceVersionToTemplateMap != null)
				{
					int[] numArray = new int[this.m_experienceVersionToTemplateMap.Count];
					this.m_experienceVersionToTemplateMap.Keys.CopyTo(numArray, 0);
					nums = new ReadOnlyCollection<int>(numArray);
				}
				else
				{
					nums = new ReadOnlyCollection<int>(new int[0]);
				}
				return nums;
			}
		}

		public bool HasMultipleExperienceVersions
		{
			get
			{
				return (this.m_experienceVersionToTemplateMap == null ? false : this.m_experienceVersionToTemplateMap.Count > 1);
			}
		}

		public int LCID
		{
			get
			{
				return this.m_iLCID;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public SPNode Parent
		{
			get
			{
				return this.m_nodeParent;
			}
		}

		public SPWebTemplate[] TemplateArray
		{
			get
			{
				SPWebTemplate[] item;
				lock (this.m_oExperienceVersionLock)
				{
					if (this.m_experienceVersionToTemplateMap != null)
					{
						item = this.m_experienceVersionToTemplateMap[this.m_iDefaultExperienceVersion];
					}
					else
					{
						item = null;
					}
				}
				return item;
			}
		}

		public SPWebTemplateCollection Templates
		{
			get
			{
				return new SPWebTemplateCollection(this.TemplateArray);
			}
		}

		public SPLanguage(string name, int languageID, SPNode parent)
		{
			this.m_sName = name;
			this.m_iLCID = languageID;
			this.m_nodeParent = parent;
		}

		public SPLanguage(XmlNode node, SPNode parent)
		{
			this.m_nodeParent = parent;
			this.FromXml(node);
		}

		private SPWebTemplate[] BuildWebTemplateArray(XmlNode collectionNode)
		{
			XmlNodeList xmlNodeLists = collectionNode.SelectNodes("./WebTemplate");
			SPWebTemplate[] sPWebTemplate = new SPWebTemplate[xmlNodeLists.Count];
			int num = 0;
			foreach (XmlNode xmlNodes in xmlNodeLists)
			{
				sPWebTemplate[num] = new SPWebTemplate(xmlNodes);
				num++;
			}
			return sPWebTemplate;
		}

		public void FromXml(XmlNode node)
		{
			this.m_sName = node.Attributes["Name"].Value;
			this.m_iLCID = int.Parse(node.Attributes["LCID"].Value);
			lock (this.m_oExperienceVersionLock)
			{
				XmlNodeList xmlNodeLists = node.SelectNodes("./ExperienceVersion");
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count != 0))
				{
					this.m_experienceVersionToTemplateMap = new Dictionary<int, SPWebTemplate[]>(xmlNodeLists.Count);
					this.m_iDefaultExperienceVersion = -1;
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						int num = int.Parse(xmlNodes.Attributes["Version"].Value);
						if (num > this.m_iDefaultExperienceVersion)
						{
							this.m_iDefaultExperienceVersion = num;
						}
						this.m_experienceVersionToTemplateMap.Add(num, this.BuildWebTemplateArray(xmlNodes));
					}
				}
				else
				{
					this.m_iDefaultExperienceVersion = 0;
					this.m_experienceVersionToTemplateMap = new Dictionary<int, SPWebTemplate[]>(1)
					{
						{ this.m_iDefaultExperienceVersion, this.BuildWebTemplateArray(node) }
					};
				}
			}
		}

		public SPWebTemplateCollection GetTemplatesForExperienceVersion(int iExperienceVersion)
		{
			SPWebTemplate[] sPWebTemplateArray;
			SPWebTemplateCollection sPWebTemplateCollections;
			sPWebTemplateCollections = (!this.m_experienceVersionToTemplateMap.TryGetValue(iExperienceVersion, out sPWebTemplateArray) ? new SPWebTemplateCollection() : new SPWebTemplateCollection(sPWebTemplateArray));
			return sPWebTemplateCollections;
		}

		public override string ToString()
		{
			string name = this.Name;
			int lCID = this.LCID;
			string str = string.Concat(name, "(", lCID.ToString(), ")");
			return str;
		}
	}
}