using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Deployment
{
    [CompilerGenerated]
    [DebuggerNonUserCode]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    internal class Resources
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
		{
			return resourceCulture;
		}
            set
		{
			resourceCulture = value;
		}
        }

        internal static Bitmap Download => (Bitmap)ResourceManager.GetObject("Download", resourceCulture);

        internal static string ForDetailsOnLatest => ResourceManager.GetString("ForDetailsOnLatest", resourceCulture);

        internal static string LabelLatestInstalled => ResourceManager.GetString("LabelLatestInstalled", resourceCulture);

        internal static string LabelNewVersionDownload => ResourceManager.GetString("LabelNewVersionDownload", resourceCulture);

        internal static string LabelNewVersionDownloadNot => ResourceManager.GetString("LabelNewVersionDownloadNot", resourceCulture);

        internal static string MetalogixSalesURL => ResourceManager.GetString("MetalogixSalesURL", resourceCulture);

        internal static string MoreRecentVersion => ResourceManager.GetString("MoreRecentVersion", resourceCulture);

        internal static string Released => ResourceManager.GetString("Released", resourceCulture);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Metalogix.UI.WinForms.Deployment.Resources", typeof(Resources).Assembly);
			}
			return resourceMan;
		}
        }

        internal static string Version => ResourceManager.GetString("Version", resourceCulture);

        internal static string VersionSmall => ResourceManager.GetString("VersionSmall", resourceCulture);

        internal static string WasReleased => ResourceManager.GetString("WasReleased", resourceCulture);

        internal Resources()
	{
	}
    }
}
