using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Management;
using System.Windows.Forms;
using aevvuploader.ScreenCapturing;

namespace aevvuploader.KeyHandling
{
    class SelectedAreaHandler : IInputHandler
    {
        public void Handle(IScreenshottableForm form, KeyboardHook hook)
        {
            if (!form.Visible)
            {
                form.Explode(CaptureArea);
            }
        }

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

        public Keys TriggerKey => Keys.D4;
    }
}
