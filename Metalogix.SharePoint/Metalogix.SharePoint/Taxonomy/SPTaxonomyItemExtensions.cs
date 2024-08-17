using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Taxonomy
{
	public static class SPTaxonomyItemExtensions
	{
		public static string ToNamePipeGuidString(this SPTaxonomyItem item)
		{
			string name = item.Name;
			Guid id = item.Id;
			string str = string.Format("{0}|{1}", name, id.ToString("D"));
			return str;
		}
	}
}