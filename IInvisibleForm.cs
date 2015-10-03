using System;
using System.Drawing;

namespace aevvuploader
{
    public interface IInvisibleForm
    {
        bool Visible { get; }
        Point Location { get; set; }
        Size Size { get; set; }
        void ToggleVisibility();
        void Show();
        void Hide();
        void Explode(Action<bool, Rectangle> implosionCallback);
        void Invoke(Action action);
        void Exit();
        void SuccessfulUpload(string url);
    }
}