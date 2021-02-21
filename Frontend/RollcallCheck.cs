using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace Frontend
{
    class RollcallCheck
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }
        
        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        //获取窗口Text 
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        //获取窗口类名 
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;
            public int width;
            public int height;
        }

        public static void init()
        {
            GetAllDesktopWindows();
        }
        public static List<WindowInfo> AllWindowList { get; set; }
        public static WindowInfo[] GetAllDesktopWindows()
        {
            //用来保存窗口对象 列表
            List<WindowInfo> wndList = new List<WindowInfo>();
            //enum all desktop windows 
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);

                //get hwnd 
                wnd.hWnd = hWnd;

                //get window name  
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();

                //get window class 
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();

                //get window rect
                RECT rollcallwindow = new RECT();
                GetWindowRect(hWnd, ref rollcallwindow);
                wnd.Left = rollcallwindow.Left;
                wnd.Right = rollcallwindow.Right;
                wnd.Bottom = rollcallwindow.Bottom;
                wnd.Top = rollcallwindow.Top;
                wnd.width = rollcallwindow.Right-rollcallwindow.Left;
                wnd.height = rollcallwindow.Bottom-rollcallwindow.Top;
                //add it into list 
                wndList.Add(wnd);
                return true;
            }, 0);
            AllWindowList = wndList;
            return wndList.ToArray();
        }
        
        public static WindowInfo detectwindow()
        {
            WindowInfo temp = new WindowInfo();
            temp.hWnd = (IntPtr)0;
            for (int i = 0; i < AllWindowList.Count(); i++)
            {
                /*
                IntPtr hwnd=AllWindowList[i].hWnd;
                RECT rollcallwindow = new RECT();
                GetWindowRect(hwnd, ref rollcallwindow);//h为窗口句柄
                int width = rollcallwindow.Right - rollcallwindow.Left;                        //窗口的宽度
                int height = rollcallwindow.Bottom - rollcallwindow.Top;                   //窗口的高度
                int x = rollcallwindow.Left;
                int y = rollcallwindow.Top;
                */
                if ((1.0 * AllWindowList[i].width / AllWindowList[i].height) > 1.88 && (1.0 * AllWindowList[i].width / AllWindowList[i].height) < 2.05 && AllWindowList[i].szClassName=="#32770" && AllWindowList[i].width <800 && AllWindowList[i].height <500 && AllWindowList[i].width >100 && AllWindowList[i].height >70 )
                {
                    //System.Windows.MessageBox.Show(AllWindowList[i].hWnd.ToString() + " " + AllWindowList[i].width.ToString() + " " + AllWindowList[i].height.ToString() + " " + AllWindowList[i].Left.ToString() + " " + AllWindowList[i].Top.ToString() + " ");
                    //autoinput(AllWindowList[i].hWnd, AllWindowList[i].width, AllWindowList[i].height, AllWindowList[i].Left, AllWindowList[i].Top);
                    return AllWindowList[i];
                }
            }
            return temp;
        }
        
    }
}
