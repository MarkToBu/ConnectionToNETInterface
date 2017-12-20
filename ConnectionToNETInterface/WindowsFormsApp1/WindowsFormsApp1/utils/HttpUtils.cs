using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;


namespace WindowsFormsApp1
{
    class HttpUtils
    {
        public static string OpenReadWithHttps(string URL, string strPostdata, string strEncoding)
        {
            Encoding encoding = Encoding.Default;
            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }
        //public static string GetWebRequest(string getUrl)
        //{
        //    string responseContent = "";

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
        //    request.ContentType = "application/json";
        //    request.Method = "GET";

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    //在这里对接收到的页面内容进行处理
        //    using (Stream resStream = response.GetResponseStream())
        //    {
        //        using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
        //        {
        //            responseContent = reader.ReadToEnd().ToString();
        //        }
        //    }
        //    return responseContent;
        //}
    }
}
