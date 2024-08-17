using Metalogix.SharePoint.Adapters;
using System;
using System.Xml;

namespace Metalogix.SharePoint.StoragePoint
{
	public class Profile
	{
		private XmlNode m_xmlNodeData;

		private Guid m_gProfileId;

		private bool m_bIsActive;

		private string m_sName;

		private ProfileEndpoint[] m_endpointList;

		public ProfileEndpoint[] EndpointList
		{
			get
			{
				return this.m_endpointList;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.m_bIsActive;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public Guid ProfileId
		{
			get
			{
				return this.m_gProfileId;
			}
		}

		private Profile(XmlNode nodeProfile)
		{
			this.FromXml(nodeProfile);
		}

		public static Profile CreateProfileFromXml(XmlNode nodeProfile)
		{
			return new Profile(nodeProfile);
		}

		private void FromXml(XmlNode nodeProfile)
		{
			this.m_xmlNodeData = nodeProfile;
			XmlAttribute itemOf = nodeProfile.Attributes[StoragePointField.ProfileId.ToString()];
			if ((itemOf == null ? false : !string.IsNullOrEmpty(itemOf.Value)))
			{
				this.m_gProfileId = new Guid(itemOf.Value);
			}
			XmlAttribute xmlAttribute = nodeProfile.Attributes[StoragePointField.IsActive.ToString()];
			if (xmlAttribute != null)
			{
				bool.TryParse(xmlAttribute.Value, out this.m_bIsActive);
			}
			XmlAttribute itemOf1 = nodeProfile.Attributes[StoragePointField.Name.ToString()];
			if (itemOf1 != null)
			{
				this.m_sName = itemOf1.Value;
			}
			XmlNodeList xmlNodeLists = nodeProfile.SelectNodes(string.Concat("./", StoragePointClassType.ProfileEndpointList.ToString(), "/", StoragePointClassType.ProfileEndpoint.ToString()));
			this.m_endpointList = new ProfileEndpoint[xmlNodeLists.Count];
			for (int i = 0; i < xmlNodeLists.Count; i++)
			{
				XmlNode xmlNodes = xmlNodeLists[i];
				this.m_endpointList[i] = ProfileEndpoint.CreateProfileEnpointFromXml(xmlNodes);
			}
		}
	}
}