using System;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.BCS
{
	public class SPExternalContentTypeOperationFilter : IEquatable<SPExternalContentTypeOperationFilter>
	{
		private string m_sName;

		private string m_sDisplayName;

		private string m_sFilterField;

		private string m_sOperator;

		private uint? m_iValue;

		private uint? m_iValueCount;

		public string DisplayName
		{
			get
			{
				return this.m_sDisplayName;
			}
		}

		public string FilterField
		{
			get
			{
				return this.m_sFilterField;
			}
		}

		public string Name
		{
			get
			{
				return this.m_sName;
			}
		}

		public string Operator
		{
			get
			{
				return this.m_sOperator;
			}
		}

		public uint? Value
		{
			get
			{
				return this.m_iValue;
			}
		}

		public uint? ValueCount
		{
			get
			{
				return this.m_iValueCount;
			}
		}

		private SPExternalContentTypeOperationFilter(string sName, string sDisplayName, string sFilterField, string sType, string sOperator, uint? iValue, uint? iValueCount)
		{
			this.m_sName = sName;
			this.m_sDisplayName = sDisplayName;
			this.m_sFilterField = sFilterField;
			this.m_sOperator = sOperator;
			this.m_iValue = iValue;
			this.m_iValueCount = iValueCount;
		}

		public bool Equals(SPExternalContentTypeOperationFilter other)
		{
			bool flag;
			bool flag1;
			if (other != null)
			{
				if (this.m_sName.Equals(other.m_sName) && this.m_sFilterField.Equals(other.m_sFilterField) && this.m_sOperator.Equals(other.m_sOperator))
				{
					uint? mIValue = this.m_iValue;
					uint? mIValueCount = other.m_iValue;
					if ((mIValue.GetValueOrDefault() != mIValueCount.GetValueOrDefault() ? true : mIValue.HasValue != mIValueCount.HasValue))
					{
						goto Label1;
					}
					mIValue = this.m_iValueCount;
					mIValueCount = other.m_iValueCount;
					flag1 = (mIValue.GetValueOrDefault() != mIValueCount.GetValueOrDefault() ? false : mIValue.HasValue == mIValueCount.HasValue);
					goto Label0;
				}
			Label1:
				flag1 = false;
			Label0:
				flag = flag1;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			flag = (obj is SPExternalContentTypeOperationFilter ? this.Equals((SPExternalContentTypeOperationFilter)obj) : false);
			return flag;
		}

		public override int GetHashCode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_sName);
			stringBuilder.Append(this.m_sFilterField);
			stringBuilder.Append(this.m_sOperator);
			stringBuilder.Append(this.m_iValue.ToString());
			stringBuilder.Append(this.m_iValueCount.ToString());
			return stringBuilder.ToString().GetHashCode();
		}

		internal static SPExternalContentTypeOperationFilter ParseFilterFromXml(XmlNode nodeFilter)
		{
			uint num = 0;
			if (nodeFilter == null)
			{
				throw new ArgumentNullException("nodeFilter");
			}
			XmlAttribute itemOf = nodeFilter.Attributes["Name"];
			if (itemOf == null)
			{
				throw new ArgumentException("Filter node must contain a Name attribute");
			}
			XmlAttribute xmlAttribute = nodeFilter.Attributes["DisplayName"];
			if (xmlAttribute == null)
			{
				throw new ArgumentException("Filter node must contain a DisplayName attribute");
			}
			XmlAttribute itemOf1 = nodeFilter.Attributes["FilterField"];
			if (itemOf1 == null)
			{
				throw new ArgumentException("Filter node must contain a FilterField attribute");
			}
			XmlAttribute xmlAttribute1 = nodeFilter.Attributes["Type"];
			if (xmlAttribute1 == null)
			{
				throw new ArgumentException("Filter node must contain a Type attribute");
			}
			XmlAttribute itemOf2 = nodeFilter.Attributes["Operator"];
			if (itemOf2 == null)
			{
				throw new ArgumentException("Filter node must contain a Operator attribute");
			}
			XmlAttribute xmlAttribute2 = nodeFilter.Attributes["Value"];
			if (xmlAttribute2 == null)
			{
				throw new ArgumentException("Filter node must contain a Value attribute");
			}
			XmlAttribute itemOf3 = nodeFilter.Attributes["ValueCount"];
			if (itemOf3 == null)
			{
				throw new ArgumentException("Filter node must contain a ValueCount attribute");
			}
			string value = itemOf.Value;
			string str = xmlAttribute.Value;
			string value1 = itemOf1.Value;
			string str1 = xmlAttribute1.Value;
			string value2 = itemOf2.Value;
			uint? nullable = null;
			if ((string.IsNullOrEmpty(xmlAttribute2.Value) ? false : uint.TryParse(xmlAttribute2.Value, out num)))
			{
				nullable = new uint?(num);
			}
			uint? nullable1 = null;
			if ((string.IsNullOrEmpty(itemOf3.Value) ? false : uint.TryParse(itemOf3.Value, out num)))
			{
				nullable1 = new uint?(num);
			}
			SPExternalContentTypeOperationFilter sPExternalContentTypeOperationFilter = new SPExternalContentTypeOperationFilter(value, str, value1, str1, value2, nullable, nullable1);
			return sPExternalContentTypeOperationFilter;
		}
	}
}