using System;
using System.Reflection;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkCorrectorTypeWrapper
	{
		private Type m_type;

		public Type InnerType
		{
			get
			{
				return this.m_type;
			}
		}

		public LinkCorrectorTypeWrapper(Type type)
		{
			this.m_type = type;
		}

		public override string ToString()
		{
			string str;
			object[] customAttributes = this.m_type.GetCustomAttributes(typeof(LinkCorrectorDisplayNameAttribute), true);
			str = ((int)customAttributes.Length <= 0 ? "Unnamed Link Corrector" : ((LinkCorrectorDisplayNameAttribute)customAttributes[0]).DisplayName);
			return str;
		}
	}
}