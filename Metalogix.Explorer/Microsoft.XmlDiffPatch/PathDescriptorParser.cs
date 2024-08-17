using System;
using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal class PathDescriptorParser
    {
        private static char[] Delimiters;

        private static char[] MultiNodesDelimiters;

        static PathDescriptorParser()
        {
            PathDescriptorParser.Delimiters = new char[] { '|', '-', '/' };
            PathDescriptorParser.MultiNodesDelimiters = new char[] { '|', '-' };
        }

        public PathDescriptorParser()
        {
        }

        private static void OnInvalidExpression(string path)
        {
            XmlPatchError.Error("Invalid XDL diffgram. '{0}' is an invalid path descriptor.", path);
        }

        private static void OnNoMatchingNode(string path)
        {
            XmlPatchError.Error("Invalid XDL diffgram. No node matches the path descriptor '{0}'.", path);
        }

        private static string ReadAttrName(string str, ref int pos)
        {
            int length = str.IndexOf('|', pos);
            if (length < 0)
            {
                length = str.Length;
            }

            string str1 = str.Substring(pos, length - pos);
            pos = length;
            return str1;
        }

        private static int ReadPosition(string str, ref int pos)
        {
            int length = str.IndexOfAny(PathDescriptorParser.Delimiters, pos);
            if (length < 0)
            {
                length = str.Length;
            }

            int num = int.Parse(str.Substring(pos, length - pos));
            pos = length;
            return num;
        }

        private static XmlNodeList SelectAbsoluteNodes(XmlNode rootNode, string path)
        {
            int num;
            int num1 = 1;
            XmlNode xmlNodes = rootNode;
            while (true)
            {
                num = num1;
                XmlNodeList childNodes = xmlNodes.ChildNodes;
                int num2 = PathDescriptorParser.ReadPosition(path, ref num1);
                if (num1 == path.Length || path[num1] == '/')
                {
                    if (childNodes.Count == 0 || num2 < 0 || num2 > childNodes.Count)
                    {
                        PathDescriptorParser.OnNoMatchingNode(path);
                    }

                    xmlNodes = childNodes.Item(num2 - 1);
                    if (num1 == path.Length)
                    {
                        XmlPatchNodeList singleNodeList = new SingleNodeList();
                        singleNodeList.AddNode(xmlNodes);
                        return singleNodeList;
                    }

                    num1++;
                }
                else
                {
                    if (path[num1] == '-' || path[num1] == '|')
                    {
                        break;
                    }

                    PathDescriptorParser.OnInvalidExpression(path);
                }
            }

            return PathDescriptorParser.SelectChildNodes(xmlNodes, path, num);
        }

        private static XmlNodeList SelectAllAttributes(XmlNode parentNode)
        {
            XmlAttributeCollection attributes = parentNode.Attributes;
            if (attributes.Count == 0)
            {
                PathDescriptorParser.OnNoMatchingNode("@*");
                return null;
            }

            if (attributes.Count == 1)
            {
                XmlPatchNodeList singleNodeList = new SingleNodeList();
                singleNodeList.AddNode(attributes.Item(0));
                return singleNodeList;
            }

            IEnumerator enumerator = attributes.GetEnumerator();
            XmlPatchNodeList multiNodeList = new MultiNodeList();
            while (enumerator.MoveNext())
            {
                multiNodeList.AddNode((XmlNode)enumerator.Current);
            }

            return multiNodeList;
        }

        private static XmlNodeList SelectAllChildren(XmlNode parentNode)
        {
            XmlNodeList childNodes = parentNode.ChildNodes;
            if (childNodes.Count == 0)
            {
                PathDescriptorParser.OnNoMatchingNode("*");
                return null;
            }

            if (childNodes.Count == 1)
            {
                XmlPatchNodeList singleNodeList = new SingleNodeList();
                singleNodeList.AddNode(childNodes.Item(0));
                return singleNodeList;
            }

            IEnumerator enumerator = childNodes.GetEnumerator();
            XmlPatchNodeList multiNodeList = new MultiNodeList();
            while (enumerator.MoveNext())
            {
                multiNodeList.AddNode((XmlNode)enumerator.Current);
            }

            return multiNodeList;
        }

        private static XmlNodeList SelectAttributes(XmlNode parentNode, string path)
        {
            int num = 1;
            XmlAttributeCollection attributes = parentNode.Attributes;
            XmlPatchNodeList multiNodeList = null;
            while (true)
            {
                string str = PathDescriptorParser.ReadAttrName(path, ref num);
                if (multiNodeList == null)
                {
                    if (num != path.Length)
                    {
                        multiNodeList = new MultiNodeList();
                    }
                    else
                    {
                        multiNodeList = new SingleNodeList();
                    }
                }

                XmlNode namedItem = attributes.GetNamedItem(str);
                if (namedItem == null)
                {
                    PathDescriptorParser.OnNoMatchingNode(path);
                }

                multiNodeList.AddNode(namedItem);
                if (num == path.Length)
                {
                    break;
                }

                if (path[num] != '|')
                {
                    PathDescriptorParser.OnInvalidExpression(path);
                }
                else
                {
                    num++;
                    if (path[num] != '@')
                    {
                        PathDescriptorParser.OnInvalidExpression(path);
                    }

                    num++;
                }
            }

            return multiNodeList;
        }

        private static XmlNodeList SelectChildNodes(XmlNode parentNode, string path, int startPos)
        {
            int num = startPos;
            XmlPatchNodeList multiNodeList = null;
            XmlNodeList childNodes = parentNode.ChildNodes;
            int num1 = PathDescriptorParser.ReadPosition(path, ref num);
            if (num != path.Length)
            {
                multiNodeList = new MultiNodeList();
            }
            else
            {
                multiNodeList = new SingleNodeList();
            }

            while (true)
            {
                if (num1 <= 0 || num1 > childNodes.Count)
                {
                    PathDescriptorParser.OnNoMatchingNode(path);
                }

                XmlNode nextSibling = childNodes.Item(num1 - 1);
                multiNodeList.AddNode(nextSibling);
                if (num == path.Length)
                {
                    break;
                }

                if (path[num] == '|')
                {
                    num++;
                }
                else if (path[num] == '-')
                {
                    num++;
                    int num2 = PathDescriptorParser.ReadPosition(path, ref num);
                    if (num2 <= 0 || num2 > childNodes.Count)
                    {
                        PathDescriptorParser.OnNoMatchingNode(path);
                    }

                    while (num1 < num2)
                    {
                        num1++;
                        nextSibling = nextSibling.NextSibling;
                        multiNodeList.AddNode(nextSibling);
                    }

                    if (num == path.Length)
                    {
                        break;
                    }

                    if (path[num] != '|')
                    {
                        PathDescriptorParser.OnInvalidExpression(path);
                    }
                    else
                    {
                        num++;
                    }
                }

                num1 = PathDescriptorParser.ReadPosition(path, ref num);
            }

            return multiNodeList;
        }

        internal static XmlNodeList SelectNodes(XmlNode rootNode, XmlNode currentParentNode, string pathDescriptor)
        {
            char chr = pathDescriptor[0];
            if (chr == '*')
            {
                if (pathDescriptor.Length == 1)
                {
                    return PathDescriptorParser.SelectAllChildren(currentParentNode);
                }

                PathDescriptorParser.OnInvalidExpression(pathDescriptor);
                return null;
            }

            if (chr == '/')
            {
                return PathDescriptorParser.SelectAbsoluteNodes(rootNode, pathDescriptor);
            }

            if (chr != '@')
            {
                return PathDescriptorParser.SelectChildNodes(currentParentNode, pathDescriptor, 0);
            }

            if (pathDescriptor.Length < 2)
            {
                PathDescriptorParser.OnInvalidExpression(pathDescriptor);
            }

            if (pathDescriptor[1] == '*')
            {
                return PathDescriptorParser.SelectAllAttributes(currentParentNode);
            }

            return PathDescriptorParser.SelectAttributes(currentParentNode, pathDescriptor);
        }
    }
}