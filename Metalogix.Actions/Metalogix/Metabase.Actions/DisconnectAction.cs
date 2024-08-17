using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Interfaces;
using Metalogix.Metabase.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Metalogix.Metabase.Actions
{
    [LaunchAsJob(true)]
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [MenuText("Disconnect Metabase")]
    [Name("Disconnect Metabase")]
    [RunAsync(true)]
    [ShowInMenus(false)]
    [ShowStatusDialog(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(Connection))]
    [UsesStickySettings(true)]
    public class DisconnectAction : MetabaseAction<DisconnectOptions>
    {
        public DisconnectAction()
        {
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            string str;
            foreach (object obj in target)
            {
                Connection connection = obj as Connection;
                if (connection == null || connection.MetabaseConnection == null)
                {
                    continue;
                }

                MetabaseConnection metabaseConnection = connection.MetabaseConnection;
                if (metabaseConnection.Adapter.AdapterType != "SqlCe")
                {
                    continue;
                }

                string adapterContext = metabaseConnection.Adapter.AdapterContext;
                metabaseConnection.Dispose();
                bool flag = false;
                using (IEnumerator<Node> enumerator = Settings.ActiveConnections.GetEnumerator())
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            Connection current = (Connection)enumerator.Current;
                            if (current != connection && current.MetabaseConnection != null &&
                                current.MetabaseConnection.MetabaseContext == adapterContext)
                            {
                                flag = true;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (!File.Exists(adapterContext))
                {
                    continue;
                }

                if (!this.ActionOptions.SaveFileMappings.TryGetValue(adapterContext, out str))
                {
                    if (flag)
                    {
                        continue;
                    }

                    File.Delete(adapterContext);
                }
                else if (!flag)
                {
                    File.Move(adapterContext, str);
                }
                else
                {
                    File.Copy(adapterContext, str);
                }
            }
        }
    }
}