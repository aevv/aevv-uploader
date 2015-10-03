using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.Network;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class CurrentWindowHandler : IInputHandler
    {
        public void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue)
        {
            var capture = new ScreenshotCreator();

            var windowBitmap = capture.GetActiveWindow();

            queue.QueueImage(windowBitmap);
        }

        public Keys TriggerKey => Keys.D2;
    }
}