using Metalogix.SharePoint.Adapters;
using System;
using System.Xml;

namespace Metalogix.SharePoint.StoragePoint
{
	public class ProfileEndpoint : Endpoint
	{
		private XmlNode m_xmlNodeData;

		private bool m_bIsActive;

		public bool IsActive
		{
			get
			{
				return this.m_bIsActive;
			}
			set
			{
				this.m_bIsActive = value;
			}
		}

		private ProfileEndpoint(XmlNode nodeEndpoint) : base(nodeEndpoint)
		{
			this.FromXml(nodeEndpoint);
		}

		public static ProfileEndpoint CreateProfileEnpointFromXml(XmlNode nodeEndpoint)
		{
			return new ProfileEndpoint(nodeEndpoint);
		}

		private void FromXml(XmlNode nodeEndpoint)
		{
			this.m_xmlNodeData = nodeEndpoint;
			XmlAttribute itemOf = nodeEndpoint.Attributes[StoragePointField.IsActive.ToString()];
			if (itemOf != null)
			{
				bool.TryParse(itemOf.Value, out this.m_bIsActive);
			}
		}
	}
}