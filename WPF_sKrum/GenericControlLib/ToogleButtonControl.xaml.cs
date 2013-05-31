using System.Windows.Controls;
using System.Windows.Media;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ToogleButtonControl : UserControl
    {
        private bool selected = false;
        private int buttonFontSize = 50;
        private string buttonText = "BUTTON";

        public ToogleButtonControl()
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

        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
                if (this.selected)
                {
                    this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x95, 0x95, 0x95));
                    this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                }
                else
                {
                    this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                    this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
                }
            }
        }

        private void OuterBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x64, 0x64, 0x64));
            this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
        }

        private void OuterBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.selected)
            {
                this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x95, 0x95, 0x95));
                this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
            }
            else
            {
                this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
            }
        }
    }
}