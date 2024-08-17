using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Permissions
{
    public class SecurityPrincipalCollection : SerializableIndexedList<SecurityPrincipal>
    {
        public override Type CollectionType
        {
            get
            {
                Type type;
                using (IEnumerator<SecurityPrincipal> enumerator =
                       ((IEnumerable<SecurityPrincipal>)this).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        type = enumerator.Current.GetType();
                    }
                    else
                    {
                        return typeof(SecurityPrincipal);
                    }
                }

                return type;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public virtual SecurityPrincipal this[string sPrincipalName]
        {
            get { return base[sPrincipalName]; }
        }

        public SecurityPrincipalCollection() : base("PrincipalNameInvariant")
        {
        }

        public SecurityPrincipalCollection(SecurityPrincipal[] principals) : base("PrincipalNameInvariant")
        {
            if (principals != null)
            {
                base.AddRange(principals);
            }
        }

        public SecurityPrincipalCollection(XmlNode xml) : base("PrincipalNameInvariant")
        {
            this.FromXML(xml);
        }

        public virtual SecurityPrincipal AddPrincipal(SecurityPrincipal principal)
        {
            base.AddToCollection(principal);
            return principal;
        }

        public virtual void DeletePrincipal(SecurityPrincipal principal)
        {
            base.RemoveFromCollection(principal);
        }

        public virtual SecurityPrincipal MapSecurityPrincipal(SecurityPrincipal securityPrincipal)
        {
            SecurityPrincipal securityPrincipal1 =
                PrincipalConverter.ConvertPrincipal(securityPrincipal, this.CollectionType);
            if (securityPrincipal1 == null)
            {
                object[] fullName = new object[] { securityPrincipal.GetType().FullName, this.CollectionType.FullName };
                throw new ArgumentException(string.Format(
                    "Cannot map principal: No conversion is defined between security principals of type {0} and {1}",
                    fullName));
            }

            float single = 0f;
            SecurityPrincipal securityPrincipal2 = null;
            using (IEnumerator<SecurityPrincipal> enumerator = ((IEnumerable<SecurityPrincipal>)this).GetEnumerator())
            {
                do
                {
                    Label0:
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    SecurityPrincipal current = enumerator.Current;
                    float similarity = securityPrincipal1.GetSimilarity(current);
                    if (similarity > single)
                    {
                        single = similarity;
                        securityPrincipal2 = current;
                    }
                    else
                    {
                        goto Label0;
                    }
                } while ((double)single != 1);
            }

            return securityPrincipal2;
        }
    }
}