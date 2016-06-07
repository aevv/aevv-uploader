using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using aevvuploader.Network;
using imguruploader.IO;

namespace aevvuploader.KeyHandling
{
    public class KeyHandler
    {
        private readonly IInvisibleForm _form;
        private readonly Action<Bitmap> _callback;
        private readonly Dictionary<Keys, IInputHandler> _keyHandlers;
        private KeyboardHook _hook;

        public KeyHandler(IInvisibleForm form, Action<Bitmap> callback)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _form = form;
            _callback = callback;
            _keyHandlers = new Dictionary<Keys, IInputHandler>();
        }

        public void Handle(object sender, KeyPressedEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (_keyHandlers.ContainsKey(args.Key))
            {
                _keyHandlers[args.Key].Handle(_form, _hook, _callback);
            }
        }

        public void RegisterKeys(KeyboardHook hook)
        {
            if (hook == null) throw new ArgumentNullException(nameof(hook));
            _hook = hook;
            // TODO: Reflect service locator-y
            // TODO: Config based
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