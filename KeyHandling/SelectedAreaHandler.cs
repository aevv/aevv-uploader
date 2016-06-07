using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.Network;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class SelectedAreaHandler : IInputHandler
    {
        private Action<Bitmap> _callback;
        public void Handle(IInvisibleForm form, KeyboardHook hook, Action<Bitmap> callback)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            if (!form.Visible)
            {
                _callback = callback;
                form.Explode(CaptureArea);
            }
        }

        public Keys TriggerKey => Keys.D4;

        private void CaptureArea(bool success, Rectangle area)
        {
            if(area == null)
                throw new ArgumentNullException(nameof(area));
            if (!success)
            {
                return;
            }

            var bitmap = ScreenshotCreator.GetArea(area);

            _callback(bitmap);
        }
    }
}