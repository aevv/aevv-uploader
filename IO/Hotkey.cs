// TODO: Refactor this
// http://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using aevvuploader.KeyHandling;

namespace imguruploader.IO
{
    public sealed class KeyboardHook : IDisposable
    {
        private readonly Window _window = new KeyboardHook.Window();
        private int _currentId;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (sender == null) throw new ArgumentNullException(nameof(sender));
                if (args == null) throw new ArgumentNullException(nameof(args));
                KeyPressed?.Invoke(this, args);
            };
        }

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (var i = _currentId; i > 0; i--)
            {
                NativeMethods.UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion

        // Registers a hot key with Windows.

        // Unregisters the hot key with Windows.

        /// <summary>
        ///     Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!NativeMethods.RegisterHotKey(_window.Handle, _currentId, (uint) modifier, (uint) key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        ///     A hot key has been pressed.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        ///     Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static readonly int WM_HOTKEY = 0x0312;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public Window()
            {
                // create the handle for the window.
                CallHandle();
            }

            private void CallHandle()
            {
                CreateHandle(new CreateParams());
            }

            #region IDisposable Members

            public void Dispose()
            {
                DestroyHandle();
            }

            #endregion

            /// <summary>
            ///     Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg != WM_HOTKEY)
                    return;
                // get the keys.
                var key = (Keys) (((int) m.LParam >> 16) & 0xFFFF);
                var modifier = (ModifierKeys) ((int) m.LParam & 0xFFFF);

                // invoke the event to notify the parent.
                KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;
        }
    }




}