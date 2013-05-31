using GenericControlLib;
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
using System.Windows.Threading;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for StoryOrderPage.xaml
    /// </summary>
    public partial class StoryOrderPage : UserControl, IFormPage
    {
        private int min;
        private int max;
        private int increment;
        private int spinnerValue;
        private DispatcherTimer minusTimer;
        private DispatcherTimer plusTimer;
        private DispatcherTimer incrementTimer;
        private bool plusPressed;
        private int incrementedCount;
        private List<SimpleStoryControl> stories;

        public StoryOrderPage(List<SimpleStoryControl> storiesin, SimpleStoryControl selectedStory, int selectedIndex)
        {
            this.InitializeComponent();
            this.stories = storiesin;
            this.SelectedStoryContainer.Children.Add(selectedStory);
            this.min = 0;
            this.max = storiesin.Count;
            this.increment = 1;

            this.SpinnerValue = selectedIndex;
            this.PageValue = selectedIndex;

            this.minusTimer = new DispatcherTimer();
            this.minusTimer.Interval = TimeSpan.FromSeconds(1);
            this.minusTimer.Tick += new EventHandler(MinusButtonHoverHandler);
            this.plusTimer = new DispatcherTimer();
            this.plusTimer.Interval = TimeSpan.FromSeconds(1);
            this.plusTimer.Tick += new EventHandler(PlusButtonHoverHandler);
            this.incrementTimer = new DispatcherTimer();
            this.incrementTimer.Tick += new EventHandler(IncrementHandler);
            this.plusPressed = false;

        }

        public int Min
        {
            set
            {
                this.min = value;
            }
        }

        public int Max
        {
            set { this.max = value; }
        }

        public int Increment
        {
            set { this.increment = value; }
            get { return this.increment; }
        }

        public int SpinnerValue
        {
            get { return this.spinnerValue; }
            set
            {
                this.spinnerValue = value;
                this.PageValue = value;
                if (this.stories != null)
                {
                    this.UpStoryContainer.Children.Clear();
                    this.DownStoryContainer.Children.Clear();
                    if (this.spinnerValue > this.min)
                        this.UpStoryContainer.Children.Add(stories[this.spinnerValue - 1]);
                    if (this.spinnerValue < this.max)
                        this.DownStoryContainer.Children.Add(stories[this.spinnerValue]);
                }
            }
        }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        private void MinusButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.minusTimer.Start();
        }

        private void MinusButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.minusTimer.Stop();
            this.incrementTimer.Stop();
        }

        private void PlusButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.plusTimer.Start();
        }

        private void PlusButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.plusTimer.Stop();
            this.incrementTimer.Stop();
        }

        private void MinusButtonHoverHandler(object sender, EventArgs e)
        {
            this.minusTimer.Stop();
            this.plusPressed = false;
            this.incrementTimer.Interval = TimeSpan.FromSeconds(0.3);
            this.incrementTimer.Start();
            this.incrementedCount = 0;
        }

        private void PlusButtonHoverHandler(object sender, EventArgs e)
        {
            this.plusTimer.Stop();
            this.plusPressed = true;
            this.incrementTimer.Interval = TimeSpan.FromSeconds(0.3);
            this.incrementTimer.Start();
            this.incrementedCount = 0;
        }

        private void IncrementHandler(object sender, EventArgs e)
        {
            // Increase speed.
            this.incrementedCount++;
            if (this.incrementedCount == 15)
            {
                this.incrementTimer.Stop();
                this.incrementTimer.Interval = TimeSpan.FromSeconds(0.1);
                this.incrementTimer.Start();
            }

            if (this.plusPressed && this.spinnerValue + this.increment <= this.max)
            {
                this.SpinnerValue += this.Increment;
            }
            else if (!this.plusPressed && this.spinnerValue - this.increment >= this.min)
            {
                this.SpinnerValue -= this.Increment;
            }
        }

        private void MinusButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.plusPressed = false;
            this.IncrementHandler(sender, e);
            this.MinusButtonHoverHandler(sender, e);
        }

        private void MinusButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MinusButton_MouseLeave(sender, e);
            this.MinusButton_MouseEnter(sender, e);
        }

        private void PlusButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.plusPressed = true;
            this.IncrementHandler(sender, e);
            this.PlusButtonHoverHandler(sender, e);
        }

        private void PlusButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.PlusButton_MouseLeave(sender, e);
            this.PlusButton_MouseEnter(sender, e);
        }
    }
}