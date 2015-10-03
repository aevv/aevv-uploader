using System.Windows.Forms;
using aevvuploader.Network;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    public interface IInputHandler
    {
        Keys TriggerKey { get; }
        void Handle(IInvisibleForm form, KeyboardHook hook, UploadQueue queue);
    }
}