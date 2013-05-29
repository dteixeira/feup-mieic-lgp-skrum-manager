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
    /// Interaction logic for SprintControl.xaml
    /// </summary>
    public partial class SprintControl : UserControl
    {
        private DateTime sprintBeginDate;
        private DateTime sprintEndDate;

        public SprintControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public DateTime SprintBeginDate
        {
            get { return this.sprintBeginDate; }
            set { this.sprintBeginDate = value; }
        }

        public DateTime SprintEndDate
        {
            get { return this.sprintEndDate; }
            set { this.sprintEndDate = value; }
        }

        public string BeginDateText
        {
            get { return this.sprintBeginDate.ToString(); }
        }

        public string EndDateText
        {
            get { return this.sprintEndDate.ToString(); }
        }
    }
}
