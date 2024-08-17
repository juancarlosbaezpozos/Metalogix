using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.InfoPathFormsServices
{
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://schemas.microsoft.com/office/infopath/2007/formsServices")]
    public enum Feature
    {
        GenericXsf,
        XsfSchema,
        GenericXsl,
        GenericXPath,
        TemplateXml,
        Layout,
        Controls,
        BusinessLogic,
        Calculations,
        Validation,
        DigitalSignatures,
        DataAdapters,
        Submit,
        Views,
        Rules,
        ConditionalFormatting,
        VersionUpgrade
    }
}