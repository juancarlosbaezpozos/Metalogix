using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Metalogix.Telemetry.MessageFormat
{
    public static class Converter
    {
        public static string TransformXmlToText(string string_0)
        {
            string str =
                "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\r\n            <xsl:output method=\"text\" omit-xml-declaration=\"yes\" indent=\"yes\"/>\r\n            <xsl:strip-space elements=\"*\"/>\r\n            <xsl:template match=\"/Action\">\r\n                <xsl:param name=\"sourceAdapter\" select=\"./@SourceAdapter\"/>  \r\n                <xsl:param name=\"sourceVersion\" select=\"./@SourceVersion\"/>  \r\n                <xsl:param name=\"isSourceSharePointOnline\" select=\"./@IsSourceSharePointOnline\"/>  \r\n                <xsl:param name=\"targetAdapter\" select=\"./@TargetAdapter\"/>  \r\n                <xsl:param name=\"targetVersion\" select=\"./@TargetVersion\"/>     \r\n                <xsl:param name=\"isTargetSharePointOnline\" select=\"./@IsTargetSharePointOnline\"/>               \r\n                <xsl:text>SourceAdpt:</xsl:text><xsl:value-of select=\"$sourceAdapter\"/>\r\n                <xsl:text>, </xsl:text>\r\n                <xsl:text>SourceVer:</xsl:text><xsl:value-of select=\"$sourceVersion\"/>\r\n                <xsl:text>, </xsl:text>\r\n                <xsl:if test=\" ($isSourceSharePointOnline != '')\">\r\n                    <xsl:text>IsSourceSharePointOnline:</xsl:text><xsl:value-of select=\"$isSourceSharePointOnline\"/>\r\n                    <xsl:text>, </xsl:text>\r\n                </xsl:if>\r\n                <xsl:text>TargetAdpt:</xsl:text><xsl:value-of select=\"$targetAdapter\"/>\r\n                <xsl:text>, </xsl:text>\r\n                <xsl:text>TargetVer:</xsl:text><xsl:value-of select=\"$targetVersion\"/>\r\n                <xsl:text>, </xsl:text>\r\n                <xsl:if test=\" ($isTargetSharePointOnline != '')\">\r\n                    <xsl:text>IsTargetSharePointOnline:</xsl:text><xsl:value-of select=\"$isTargetSharePointOnline\"/>\r\n                    <xsl:text>, </xsl:text>\r\n                </xsl:if>\r\n                <xsl:apply-templates/>\r\n            </xsl:template>            \r\n            <xsl:template match=\"*\">            \r\n            <xsl:if test= \"not(@Type = 'System.String' or @Type = 'Metalogix.Transformers.TransformerCollection')\">  <!-- Skiping string type and transformer type options -->              \r\n                <xsl:if test= \"not(contains(name(), 'FilterExpression') )\">   <!-- Skiping filter expression -->              \r\n                    <xsl:if test= \"(text() != '')\">                           \r\n                        <xsl:value-of select=\"name()\"/><xsl:text>:</xsl:text><xsl:value-of select=\"text()\"/><xsl:text>, </xsl:text> <!-- Parent node -->       \r\n                    </xsl:if>                               \r\n                    <xsl:variable name=\"parentNodeVariable\" select=\"name()\" />                \r\n                    <xsl:if test= \"not(@Type)\">\r\n                        <xsl:for-each select=\"@*\">                            \r\n                            <xsl:value-of select=\"concat($parentNodeVariable,'_',name(), ':', ., ', ')\"/> <!-- Parent node attributes -->\r\n                        </xsl:for-each>\r\n                    </xsl:if>                \r\n                    <xsl:for-each select=\"*\">\r\n                        <xsl:if test= \"not(contains(name(), 'FilterExpression') )\">\r\n                            <xsl:if test= \"(text() != '')\">                        \r\n                                <xsl:value-of select=\"name()\"/><xsl:text>:</xsl:text><xsl:value-of select=\"text()\"/><xsl:text>, </xsl:text> <!-- Child (First level) node -->\r\n                            </xsl:if>                            \r\n                            <xsl:variable name=\"childNodeVariable\" select=\"name()\" />                \r\n                            <xsl:if test= \"not(@Type)\">\r\n                                <xsl:for-each select=\"@*\">                \r\n                                    <xsl:value-of select=\"concat($childNodeVariable,'_',name(), ':', ., ', ')\"/> <!-- Child (First level) node attributes -->\r\n                                </xsl:for-each>\r\n                            </xsl:if>\r\n                            <xsl:for-each select=\"*\">\r\n                                <xsl:if test= \"not(contains(name(), 'FilterExpression') )\"> \r\n                                    <xsl:if test= \"(text() != '')\">                                       \r\n                                        <xsl:value-of select=\"name()\"/><xsl:text>:</xsl:text><xsl:value-of select=\"text()\"/><xsl:text>, </xsl:text> <!-- Child (Second level) node -->\r\n                                    </xsl:if>\r\n                                    <xsl:variable name=\"lastNodeVariable\" select=\"name()\" />                              \r\n                                    <xsl:if test= \"not(@Type)\">                  \r\n                                        <xsl:for-each select=\"@*\">                \r\n                                            <xsl:value-of select=\"concat($lastNodeVariable,'_',name(), ':', ., ', ')\"/> <!-- Child (Second level) node attributes -->\r\n                                        </xsl:for-each>\r\n                                    </xsl:if>\r\n                                </xsl:if>\r\n                            </xsl:for-each>\r\n                        </xsl:if>\r\n                    </xsl:for-each>        \r\n                </xsl:if>\r\n            </xsl:if>\r\n            </xsl:template></xsl:stylesheet>";
            StringBuilder stringBuilder = new StringBuilder();
            XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
            using (StringReader stringReader = new StringReader(string_0))
            {
                using (StringReader stringReader1 = new StringReader(str))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        using (XmlReader xmlReader1 = XmlReader.Create(stringReader1))
                        {
                            xslCompiledTransform.Load(xmlReader1);
                            using (XmlWriter xmlWriter =
                                   XmlWriter.Create(stringBuilder, xslCompiledTransform.OutputSettings))
                            {
                                xslCompiledTransform.Transform(xmlReader, xmlWriter);
                            }
                        }
                    }
                }
            }

            return stringBuilder.ToString().Trim().TrimEnd(new char[] { ',' });
        }
    }
}