using System;

namespace Microsoft.XmlDiffPatch
{
    internal class HashAlgorithm
    {
        private ulong _hash;

        internal ulong Hash
        {
            get { return this._hash; }
        }

        internal HashAlgorithm()
        {
        }

        internal void AddInt(int i)
        {
            this._hash += (this._hash << 11) + (ulong)((long)i);
        }

        internal void AddString(string data)
        {
            this._hash = HashAlgorithm.GetHash(data, this._hash);
        }

        internal void AddULong(ulong u)
        {
            HashAlgorithm hashAlgorithm = this;
            hashAlgorithm._hash = hashAlgorithm._hash + (this._hash << 11) + u;
        }

        internal static ulong GetHash(string data)
        {
            return HashAlgorithm.GetHash(data, (ulong)0);
        }

        private static ulong GetHash(string data, ulong hash)
        {
            hash += (hash << 13) + (ulong)((long)data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                hash += (hash << 17) + (ulong)data[i];
            }

            return hash;
        }
    }
}