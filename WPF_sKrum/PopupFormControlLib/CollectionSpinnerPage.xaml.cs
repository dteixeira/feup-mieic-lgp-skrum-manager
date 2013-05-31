using System.Collections.Generic;
using System.Windows.Controls;

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