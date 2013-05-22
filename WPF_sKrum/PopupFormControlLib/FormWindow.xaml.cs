using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class FormWindow : Window
    {
        public bool Success { get; private set; }

        public FormWindow()
        {
            InitializeComponent();
            this.Success = false;
        }

        private void Close_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
