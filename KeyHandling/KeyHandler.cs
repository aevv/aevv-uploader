using System.Collections.Generic;
using System.Windows.Forms;
using aevvuploader.Network;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    public class KeyHandler
    {
        private readonly IInvisibleForm _form;
        private readonly UploadQueue _queue;
        private readonly Dictionary<Keys, IInputHandler> _keyHandlers;
        private KeyboardHook _hook;

        public KeyHandler(IInvisibleForm form, UploadQueue queue)
        {
            _form = form;
            _queue = queue;
            _keyHandlers = new Dictionary<Keys, IInputHandler>();
        }

        public void Handle(object sender, KeyPressedEventArgs args)
        {
            if (_keyHandlers.ContainsKey(args.Key))
            {
                _keyHandlers[args.Key].Handle(_form, _hook, _queue);
            }
        }

        public void RegisterKeys(KeyboardHook hook)
        {
            _hook = hook;
            // TODO: Reflect service locator-y
            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D1);
            _keyHandlers.Add(Keys.D1, new SingleMonitorHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D2);
            _keyHandlers.Add(Keys.D2, new CurrentWindowHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D3);
            _keyHandlers.Add(Keys.D3, new FullScreenHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D4);
            _keyHandlers.Add(Keys.D4, new SelectedAreaHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.X);
            _keyHandlers.Add(Keys.X, new ExitKeyHandler());
        }
    }
}