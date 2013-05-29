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
        private DateTime? sprintEndDate;
        private int sprintNumber;

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

        public DateTime? SprintEndDate
        {
            get { return this.sprintEndDate; }
            set { this.sprintEndDate = value; }
        }

        public int SprintNumber
        {
            get { return this.sprintNumber; }
            set { this.sprintNumber = value; }
        }

        public string BeginDateText
        {
            get { return this.sprintBeginDate.ToShortDateString(); }
        }

        public string SprintName
        {
            get { return "SPRINT" + this.sprintNumber.ToString(); }
        }

        public string EndDateText
        {
            get
            {
                if (this.sprintEndDate.HasValue)
                {
                    return this.sprintEndDate.Value.ToShortDateString();
                }
                else
                {
                    return "Indisponível";
                }
            }
        }
    }
}
