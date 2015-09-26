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
    class FullScreenHandler : IInputHandler
    {
        public void Handle(IVisibleForm form, KeyboardHook hook)
        {
            var capture = new ScreenshotCreator();
            var fullScreen = capture.GetAllMonitors();

            fullScreen.Save("C:\\Temp\\Image2.png", ImageFormat.Png);
        }

        public Keys TriggerKey => Keys.D3;
    }
}
