namespace PopupSelectionControlLib
{
    public interface IFormPage
    {
        string PageTitle { get; set; }

        object PageValue { get; set; }

        SelectionWindow FormWindow { get; set; }
    }
}