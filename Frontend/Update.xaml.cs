using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Frontend
{
    /// <summary>
    /// Update.xaml 的交互逻辑
    /// </summary>
    public partial class Update : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool isLoaded = false;
        public Update()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Start();
            isLoaded = true;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (isLoaded == true)
            {
                timer.Stop();
                LatestVersion.Content = GetHtmlStr("https://gitee.com/theunknownthing/auto-rollcall-wpfnotice/raw/master/version.txt", "UTF8");
                UpdateFunction.Text = GetHtmlStr("https://gitee.com/theunknownthing/auto-rollcall-wpfnotice/raw/master/Logs", "UTF8");
                if (NowVersion.Content.ToString() == LatestVersion.Content.ToString()) {
                    isUpdated.Content = "已是最新版本";
                    UpdatedImage.Visibility = System.Windows.Visibility.Visible;
                    HaveUpdateImage.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    isUpdated.Content = "有新版本可用";
                    HaveUpdateImage.Visibility = System.Windows.Visibility.Visible;
                    UpdatedImage.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
        public static string GetHtmlStr(string url, string encoding)
        {
            string htmlStr = "";
            try
            {
                if (!String.IsNullOrEmpty(url))
                {
                    WebRequest request = WebRequest.Create(url);            //实例化WebRequest对象  
                    WebResponse response = request.GetResponse();           //创建WebResponse对象  
                    Stream datastream = response.GetResponseStream();       //创建流对象  
                    Encoding ec = Encoding.Default;
                    if (encoding == "UTF8")
                    {
                        ec = Encoding.UTF8;
                    }
                    else if (encoding == "Default")
                    {
                        ec = Encoding.Default;
                    }
                    StreamReader reader = new StreamReader(datastream, ec);
                    htmlStr = reader.ReadToEnd();                  //读取网页内容  
                    reader.Close();
                    datastream.Close();
                    response.Close();
                }
            }
            catch { }
            return htmlStr;
        }
    }
}
