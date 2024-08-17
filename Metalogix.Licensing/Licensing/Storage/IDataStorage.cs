using System;

namespace Metalogix.Licensing.Storage
{
    public interface IDataStorage : IDisposable
    {
        bool Exists { get; }

        string GetValue(string valueName);

        void Load();

        void Save();

        void SetValue(string name, string value);
    }
}