using System;
using System.Drawing;

namespace aevvuploader
{
    public interface IVisibleForm
    {
        bool Visible { get; }
        void ToggleVisibility();
        void Show();
        void Hide();
        Point Location { get; set; }
        void RegisterCancellation(Action cancelMe);
        void Invoke(Action action);

        Size Size { get; set; }
    }
}