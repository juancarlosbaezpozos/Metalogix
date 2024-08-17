using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Metalogix.DataStructures
{
    public class X509CertificateWrapperCollection : List<X509CertificateWrapper>
    {
        public const string ROOT_ELEMENT_NAME = "IncludedCertificates";

        public X509CertificateWrapperCollection()
        {
        }

        public X509CertificateWrapperCollection(int capacity) : base(capacity)
        {
        }

        public X509CertificateWrapperCollection(IEnumerable<X509CertificateWrapper> collection) : base(collection)
        {
        }

        protected X509CertificateWrapperCollection(string sCollectionXml)
        {
            this.FromXml(sCollectionXml);
        }

        protected X509CertificateWrapperCollection(XmlNode collectionXml)
        {
            this.FromXml(collectionXml);
        }

        public static X509CertificateWrapperCollection BuildCollectionFromXml(string sXml)
        {
            return new X509CertificateWrapperCollection(sXml);
        }

        public static X509CertificateWrapperCollection BuildCollectionFromXml(XmlNode node)
        {
            return new X509CertificateWrapperCollection(node);
        }

        public void CopyCertificatesToCollection(X509CertificateCollection collection)
        {
            foreach (X509CertificateWrapper x509CertificateWrapper in this)
            {
                collection.Add(x509CertificateWrapper);
            }
        }

        private void FromXml(string sXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXml(xmlDocument.DocumentElement);
        }

        private void FromXml(XmlNode node)
        {
            base.Clear();
            foreach (XmlNode xmlNodes in node.SelectNodes(".//Certificate"))
            {
                try
                {
                    base.Add(new X509CertificateWrapper(xmlNodes));
                }
                catch (Exception exception)
                {
                }
            }
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(1000);
            XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            this.ToXml(xmlTextWriter);
            xmlTextWriter.Flush();
            return stringBuilder.ToString();
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("IncludedCertificates");
            foreach (X509CertificateWrapper x509CertificateWrapper in this)
            {
                x509CertificateWrapper.ToXML(writer);
            }

            writer.WriteEndElement();
        }
    }
}