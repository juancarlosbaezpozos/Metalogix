using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Metalogix.UI.WinForms.Attributes
{
	public class ApplicationIconAttribute : UIAttribute
	{
		private readonly string _applicationIcon;

		public string ApplicationIcon
		{
			get
			{
				return this._applicationIcon;
			}
		}

		public ApplicationIconAttribute(string applicationIcon)
		{
			this._applicationIcon = applicationIcon;
		}

		public Image GetApplicationIcon()
		{
			Image image;
			using (Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(this.ApplicationIcon))
			{
				if (manifestResourceStream == null)
				{
					return null;
				}
				else
				{
					image = Image.FromStream(manifestResourceStream);
				}
			}
			return image;
		}
	}
}