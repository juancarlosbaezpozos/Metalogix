using System;
using System.Data;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public static class ExternalizationUtils
    {
        public static byte[] GetBlankBlobRef()
        {
            byte[] numArray = new byte[32];
            (new Guid("137D2254-9895-4ab5-B9B0-EAEFDC186820")).ToByteArray().CopyTo(numArray, 0);
            (new Guid("852C3799-6779-45ab-B585-79814BF72CD0")).ToByteArray().CopyTo(numArray, 16);
            return numArray;
        }

        public static bool IsExternalized(DataRow dr)
        {
            bool flag = false;
            if (dr.Table.Columns.Contains("RbsId") && !(dr["RbsId"] is DBNull))
            {
                flag = true;
            }

            if (dr.Table.Columns.Contains("_DocFlags") && !(dr["_DocFlags"] is DBNull) &&
                (Convert.ToInt32(dr["_DocFlags"].ToString()) & 65536) == 65536)
            {
                flag = true;
            }

            return flag;
        }

        public static bool IsExternalizedContent(XmlAttribute externalizedAttr)
        {
            if (externalizedAttr == null)
            {
                return false;
            }

            bool flag = false;
            if (!bool.TryParse(externalizedAttr.Value, out flag))
            {
                return false;
            }

            return flag;
        }

        public static void WriteExternalizationData(DataRow drExternalization, XmlWriter xmlWriter)
        {
            bool flag = ExternalizationUtils.IsExternalized(drExternalization);
            xmlWriter.WriteAttributeString("IsExternalized", flag.ToString());
            if (drExternalization.Table.Columns.Contains("RbsId"))
            {
                if (drExternalization["RbsId"] is DBNull)
                {
                    xmlWriter.WriteAttributeString("RbsId", "");
                    return;
                }

                object item = drExternalization["RbsId"];
                StringBuilder stringBuilder = new StringBuilder(100);
                stringBuilder.Append("0x");
                byte[] numArray = (byte[])item;
                for (int i = 0; i < (int)numArray.Length; i++)
                {
                    byte num = numArray[i];
                    stringBuilder.Append(string.Format("{0:X2}", num));
                }

                xmlWriter.WriteAttributeString("RbsId", stringBuilder.ToString());
            }
        }
    }
}