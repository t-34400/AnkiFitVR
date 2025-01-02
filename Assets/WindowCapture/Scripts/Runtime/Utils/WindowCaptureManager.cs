using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using UnityEngine;

namespace WindowCapture
{
    public class WindowCaptureManager
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);


        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdcDest,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            int dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hWnd, out RECT lpRect);

        private IntPtr _windowHandle;

        public WindowCaptureManager(string targetProcessName)
        {
            _windowHandle = WindowLister.GetWindowHandleByProcessName(targetProcessName);
            if (_windowHandle == IntPtr.Zero)
            {
                throw new Exception($"Window with process name '{targetProcessName}' not found.");
            }
        }

        public bool IsValidWindow => _windowHandle != IntPtr.Zero && IsWindow(_windowHandle);

        public Func<Texture2D, Texture2D> CaptureWindow()
        {
            IntPtr hdcWindow = GetDC(_windowHandle);
            IntPtr hdcMemDC = CreateCompatibleDC(hdcWindow);

            Rectangle windowRect = GetWindowRectangle();
            int width = windowRect.Width;
            int height = windowRect.Height;

            IntPtr hBitmap = CreateCompatibleBitmap(hdcWindow, width, height);
            IntPtr hOld = SelectObject(hdcMemDC, hBitmap);

            const int SRCCOPY = 0x00CC0020;
            BitBlt(hdcMemDC, 0, 0, width, height, hdcWindow, 0, 0, SRCCOPY);

            Bitmap bitmap = Image.FromHbitmap(hBitmap);

            SelectObject(hdcMemDC, hOld);
            DeleteObject(hBitmap);
            DeleteDC(hdcMemDC);
            ReleaseDC(_windowHandle, hdcWindow);

            return (targetTexture) => BitmapToTexture2D(bitmap, targetTexture);
        }

        private Rectangle GetWindowRectangle()
        {
            RECT rect;
            GetClientRect(_windowHandle, out rect);

            int dpi = GetDpiForWindow(_windowHandle);
            float scale = dpi / 96.0f;

            return new Rectangle(
                (int)(rect.Left * scale),
                (int)(rect.Top * scale),
                (int)((rect.Right - rect.Left) * scale),
                (int)((rect.Bottom - rect.Top) * scale)
            );
        }

        private Texture2D BitmapToTexture2D(Bitmap bitmap, Texture2D targetTexture = null)
        {
            targetTexture ??= new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.RGBA32, false);

            if (targetTexture.width != bitmap.Width || targetTexture.height != bitmap.Height)
            {
                targetTexture.Reinitialize(bitmap.Width, bitmap.Height);
            }

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            targetTexture.LoadRawTextureData(data.Scan0, data.Stride * bitmap.Height);
            targetTexture.Apply();

            bitmap.UnlockBits(data);
            bitmap.Dispose();

            return targetTexture;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
