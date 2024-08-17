using Metalogix.Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Metalogix.Office365
{
    public class AzureGraphAPIHelper
    {
        private static string token_type;

        private static string access_token;

        public AzureGraphAPIHelper()
        {
        }

        private static void LoginToAzure(string appClientId, string appSecret, string tenantURL)
        {
            HttpWebRequest length =
                (HttpWebRequest)WebRequest.Create(
                    string.Format("https://login.windows.net/{0}/oauth2/token", tenantURL));
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            string str = string.Format("grant_type=client_credentials&resource={0}&client_id={1}&client_secret={2}",
                HttpUtility.UrlEncode("https://graph.windows.net"), HttpUtility.UrlEncode(appClientId),
                HttpUtility.UrlEncode(appSecret));
            byte[] bytes = aSCIIEncoding.GetBytes(str);
            length.Method = "POST";
            length.ContentType = "application/x-www-form-urlencoded";
            length.ContentLength = (long)((int)bytes.Length);
            using (Stream requestStream = length.GetRequestStream())
            {
                requestStream.Write(bytes, 0, (int)bytes.Length);
            }

            using (WebResponse response = length.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        string end = streamReader.ReadToEnd();
                        Dictionary<string, string> strs =
                            (new JavaScriptSerializer()).Deserialize<Dictionary<string, string>>(end);
                        AzureGraphAPIHelper.token_type = strs["token_type"];
                        AzureGraphAPIHelper.access_token = strs["access_token"];
                    }
                }
            }
        }

        public static ADGraphUserResponse ValidateADUser(string userLoginName, string appClientID, string appSecret,
            string tenantURL)
        {
            ADGraphUserResponse aDGraphUserResponse = new ADGraphUserResponse();
            try
            {
                AzureGraphAPIHelper.LoginToAzure(appClientID, appSecret, tenantURL);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(
                    string.Format("https://graph.windows.net/{0}/users/{1}?api-version=1.5", tenantURL, userLoginName));
                WebHeaderCollection headers = httpWebRequest.Headers;
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                object[] tokenType = new object[] { AzureGraphAPIHelper.token_type, AzureGraphAPIHelper.access_token };
                headers.Add("Authorization", string.Format(invariantCulture, "{0} {1}", tokenType));
                httpWebRequest.Method = "GET";
                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            string end = streamReader.ReadToEnd();
                            aDGraphUserResponse.Success = true;
                            aDGraphUserResponse.Details = end;
                        }
                    }
                }
            }
            catch (WebException webException1)
            {
                WebException webException = webException1;
                aDGraphUserResponse.Success = false;
                HttpWebResponse httpWebResponse = (HttpWebResponse)webException.Response;
                if (httpWebResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    aDGraphUserResponse.UserNotFoundStatus = true;
                    aDGraphUserResponse.Details = string.Format("User Not Found. Error: {0}{1}{2}",
                        webException.Message, Environment.NewLine, webException.StackTrace);
                }
                else if (httpWebResponse.StatusCode != HttpStatusCode.BadRequest)
                {
                    aDGraphUserResponse.Details = string.Format("{0}{1}{2}", webException.Message, Environment.NewLine,
                        webException.StackTrace);
                }
                else
                {
                    aDGraphUserResponse.BadRequestStatus = true;
                    aDGraphUserResponse.Details = string.Format("Bad Request. Error: {0}{1}{2}", webException.Message,
                        Environment.NewLine, webException.StackTrace);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                aDGraphUserResponse.Success = false;
                aDGraphUserResponse.Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine,
                    exception.StackTrace);
            }

            return aDGraphUserResponse;
        }
    }
}