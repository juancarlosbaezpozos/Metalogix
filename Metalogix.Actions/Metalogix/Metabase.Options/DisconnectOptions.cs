using Metalogix.Actions;
using System;
using System.Collections.Generic;

namespace Metalogix.Metabase.Options
{
    public class DisconnectOptions : ActionOptions
    {
        private Dictionary<string, string> m_saveFileMappings = new Dictionary<string, string>();

        public Dictionary<string, string> SaveFileMappings
        {
            get { return this.m_saveFileMappings; }
        }

        public DisconnectOptions()
        {
        }
    }
}