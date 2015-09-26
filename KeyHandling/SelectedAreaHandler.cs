using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aevvuploader.KeyHandling
{
    class SelectedAreaHandler : IInputHandler
    {
        public void Handle(IVisibleForm form, KeyboardHook hook)
        {
            if (!form.Visible)
            {
                form.Invoke(() => form.Size = new Size(0,0));
                form.Show();
                form.Location = new Point(Cursor.Position.X, Cursor.Position.Y);
                Task.Factory.StartNew(() => ResizeConstantly(form, hook));
            }
        }

        private void ResizeConstantly(IVisibleForm form, KeyboardHook hook)
        {
            bool cancel = false;

            form.RegisterCancellation(() => cancel = true);

            Point baseLoc = form.Location;

            while (!cancel)
            {
                form.Invoke(() => form.Size = new Size(Cursor.Position.X - baseLoc.X, Cursor.Position.Y - baseLoc.Y));
                Thread.Sleep(50);
            }

            form.Invoke(() => form.Hide());
            form.Invoke(() => form.Size = new Size(0, 0));
        }

        public Keys TriggerKey => Keys.D4;
    }
}
