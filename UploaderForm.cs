using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.KeyHandling;
using aevvuploader.Network;
using imguruploader.IO;

namespace aevvuploader
{
    internal class UploaderForm : Form, IInvisibleForm
    {
        public delegate void ActivateDelegate();

        public delegate void TestCallback(string s);

        // Keep reference to this alive due to unmanaged memory bla bla
        private readonly KeyHandler _handler;
        private readonly KeyboardHook _hook;
        private bool _buttonDown;
        private bool _cancel;
        private Form _clickForm;
        private bool _firstTimeShown = true;
        private Point _startingLocation;
        private bool _success;
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        public UploaderForm()
        {
            ConfigureInvisibleForm();
            ConfigureTrayIcon();

            _handler = new KeyHandler(this, new UploadQueue(new ImageUploader(), new UploadResultHandler(this)));
            _hook = new KeyboardHook();
            _handler.RegisterKeys(_hook);
            _hook.KeyPressed += _handler.Handle;

            FormClosed += UploaderForm_FormClosed;
        }

        public void Exit()
        {
            Close();
            Environment.Exit(0);
        }

        public void SuccessfulUpload(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            Invoke(() => UploadComplete(url));
        }

        private void UploadComplete(string url)
        {
            Clipboard.SetText(url);
            _trayIcon.BalloonTipText = url;
            _trayIcon.ShowBalloonTip(1000);
        }


        public void ToggleVisibility()
        {
            if (Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void Invoke(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (InvokeRequired)
            {
                Invoke(new Invoker(() => action?.Invoke()));
            }
            else
            {
                action?.Invoke();
            }
        }

        public void Explode(Action<bool, Rectangle> implosionCallback)
        {
            if (implosionCallback == null) throw new ArgumentNullException(nameof(implosionCallback));
            Location = new Point(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top);
            Size = new Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

            Show();
            Task.Factory.StartNew(() => DrawSelection(implosionCallback));

            KeyDown += Cancel;
            CreateClickForm();
        }

        private void ConfigureInvisibleForm()
        {
            BackColor = Color.Purple;
            TopMost = true;
            DoubleBuffered = true;
            ShowInTaskbar = false;
            TransparencyKey = Color.Purple;
            Opacity = 0.3f;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(-5000, -5000);
            Hide();
            Shown += UploaderForm_Shown;
        }

        private void ConfigureTrayIcon()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add(nameof(Exit), (sender, args) => Application.Exit());
            _trayIcon = new NotifyIcon
            {
                Text = nameof(aevvuploader),
                Icon = new Icon(SystemIcons.Exclamation, 40, 40),
                ContextMenu = _trayMenu,
                Visible = true
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = nameof(_hook))]
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void UploaderForm_Shown(object sender, EventArgs e)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (e == null) throw new ArgumentNullException(nameof(e));
            if (_firstTimeShown)
            {
                Shown -= UploaderForm_Shown;
                _firstTimeShown = false;
                Hide();
            }
        }

        private void Cancel(object sender, KeyEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.KeyCode == Keys.Escape)
            {
                KeyDown -= Cancel;
                // TODO: Race conditions?
                _success = false;
                _cancel = true;

                _clickForm.MouseUp -= MouseUpEventHandler;
                _clickForm.MouseDown -= MouseDownEventHandler;
                _buttonDown = false;
            }
        }

        private void MouseDownEventHandler(object sender, MouseEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Button == MouseButtons.Left && !_buttonDown)
            {
                _buttonDown = true;
                _startingLocation = new Point(Cursor.Position.X - SystemInformation.VirtualScreen.Left,
                    Cursor.Position.Y - SystemInformation.VirtualScreen.Top);
                _clickForm.MouseDown -= MouseDownEventHandler;
            }
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Button == MouseButtons.Left && _buttonDown)
            {
                _success = true;
                _clickForm.MouseUp -= MouseUpEventHandler;
            }
        }

        // TODO: tidy _clickForm hack - probably cant remove but at least tidy it
        private void CreateClickForm()
        {
            _clickForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                Location = Location,
                Size = Size,
                Opacity = 0.01d,
                Cursor = Cursors.SizeAll
            };

            _clickForm.MouseDown += MouseDownEventHandler;
            _clickForm.MouseUp += MouseUpEventHandler;

            _clickForm.Show();
            
        }

        public void Implode()
        {
            Location = new Point(-5000, -5000);
            Size = new Size(0, 0);

            _clickForm.Cursor = Cursors.Default;

            _clickForm.Close();
        }

        // TODO: Better management of state between threads
        // TODO: remove duplication
        private void DrawSelection(Action<bool, Rectangle> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            while (!_cancel && !_success)
            {
                if (_buttonDown)
                {
                    Invoke(() => DrawRect(_startingLocation,
                        new Point(Cursor.Position.X - SystemInformation.VirtualScreen.Left, Cursor.Position.Y - SystemInformation.VirtualScreen.Top)));
                }
                Thread.Sleep(50);
            }
            Invoke(Implode);

            var endPosition = new Point(Cursor.Position.X - SystemInformation.VirtualScreen.Left, Cursor.Position.Y - SystemInformation.VirtualScreen.Top);
            callback?.Invoke(_success, new Rectangle(Math.Min(_startingLocation.X, endPosition.X), Math.Min(_startingLocation.Y, endPosition.Y),
                Math.Abs(endPosition.X - _startingLocation.X), Math.Abs(endPosition.Y - _startingLocation.Y)));
        }

        private void DrawRect(Point topLeft, Point bottomRight)
        {
            using (var graphics = CreateGraphics())
            {
                var brush = new SolidBrush(Color.DimGray);
                graphics.Clear(Color.Purple);
                graphics.FillRectangle(brush, Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y),
                    Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y));
                brush.Dispose();
            }
        }

        [STAThread]
        public static void Main()
        {
            using (var uploaderForm = new UploaderForm())
            {
                Application.Run(uploaderForm);
            }
        }

        private delegate void Invoker();

        private void UploaderForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender == null) throw new ArgumentNullException(nameof(sender));
            if (e == null) throw new ArgumentNullException(nameof(e));
            _hook.Dispose();
        }
    }
}