using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Tools.pages;

namespace Tools.utils
{

    public class HttpServer
    {

        private string url;

        public HttpServer(string url)
        {
            this.url = url;
        }

        public void start()
        {

            new Thread(delegate () 
            {
                using (HttpListener listerner = new HttpListener())
                {
                    if (!listerner.IsListening)
                    {
                        listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous; //指定身份验证 Anonymous匿名访问
                        listerner.Prefixes.Add(url);
                        listerner.Start();

                        Console.WriteLine("WebServer Start Successed.......");
                        while (true)
                        {
                            //等待请求连接
                            //没有请求则GetContext处于阻塞状态
                            HttpListenerContext request = listerner.GetContext();

                            //获取客户端写入的信息
                            string strRequest = ShowRequestData(request.Request);

                            Thread threadsub = new Thread(new ParameterizedThreadStart((requestcontext) =>
                            {
                                //Response
                                request.Response.StatusCode = 200;
                                request.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                                request.Response.ContentType = "*/*";
                                request.Response.ContentEncoding = Encoding.UTF8;
                                string msg = "";
                                if (!strRequest.Trim().Equals(""))
                                {
                                    msg = "提交成功";
                                }
                                else
                                {
                                    msg = "请求消息拿不到";
                                }
                                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = "true", msg = msg }));
                                request.Response.ContentLength64 = buffer.Length;
                                var output = request.Response.OutputStream;
                                output.Write(buffer, 0, buffer.Length);
                                output.Close();
                            }));
                            threadsub.Start(request);
                        }
                    }
                }
            }).Start();
        }

        public string ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return "";
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);
            Console.WriteLine("Start of client data:");
            // Convert the data to a string and display it on the console.
            string s = Encoding.GetEncoding("utf-8").GetString(request.ContentEncoding.GetBytes(reader.ReadToEnd()));
            Console.WriteLine(s);
            Console.WriteLine("End of client data:");
            body.Close();
            reader.Close();
            // If you are finished with the request, it should be closed also.
            return s;
        }
    }
}
