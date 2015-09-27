using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using aevvuploader.KeyHandling;

namespace aevvuploader
{
    partial class UploaderForm : Form, IScreenshottableForm
    {
        public delegate void ActivateDelegate();

        public delegate void TestCallback(string s);

        // Keep reference to this alive due to unmanaged memory bla bla
        private readonly KeyHandler _handler;
        private readonly KeyboardHook _hook;
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;
        private bool _firstTimeShown = true;

        private bool _cancel = false;
        private bool _success = false;

        public UploaderForm()
        {
            _handler = new KeyHandler(this);
            _hook = new KeyboardHook();
            _handler.RegisterKeys(_hook);
            _hook.KeyPressed += _handler.Handle;

            ConfigureInvisibleForm();
            ConfigureTrayIcon();
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
            _trayMenu.MenuItems.Add("Exit", (sender, args) => Application.Exit());
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "aevvuploader";
            _trayIcon.Icon = new Icon(SystemIcons.Exclamation, 40, 40);

            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
        }

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
            if (_firstTimeShown)
            {
                Shown -= UploaderForm_Shown;
                _firstTimeShown = false;
                Hide();
            }
        }

        delegate void Invoker();

        public void Invoke(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(new Invoker(() => action()));
            }
            else
            {
                action();
            }
        }

        private void Cancel(object sender, KeyEventArgs args)
        {
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

        private bool _buttonDown;

        private void MouseDownEventHandler(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left)
            {
                if (!_buttonDown)
                {
                    _buttonDown = true;
                    _startingLocation = new Point(Cursor.Position.X - SystemInformation.VirtualScreen.Left,
                        Cursor.Position.Y - SystemInformation.VirtualScreen.Top);
                    _clickForm.MouseDown -= MouseDownEventHandler;
                }
            }
        }

        private void MouseUpEventHandler(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left && _buttonDown)
            {
                _success = true;
                _clickForm.MouseUp -= MouseUpEventHandler;
            }
        }

        // TODO: tidy _clickForm hack - probably cant remove but at least tidy it
        private Form _clickForm;
        public void Explode(Action<bool, Rectangle> implosionCallback)
        {
            Location = new Point(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top);
            Size = new Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

            Show();
            Task.Factory.StartNew(() => DrawSelection(implosionCallback));

            KeyDown += Cancel;
            CreateClickForm();
        }

        private void CreateClickForm()
        {
            _clickForm = new Form();
            _clickForm.FormBorderStyle = FormBorderStyle.None;
            _clickForm.MouseDown += MouseDownEventHandler;
            _clickForm.MouseUp += MouseUpEventHandler;

            _clickForm.Show();
            _clickForm.Location = Location;
            _clickForm.Size = Size;
            _clickForm.Opacity = 0.01d;
        }

        public void Implode()
        {
            Location = new Point(-5000, -5000);
            Size = new Size(0, 0);

            _clickForm.Close();
        }

        // TODO: Better management of state between threads
        // TODO: remove duplication
        private Point _startingLocation;
        private void DrawSelection(Action<bool, Rectangle> callback)
        {
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
            callback(_success, new Rectangle(Math.Min(_startingLocation.X, endPosition.X), Math.Min(_startingLocation.Y, endPosition.Y),
                    Math.Abs(endPosition.X - _startingLocation.X), Math.Abs(endPosition.Y - _startingLocation.Y)));
        }

        private void DrawRect(Point topLeft, Point bottomRight)
        {
            using (var graphics = CreateGraphics())
            {
                var brush = new SolidBrush(Color.DimGray);
                graphics.Clear(Color.Purple);
                // TODO: backwards rect
                graphics.FillRectangle(brush, Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y),
                    Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y));
                brush.Dispose();
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new UploaderForm());
        }
    }
}