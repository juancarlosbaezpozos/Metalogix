using System;
using System.Text.RegularExpressions;

namespace Metalogix.UI.CommandLine
{
	internal class CommandLineParser
	{
		public CommandLineParser()
		{
		}

		public CommandLineParamsCollection Parse(string[] args)
		{
			CommandLineParamsCollection commandLineParamsCollection = new CommandLineParamsCollection();
			Regex regex = new Regex("^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			Regex regex1 = new Regex("^['\"]?(.*?)['\"]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			string str = null;
			string[] strArrays = args;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string[] strArrays1 = regex.Split(strArrays[i], 3);
				switch ((int)strArrays1.Length)
				{
					case 1:
					{
						if (str == null)
						{
							break;
						}
						if (!commandLineParamsCollection.Contains(str))
						{
							strArrays1[0] = regex1.Replace(strArrays1[0], "$1");
							commandLineParamsCollection.Add(str, strArrays1[0]);
						}
						str = null;
						break;
					}
					case 2:
					{
						if (str != null && !commandLineParamsCollection.Contains(str))
						{
							commandLineParamsCollection.Add(str);
						}
						str = strArrays1[1];
						break;
					}
					case 3:
					{
						if (str != null && !commandLineParamsCollection.Contains(str))
						{
							commandLineParamsCollection.Add(str);
						}
						str = strArrays1[1];
						if (!commandLineParamsCollection.Contains(str))
						{
							strArrays1[2] = regex1.Replace(strArrays1[2], "$1");
							commandLineParamsCollection.Add(str, strArrays1[2]);
						}
						str = null;
						break;
					}
				}
			}
			if (str != null && !commandLineParamsCollection.Contains(str))
			{
				commandLineParamsCollection.Add(str);
			}
			return commandLineParamsCollection;
		}
	}
}