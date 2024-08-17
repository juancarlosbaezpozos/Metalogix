using System;
using System.Collections.Generic;

namespace Metalogix.Core.ConfigVariables
{
    public interface IConfigDatabaseAdapter
    {
        string ConnectionString { get; set; }

        void AddVariable(string scope, string name, string value);

        void ClearVariables(string scope);

        bool DeleteVariable(string scope, string name);

        IEnumerable<KeyValuePair<string, string>> GetVariables(string scope);

        void InitializeAdapter();

        bool UpdateVariable(string scope, string name, string value);
    }
}