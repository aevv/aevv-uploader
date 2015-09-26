using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using aevvuploader.KeyHandling;

namespace aevvuploader
{
    partial class UploaderForm : Form, IVisibleForm
    {
        public delegate void ActivateDelegate();

        public delegate void TestCallback(string s);

        private const int WH_MOUSE_LL = 14;
        public static object Lock = new object();
        public static object QueueLock = new object();
        private static int _mouseHookHandle;
        private readonly Dictionary<Keys, bool> _currentState = new Dictionary<Keys, bool>();
        // Keep reference to this alive due to unmanaged memory bla bla
        private readonly KeyHandler _handler;
        private readonly KeyboardHook _hook;
        private readonly Queue<Bitmap> _uploads = new Queue<Bitmap>();
        private int _counter;
        private bool _initial;
        private Point _lastPoint;
        private bool _run, _canShow;
        private Rectangle _selected;
        private Point? _start;
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

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
            BackColor = Color.DarkGray;
            TopMost = true;
            DoubleBuffered = true;
            ShowInTaskbar = false;
            Opacity = .3;
            FormBorderStyle = FormBorderStyle.None;
            Hide();
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

        private void uploadWorker()
        {
            Bitmap toUp = null;
            lock (QueueLock)
            {
                toUp = _uploads.Dequeue();
            }
            var link = "";

            var n = string.Format("img-{0:yyyy-MM-dd_hh-mm-ss-tt-fffffff}.bmp", DateTime.Now);
            toUp.Save(n);
            var nvc = new NameValueCollection();
            nvc.Add("key", "2036d03fad91b54fe094c625040892d2");
            var response = WebStuff.HttpUploadFile("http://api.imgur.com/3/upload.xml", n, "image", "image/bmp", nvc);

            try
            {
                using (var reader = XmlReader.Create(new StringReader(response)))
                {
                    reader.ReadToFollowing("original");
                    link = reader.ReadElementContentAsString();
                }
                Invoke(new TestCallback(delegate(string s)
                {
                    Clipboard.SetText(s);
                    _trayIcon.ShowBalloonTip(5000, "Upload done", s, ToolTipIcon.None);
                    _trayMenu.MenuItems.Add(s, delegate { Clipboard.SetText(s); });
                }), link);
            }
            catch
            {
                //error
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new UploaderForm());
        }
    }
}