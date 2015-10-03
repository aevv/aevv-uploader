using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Linq;
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
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            var mouseLocation = Cursor.Position;
            foreach (var bitmap in from screen in Screen.AllScreens where screen.Bounds.Contains(mouseLocation) select ScreenshotCreator.GetSingleMonitor(screen))
            {
                queue.QueueImage(bitmap);

                return;
            }
        }

        public Keys TriggerKey => Keys.D1;
    }
}