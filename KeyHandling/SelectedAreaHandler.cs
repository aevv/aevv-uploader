using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class SelectedAreaHandler : IInputHandler
    {
        public void Handle(IScreenshottableForm form, KeyboardHook hook)
        {
            if (!form.Visible)
            {
                form.Explode(CaptureArea);
            }
        }

        public Keys TriggerKey => Keys.D4;

        private void CaptureArea(bool success, Rectangle area)
        {
            if (!success)
            {
                return;
            }

            var capture = new ScreenshotCreator();
            var bitmap = capture.GetArea(area);

            bitmap.Save("C:\\Temp\\Area.png", ImageFormat.Png);
        }
    }
}