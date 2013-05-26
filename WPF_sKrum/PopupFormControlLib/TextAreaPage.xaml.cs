using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void TextValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.PageValue = this.TextValue.Text;
        }
	}
}