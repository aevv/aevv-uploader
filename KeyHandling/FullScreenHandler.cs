using System;
using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.Network;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class FullScreenHandler : IInputHandler
    {
        public void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            var fullScreen = ScreenshotCreator.GetAllMonitors();

            queue.QueueImage(fullScreen);
        }

        public Keys TriggerKey => Keys.D3;
    }
}