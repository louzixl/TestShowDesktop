using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp2
{
    public static class Win32Api
    {
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public const int SWP_NOSIZE = 0x1;
        public const int SWP_NOMOVE = 0x2;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        public static void SetForegroundWindowCustom(IntPtr hwnd)
        {
            IntPtr currentWindow = GetForegroundWindow();
            if (currentWindow == hwnd)
            {
                Console.WriteLine("应用已在顶层");
                return;
            }

            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);

            bool result = SetForegroundWindow(hwnd);
            if (result)
            {
                Console.WriteLine("置顶应用成功");
            }
            else
            {
                Console.WriteLine("置顶应用失败");
            }
        }

        // from lindexi
        public static void SetWindowToForegroundWithAttachThreadInput(Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            // 以下 Win32 方法可以在 https://github.com/kkwpsv/lsjutil/tree/master/Src/Lsj.Util.Win32 找到
            var thisWindowThreadId = GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindow = GetForegroundWindow();
            var currentForegroundWindowThreadId = GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

            // [c# - Bring a window to the front in WPF - Stack Overflow](https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf )
            // [SetForegroundWindow的正确用法 - 子坞 - 博客园](https://www.cnblogs.com/ziwuge/archive/2012/01/06/2315342.html )
            /*
               　　1.得到窗口句柄FindWindow 
            　　　　2.切换键盘输入焦点AttachThreadInput 
            　　　　3.显示窗口ShowWindow(有些窗口被最小化/隐藏了) 
            　　　　4.更改窗口的Zorder，SetWindowPos使之最上，为了不影响后续窗口的Zorder,改完之后，再还原 
            　　　　5.最后SetForegroundWindow 
             */
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            window.Show();
            window.Activate();
            // 去掉和其他线程的输入链接
            AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);

            // 用于踢掉其他的在上层的窗口
            window.Topmost = true;
            window.Topmost = false;
        }
    }
}
