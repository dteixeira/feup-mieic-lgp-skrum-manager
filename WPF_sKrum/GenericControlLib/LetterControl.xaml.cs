using System.Windows.Controls;
using System.Windows.Shapes;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for LetterControl.xaml
    /// </summary>
    public partial class LetterControl : UserControl
    {
        private string letterText = "A";
        private int letterSize = 100;

        public LetterControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string LetterText
        {
            get { return letterText; }
            set { letterText = value; }
        }

        public int LetterSize
        {
            get { return letterSize; }
            set { letterSize = value; }
        }

        public string BackgroundRectangleStyle
        {
            set
            {
                this.RectBackground.SetResourceReference(Rectangle.StyleProperty, value);
            }
        }

        public string LetterStyle
        {
            set
            {
                this.Letter.SetResourceReference(TextBlock.StyleProperty, value);
            }
        }
    }
}