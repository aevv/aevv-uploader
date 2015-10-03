using imguruploader.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using aevvuploader.KeyHandling;

namespace aevvuploader.ScreenCapturing
{
    internal static  class ScreenshotCreator
    {
        public static Bitmap GetAllMonitors()
        {
            var bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top, 0, 0, bitmap.Size,
                    CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public static Bitmap GetSingleMonitor(Screen screen)
        {
            if (screen == null) throw new ArgumentNullException(nameof(screen));
            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public static Bitmap GetSingleMonitor(int monitorNumber)
        {
            var screen = Screen.AllScreens[monitorNumber];

            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                return bitmap;
            }
        }

        public static Bitmap GetActiveWindow()
        {
            var activeWindow = NativeMethods.GetForegroundWindow();
            Rect rect;
            
            NativeMethods.GetWindowRect(activeWindow, out rect);

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

        public static Bitmap GetArea(Rectangle area)
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



    }
}