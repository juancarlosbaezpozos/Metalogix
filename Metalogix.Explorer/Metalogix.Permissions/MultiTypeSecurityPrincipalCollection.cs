using Metalogix;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public class MultiTypeSecurityPrincipalCollection : SecurityPrincipalCollection
    {
        protected SecurityPrincipalCollection[] m_data;

        public override SecurityPrincipal this[SecurityPrincipal key]
        {
            get
            {
                SecurityPrincipalCollection[] mData = this.m_data;
                for (int i = 0; i < (int)mData.Length; i++)
                {
                    SecurityPrincipal item = mData[i][key];
                    if (item != null && key.GetType() == item.GetType())
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public override SecurityPrincipal this[string sPrincipalName]
        {
            get
            {
                SecurityPrincipalCollection[] mData = this.m_data;
                for (int i = 0; i < (int)mData.Length; i++)
                {
                    SecurityPrincipal item = mData[i][sPrincipalName];
                    if (item != null)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public MultiTypeSecurityPrincipalCollection()
        {
            this.m_data = new SecurityPrincipalCollection[0];
        }

        public MultiTypeSecurityPrincipalCollection(SecurityPrincipalCollection[] collections)
        {
            this.m_data = collections;
            if (collections != null)
            {
                SecurityPrincipalCollection[] securityPrincipalCollectionArray = collections;
                for (int i = 0; i < (int)securityPrincipalCollectionArray.Length; i++)
                {
                    base.AddRangeToCollection(securityPrincipalCollectionArray[i].ToArray());
                }
            }
        }

        public MultiTypeSecurityPrincipalCollection(SecurityPrincipal[] principals) : base(principals)
        {
            this.m_data = (principals == null
                ? new SecurityPrincipalCollection[0]
                : new SecurityPrincipalCollection[] { new SecurityPrincipalCollection(principals) });
        }

        public override SecurityPrincipal AddPrincipal(SecurityPrincipal principal)
        {
            SecurityPrincipalCollection[] mData = this.m_data;
            for (int i = 0; i < (int)mData.Length; i++)
            {
                SecurityPrincipalCollection securityPrincipalCollection = mData[i];
                if (securityPrincipalCollection.CollectionType == principal.GetType())
                {
                    return securityPrincipalCollection.AddPrincipal(principal);
                }
            }

            SecurityPrincipal securityPrincipal = null;
            SecurityPrincipalCollection[] securityPrincipalCollectionArray = this.m_data;
            for (int j = 0; j < (int)securityPrincipalCollectionArray.Length; j++)
            {
                SecurityPrincipalCollection securityPrincipalCollection1 = securityPrincipalCollectionArray[j];
                securityPrincipal =
                    PrincipalConverter.ConvertPrincipal(principal, securityPrincipalCollection1.CollectionType);
                if (securityPrincipal != null)
                {
                    securityPrincipalCollection1.AddPrincipal(securityPrincipal);
                }
            }

            return principal;
        }

        public override void DeletePrincipal(SecurityPrincipal principal)
        {
            SecurityPrincipalCollection[] mData = this.m_data;
            int num = 0;
            while (true)
            {
                if (num < (int)mData.Length)
                {
                    SecurityPrincipalCollection securityPrincipalCollection = mData[num];
                    if (securityPrincipalCollection.CollectionType != principal.GetType())
                    {
                        num++;
                    }
                    else
                    {
                        securityPrincipalCollection.DeletePrincipal(principal);
                        break;
                    }
                }
                else
                {
                    SecurityPrincipal securityPrincipal = null;
                    SecurityPrincipalCollection[] securityPrincipalCollectionArray = this.m_data;
                    int num1 = 0;
                    while (num1 < (int)securityPrincipalCollectionArray.Length)
                    {
                        SecurityPrincipalCollection securityPrincipalCollection1 =
                            securityPrincipalCollectionArray[num1];
                        securityPrincipal =
                            PrincipalConverter.ConvertPrincipal(principal, securityPrincipalCollection1.CollectionType);
                        if (securityPrincipal == null)
                        {
                            num1++;
                        }
                        else
                        {
                            securityPrincipalCollection1.DeletePrincipal(securityPrincipal);
                        }
                    }
                }
            }
        }

        public override void FromXML(XmlNode xmlNode)
        {
            foreach (XmlNode xmlNodes in xmlNode.SelectSingleNode("//PrincipalCollections")
                         .SelectNodes("./PrincipalCollection"))
            {
                string value = xmlNodes.Attributes["Type"].Value;
                SecurityPrincipalCollection securityPrincipalCollection =
                    (SecurityPrincipalCollection)Activator.CreateInstance(Type.GetType(TypeUtils.UpdateType(value)));
                securityPrincipalCollection.FromXML(xmlNodes);
            }
        }

        public override SecurityPrincipal MapSecurityPrincipal(SecurityPrincipal securityPrincipal)
        {
            SecurityPrincipalCollection[] mData = this.m_data;
            for (int i = 0; i < (int)mData.Length; i++)
            {
                SecurityPrincipalCollection securityPrincipalCollection = mData[i];
                if (securityPrincipalCollection.CollectionType == securityPrincipal.GetType())
                {
                    return securityPrincipalCollection.MapSecurityPrincipal(securityPrincipal);
                }
            }

            Dictionary<SecurityPrincipal, SecurityPrincipal> securityPrincipals = null;
            SecurityPrincipal securityPrincipal1 = null;
            SecurityPrincipalCollection[] securityPrincipalCollectionArray = this.m_data;
            for (int j = 0; j < (int)securityPrincipalCollectionArray.Length; j++)
            {
                SecurityPrincipalCollection securityPrincipalCollection1 = securityPrincipalCollectionArray[j];
                securityPrincipal1 =
                    PrincipalConverter.ConvertPrincipal(securityPrincipal, securityPrincipalCollection1.CollectionType);
                if (securityPrincipal1 != null)
                {
                    securityPrincipals.Add(securityPrincipal1,
                        securityPrincipalCollection1.MapSecurityPrincipal(securityPrincipal1));
                }
            }

            if (securityPrincipals.Count == 0)
            {
                throw new ArgumentException(string.Format(
                    "Cannot map principal: No conversion is defined between a principal of type{0} and any of the principal types contained within this collection",
                    securityPrincipal.GetType().Name));
            }

            float single = 0f;
            SecurityPrincipal securityPrincipal2 = null;
            SecurityPrincipal item = null;
            foreach (SecurityPrincipal key in securityPrincipals.Keys)
            {
                item = securityPrincipals[key];
                float similarity = key.GetSimilarity(item);
                if (similarity <= single)
                {
                    continue;
                }

                single = similarity;
                securityPrincipal2 = item;
            }

            return securityPrincipal2;
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("PrincipalCollections");
            SecurityPrincipalCollection[] mData = this.m_data;
            for (int i = 0; i < (int)mData.Length; i++)
            {
                SecurityPrincipalCollection securityPrincipalCollection = mData[i];
                xmlWriter.WriteStartElement("PrincipalCollection");
                xmlWriter.WriteAttributeString("Type", securityPrincipalCollection.GetType().AssemblyQualifiedName);
                securityPrincipalCollection.ToXML(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }
    }
}