using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Tools.BeanModel;
using Tools.control;
using Tools.utils;
using Tools.ViewModel;

namespace Tools.pages
{
    /// <summary>
    /// SimulationView.xaml 的交互逻辑
    /// </summary>
    public partial class SimulationView : Page
    {
       

        public SimulationView()
        {
            InitializeComponent();
            DataContext = new SimulationViewModel();
            
        }

        private void back_for_main(object sender, RoutedEventArgs e)
        {
            var window = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
            //var view = "/pages/Index.xaml";
            window.PageUrl = "pack://application:,,,/pages/Index.xaml";
        }

        private void openSimulationPage(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                var productId = textBlock.Tag;
                Pid.Text = productId.ToString();
                if (File.Exists(@"Resources\json\simulationProduct\" + productId + ".json")) 
                {
                    var json = File.ReadAllText(@"Resources\json\simulationProduct\" + productId + ".json");
                    SimulationProductInfo simulationProductInfo = JsonConvert.DeserializeObject<SimulationProductInfo>(json);

                    ProductTitle.Text = simulationProductInfo.title;
                    ProductAnnotation.Text = simulationProductInfo.annotation;
                    requestType.SelectedIndex = simulationProductInfo.method;
                    samplenoSource.SelectedIndex = simulationProductInfo.protocol;
                    ipAddress.Text = simulationProductInfo.ip;
                    port.Text = simulationProductInfo.port.ToString();
                    uri.Text = simulationProductInfo.uri;
                    contentType.SelectedIndex = simulationProductInfo.contentType;


                    //jsonContext.Document = simulationProductInfo.jsonContext;
                    //responseJsonContext1.Document = simulationProductInfo.responseJsonContext1;
                    //responseJsonContext2.Document = simulationProductInfo.responseJsonContext2;
                    setContext(jsonContext, simulationProductInfo.jsonContext);
                    setContext(responseJsonContext1, simulationProductInfo.responseJsonContext1);
                    setContext(responseJsonContext2, simulationProductInfo.responseJsonContext2);
                }
                else
                {
                    MessageBox.Show("空");
                }
                
            }
        }
        

        private void send(object sender, RoutedEventArgs e)
        {
            string method = requestType.SelectedValue.ToString().Split(':')[1].Trim();
            string protocol = samplenoSource.SelectedValue.ToString().Split(':')[1].Trim();
            string url = protocol + "://" + ipAddress.Text.Trim() + ":" + port.Text.Trim() + uri.Text.Trim();
            TextRange textRange = new TextRange(jsonContext.Document.ContentStart, jsonContext.Document.ContentEnd);
            Task<string> result = null;
            if (textRange.Text.Trim().Length > 0) 
            {
                if (method.Trim().Equals("POST"))
                {
                    //json提交
                    if (contentType.SelectedValue.ToString().Split(':')[1].Trim().Equals("application/json"))
                    {
                        result = RequestClient.HttpPostAsync(url, textRange.Text.Trim());
                    }
                    //表单提交
                    if (contentType.SelectedValue.ToString().Split(':')[1].Trim().Equals("application/x-www-form-urlencoded"))
                    {
                        string[] data = StringUtils.formatTransformStr(textRange.Text).Trim(new char[] { '{', '}' }).Split(',');
                        Dictionary<string, string> param = data.ToDictionary(s => Regex.Split(s, "\":\"", RegexOptions.IgnoreCase)[0].Trim('"'), s => Regex.Split(s, "\":\"", RegexOptions.IgnoreCase)[1].Trim('"'));
                        result = RequestClient.HttpPostAsync(url, param);
                    }
                    //文件表单提交
                    if (contentType.SelectedValue.ToString().Split(':')[1].Trim().Equals("multipart/form-data"))
                    {
                        /* 我的第一个 C# 程序 */
                        string[] data = StringUtils.formatTransformStr(textRange.Text).Trim(new char[] { '{', '}' }).Split(',');
                        Dictionary<string, string> param = data.ToDictionary(p => Regex.Split(p, "\":\"", RegexOptions.IgnoreCase)[0].Trim('"'), p => Regex.Split(p, "\":\"", RegexOptions.IgnoreCase)[1].Trim('"'));
                        result = RequestClient.HttpPostAsyncMultipartFormData(url, param);
                    }

                }
                if (method.Trim().Equals("GET"))
                {
                    result = RequestClient.HttpGetAsync(url);
                }
                if (method.Trim().Equals("DELETE"))
                {
                    result = RequestClient.HttpDeleteAsync(url);
                }
                if (method.Trim().Equals("PUT"))
                {
                    MessageBox.Show("PUT 没写");
                }

                if (result != null)
                {
                    Console.WriteLine("返回结果集：" + result.Result);
                    FlowDocument doc = new FlowDocument();
                    doc = writeDocuments(doc, result.Result, "{");
                    responseJsonContext1.Document = doc;
                    //MessageBox.Show(result.Result);
                }
            }
            else
            {
                MessageBox.Show("发送数据不能为空");
            }

        }

        private void imosSend(object sender, RoutedEventArgs e)
        {
            string method = requestType.SelectedValue.ToString().Split(':')[1].Trim();
            string protocol = samplenoSource.SelectedValue.ToString().Split(':')[1].Trim();
            string url = protocol + "://" + ipAddress.Text.Trim() + ":" + port.Text.Trim() + uri.Text.Trim();
            TextRange textRange = new TextRange(jsonContext.Document.ContentStart, jsonContext.Document.ContentEnd);
            //第一次登录
            Task<string> result = RequestClient.HttpPostAsync(url, "");
            if (result != null)
            {

                Console.WriteLine(StringUtils.formatTransformStr(textRange.Text));
                JObject acount = (JObject)JsonConvert.DeserializeObject(StringUtils.formatTransformStr(textRange.Text));
                Console.WriteLine(result.Result);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result.Result);
                Console.WriteLine(jo["AccessCode"].ToString());
                StringUtils.GenerateMD5(StringUtils.EncodeBase64(acount["username"].ToString()) + jo["AccessCode"].ToString() + StringUtils.GenerateMD5(acount["password"].ToString()));
                string param = "{\"UserName\":\""+ acount["username"].ToString() + "\",\"AccessCode\":\""+ jo["AccessCode"].ToString() + "\",\"LoginSignature\":\"4f141e6dd693cf83329027fbd8069c5c\"}";
                Console.WriteLine(param);
                //第二次登录

                result = RequestClient.HttpPostAsync(url, param);
                JObject resultJson = (JObject)JsonConvert.DeserializeObject(result.Result);
                string accessToken = resultJson["AccessToken"].ToString();
                RequestClient.httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);

                Console.WriteLine("返回结果集：" + result.Result);
                FlowDocument doc = new FlowDocument();
                doc = writeDocuments(doc, result.Result, "{");
                responseJsonContext1.Document = doc;
                //MessageBox.Show(result.Result);
            }
        }

