using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Actions.Administration.CheckLinks;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.BCS;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

namespace Metalogix.SharePoint.Actions.BCS
{
	public static class BCSHelper
	{
		private readonly static string[] s_crgstrUrlHexValue;

		static BCSHelper()
		{
			string[] strArrays = new string[] { "%00", "%01", "%02", "%03", "%04", "%05", "%06", "%07", "%08", "%09", "%0A", "%0B", "%0C", "%0D", "%0E", "%0F", "%10", "%11", "%12", "%13", "%14", "%15", "%16", "%17", "%18", "%19", "%1A", "%1B", "%1C", "%1D", "%1E", "%1F", "%20", "%21", "%22", "%23", "%24", "%25", "%26", "%27", "%28", "%29", "%2A", "%2B", "%2C", "%2D", "%2E", "%2F", "%30", "%31", "%32", "%33", "%34", "%35", "%36", "%37", "%38", "%39", "%3A", "%3B", "%3C", "%3D", "%3E", "%3F", "%40", "%41", "%42", "%43", "%44", "%45", "%46", "%47", "%48", "%49", "%4A", "%4B", "%4C", "%4D", "%4E", "%4F", "%50", "%51", "%52", "%53", "%54", "%55", "%56", "%57", "%58", "%59", "%5A", "%5B", "%5C", "%5D", "%5E", "%5F", "%60", "%61", "%62", "%63", "%64", "%65", "%66", "%67", "%68", "%69", "%6A", "%6B", "%6C", "%6D", "%6E", "%6F", "%70", "%71", "%72", "%73", "%74", "%75", "%76", "%77", "%78", "%79", "%7A", "%7B", "%7C", "%7D", "%7E", "%7F", "%80", "%81", "%82", "%83", "%84", "%85", "%86", "%87", "%88", "%89", "%8A", "%8B", "%8C", "%8D", "%8E", "%8F", "%90", "%91", "%92", "%93", "%94", "%95", "%96", "%97", "%98", "%99", "%9A", "%9B", "%9C", "%9D", "%9E", "%9F", "%A0", "%A1", "%A2", "%A3", "%A4", "%A5", "%A6", "%A7", "%A8", "%A9", "%AA", "%AB", "%AC", "%AD", "%AE", "%AF", "%B0", "%B1", "%B2", "%B3", "%B4", "%B5", "%B6", "%B7", "%B8", "%B9", "%BA", "%BB", "%BC", "%BD", "%BE", "%BF", "%C0", "%C1", "%C2", "%C3", "%C4", "%C5", "%C6", "%C7", "%C8", "%C9", "%CA", "%CB", "%CC", "%CD", "%CE", "%CF", "%D0", "%D1", "%D2", "%D3", "%D4", "%D5", "%D6", "%D7", "%D8", "%D9", "%DA", "%DB", "%DC", "%DD", "%DE", "%DF", "%E0", "%E1", "%E2", "%E3", "%E4", "%E5", "%E6", "%E7", "%E8", "%E9", "%EA", "%EB", "%EC", "%ED", "%EE", "%EF", "%F0", "%F1", "%F2", "%F3", "%F4", "%F5", "%F6", "%F7", "%F8", "%F9", "%FA", "%FB", "%FC", "%FD", "%FE", "%FF" };
			BCSHelper.s_crgstrUrlHexValue = strArrays;
		}

		private static string BCSCombineStrings(string[] strings)
		{
			object str;
			StringBuilder stringBuilder = new StringBuilder();
			int length = 0;
			for (int i = 0; i < (int)strings.Length; i++)
			{
				string str1 = strings[i];
				if (str1 != null)
				{
					int num = str1.Length + 1;
					str = num.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					str = string.Empty;
				}
				string str2 = string.Concat(str, ' ');
				stringBuilder.Insert(length, str2);
				length += str2.Length;
				stringBuilder.Append(string.Concat(str1, ' '));
			}
			stringBuilder.Append(length.ToString(CultureInfo.InvariantCulture));
			return BCSHelper.UrlKeyValueEncode(stringBuilder.ToString());
		}

