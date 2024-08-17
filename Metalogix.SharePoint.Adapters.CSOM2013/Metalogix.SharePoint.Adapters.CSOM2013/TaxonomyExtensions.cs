using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.CSOM2013
{
	public static class TaxonomyExtensions
	{
		public static string Serialize(this TaxonomyFieldValue value)
		{
			if (value == null || string.IsNullOrEmpty(value.Label))
			{
				return "";
			}
			return string.Concat(value.Label, '|', value.TermGuid);
		}

		public static string Serialize(this TaxonomyFieldValueCollection coll)
		{
			string str = "";
			if (coll != null && coll.Count > 0)
			{
				foreach (TaxonomyFieldValue taxonomyFieldValue in coll)
				{
					str = string.Concat(str, taxonomyFieldValue.Serialize(), ';');
				}
				str = str.TrimEnd(new char[] { ';' });
			}
			return str;
		}
	}
}