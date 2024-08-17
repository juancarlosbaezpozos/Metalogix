using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalogix.SharePoint.Options.Transform
{
	public class TransformUtils
	{
		public const string REGEX_TAXONOMY_ANCHOR_DUPLICATE = "\\x3E+";

		public const byte MAX_TERM_LENGTH = 255;

		public readonly static char[] INVALID_TERM_CHARACTERS;

		public readonly static char[] INVALID_ANCHOR_CHARACTERS;

		public readonly static char[] ANCHOR_MARKER_CHAR;

		static TransformUtils()
		{
			TransformUtils.INVALID_TERM_CHARACTERS = new char[] { '\"', ';', '<', '>', '|', '\t' };
			TransformUtils.INVALID_ANCHOR_CHARACTERS = new char[] { '\"', ';', '<', '|', '\t' };
			TransformUtils.ANCHOR_MARKER_CHAR = new char[] { '>' };
		}

		public TransformUtils()
		{
		}

		public static string SanitiseForTaxonomy(string sourceValue)
		{
			if (string.IsNullOrEmpty(sourceValue))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = (new StringBuilder(sourceValue.Trim())).Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
			Array.ForEach<char>(TransformUtils.INVALID_TERM_CHARACTERS, (char e) => stringBuilder.Replace(e, '\u005F'));
			stringBuilder.Replace('&', '＆');
			if (stringBuilder.Length > 255)
			{
				stringBuilder.Length = 255;
			}
			return stringBuilder.ToString();
		}

		public static string SanitiseTaxonomyAnchorConfiguration(string anchorValue)
		{
			if (string.IsNullOrEmpty(anchorValue))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(Regex.Replace(anchorValue.Trim().Trim(TransformUtils.ANCHOR_MARKER_CHAR), "\\x3E+", ">"));
			Array.ForEach<char>(TransformUtils.INVALID_ANCHOR_CHARACTERS, (char e) => stringBuilder.Replace(e, '\u005F'));
			stringBuilder.Replace('&', '＆');
			if (stringBuilder.Length > 255)
			{
				stringBuilder.Length = 255;
			}
			return stringBuilder.ToString();
		}
	}
}