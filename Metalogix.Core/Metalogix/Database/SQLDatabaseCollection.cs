using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.Database
{
    public class SQLDatabaseCollection : List<SQLDatabase>
    {
        public SQLDatabaseCollection(XmlNode xml)
        {
            if (xml == null)
            {
                return;
            }

            foreach (XmlNode childNode in xml.ChildNodes)
            {
                base.Add(new SQLDatabase(childNode));
            }
        }
    }
}