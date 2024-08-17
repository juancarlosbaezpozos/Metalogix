using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class OptionsSummary
	{
		public int Level
		{
			get;
			private set;
		}

		public string Text
		{
			get;
			private set;
		}

		public OptionsSummary(string text, int level)
		{
			this.Text = text;
			this.Level = level;
		}
	}
}