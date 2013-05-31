using System.Windows.Controls;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for TextBoxPage.xaml
    /// </summary>
    public partial class PasswordBoxPage : UserControl, IFormPage
    {
        public PasswordBoxPage()
        {
            this.InitializeComponent();
            this.Changed = false;
        }

        public bool Changed { get; set; }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        public string DefaultValue
        {
            set
            {
                this.PageValue = value;
                this.TextValue.Password = value;
                this.Changed = false;
            }
        }

        private void TextValue_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.PageValue = this.TextValue.Password;
            this.Changed = true;
        }
    }
}