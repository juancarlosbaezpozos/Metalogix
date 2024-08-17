using System;

namespace Metalogix.SharePoint.Adapters
{
    public struct InternalSYSTEM_INFO
    {
        private Architecture wProcessorArchitecture;

        private short wReserved;

        public Architecture ProcessorArchitecture
        {
            get { return this.wProcessorArchitecture; }
            set { this.wProcessorArchitecture = value; }
        }

        public short Reserved
        {
            get { return this.wReserved; }
            set { this.wReserved = value; }
        }
    }
}