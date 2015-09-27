using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class CurrentWindowHandler : IInputHandler
    {
        public void Handle(IScreenshottableForm form, KeyboardHook hook)
        {
            var capture = new ScreenshotCreator();

            var windowBitmap = capture.GetActiveWindow();

            windowBitmap.Save("C:\\Temp\\Active.png", ImageFormat.Png);
        }

        public Keys TriggerKey => Keys.D2;
    }
}