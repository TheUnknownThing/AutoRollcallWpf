using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Windows;

namespace Frontend
{
    class RollcallCheckNew
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
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);



        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }
        [StructLayout(LayoutKind.Explicit)]
        struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [Flags]
        enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }
        enum SendInputEventType : int
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }


        enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }
        public static void ClickLeftMouseButton(int x, int y)
        {
            INPUT mouseInput = new INPUT();
            mouseInput.type = SendInputEventType.InputMouse;
            mouseInput.mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
            mouseInput.mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
            mouseInput.mkhi.mi.mouseData = 0;

            mouseInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
            SendInput(1, ref mouseInput, Marshal.SizeOf(new INPUT()));

            mouseInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
            SendInput(1, ref mouseInput, Marshal.SizeOf(new INPUT()));

            mouseInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
            SendInput(1, ref mouseInput, Marshal.SizeOf(new INPUT()));
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
        
        public static void detectwindow()
        {
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
                    autoinput(AllWindowList[i].hWnd, AllWindowList[i].width, AllWindowList[i].height, AllWindowList[i].Left, AllWindowList[i].Top);
                }
            }
        }
        public static void autoinput(IntPtr hwnd, int width, int height, int x, int y)
        {
            int startpointx = x + width / 7;
            int startpointy = (int)(y + (0.73 * height));
            /*
            SetCursorPos(startpointx, startpointy);
            //System.Windows.MessageBox.Show(k.ToString());
            uint k = GetLastError();
            System.Windows.MessageBox.Show(k.ToString());
            
            int k = SetCursorPos(startpointx, startpointy);
            
            while (k == 0)
            {
                k = SetCursorPos(startpointx, startpointy);
            }
            int endpointy = startpointy;
            int movedistance =  (width *8)/10;
            for (int i = 1; i <= movedistance; i++)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_MOVE, 1, 0, 0, 0);
                if (i == movedistance)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                Thread.Sleep(1);
            }
            
            startmusic();
            // For Test Use

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, 1000, 1000, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            */
            ClickLeftMouseButton(50, 50);
        }
        public static void startmusic()
        {
            //System.Media.SystemSounds.Beep.Play();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            string nowDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            player.SoundLocation = nowDirectory + "rollcall.wav";
            player.Play();
        }
    }
}
