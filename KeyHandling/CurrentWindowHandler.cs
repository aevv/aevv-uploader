using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;

namespace aevvuploader.KeyHandling
{
    class CurrentWindowHandler : IInputHandler
    {
        public void Handle(IVisibleForm form)
        {
            var capture = new ScreenshotCreator();

            var windowBitmap = capture.GetActiveWindow();

            windowBitmap.Save("C:\\Temp\\Active.png", ImageFormat.Png);
        }

        public Keys TriggerKey => Keys.D2;
    }
}
