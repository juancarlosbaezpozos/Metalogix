using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.Data.CSV
{
    public class CsvDocument
    {
        private List<string> headers;

        private List<CsvRow> rows;

        public string[] Headers
        {
            get { return this.headers.ToArray(); }
        }

        public bool IgnoreDuplicateHeaders { get; set; }

        public CsvRow[] Rows
        {
            get { return this.rows.ToArray(); }
        }

        public CsvDocument()
        {
            this.headers = new List<string>();
            this.rows = new List<CsvRow>();
        }

        public void AddRow(params string[] sValues)
        {
            if (sValues == null)
            {
                throw new ArgumentNullException();
            }

            if ((int)sValues.Length > this.headers.Count)
            {
                throw new RowItemsExceedException();
            }

            CsvRow csvRow = new CsvRow(sValues);
            if ((int)sValues.Length < this.headers.Count)
            {
                csvRow.SetRowSize(this.headers.Count);
            }

            this.rows.Add(csvRow);
        }

        public void AddRow(CsvRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException();
            }

            if ((int)row.Values.Length > this.headers.Count)
            {
                throw new RowItemsExceedException();
            }

            if ((int)row.Values.Length < this.headers.Count)
            {
                row.SetRowSize(this.headers.Count);
            }

            this.rows.Add(row);
        }

        public bool AppendColumn(string sNewColumn)
        {
            if (string.IsNullOrEmpty(sNewColumn))
            {
                throw new ArgumentNullException();
            }

            if (!this.headers.Contains(sNewColumn))
            {
                this.headers.Add(sNewColumn);
                return true;
            }

            if (!this.IgnoreDuplicateHeaders)
            {
                throw new DuplicateHeaderException(sNewColumn);
            }

            return false;
        }

        public void AppendColumns(params string[] sNewColumns)
        {
            if (sNewColumns == null)
            {
                throw new ArgumentNullException();
            }

            string[] strArrays = sNewColumns;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                this.AppendColumn(strArrays[i]);
            }
        }

        public bool ContainsHeader(string sHeader)
        {
            if (string.IsNullOrEmpty(sHeader))
            {
                throw new ArgumentNullException();
            }

            return this.headers.Contains(sHeader);
        }

        private string EscapeStringToCSV(string value)
        {
            string str = value ?? "";
            if (str.Contains(",") || str.Contains("\n") || str.Contains("\t") || str.Contains("\""))
            {
                if (str.Contains("\""))
                {
                    string[] strArrays = str.Split(new char[] { '\"' });
                    str = strArrays[0];
                    for (int i = 1; i < (int)strArrays.Length; i++)
                    {
                        str = string.Concat(str, "\"\"", strArrays[i]);
                    }
                }

                str = string.Concat("\"", str, "\"");
                if (str.Contains("\r\n"))
                {
                    string[] strArrays1 = str.Split(new char[] { '\r' });
                    str = "";
                    string[] strArrays2 = strArrays1;
                    for (int j = 0; j < (int)strArrays2.Length; j++)
                    {
                        str = string.Concat(str, strArrays2[j]);
                    }
                }
            }

            return str;
        }

        public string FormatHeaders(string header)
        {
            string i = header;
            int num = 1;
            if (!string.IsNullOrEmpty(i))
            {
                while (this.headers.Contains(i))
                {
                    i = string.Concat(header, "_", num);
                    num++;
                }
            }
            else
            {
                for (i = string.Concat("Header_", num); this.headers.Contains(i); i = string.Concat("Header_", num))
                {
                    num++;
                }
            }

            this.headers.Add(i);
            return i;
        }

        private string GetDelimiter(string content)
        {
            char[] chrArray = new char[] { '\t', ',' };
            int num = content.IndexOfAny(chrArray);
            return ((num != -1 ? content[num] : ',')).ToString();
        }

        public int GetHeaderCount()
        {
            return this.headers.Count;
        }

        public int GetIndexOfHeader(string sHeader)
        {
            if (string.IsNullOrEmpty(sHeader))
            {
                throw new ArgumentNullException();
            }

            return this.headers.IndexOf(sHeader);
        }

        public int GetRowCount()
        {
            return this.rows.Count;
        }

        public void InsertColumn(int iColIndex, string sNewColumn)
        {
            if (string.IsNullOrEmpty(sNewColumn))
            {
                throw new ArgumentNullException();
            }

            if (this.headers.Contains(sNewColumn))
            {
                throw new DuplicateHeaderException(sNewColumn);
            }

            this.headers.Insert(iColIndex, sNewColumn);
        }

        public void Load(string sFilePath)
        {
            if (sFilePath == null)
            {
                throw new ArgumentNullException();
            }

            this.headers.Clear();
            this.rows.Clear();
            using (StreamReader streamReader = new StreamReader(sFilePath))
            {
                using (TextFieldParser textFieldParser = new TextFieldParser(streamReader))
                {
                    string str = textFieldParser.PeekChars(500);
                    string[] delimiter = new string[] { this.GetDelimiter(str) };
                    textFieldParser.Delimiters = delimiter;
                    string[] strArrays = textFieldParser.ReadFields();
                    string[] strArrays1 = strArrays;
                    if (strArrays != null)
                    {
                        this.LoadHeaders(strArrays1);
                    }

                    while (true)
                    {
                        string[] strArrays2 = textFieldParser.ReadFields();
                        strArrays1 = strArrays2;
                        if (strArrays2 == null)
                        {
                            break;
                        }

                        int length = (int)strArrays1.Length - this.headers.Count;
                        if (length > 0)
                        {
                            List<string> strs = new List<string>();
                            for (int i = 0; i < length; i++)
                            {
                                strs.Add("");
                            }

                            this.LoadHeaders(strs.ToArray());
                        }

                        this.AddRow(strArrays1);
                    }
                }
            }
        }

        private void LoadHeaders(string[] sHeaders)
        {
            string[] strArrays = sHeaders;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                this.FormatHeaders(strArrays[i]);
            }
        }

        public void RemoveColumn(int iColIndex)
        {
            this.headers.RemoveAt(iColIndex);
        }

        public void RemoveRow(CsvRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException();
            }

            if (!this.rows.Remove(row))
            {
                throw new RowNotFoundException();
            }
        }

        public void RenameColumn(int iColIndex, string sNewColumn)
        {
            if (string.IsNullOrEmpty(sNewColumn))
            {
                throw new ArgumentNullException();
            }

            if (iColIndex < 0 || iColIndex > this.headers.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (this.headers.Contains(sNewColumn))
            {
                throw new DuplicateHeaderException(sNewColumn);
            }

            this.headers[iColIndex] = sNewColumn;
        }

        public void SaveToFile(string sFilePath, CsvDelimiterType delimiterType)
        {
            if (sFilePath == null)
            {
                throw new ArgumentNullException();
            }

            using (StreamWriter streamWriter = new StreamWriter(sFilePath, false))
            {
                char hashCode = (char)delimiterType.GetHashCode();
                string str = "";
                string[] headers = this.Headers;
                for (int i = 0; i < (int)headers.Length; i++)
                {
                    string str1 = headers[i];
                    str = string.Concat(str, this.EscapeStringToCSV(str1), hashCode);
                }

                streamWriter.WriteLine(str.Remove(str.Length - 1));
                str = "";
                CsvRow[] rows = this.Rows;
                for (int j = 0; j < (int)rows.Length; j++)
                {
                    string[] values = rows[j].Values;
                    for (int k = 0; k < (int)values.Length; k++)
                    {
                        string str2 = values[k];
                        str = string.Concat(str, this.EscapeStringToCSV(str2), hashCode);
                    }

                    streamWriter.WriteLine(str.Remove(str.Length - 1));
                    str = "";
                }
            }
        }
    }
}