using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tools.utils
{
    public class RequestClient
    {
        public static HttpClientHandler handler = new HttpClientHandler() { UseCookies = true };
        #region Field
        public static readonly HttpClient httpClient = new HttpClient(handler);// { Timeout = new TimeSpan(5000), } 
        #endregion
        
        public static void setHeader()
        {
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
            httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
        }

        /// <summary>
        /// http post 异步请求
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="jsonObject">请求参数 json</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string jsonObject)
        {
            try
            {
                setHeader();
                Console.WriteLine("发送请求：" + url);
                Console.WriteLine("请求参数：" + jsonObject);
                //httpClient.Timeout = new TimeSpan(5000);
                var ret = httpClient.PostAsync(url, new StringContent(jsonObject, Encoding.UTF8, "application/json"));
                ret.Wait();
                if (!ret.IsFaulted)
                {
                    Console.WriteLine("请求状态码：200");
                    return ret.Result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (SocketException e)
            {
                //throw new Exception(e.Message);
                Console.WriteLine("1=="+e.Message);
                Console.WriteLine("1--"+e.InnerException);
            }
            catch (Exception e)
            {
                Console.WriteLine("2==" + e.Message);
                Console.WriteLine("2--" + e.InnerException);
            }
            Console.WriteLine("请求ERROR");
            return "请求ERROR";
        }

        /// <summary>
        /// http post 异步请求
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="param">请求参数 表单</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, Dictionary<string, string> param)
        {
            try
            {
                setHeader();
                Console.WriteLine("发送请求：" + url);
                Console.WriteLine("请求参数：" + param);
                var ret = httpClient.PostAsync(url, new FormUrlEncodedContent(param));
                ret.Wait();
                if (!ret.IsFaulted)
                {
                    Console.WriteLine("请求状态码：200");
                    return ret.Result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("请求ERROR");
            return null;
        }

        public static async Task<string> HttpPostAsyncMultipartFormData(string url, Dictionary<string, string> param)
        {
            try
            {
                setHeader();
                Console.WriteLine("发送请求：" + url);
                Console.WriteLine("请求参数：" + param);
                String file = "";
                string filePareamName = "";
                var content = new MultipartFormDataContent();
                //添加字符串参数，参数名为qq
                foreach (KeyValuePair<string, string> kvp in param)
                {
                    Console.WriteLine(kvp.Key.Split('&').Length);
                    if (kvp.Key.Split('&').Length > 1 && kvp.Key.Split('&')[1] != null)
                    {
                        filePareamName = kvp.Key.Split('&')[1];
                        file = kvp.Value;
                    }
                    else
                    {
                        content.Add(new StringContent(kvp.Value), kvp.Key);
                    }
                }
                string filename = Path.GetFileNameWithoutExtension(file);
                string extension = Path.GetExtension(file);
                string path = Path.Combine(System.Environment.CurrentDirectory, file);
                //添加文件参数，参数名为files，文件名为123.png
                content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(path)), filePareamName, filename + extension);
                var ret = httpClient.PostAsync(url, content);
                ret.Wait();
                if (!ret.IsFaulted)
                {
                    Console.WriteLine("请求状态码：200");
                    return ret.Result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("请求ERROR");
            return null;
        }

        public static async Task<string> HttpGetAsync(string url)
        {
            try
            {
                setHeader();
                Console.WriteLine("发送请求：" + url);
                var ret = httpClient.GetByteArrayAsync(url);
                ret.Wait();
                if (!ret.IsFaulted)
                {
                    Console.WriteLine("请求状态码：200");
                    byte[] bresult = ret.Result;
                    return System.Text.Encoding.Default.GetString(bresult);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("请求ERROR");
            return null;
        }

        public static async Task<string> HttpDeleteAsync(string url)
        {
            try
            {
                setHeader();
                Console.WriteLine("发送请求：" + url);
                var ret = httpClient.DeleteAsync(url);
                ret.Wait();
                if (!ret.IsFaulted)
                {
                    Console.WriteLine("请求状态码：200");
                    HttpResponseMessage response = ret.Result;
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("请求ERROR");
            return null;
        }
        

    }
}
