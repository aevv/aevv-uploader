using System;
using System.Drawing;

namespace aevvuploader
{
    public interface IScreenshottableForm
    {
        bool Visible { get; }
        void ToggleVisibility();
        void Show();
        void Hide();

        Point Location { get; set; }
        Size Size { get; set; }

        void Explode(Action<bool, Rectangle> implosionCallback);

        void Invoke(Action action);

    }
}