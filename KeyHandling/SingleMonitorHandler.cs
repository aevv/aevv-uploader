using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;

namespace aevvuploader.KeyHandling
{
    class SingleMonitorHandler : IInputHandler
    {
        public void Handle(IVisibleForm form, KeyboardHook hook)
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
