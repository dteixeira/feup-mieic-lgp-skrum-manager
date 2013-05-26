namespace PopupSelectionControlLib
{
    public interface IFormPage
    {
        string PageName { get; set; }
        string PageTitle { get; set; }
        object PageValue { get; set; }
    }
}
