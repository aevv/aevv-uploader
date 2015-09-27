using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace aevvuploader.KeyHandling
{
    public class KeyHandler
    {
        private readonly IScreenshottableForm _form;
        private KeyboardHook _hook;

        private readonly Dictionary<Keys, IInputHandler> _keyHandlers;

        public KeyHandler(IScreenshottableForm form)
        {
            _form = form;

            _keyHandlers = new Dictionary<Keys, IInputHandler>();
        }

        public void Handle(object sender, KeyPressedEventArgs args)
        {
            if (_keyHandlers.ContainsKey(args.Key))
            {
                _keyHandlers[args.Key].Handle(_form, _hook);
            }
        }

        public void RegisterKeys(KeyboardHook hook)
        {
            _hook = hook;
            // TODO: Reflect service locator-y
            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.I);
            _keyHandlers.Add(Keys.I, new TestHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D1);
            _keyHandlers.Add(Keys.D1, new SingleMonitorHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D2);
            _keyHandlers.Add(Keys.D2, new CurrentWindowHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D3);
            _keyHandlers.Add(Keys.D3, new FullScreenHandler());

            _hook.RegisterHotKey(ModifierKeys.Control | ModifierKeys.Shift, Keys.D4);
            _keyHandlers.Add(Keys.D4, new SelectedAreaHandler());
        }
    }
}