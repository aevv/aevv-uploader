using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class SingleMonitorHandler : IInputHandler
    {
        public void Handle(IScreenshottableForm form, KeyboardHook hook)
        {
            var mouseLocation = Cursor.Position;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(mouseLocation))
                {
                    var bitmap = new ScreenshotCreator().GetSingleMonitor(screen);

                    bitmap.Save("C:\\Temp\\Custom.png", ImageFormat.Png);
                    return;
                }
            }
        }

        public Keys TriggerKey => Keys.D1;
    }
}