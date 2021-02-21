using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Data;

namespace Frontend
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public class RollcallInformation
        {
            public string time { get; set; }
            public int num { get; set; }
        }
        DispatcherTimer timer = new DispatcherTimer();
        public static int checknum { get; set; }
        public static int isChecking { get; set; }
        public static ObservableCollection<RollcallInformation> rollcall = new ObservableCollection<RollcallInformation>();
        
        public MainWindow()
        {
            InitializeComponent();
            isChecking = 0;
            checknum = 0;

            Notice.Text = GetHtmlStr("https://gitee.com/theunknownthing/auto-rollcall-wpfnotice/raw/master/notice.txt", "UTF8");
        }
        public static void autoinput(IntPtr hwnd, int width, int height, int x, int y)
        {
            startmusic();
            isChecking = 1;
            int startpointx = x + width / 7;
            int startpointy = (int)(y + (0.73 * height));
            /*
            SetCursorPos(startpointx, startpointy);
            //System.Windows.MessageBox.Show(k.ToString());
            uint k = GetLastError();
            System.Windows.MessageBox.Show(k.ToString());
            */
            SetCursorPos(startpointx, startpointy);
            /*while (k == 0)
            {
                k = SetCursorPos(startpointx, startpointy);
            }*/
            int endpointy = startpointy;
            int movedistance = (width * 8) / 10;
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            //System.Windows.MessageBox.Show(movedistance.ToString());
            for (int i = 1; i <= movedistance; i++)
            {
                
                SetCursorPos(startpointx+i, startpointy);
                Thread.Sleep(1);
            }
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            
            checknum++;
            //RollcallNum.Content = checknum;
            rollcall.Add(new RollcallInformation()
            {
                num = checknum,
                time = DateTime.Now.ToString("HH:mm:ss")
            });
            

            //RollcallInformations.ItemsSource = rollcall;
            isChecking = 0;
            // For Test Use
            /*
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 1000, 1000, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            */
        }
        public static void startmusic()
        {
            //System.Media.SystemSounds.Beep.Play();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            string nowDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            player.SoundLocation = nowDirectory + "rollcall.wav";
            player.Play();
        }
        private void isAutoRollcallEnabled_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    if (int.Parse(RollcalllCheckSeconds.Text) < 6) RollcalllCheckSeconds.Text = "6";
                    else if (int.Parse(RollcalllCheckSeconds.Text) > 10) RollcalllCheckSeconds.Text = "10";
                    ShowAutoRollcallService.IsIndeterminate = true;
                    ShowAutoRollcallService.Visibility = Visibility.Visible;
                    WelcomeLabel.Content = "自动签到正在检测";
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = TimeSpan.FromSeconds(int.Parse(RollcalllCheckSeconds.Text));
                    timer.Start();
                }
                else
                {
                    ShowAutoRollcallService.IsIndeterminate = false;
                    ShowAutoRollcallService.Visibility = Visibility.Collapsed;
                    WelcomeLabel.Content = "欢迎使用自动签到";
                    timer.Stop();
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            /*
            string nowDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            Process pr = new Process();
            pr.StartInfo.FileName = nowDirectory + "AutoRollcallWPF.exe";
            pr.Start();
            */

            Thread thread = new Thread(rollcall_check);
            thread.Start();

        }

        private void rollcall_check()
        {
            if (isChecking == 0)
            {
                RollcallCheck.init();
                RollcallCheck.WindowInfo rollcallwindowinfo = new RollcallCheck.WindowInfo();
                rollcallwindowinfo = RollcallCheck.detectwindow();
                if (rollcallwindowinfo.hWnd != (IntPtr)0)
                {
                    autoinput(rollcallwindowinfo.hWnd, rollcallwindowinfo.width, rollcallwindowinfo.height, rollcallwindowinfo.Left, rollcallwindowinfo.Top);
                }
            }
        }
        private void isSoundEnabled_Toggled(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSource_Click(object sender, RoutedEventArgs e)
        {
            OpenSource k = new OpenSource();
            k.Show();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About k = new About();
            k.Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Update k = new Update();
            k.Show();
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

        private void OpenThanks_Click(object sender, RoutedEventArgs e)
        {
            Thanks k = new Thanks();
            k.Show();
        }
    }
}
