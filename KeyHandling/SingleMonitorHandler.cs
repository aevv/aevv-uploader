using System.Collections;
using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.Network;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class SingleMonitorHandler : IInputHandler
    {
        public void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue)
        {
            var mouseLocation = Cursor.Position;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(mouseLocation))
                {
                    var bitmap = new ScreenshotCreator().GetSingleMonitor(screen);

                    queue.QueueImage(bitmap);

                    return;
                }
            }
        }

        public Keys TriggerKey => Keys.D1;
    }
}