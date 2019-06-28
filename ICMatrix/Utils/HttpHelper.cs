using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ICMatrix.Utils
{
    public class HttpHelper
    {
        //Helper functions
        public static string GetData(string uri)
        {
            HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return GetResponseData(response);
            }
        }
        public static async Task<string> GetDataAsync(string uri)
        {
            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public static string PostData(string uri, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = "text/json;charset=utf-8";
            request.Method = "POST";

            using (Stream requestData = request.GetRequestStream())
            {
                requestData.Write(dataBytes, 0, dataBytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return GetResponseData(response);
            }
        }
        private static string GetResponseData(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                string contentData = sr.ReadToEnd();
                return contentData;
            }
        }
    }
}
