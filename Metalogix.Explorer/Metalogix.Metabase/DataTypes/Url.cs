using System;
using System.ComponentModel;

namespace Metalogix.Metabase.DataTypes
{
    [TypeConverter(typeof(UrlTypeConverter))]
    public class Url : IComparable, IFormattable, IUrl, IMetabaseDataType
    {
        private System.Uri m_uri;

        private string m_strAbsoluteUrl;

        [Browsable(true)]
        [Description("The AbsolutePath portion of the URL.")]
        [DisplayName("Absolute Path")]
        public string AbsolutePath
        {
            get
            {
                if (this.Uri == null)
                {
                    return null;
                }

                return this.Uri.AbsolutePath;
            }
        }

        [Browsable(true)]
        [Description("The absolute path of the Url.")]
        [DisplayName("Absolute Url")]
        public string AbsoluteUrl
        {
            get { return this.m_strAbsoluteUrl; }
        }

        [Browsable(true)]
        [Description("The file name portion of the URL.")]
        [DisplayName("FileName")]
        public string FileName
        {
            get
            {
                string str;
                try
                {
                    char[] chrArray = new char[] { '/', '\\' };
                    int num = this.m_strAbsoluteUrl.LastIndexOf("?");
                    int num1 = (num < 0 ? this.m_strAbsoluteUrl.Length : num);
                    int num2 = this.m_strAbsoluteUrl.Substring(0, num1).LastIndexOfAny(chrArray);
                    if (num2 >= 0)
                    {
                        int num3 = num2 + 1;
                        str = this.m_strAbsoluteUrl.Substring(num3, num1 - num3);
                    }
                    else
                    {
                        str = null;
                    }
                }
                catch (Exception exception)
                {
                    str = null;
                }

                return str;
            }
        }

        [Browsable(true)]
        [Description("The Domain name, or IP address portion of the Uri.")]
        [DisplayName("Host")]
        public string Host
        {
            get
            {
                if (this.Uri == null)
                {
                    return null;
                }

                return this.Uri.Host;
            }
        }

        [Browsable(true)]
        [Description("The Host And Path portion of the URL.")]
        [DisplayName("Host And Path")]
        public string HostAndPath
        {
            get
            {
                string str;
                try
                {
                    string str1 = this.m_strAbsoluteUrl.Replace("\\", "/");
                    char[] chrArray = new char[] { '/' };
                    int num = str1.IndexOfAny(chrArray);
                    if (num >= 0)
                    {
                        int length = str1.LastIndexOfAny(chrArray);
                        if (length >= 0)
                        {
                            if (length == num || length == num + 1)
                            {
                                length = str1.Length;
                            }

                            string str2 = str1.Substring(num, length - num);
                            str = str2.Trim(chrArray);
                        }
                        else
                        {
                            str = null;
                        }
                    }
                    else
                    {
                        str = null;
                    }
                }
                catch
                {
                    str = null;
                }

                return str;
            }
        }

        [Browsable(true)]
        [Description("The Path and Query portion of the URL.")]
        [DisplayName("Path and Query")]
        public string PathAndQuery
        {
            get
            {
                if (this.Uri == null)
                {
                    return null;
                }

                return this.Uri.PathAndQuery;
            }
        }

        [Browsable(true)]
        [Description("The Query portion of the URL.")]
        [DisplayName("Query")]
        public string Query
        {
            get
            {
                if (this.Uri == null)
                {
                    return null;
                }

                return this.Uri.Query;
            }
        }

        [Browsable(true)]
        [Description("The Scheme of the URL.")]
        [DisplayName("Scheme")]
        public string Scheme
        {
            get
            {
                if (this.Uri == null)
                {
                    return null;
                }

                return this.Uri.Scheme;
            }
        }

        private System.Uri Uri
        {
            get
            {
                if (string.IsNullOrEmpty(this.m_strAbsoluteUrl))
                {
                    return null;
                }

                if (this.m_uri == null)
                {
                    this.m_uri = new System.Uri(this.m_strAbsoluteUrl);
                }

                return this.m_uri;
            }
        }

        public Url(string strUrl)
        {
            this.m_strAbsoluteUrl = strUrl;
        }

        public int CompareTo(object obj)
        {
            if (this.AbsoluteUrl == null)
            {
                return -1;
            }

            return this.AbsoluteUrl.CompareTo(((Url)obj).AbsoluteUrl);
        }

        public override bool Equals(object obj)
        {
            Url url = obj as Url;
            if (url == null)
            {
                return false;
            }

            return this.m_strAbsoluteUrl == url.m_strAbsoluteUrl;
        }

        public override int GetHashCode()
        {
            return this.m_strAbsoluteUrl.GetHashCode() + 1;
        }

        public static bool IsUrl(string sUrl)
        {
            bool flag;
            try
            {
                System.Uri uri = new System.Uri(sUrl);
                flag = true;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        string System.IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            if (this.m_strAbsoluteUrl == null)
            {
                return null;
            }

            if (format == "HostAndPath")
            {
                return this.HostAndPath;
            }

            return this.m_strAbsoluteUrl;
        }

        public override string ToString()
        {
            return this.AbsoluteUrl;
        }
    }
}