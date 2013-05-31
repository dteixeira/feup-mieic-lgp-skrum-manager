using System.Windows.Controls;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for TestAreaPage.xaml
    /// </summary>
    public partial class TextAreaPage : UserControl, IFormPage
    {
        public TextAreaPage()
        {
            this.InitializeComponent();
        }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        public string DefaultValue
        {
            set
            {
                this.PageValue = value;
                this.TextValue.Text = value;
            }
        }

        private void TextValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.PageValue = this.TextValue.Text;
        }
    }
}