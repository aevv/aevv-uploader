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
        private UploadQueue _queue;
        public void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (!form.Visible)
            {
                _queue = queue;
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

            _queue?.QueueImage(bitmap);
        }
    }
}