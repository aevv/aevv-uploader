namespace aevvuploader
{
    public interface IVisibleForm
    {
        bool Visible { get; }
        void ToggleVisibility();
        void Show();
        void Hide();
    }
}