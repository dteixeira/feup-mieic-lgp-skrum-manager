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
using PageTransitions;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class FormWindow : Window
    {
        public bool Success { get; private set; }
        public List<IFormPage> FormPages { get; private set; }
        private int CurrentPageIndex { get; set; }
        public IFormPage this[string key]
        {
            get { return this.FormPages.FirstOrDefault(f => f.PageName == key); }
        }

        public FormWindow()
        {
            InitializeComponent();
            this.Success = false;
            this.FormPages = new List<IFormPage>();
            this.CurrentPageIndex = -1;
        }

        private void Close_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            this.NextPage();
        }

        private void NextPage()
        {
            if (this.CurrentPageIndex < this.FormPages.Count - 1)
            {
                this.CurrentPageIndex++;
                this.SetupButtons();
                this.PageTransitionLayer.TransitionType = PageTransitionType.Fade;
                this.PageTransitionLayer.ShowPage((UserControl)this.FormPages[this.CurrentPageIndex]);
            }
        }

        private void PreviousPage()
        {
            if (this.CurrentPageIndex > 0)
            {
                this.CurrentPageIndex--;
                this.SetupButtons();
                this.PageTransitionLayer.TransitionType = PageTransitionType.Fade;
                this.PageTransitionLayer.ShowPage((UserControl)this.FormPages[this.CurrentPageIndex]);
            }
        }

        private void SetupButtons()
        {
            // Set title.
            this.FieldNameLabel.Content = this.FormPages[this.CurrentPageIndex].PageTitle;

            // Set buttons.
            if (this.FormPages.Count <= 1)
            {
                this.PreviousButton.Visibility = System.Windows.Visibility.Hidden;
                this.NextButton.Visibility = System.Windows.Visibility.Hidden;
                this.OkButton.Visibility = System.Windows.Visibility.Visible;
            }
            else if (this.CurrentPageIndex == 0)
            {
                this.PreviousButton.Visibility = System.Windows.Visibility.Hidden;
                this.NextButton.Visibility = System.Windows.Visibility.Visible;
                this.OkButton.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (this.CurrentPageIndex == this.FormPages.Count - 1)
            {
                this.PreviousButton.Visibility = System.Windows.Visibility.Visible;
                this.NextButton.Visibility = System.Windows.Visibility.Hidden;
                this.OkButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.PreviousButton.Visibility = System.Windows.Visibility.Visible;
                this.NextButton.Visibility = System.Windows.Visibility.Visible;
                this.OkButton.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void NextButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.NextPage();
        }

        private void OkButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Success = true;
            this.Close();
        }

        private void PreviousButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PreviousPage();
        }
    }
}
