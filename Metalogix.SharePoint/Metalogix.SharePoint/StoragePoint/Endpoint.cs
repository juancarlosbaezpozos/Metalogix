using Metalogix.SharePoint.Adapters;
using System;
using System.Xml;

namespace Metalogix.SharePoint.StoragePoint
{
	public class Endpoint
	{
		private XmlNode m_xmlNodeData;

		private string m_sAdapterName;

		private string m_sConnection;

		private Guid m_gEndpointId;

		private string m_sName;

		public string AdapterName
		{
			get
			{
				return this.m_sAdapterName;
			}
			set
			{
				this.m_sAdapterName = value;
			}
		}

		public string Connection
		{
			get
			{
				return this.m_sConnection;
			}
			set
			{
				this.m_sConnection = value;
			}
		}

		public Guid EndpointId
		{
			get
			{
				return this.m_gEndpointId;
			}
			set
			{
				this.m_gEndpointId = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
			set
			{
				this.m_sName = value;
			}
		}

		protected Endpoint(XmlNode nodeEndpoint)
		{
			this.FromXml(nodeEndpoint);
		}

		public static Endpoint CreateEndpointFromXml(XmlNode nodeEndpoint)
		{
			return new Endpoint(nodeEndpoint);
		}

		private void FromXml(XmlNode nodeEndpoint)
		{
			this.m_xmlNodeData = nodeEndpoint;
			XmlAttribute itemOf = nodeEndpoint.Attributes[StoragePointField.AdapterName.ToString()];
			if (itemOf != null)
			{
				this.m_sAdapterName = itemOf.Value;
			}
			XmlAttribute xmlAttribute = nodeEndpoint.Attributes[StoragePointField.Connection.ToString()];
			if (xmlAttribute != null)
			{
				this.m_sConnection = xmlAttribute.Value;
			}
			XmlAttribute itemOf1 = nodeEndpoint.Attributes[StoragePointField.EndpointId.ToString()];
			if ((itemOf1 == null ? false : !string.IsNullOrEmpty(itemOf1.Value)))
			{
				this.m_gEndpointId = new Guid(itemOf1.Value);
			}
			XmlAttribute xmlAttribute1 = nodeEndpoint.Attributes[StoragePointField.Name.ToString()];
			if (xmlAttribute1 != null)
			{
				this.m_sName = xmlAttribute1.Value;
			}
		}
	}
}