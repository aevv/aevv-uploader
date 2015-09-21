using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication4
{
    //TODO USE FORM FOR BOUNDING BOX INSTEAD SO CAN STRETCH ACROSS MONITORS!
    //First revision just fullscreen transparent window so it worked on one monitor
    //2nd revision -- transparent window that stretched over every monitor (not fullscreen) but still used the black box with the transparency key
    //this time -- resizes window itself instead of having a black box, no transparency key and errors don't fuck up as bad


    partial class UploaderForm : Form
    {
        public delegate void ActivateDelegate();

        public delegate void TestCallback(string s);

        private const int WH_MOUSE_LL = 14;
        public static object Lock = new object();
        public static object QueueLock = new object();
        private static int _mouseHookHandle;
        private static HookProc _mouseDelegate;
        private readonly Dictionary<Keys, bool> _currentState = new Dictionary<Keys, bool>();
        private readonly Dictionary<Keys, bool> _pastState = new Dictionary<Keys, bool>();
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenu _trayMenu;
        private readonly Queue<Bitmap> _uploads = new Queue<Bitmap>();
        private int _counter;
        private bool _initial;
        private Point _lastPoint;
        private bool _run, _canShow;
        private Rectangle _selected;
        private Point? _start;

        public UploaderForm()
        {
            var w = 0;
            var h = 0;
            foreach (var i in Screen.AllScreens)
            {
                w += i.Bounds.Width;
                h += i.Bounds.Height;
            }

            //   this.Bounds = new Rectangle(0, 0, w, h);
            TopMost = true;
            DoubleBuffered = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            //   this.WindowState = FormWindowState.Maximized;
            BackColor = Color.DarkGray;
            Opacity = .3;
            //this.TransparencyKey = System.Drawing.Color.Purple;
            var values = Enum.GetValues(typeof (Keys));
            foreach (Keys i in values)
            {
                if (!_currentState.ContainsKey(i))
                    _currentState.Add(i, false);
            }
            //  HookManager.MouseDown += new MouseEventHandler(HookManager_MouseDown);
            //    HookManager.MouseUp += new MouseEventHandler(HookManager_MouseUp);
            //    HookManager.MouseClick += new MouseEventHandler(HookManager_MouseClickExt);
            //     int hookID = SetWindowsHookEx(WH_KEYBOARD_LL,,
            //        GetModuleHandle(curModule.ModuleName), 0);
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Exit", OnExit);
            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            _trayIcon = new NotifyIcon();
            _trayIcon.Text = "imgur uploader";
            _trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            _trayIcon.ContextMenu = _trayMenu;
            _trayIcon.Visible = true;
            KeyPress += uploader_KeyPress;
            var key2 = new GlobalHotkey(Constants.CTRL | Constants.SHIFT, Keys.D2, this);
            key2.Register();
            var key = new GlobalHotkey(Constants.CTRL | Constants.SHIFT, Keys.D1, this);
            key.Register();
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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                var vk = (Keys) (((int) m.LParam >> 16) & 0xFFFF);
                handleKey(vk);
            }
            base.WndProc(ref m);
        }

        private void activateUI()
        {
            Activate();
            _start = Cursor.Position;
            SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
        }

        private void handleKey(Keys k)
        {
            if (k == Keys.D1)
            {
                if (!_run)
                    new Thread(new ThreadStart(delegate
                    {
                        _run = true;
                        if (this.InvokeRequired && _canShow)
                        {
                            this.Invoke(new ActivateDelegate(this.Show));
                            _canShow = false;
                        }
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new ActivateDelegate(this.activateUI));
                        }

                        while (true)
                        {
                            this.Invalidate();
                            Thread.Sleep(15);
                            if (!(_currentState[Keys.LControlKey] && _currentState[Keys.LShiftKey] && _currentState[Keys.D1]))
                            {
                                break;
                            }
                        }

                        _run = false;
                        if (_start != null && _selected != null)
                        {
                            if (this.Width > 0 && this.Height > 0)
                            {
                                // _trayIcon.ShowBalloonTip(100000, "Uploading", "Uploading Image", ToolTipIcon.Info);
                                //       ShowWindowAsync(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle, 2);
                                var bm = new Bitmap(this.Width, this.Height);
                                //        TryUnsubscribeFromGlobalMouseEvents();
                                using (var g = Graphics.FromImage(bm))
                                {
                                    g.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, this.Size);
                                }
                                lock (QueueLock)
                                {
                                    _uploads.Enqueue(bm);
                                }
                                new Thread(uploadWorker).Start();
                                if (this.InvokeRequired)
                                {
                                    this.Invoke(new ActivateDelegate(this.Hide));
                                    _canShow = true;
                                }
                            }
                        }
                    })).Start();
            }
            else if (k == Keys.D2)
            {
                RECT rct;
                var active = GetForegroundWindow();
                GetWindowRect(active, out rct);
                if (rct.Top != rct.Bottom && rct.Left != rct.Right)
                {
                    var sz = new Size(Math.Abs(rct.Right - rct.Left), Math.Abs(rct.Bottom - rct.Top));
                    var bm = new Bitmap(sz.Width, sz.Height);
                    using (var g = Graphics.FromImage(bm))
                    {
                        g.CopyFromScreen(rct.Top, rct.Left, 0, 0, sz);
                    }
                    lock (QueueLock)
                    {
                        _uploads.Enqueue(bm);
                    }
                    new Thread(uploadWorker).Start();
                }
            }
        }

        private void uploader_KeyPress(object sender, KeyPressEventArgs e)
        {
            Location = new Point(Location.X, Location.Y + 1);
        }

        private void HookManager_MouseClickExt(object sender, MouseEventArgs e)
        {
            Invalidate();
            //  Console.WriteLine(e.Button);
        }

        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            _currentState[e.KeyData] = false;
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_currentState.ContainsKey(e.KeyData))
            {
                _currentState.Add(e.KeyData, true);
            }
            else
            {
                _currentState[e.KeyData] = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        //  delegate void Hook();

        private bool isPressed(Keys k)
        {
            return _currentState[k] && !_pastState[k];
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // this.Location = Cursor.Position;
            if (_start != null)
            {
                var current = (Cursor.Position);
                Size = new Size(Math.Abs(current.X - _start.Value.X), Math.Abs(current.Y - _start.Value.Y));
                Console.WriteLine(Size.Width);
                Location = new Point(current.X < _start.Value.X ? current.X : _start.Value.X, current.Y < _start.Value.Y ? current.Y : _start.Value.Y);
                //_selected = new Rectangle(current.X < _start.Value.X ? current.X : _start.Value.X, current.Y < _start.Value.Y ? current.Y : _start.Value.Y, Math.Abs(current.X - _start.Value.X), Math.Abs(current.Y - _start.Value.Y));
                //e.Graphics.DrawRectangle(Pens.Black, _selected);
            }
            //this.Invalidate()
            //Update();
            // base.OnPaint(e);
        }

        private static void TryUnsubscribeFromGlobalMouseEvents()
        {
            ForceUnsunscribeFromGlobalMouseEvents();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                _trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        /*   private static void EnsureSubscribedToGlobalMouseEvents()
           {
               // install Mouse hook only if it is not installed and must be installed
               if (_mouseHookHandle == 0)
               {
                   //See comment of this field. To avoid GC to clean it up.
                   _mouseDelegate = MouseHookProc;
                   //install hook
                   _mouseHookHandle = SetWindowsHookEx(
                       WH_MOUSE_LL,
                       _mouseDelegate,
                             System.Diagnostics.Process.GetCurrentProcess().MainModule.BaseAddress,
                       0);
                   //If SetWindowsHookEx fails.
                   if (_mouseHookHandle == 0)
                   {
                       //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set.
                       int errorCode = Marshal.GetLastWin32Error();
                       //do cleanup

                       //Initializes and throws a new instance of the Win32Exception class with the specified error.
                       // throw new Win32Exception(errorCode);
                   }
               }
           }
           */

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new UploaderForm());
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            //
            // uploader
            //
            BackColor = Color.DimGray;
            ClientSize = new Size(284, 262);
            FormBorderStyle = FormBorderStyle.None;
            Name = "UploaderForm";
            Opacity = 0.3D;
            Load += uploader_Load;
            MouseDown += test_MouseDown;
            MouseUp += test_MouseUp;
            ResumeLayout(false);
        }

        private void test_MouseDown(object sender, MouseEventArgs e)
        {
            _start = new Point(e.X, e.Y);
        }

        private void test_MouseUp(object sender, MouseEventArgs e)
        {
            _start = null;
        }

        private void uploader_Load(object sender, EventArgs e)
        {
        }

        private struct KBDLLHOOKSTRUCT
        {
            private int dwExtraInfo;
            public int flags;
            private int scanCode;
            private int time;
            public int vkCode;
        }

        private delegate IntPtr HookHandlerDelegate(
            int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
    }
}