        public FlowDocument writeDocuments(FlowDocument doc, string str, string split)
        {
            //{"Id":"admin","Pwd":"212","data":{"d1":"d1","d2":"d2"}}
            string nbsp = "    ";
            string[] run = str.Split(new string[] { split }, StringSplitOptions.None);
            for(int i = 0; i < run.Length; i++)
            {
                Paragraph p = null;
                Run r = null;
                if (run[i].Equals(""))
                {
                    p = new Paragraph();
                    r = new Run("{");
                    p.Inlines.Add(r);
                    doc.Blocks.Add(p);
                }
                p = new Paragraph();
                r = new Run(nbsp + run[i]);
                p.Inlines.Add(r);
                doc.Blocks.Add(p);
            }
            return doc;
        }

        private void regMsgConsumer(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(jsonContext.Document.ContentStart, jsonContext.Document.ContentEnd);
            if (!textRange.Text.Trim().Equals(""))
            {
                Consumer consumer = Newtonsoft.Json.JsonConvert.DeserializeObject<Consumer>(textRange.Text.Trim());
                string url = "http://" + InternetUtils.GetAddressIP() + ":" + consumer.port + consumer.uri;
                Console.WriteLine(url);
                Console.WriteLine(url.Substring(url.Length - 1));
                if (!url.Substring(url.Length - 1).Equals("/"))
                {
                    url = url + "/";
                }
                HttpServer server = new HttpServer(url);
                server.start();
            }
        }

        private void setContext(RichTextBox context, string text) 
        {
            //string filename = @"Resources\json\simulationProduct\temp.xaml";
            if (text != null && !text.Trim().Equals(""))
            {
                //FileStream stream = File.OpenRead(filename);
                //FlowDocument doc = StringUtils.DeserializeToObject<FlowDocument>(text);
                TextRange documentTextRange = new TextRange(context.Document.ContentStart, context.Document.ContentEnd);
                byte[] array = Encoding.UTF8.GetBytes(text);
                MemoryStream stream = new MemoryStream(array);             //convert stream 2 string      
                documentTextRange.Load(stream, DataFormats.Xaml);
                //context.Document = doc;
            }
            else
            {
                context.Document = new FlowDocument();
            }
        }

        public string getContext(RichTextBox context, string filename)
        {
            //string filename = @"Resources\json\simulationProduct\temp.xaml";
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException();
            }
            FileStream stream = new FileStream(filename, FileMode.Truncate, FileAccess.ReadWrite);//清空文件内容
            stream.Close();
            using (stream = File.OpenWrite(filename))
            {
                TextRange documentTextRange = new TextRange(context.Document.ContentStart, context.Document.ContentEnd);
                documentTextRange.Save(stream, DataFormats.Xaml);
                stream.Close();
                return System.IO.File.ReadAllText(filename);
            }
                
        }

