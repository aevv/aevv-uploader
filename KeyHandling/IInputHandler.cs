using System.Windows.Forms;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    public interface IInputHandler
    {
        Keys TriggerKey { get; }
        void Handle(IScreenshottableForm form, KeyboardHook hook);
    }
}