using System;

namespace Metalogix.SharePoint
{
	public class FilterString
	{
		private string _fieldvalue = string.Empty;

		public string FieldValue
		{
			get
			{
				return this._fieldvalue;
			}
			set
			{
				this._fieldvalue = value ?? string.Empty;
			}
		}

		public FilterString(string value)
		{
			this._fieldvalue = value ?? string.Empty;
		}

		public override bool Equals(object obj)
		{
			return this._fieldvalue.Equals(obj);
		}

		public override int GetHashCode()
		{
			return this._fieldvalue.GetHashCode();
		}

		public override string ToString()
		{
			return this._fieldvalue.ToString();
		}
	}
}