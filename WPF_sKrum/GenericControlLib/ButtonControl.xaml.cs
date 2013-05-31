using System.Windows.Controls;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ButtonControl : UserControl
    {
        private int buttonFontSize = 50;
        private string buttonText = "BUTTON";

        public ButtonControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public int ButtonFontSize
        {
            get { return this.buttonFontSize; }
            set { this.buttonFontSize = value; }
        }

        public string ButtonText
        {
            get { return this.buttonText; }
            set { this.buttonText = value; }
        }
    }
}