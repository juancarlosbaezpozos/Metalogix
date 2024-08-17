using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Attributes
{
	public class SplashScreenAttribute : UIAttribute
	{
		public string ExpressResourceName
		{
			get;
			private set;
		}

		public string ResourceName
		{
			get;
			private set;
		}

		public SplashScreenAttribute(string imageResourceName, string expressImageResourceName)
		{
			this.ResourceName = imageResourceName;
			this.ExpressResourceName = expressImageResourceName;
		}

		public SplashScreenAttribute(string imageResourceName)
		{
			this.ResourceName = imageResourceName;
			this.ExpressResourceName = null;
		}

		public Image GetExpressImage()
		{
			Image image;
			string expressResourceName = this.ExpressResourceName ?? this.ResourceName;
			using (Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(expressResourceName))
			{
				image = Image.FromStream(manifestResourceStream);
			}
			return image;
		}

		public Image GetImage()
		{
			Image image;
			using (Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(this.ResourceName))
			{
				image = Image.FromStream(manifestResourceStream);
			}
			return image;
		}
	}
}