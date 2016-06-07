using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using aevvuploader.Network;
using aevvuploader.ScreenCapturing;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    internal class CurrentWindowHandler : IInputHandler
    {
        public void Handle(IInvisibleForm form, KeyboardHook hook, Action<Bitmap> callback)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (callback == null) throw new ArgumentNullException(nameof(callback));


            var windowBitmap = ScreenshotCreator.GetActiveWindow();

            callback(windowBitmap);
        }

        public Keys TriggerKey => Keys.D2;
    }
}