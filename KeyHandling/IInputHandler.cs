using System.Windows.Forms;

namespace aevvuploader.KeyHandling
{
    public interface IInputHandler
    {
        void Handle(IVisibleForm form, KeyboardHook hook);
        Keys TriggerKey { get; }
    }
}