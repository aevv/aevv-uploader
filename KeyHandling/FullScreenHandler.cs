using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class FullScreenHandler : IInputHandler
    {
        public void Handle(IScreenshottableForm form, KeyboardHook hook)
        {
            var capture = new ScreenshotCreator();
            var fullScreen = capture.GetAllMonitors();

            fullScreen.Save("C:\\Temp\\Image2.png", ImageFormat.Png);
        }

        public Keys TriggerKey => Keys.D3;
    }
}