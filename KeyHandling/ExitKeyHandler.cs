using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.Network;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    class ExitKeyHandler : IInputHandler
    {
        public Keys TriggerKey => Keys.X;

        public void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            form.Exit();
        }
    }
}
