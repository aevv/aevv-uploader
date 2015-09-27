using System.Windows.Forms;

namespace aevvuploader.KeyHandling
{
    public interface IInputHandler
    {
        void Handle(IScreenshottableForm form, KeyboardHook hook);
        Keys TriggerKey { get; }
    }
}