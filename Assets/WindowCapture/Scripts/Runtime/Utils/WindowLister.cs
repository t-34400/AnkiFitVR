using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowCapture
{
    public class WindowLister
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static List<string> GetWindowTitles()
        {
            List<string> windowTitles = new List<string>();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hWnd, title, title.Capacity);

                    if (title.Length > 0)
                    {
                        windowTitles.Add(title.ToString());
                    }
                }
                return true;
            }, IntPtr.Zero);

            return windowTitles;
        }

        public static List<(string windowName, string processName)> GetWindowNames()
        {
            List<(string windowName, string processName)> windowNames = new List<(string, string)>();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hWnd, title, title.Capacity);

                    uint processId;
                    GetWindowThreadProcessId(hWnd, out processId);

                    string processName = GetProcessNameById(processId);

                    windowNames.Add((title.ToString(), processName));
                }

                return true;
            }, IntPtr.Zero);

            return windowNames;
        }

        public static IntPtr GetWindowHandleByProcessName(string targetProcessName)
        {
            IntPtr windowHandle = IntPtr.Zero;

            List<(string windowName, string processName)> processNameList = new List<(string, string)>();

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder title = new StringBuilder(256);
                    GetWindowText(hWnd, title, title.Capacity);

                    uint processId;
                    GetWindowThreadProcessId(hWnd, out processId);

                    string processName = GetProcessNameById(processId);

                    processNameList.Add((title.ToString(), processName));

                    if (processName.ToLower() == targetProcessName.ToLower())
                    {
                        windowHandle = hWnd;
                        return false;
                    }
                }

                return true;
            }, IntPtr.Zero);

            UnityEngine.Debug.Log($"Process names: {string.Join(", ", processNameList)}");

            return windowHandle;
        }

        private static string GetProcessNameById(uint processId)
        {
            try
            {
                Process process = Process.GetProcessById((int)processId);
                return process.ProcessName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
