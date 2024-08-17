using Metalogix.Explorer;
using System;

namespace Metalogix.SharePoint
{
	public class ResultsField : TypedField, Field
	{
		private string m_sName = null;

		private string m_sDisplayName = null;

		private Type m_UnderlyingType = null;

		public string DisplayName
		{
			get
			{
				return this.m_sDisplayName;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public Type UnderlyingType
		{
			get
			{
				return this.m_UnderlyingType;
			}
		}

		public ResultsField(string sName, Type underlyingColumnType)
		{
			this.m_sName = sName;
			this.m_sDisplayName = sName;
			this.m_UnderlyingType = underlyingColumnType;
		}

		public ResultsField(string sName, string sDisplayName, Type underlyingColumnType)
		{
			this.m_sName = sName;
			this.m_sDisplayName = sDisplayName;
			this.m_UnderlyingType = underlyingColumnType;
		}
	}
}