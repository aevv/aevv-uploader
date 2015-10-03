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
            var capture = new ScreenshotCreator();
            var fullScreen = capture.GetAllMonitors();

            queue.QueueImage(fullScreen);
        }

        public Keys TriggerKey => Keys.D3;
    }
}