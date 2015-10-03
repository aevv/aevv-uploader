using System.Windows.Forms;
using aevvuploader.KeyHandling;

namespace imguruploader.IO
{
    public class KeyPressedEventArgs
    {
        public KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public ModifierKeys Modifier { get; }
        public Keys Key { get; }
    }
}