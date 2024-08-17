using Metalogix.SharePoint.Options.Properties;
using System;
using System.ComponentModel;
using System.Resources;

namespace Metalogix.SharePoint.Options.Transform
{
	public class LocalizedDisplayNameAttribute : DisplayNameAttribute
	{
		public LocalizedDisplayNameAttribute(string resourceId) : base(LocalizedDisplayNameAttribute.GetMessageFromResource(resourceId))
		{
		}

		private static string GetMessageFromResource(string resourceId)
		{
			return Resources.ResourceManager.GetString(resourceId);
		}
	}
}