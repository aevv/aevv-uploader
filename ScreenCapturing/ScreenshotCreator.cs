using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.KeyHandling;

namespace aevvuploader.ScreenCapturing
{
    class ScreenshotCreator
    {
        public Bitmap GetAllMonitors()
        {
            var bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public Bitmap GetSingleMonitor(Screen screen)
        {
            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public Bitmap GetSingleMonitor(int monitorNumber)
        {
            var screen = Screen.AllScreens[monitorNumber];

            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public Bitmap GetActiveWindow()
        {
            var activeWindow = GetForegroundWindow();
            Rect rect;
            GetWindowRect(activeWindow, out rect);

            if (rect.Top != rect.Bottom && rect.Left != rect.Right)
            {
                var size = new Size(Math.Abs(rect.Right - rect.Left), Math.Abs(rect.Bottom - rect.Top));

                var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, size);
                    return bitmap;
                }
            }

            return null;
        }

        public Bitmap GetArea(Rectangle area)
        {
            var bitmap = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var left = SystemInformation.VirtualScreen.Left + area.Left;
                var top = SystemInformation.VirtualScreen.Top + area.Top;
                graphics.CopyFromScreen(left, top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
