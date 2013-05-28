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

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for PercentageShowerControl.xaml
    /// </summary>
    public partial class PercentageShowerControl : UserControl
    {
        private int done;
        private int todo;

        public int Done
        {
            get { return this.done; }
            set { this.done = value; }
        }

        public int Todo
        {
            get { return this.todo; }
            set { this.todo = value; }
        }

        public double Percentage
        {
            get 
            {
                if (todo + done > 0)
                    return (((double)done) / (todo + done));
                else
                    return 0;
            }
        }

        public string PercentageText
        {
            get
            {
                return ((int)(Percentage*100)).ToString() + "%";
            }
        }

        public string StatsText
        {
            get
            {
                return (done.ToString() + "/" + ((todo + done).ToString()));
            }
        }

        public PercentageShowerControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Bar.Width = this.WholeArea.RenderSize.Width * Percentage;
            if (Percentage > 0.2)
            {
                this.PercentageShower.Text = this.PercentageText;
                this.StatShower.Text = this.StatsText;
            }
        }
    }
}
