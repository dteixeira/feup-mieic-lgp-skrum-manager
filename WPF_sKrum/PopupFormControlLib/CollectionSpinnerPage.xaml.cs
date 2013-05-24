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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for CollectionSpinnerPage.xaml
    /// </summary>
    public partial class CollectionSpinnerPage : UserControl, IFormPage
    {
        public CollectionSpinnerPage()
        {
            InitializeComponent();
            this.PageValue = 0.0;
            this.NumericCollectionSpinner.SpinnerChangedEvent += new GenericControlLib.NumericCollectionSpinnerControl.SpinnerChangedHandler(SpinnerChangeHandler);
        }

        public List<double> ValueCollection
        {
            set { this.NumericCollectionSpinner.ValueCollection = value; }
        }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        public void SpinnerChangeHandler(double value)
        {
            this.PageValue = value;
        }
    }
}
