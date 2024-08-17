using System;

namespace Metalogix.SharePoint
{
	public class SPPath
	{
		private string m_sPathValue = null;

		private bool m_bIsWildcard = false;

		public bool IsWildcard
		{
			get
			{
				return this.m_bIsWildcard;
			}
		}

		public string PathValue
		{
			get
			{
				return this.m_sPathValue;
			}
		}

		public SPPath(string sPathValue, bool bIsWildcard)
		{
			this.m_sPathValue = sPathValue;
			this.m_bIsWildcard = bIsWildcard;
		}

		public override string ToString()
		{
			string pathValue = this.PathValue;
			if (!pathValue.StartsWith("/"))
			{
				pathValue = string.Concat("/", pathValue);
			}
			if ((!this.IsWildcard ? false : !pathValue.EndsWith("/")))
			{
				pathValue = string.Concat(pathValue, "/");
			}
			return pathValue;
		}
	}
}