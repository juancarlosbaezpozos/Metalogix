using Metalogix.SharePoint.Adapters.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Adapters
{
    public static class AspxUtils
    {
        public static void GetAreaBetweenTags(AspxTagInfo openingTag, AspxTagInfo closingTag, out int startIdx,
            out int length)
        {
            startIdx = -1;
            length = -1;
            if (openingTag == null)
            {
                throw new ArgumentNullException("openingTag");
            }

            if (closingTag == null)
            {
                throw new ArgumentNullException("closingTag");
            }

            startIdx = openingTag.EndIndex + 1;
            length = closingTag.StartIndex - startIdx;
        }

        public static void GetAreaBetweenTags(string text, int index, out int startIdx, out int length)
        {
            AspxTagInfo aspxTagInfo;
            AspxTagInfo aspxTagInfo1;
            AspxUtils.GetOpeningAndClosingTagsFromAspx(text, index, out aspxTagInfo, out aspxTagInfo1);
            AspxUtils.GetAreaBetweenTags(aspxTagInfo, aspxTagInfo1, out startIdx, out length);
        }

        private static AspxTagInfo GetNextTag(string text, int idx)
        {
            string str;
            AspxTagType aspxTagType;
            idx = text.IndexOf('<', idx);
            if (idx < 0)
            {
                throw new Exception(Resources.UnexpectedEndOfTextNotAllTagsClosed);
            }

            if (idx == text.Length - 1)
            {
                throw new Exception(Resources.UnexpectedEndOfTextLastTagIncomplete);
            }

            AspxTagInfo aspxTagInfo = new AspxTagInfo()
            {
                Name = AspxUtils.GetTagName(text, idx),
                StartIndex = idx
            };
            AspxUtils.GetTagTypeAndClosingBracket(text, idx, out aspxTagType, out str);
            aspxTagInfo.Type = aspxTagType;
            idx = text.IndexOf(str, idx, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                throw new Exception(Resources.UnexpectedEndOfTextFailedToLocateClosingBracket);
            }

            aspxTagInfo.EndIndex = idx + str.Length - 1;
            if (aspxTagInfo.Type == AspxTagType.OpeningTag && text[aspxTagInfo.EndIndex - 1] == '/')
            {
                aspxTagInfo.Type = AspxTagType.ContainedTag;
            }

            return aspxTagInfo;
        }

        public static void GetOpeningAndClosingTagsFromAspx(string text, int start, out AspxTagInfo firstTag,
            out AspxTagInfo closingTag)
        {
            firstTag = AspxUtils.GetTag(text, start);
            closingTag = null;
            if (firstTag.Type != AspxTagType.OpeningTag)
            {
                return;
            }

            Stack<AspxTagInfo> aspxTagInfos = new Stack<AspxTagInfo>();
            aspxTagInfos.Push(firstTag);
            AspxTagInfo nextTag = firstTag;
            while (aspxTagInfos.Count > 0)
            {
                nextTag = AspxUtils.GetNextTag(text, nextTag.EndIndex);
                if (nextTag.Type != AspxTagType.ClosingTag)
                {
                    if (nextTag.Type != AspxTagType.OpeningTag)
                    {
                        continue;
                    }

                    aspxTagInfos.Push(nextTag);
                }
                else
                {
                    Stack<AspxTagInfo> aspxTagInfos1 = new Stack<AspxTagInfo>();
                    while (aspxTagInfos.Count > 0 && !string.Equals(aspxTagInfos.Peek().Name, nextTag.Name,
                               StringComparison.OrdinalIgnoreCase))
                    {
                        aspxTagInfos1.Push(aspxTagInfos.Pop());
                    }

                    if (aspxTagInfos.Count != 0)
                    {
                        aspxTagInfos.Pop();
                        if (aspxTagInfos.Count != 0)
                        {
                            continue;
                        }

                        closingTag = nextTag;
                        return;
                    }
                    else
                    {
                        while (aspxTagInfos1.Count > 0)
                        {
                            aspxTagInfos.Push(aspxTagInfos1.Pop());
                        }
                    }
                }
            }
        }

        public static AspxTagInfo GetTag(string text, int idx)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException(Resources.UnableToParseTextValueIsEmpty, "text");
            }

            if (idx < 0 || idx >= text.Length)
            {
                throw new ArgumentException(Resources.StartIndexOutsideTheBoundsOfString, "idx");
            }

            idx = text.LastIndexOf('<', idx);
            if (idx < 0)
            {
                throw new Exception("Failed to locate tag in given text.");
            }

            return AspxUtils.GetNextTag(text, idx);
        }

        private static string GetTagName(string text, int idx)
        {
            idx++;
            if (idx >= text.Length)
            {
                throw new Exception(Resources.UnexpectedEndOfAspxContent);
            }

            if (text[idx] == '/' || text[idx] == '%' || text[idx] == '!')
            {
                idx++;
                if (idx >= text.Length)
                {
                    throw new Exception(Resources.UnexpectedEndOfAspxContent);
                }
            }

            char[] chrArray = new char[] { '>', ' ' };
            int num = text.IndexOfAny(chrArray, idx);
            if (num < 0)
            {
                return null;
            }

            return text.Substring(idx, num - idx);
        }

        private static void GetTagTypeAndClosingBracket(string text, int idx, out AspxTagType type,
            out string closingBracket)
        {
            closingBracket = ">";
            if (idx < text.Length - 1 && text[idx + 1] == '/')
            {
                type = AspxTagType.ClosingTag;
                return;
            }

            if (idx >= text.Length - 1 || text[idx + 1] != '%')
            {
                if (text[idx + 1] == '!')
                {
                    type = AspxTagType.CodeBlock;
                    if (idx < text.Length - 3 && text[idx + 2] == '-' && text[idx + 3] == '-')
                    {
                        closingBracket = "-->";
                        return;
                    }

                    closingBracket = "]]>";
                    return;
                }

                type = AspxTagType.OpeningTag;
            }
            else
            {
                type = AspxTagType.CodeBlock;
                closingBracket = "%>";
                if (idx < text.Length - 3 && text[idx + 2] == '-' && text[idx + 3] == '-')
                {
                    closingBracket = "--%>";
                    return;
                }
            }
        }

        public static void GetTotalAreaOfTag(AspxTagInfo tag, out int startIndex, out int length)
        {
            AspxUtils.GetTotalAreaOfTag(tag, null, out startIndex, out length);
        }

        public static void GetTotalAreaOfTag(AspxTagInfo tag, AspxTagInfo closingTag, out int startIndex,
            out int length)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            startIndex = tag.StartIndex;
            length = startIndex - (closingTag != null ? closingTag.EndIndex : tag.EndIndex) + 1;
        }

        public static void GetTotalAreaOfTag(string text, int index, out int startIndex, out int length)
        {
            AspxTagInfo aspxTagInfo;
            AspxTagInfo aspxTagInfo1;
            AspxUtils.GetOpeningAndClosingTagsFromAspx(text, index, out aspxTagInfo, out aspxTagInfo1);
            AspxUtils.GetTotalAreaOfTag(aspxTagInfo, aspxTagInfo1, out startIndex, out length);
        }
    }
}