        private void save(object sender, RoutedEventArgs e)
        {
            if (!Pid.Text.Equals("0"))
            {
                SimulationProductInfo info = new SimulationProductInfo();
                info.title = ProductTitle.Text;
                info.annotation = ProductAnnotation.Text;
                info.method = requestType.SelectedIndex;
                info.protocol = samplenoSource.SelectedIndex;
                info.ip = ipAddress.Text;
                info.port = int.Parse(port.Text);
                info.uri = uri.Text;
                info.contentType = contentType.SelectedIndex;
                info.jsonContext = getContext(jsonContext, @"Resources\json\simulationProduct\jsonContext.xaml");
                info.responseJsonContext2 = getContext(responseJsonContext2, @"Resources\json\simulationProduct\responseJsonContext2.xaml");
                string json = JsonConvert.SerializeObject(info);
                FileUtils.writeJsonFile(@"Resources\json\simulationProduct\" + Pid.Text + ".json", json);
                MessageBox.Show("保存");
            }

        }



        private void txtContect_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// 鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContect_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //timer1.Enabled = true;
            //MessageBox.Show("0");
        }

        private int RowLineNum = 1;


        /// <summary>
        /// 上、下键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                SetScrollBar();
            if (e.Key == Key.Enter)
            {
                FlowDocument doc = rowLineNum.Document;
                Paragraph p = new Paragraph();  // Paragraph 类似于 html 的 P 标签
                RowLineNum += 1;
                Run r = new Run(RowLineNum.ToString());      // Run 是一个 Inline 的标签
                p.Inlines.Add(r);
                doc.Blocks.Add(p);
                rowLineNum.Document = doc;
            }
            if (e.Key == Key.Back)
            {
                rowLineNum.Document.Blocks.Remove(rowLineNum.Document.Blocks.LastBlock);
                if (RowLineNum > 1) 
                {
                    RowLineNum -= 1;
                }
                else
                {
                    FlowDocument doc = new FlowDocument();
                    Paragraph p = new Paragraph();
                    Run r = new Run(RowLineNum.ToString());
                    p.Inlines.Add(r);
                    doc.Blocks.Add(p);
                    rowLineNum.Document = doc;
                }
            }
            //MessageBox.Show("上、下键按下");
        }

        /// <summary>
        /// 上、下键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContent_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                ShowCursorLine();
                //MessageBox.Show("上、下键抬起");
        }

        /// <summary>
        /// 点击滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vScrollBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MessageBox.Show("点击滚动条");
            //int t = SetScrollPos(this.txtContent.Handle, 1, vScrollBar1.Value, true);
            //SendMessage(this.txtContent.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * vScrollBar1.Value, 0);
            //ShowRow();
        }

        /// <summary>
        /// 显示光标行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("显示光标行");
            //if (e.Button == System.Windows.Forms.MouseButtons.Left) 
            //    isLeftDown = true;
            ShowCursorLine();
        }

        /// <summary>
        /// 鼠标选择内容上下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtContent_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("鼠标选择内容上下移动");
            SetScrollBar();
        }

        private void txtContent_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //isLeftDown = false;
        }

        /// <summary>
        /// 行显示栏宽度自适应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (this.txtRow.Lines.Length > 0)
            //{
            //    System.Drawing.SizeF s = this.txtRow.CreateGraphics().MeasureString(this.txtRow.Lines[this.txtRow.Lines.Length - 1], this.txtRow.Font);
            //    this.txtRow.Width = (int)s.Width;
            //}
        }

        private void txtRow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.txtContent.Location = new Point(this.txtRow.Width, this.txtContent.Location.Y);
            //this.txtContent.Width = this.ClientSize.Width - this.txtRow.Width;
        }

        private void ShowRow()
        {
            //int firstLine = txtContent.GetLineFromCharIndex(txtContent.GetCharIndexFromPosition(new Point(0, 2)));
            //string[] lin = new string[pageLine];
            //for (int i = 0; i < pageLine; i++)
            //{
            //    lin[i] = (i + firstLine + 1).ToString();
            //}
            //txtRow.Lines = lin;
        }

        private void SetScrollBar()
        {
            //SCROLLINFO si = new SCROLLINFO();
            //si.cbSize = (uint)Marshal.SizeOf(si);
            //si.fMask = SIF_ALL;
            //int r = GetScrollInfo(this.txtContent.Handle, SB_VERT, ref si);
            //pageLine = (int)si.nPage;
            //this.vScrollBar1.LargeChange = pageLine;

            //if (si.nMax >= si.nPage)
            //{
            //    this.vScrollBar1.Visible = true;
            //    this.vScrollBar1.Maximum = si.nMax;
            //    this.vScrollBar1.Value = si.nPos;
            //}
            //else
            //    this.vScrollBar1.Visible = false;
        }
        private void ShowCursorLine()
        {
            //toolStripStatusLabel1.Text = "行: " + (this.txtContent.GetLineFromCharIndex(this.txtContent.SelectionStart) + 1);
        }

        private void A_key_test(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("嘿嘿，没做");
        }
    }
}
