using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Data.CSV
{
    public class CsvRow
    {
        private List<string> lstValues;

        public string this[int iColumn]
        {
            get { return this.lstValues[iColumn]; }
            set { this.lstValues[iColumn] = value; }
        }

        public string[] Values
        {
            get { return this.lstValues.ToArray(); }
        }

        public CsvRow(int iRowSize)
        {
            this.lstValues = new List<string>();
            for (int i = 0; i < iRowSize; i++)
            {
                this.lstValues.Add(null);
            }
        }

        public CsvRow(params string[] sValues)
        {
            if (sValues == null)
            {
                throw new ArgumentNullException();
            }

            this.lstValues = new List<string>();
            string[] strArrays = sValues;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                this.lstValues.Add(str);
            }
        }

        public void RemoveItem(int index)
        {
            this.lstValues.RemoveAt(index);
        }

        public void SetRowSize(int iSize)
        {
            if (iSize > this.lstValues.Count)
            {
                this.lstValues.AddRange(new string[iSize - this.lstValues.Count]);
                return;
            }

            if (iSize < this.lstValues.Count)
            {
                this.lstValues.RemoveRange(iSize, this.lstValues.Count);
            }
        }
    }
}