		private static int FromHexNoCheck(char digit)
		{
			if (digit <= '9')
			{
				return digit - 48;
			}
			if (digit <= 'F')
			{
				return digit - 65 + 10;
			}
			return digit - 97 + 10;
		}

		public static bool HasExternalLists(IXMLAbleList nodes)
		{
			bool flag;
			if (nodes == null)
			{
				return false;
			}
			IEnumerator enumerator = nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (!(current is SPList) || ((SPList)current).BaseTemplate != ListTemplateType.ExternalList)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public static bool HasExternalListsOnly(IXMLAbleList nodes)
		{
			bool flag;
			if (nodes == null)
			{
				return false;
			}
			IEnumerator enumerator = nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (!(current is SPList))
					{
						flag = false;
						return flag;
					}
					else
					{
						if (((SPList)current).BaseTemplate == ListTemplateType.ExternalList)
						{
							continue;
						}
						flag = false;
						return flag;
					}
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		private static bool IsHexDigit(char digit)
		{
			if (48 <= digit && digit <= '9' || 97 <= digit && digit <= 'f')
			{
				return true;
			}
			if (65 > digit)
			{
				return false;
			}
			return digit <= 'F';
		}

		private static bool IsHexEscapedChar(string str, int nIndex, int nPathLength)
		{
			if (nIndex + 2 >= nPathLength || str[nIndex] != '%' || !BCSHelper.IsHexDigit(str[nIndex + 1]) || !BCSHelper.IsHexDigit(str[nIndex + 2]))
			{
				return false;
			}
			if (str[nIndex + 1] != '0')
			{
				return true;
			}
			return str[nIndex + 2] != '0';
		}

		internal static bool ModifyBDCOrBCSFieldsXmlAttributes(SPList targetList)
		{
			bool flag = false;
			string d = targetList.ID;
			if (!d.StartsWith("{"))
			{
				d = string.Concat("{", d.ToLower(), "}");
			}
			XmlNode listXML = targetList.GetListXML(false);
			foreach (XmlNode xmlNodes in listXML.SelectNodes("//Field[@BdcField]"))
			{
				if (xmlNodes.Attributes["SourceID"] == null)
				{
					continue;
				}
				flag = true;
				xmlNodes.Attributes["SourceID"].Value = d;
			}
			if (flag)
			{
				listXML.SelectSingleNode("./Views");
				listXML.Attributes.RemoveAll();
				targetList.UpdateList(listXML.OuterXml, false, false);
			}
			return flag;
		}

		public static string ModifyFieldsXmlFromBDCToBCS(ref string sTargetXml, SPList sourceList, SPWeb targetWeb)
		{
			string str;
			string message = "";
			if (!sourceList.Adapter.SharePointVersion.IsSharePoint2007 || !targetWeb.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				return message;
			}
			XmlNode xmlNode = XmlUtility.StringToXmlNode(sTargetXml);
			IEnumerator enumerator = xmlNode.SelectNodes("//Field[@Entity]").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode current = (XmlNode)enumerator.Current;
					if (current == null || current.Attributes["Entity"] == null)
					{
						continue;
					}
					XmlAttribute value = current.OwnerDocument.CreateAttribute("EntityName");
					value.Value = current.Attributes["SystemInstance"].Value;
					current.Attributes.Append(value);
					string externalDataSource = "";
					SPExternalContentType sPExternalContentType = null;
					try
					{
						if (targetWeb.ExternalContentTypes.Count != 0)
						{
							List<SPExternalContentType>.Enumerator enumerator1 = targetWeb.ExternalContentTypes.GetEnumerator();
							try
							{
								while (enumerator1.MoveNext())
								{
									SPExternalContentType current1 = enumerator1.Current;
									if (current1.Name != current.Attributes["SystemInstance"].Value)
									{
										continue;
									}
									if (sPExternalContentType == null)
									{
										sPExternalContentType = current1;
										externalDataSource = sPExternalContentType.ExternalDataSource;
									}
									else
									{
										message = string.Format("There are more than one external content types with the name {0} on target {1}. Please ensure that you have correctly deployed your LOB system definitions.", current.Attributes["SystemInstance"].Value, targetWeb.Url.ToLower());
										str = message;
										return str;
									}
								}
							}
							finally
							{
								((IDisposable)enumerator1).Dispose();
							}
						}
						else
						{
							message = "Can't retrieve external content types";
							str = message;
							return str;
						}
					}
					catch (Exception exception)
					{
						message = exception.Message;
						str = message;
						return str;
					}
					if (sPExternalContentType == null || string.IsNullOrEmpty(externalDataSource))
					{
						message = string.Format("Can't find external content type {0} in namespace {1}. Please ensure that you have correctly deployed your LOB system definitions and they are using the same names. ", current.Attributes["SystemInstance"].Value, targetWeb.Url.ToLower());
						str = message;
						return str;
					}
					else
					{
						current.Attributes["SystemInstance"].Value = externalDataSource;
						value = current.OwnerDocument.CreateAttribute("EntityNamespace");
						value.Value = sPExternalContentType.Namespace.ToLower();
						current.Attributes.Append(value);
						XmlAttribute itemOf = current.Attributes["Profile"];
						string[] strArrays = new string[] { "/_layouts/ActionRedirect.aspx?EntityNamespace=", current.Attributes["EntityNamespace"].Value, "&EntityName=", current.Attributes["EntityName"].Value, "&LOBSystemInstanceName=", current.Attributes["SystemInstance"].Value, "&ItemID=" };
						itemOf.Value = string.Concat(strArrays);
						current.Attributes.Remove(current.Attributes["Entity"]);
						if (current.Attributes["RelatedField"] == null)
						{
							continue;
						}
						string value1 = current.Attributes["RelatedField"].Value;
						if (!string.IsNullOrEmpty(value1))
						{
							XmlNode xmlNodes = xmlNode.SelectSingleNode(string.Concat("//Field[@StaticName='", value1, "']"));
							if (xmlNodes != null && xmlNodes.Attributes["BdcField"] != null)
							{
								string str1 = string.Concat(current.Attributes["EntityName"].Value, "_", current.Attributes["BdcField"].Value.ToUpper());
								xmlNodes.Attributes["BdcField"].Value = str1;
								xmlNodes.Attributes["StaticName"].Value = str1;
								xmlNodes.Attributes["Name"].Value = str1;
								xmlNodes.Attributes["DisplayName"].Value = str1;
								current.Attributes["RelatedField"].Value = str1;
								current.Attributes["RelatedFieldWssStaticName"].Value = str1;
							}
						}
						string str2 = BCSHelper.UrlKeyValueDecode(current.Attributes["SecondaryFieldBdcNames"].Value);
						string[] strArrays1 = str2.Split(new char[] { ':' });
						string[] strArrays2 = current.Attributes["SecondaryFieldWssNames"].Value.Split(new char[] { ':' });
						string str3 = BCSHelper.BCSCombineStrings(strArrays1);
						string str4 = BCSHelper.BCSCombineStrings(strArrays2);
						current.Attributes["SecondaryFieldBdcNames"].Value = str3;
						current.Attributes["SecondaryFieldWssNames"].Value = str4;
						current.Attributes["SecondaryFieldsWssStaticNames"].Value = str4;
					}
				}
				sTargetXml = xmlNode.OuterXml;
				message = "";
				return message;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return str;
		}

		public static bool SharePointActionEnabledOn(Metalogix.Actions.Action action, IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			Type type = action.GetType();
			if ((typeof(CopyAlertsAction).IsAssignableFrom(type) || typeof(PasteListEmailNotificationAction).IsAssignableFrom(type) || typeof(PasteListItemAction).IsAssignableFrom(type) || typeof(PasteListViewsAction).IsAssignableFrom(type) || typeof(CompareListAction).IsAssignableFrom(type) || typeof(SearchAction).IsAssignableFrom(type) || typeof(AnalyzeChurnAction).IsAssignableFrom(type) || typeof(CheckLinksAction).IsAssignableFrom(type) || typeof(CreateFolderAction).IsAssignableFrom(type) || typeof(CompareListAction).IsAssignableFrom(type) ? false : !typeof(SearchAction).IsAssignableFrom(type)) || typeof(PasteListAction).IsAssignableFrom(type))
			{
				return true;
			}
			if ((typeof(AnalyzeChurnAction).IsAssignableFrom(type) || typeof(CheckLinksAction).IsAssignableFrom(type) || typeof(CreateFolderAction).IsAssignableFrom(type) || typeof(CompareListAction).IsAssignableFrom(type) ? false : !typeof(SearchAction).IsAssignableFrom(type)) && BCSHelper.HasExternalLists(sourceSelections))
			{
				return false;
			}
			return !BCSHelper.HasExternalLists(targetSelections);
		}

		public static string[] SplitStrings(string combinedEncoded)
		{
			string[] strArrays = null;
			ArrayList arrayLists = new ArrayList();
			if ("0" != combinedEncoded)
			{
				try
				{
					string str = BCSHelper.UrlKeyValueDecode(combinedEncoded);
					char[] chrArray = new char[] { ' ' };
					string[] strArrays1 = str.Split(chrArray, StringSplitOptions.None);
					int num = 0;
					if (strArrays1 == null || !int.TryParse(strArrays1[(int)strArrays1.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
					{
						throw new ArgumentException(string.Empty, "combinedEncoded");
					}
					int num1 = str.LastIndexOf(' ');
					string str1 = str.Substring(num, num1 - num);
					int length = str1.Length;
					int num2 = 0;
					int num3 = 0;
					while (num3 < length)
					{
						string str2 = strArrays1[num2];
						int num4 = 1;
						if (str2 == null || str2.Length != 0)
						{
							if (!int.TryParse(str2, NumberStyles.Integer, CultureInfo.InvariantCulture, out num4))
							{
								throw new ArgumentException(string.Empty, "combinedEncoded");
							}
							arrayLists.Add(str1.Substring(num3, num4 - 1));
						}
						else
						{
							arrayLists.Add(null);
						}
						num3 += num4;
						num2++;
					}
					strArrays = new string[arrayLists.Count];
					arrayLists.CopyTo(strArrays);
				}
				catch (Exception exception)
				{
					throw new ArgumentException(string.Empty, "combinedEncoded", exception);
				}
			}
			else
			{
				strArrays = new string[0];
			}
			return strArrays;
		}

		private static string UrlDecodeHelper(string stringToDecode, int length, bool decodePlus)
		{
			if (stringToDecode == null || stringToDecode.Length == 0)
			{
				return stringToDecode;
			}
			StringBuilder stringBuilder = new StringBuilder(length);
			byte[] numArray = null;
			int num = 0;
			while (num < length)
			{
				char chr = stringToDecode[num];
				if (chr < ' ')
				{
					num++;
				}
				else if (decodePlus && chr == '+')
				{
					stringBuilder.Append(" ");
					num++;
				}
				else if (!BCSHelper.IsHexEscapedChar(stringToDecode, num, length))
				{
					stringBuilder.Append(chr);
					num++;
				}
				else
				{
					if (numArray == null)
					{
						numArray = new byte[(length - num) / 3];
					}
					int num1 = 0;
					do
					{
						int num2 = BCSHelper.FromHexNoCheck(stringToDecode[num + 1]) * 16 + BCSHelper.FromHexNoCheck(stringToDecode[num + 2]);
						int num3 = num1;
						num1 = num3 + 1;
						numArray[num3] = (byte)num2;
						num += 3;
					}
					while (BCSHelper.IsHexEscapedChar(stringToDecode, num, length));
					stringBuilder.Append(Encoding.UTF8.GetChars(numArray, 0, num1));
				}
			}
			if (length < stringToDecode.Length)
			{
				stringBuilder.Append(stringToDecode.Substring(length));
			}
			return stringBuilder.ToString();
		}

		private static void UrlEncodeUnicodeChar(TextWriter output, char ch, char chNext, out bool fUsedNextChar)
		{
			bool flag = false;
			BCSHelper.UrlEncodeUnicodeChar(output, ch, chNext, ref flag, out fUsedNextChar);
		}

		private static void UrlEncodeUnicodeChar(TextWriter output, char ch, char chNext, ref bool fInvalidUnicode, out bool fUsedNextChar)
		{
			int num = 192;
			int num1 = 224;
			int num2 = 240;
			int num3 = 128;
			int num4 = 55296;
			int num5 = 64512;
			int num6 = 65536;
			fUsedNextChar = false;
			if (ch <= '\u007F')
			{
				output.Write(BCSHelper.s_crgstrUrlHexValue[ch]);
				return;
			}
			if (ch <= '\u07FF')
			{
				int num7 = num | ch >> '\u0006';
				output.Write(BCSHelper.s_crgstrUrlHexValue[num7]);
				num7 = num3 | ch & '?';
				output.Write(BCSHelper.s_crgstrUrlHexValue[num7]);
				return;
			}
			if ((ch & (char)num5) != num4)
			{
				int num8 = num1 | ch >> '\f';
				output.Write(BCSHelper.s_crgstrUrlHexValue[num8]);
				num8 = num3 | (ch & '\u0FC0') >> 6;
				output.Write(BCSHelper.s_crgstrUrlHexValue[num8]);
				num8 = num3 | ch & '?';
				output.Write(BCSHelper.s_crgstrUrlHexValue[num8]);
				return;
			}
			if (chNext == 0)
			{
				fInvalidUnicode = true;
				return;
			}
			int num9 = (ch & 'Ͽ') << 10;
			fUsedNextChar = true;
			num9 = num9 | chNext & 'Ͽ';
			num9 += num6;
			int num10 = num2 | num9 >> 18;
			output.Write(BCSHelper.s_crgstrUrlHexValue[num10]);
			num10 = num3 | (num9 & 258048) >> 12;
			output.Write(BCSHelper.s_crgstrUrlHexValue[num10]);
			num10 = num3 | (num9 & 4032) >> 6;
			output.Write(BCSHelper.s_crgstrUrlHexValue[num10]);
			num10 = num3 | num9 & 63;
			output.Write(BCSHelper.s_crgstrUrlHexValue[num10]);
		}

		public static string UrlKeyValueDecode(string keyOrValueToDecode)
		{
			if (string.IsNullOrEmpty(keyOrValueToDecode))
			{
				return keyOrValueToDecode;
			}
			return BCSHelper.UrlDecodeHelper(keyOrValueToDecode, keyOrValueToDecode.Length, true);
		}

		public static string UrlKeyValueEncode(string keyOrValueToEncode)
		{
			if (keyOrValueToEncode == null || keyOrValueToEncode.Length == 0)
			{
				return keyOrValueToEncode;
			}
			StringBuilder stringBuilder = new StringBuilder(255);
			HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter(stringBuilder, CultureInfo.InvariantCulture));
			BCSHelper.UrlKeyValueEncode(keyOrValueToEncode, htmlTextWriter);
			return stringBuilder.ToString();
		}

		public static void UrlKeyValueEncode(string keyOrValueToEncode, TextWriter output)
		{
			if (keyOrValueToEncode == null || keyOrValueToEncode.Length == 0 || output == null)
			{
				return;
			}
			bool flag = false;
			int num = 0;
			int num1 = 0;
			int length = keyOrValueToEncode.Length;
			for (int i = 0; i < length; i++)
			{
				char chr = keyOrValueToEncode[i];
				if ((48 > chr || chr > '9') && (97 > chr || chr > 'z') && (65 > chr || chr > 'Z'))
				{
					if (num1 > 0)
					{
						output.Write(keyOrValueToEncode.Substring(num, num1));
						num1 = 0;
					}
					BCSHelper.UrlEncodeUnicodeChar(output, keyOrValueToEncode[i], (i < length - 1 ? keyOrValueToEncode[i + 1] : '\0'), out flag);
					if (flag)
					{
						i++;
					}
					num = i + 1;
				}
				else
				{
					num1++;
				}
			}
			if (num < length && output != null)
			{
				output.Write(keyOrValueToEncode.Substring(num));
			}
		}
	}
}