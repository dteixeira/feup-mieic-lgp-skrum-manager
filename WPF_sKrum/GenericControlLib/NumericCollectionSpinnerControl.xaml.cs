using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for NumericSpinnerControl.xaml
    /// </summary>
    public partial class NumericCollectionSpinnerControl : UserControl
    {
        private double spinnerValue;
        private DispatcherTimer minusTimer;
        private DispatcherTimer plusTimer;
        private DispatcherTimer incrementTimer;
        private bool plusPressed;
        private int currentIndex;
        private List<double> valueCollection;

        public delegate void SpinnerChangedHandler(double value);

        public event SpinnerChangedHandler SpinnerChangedEvent;

        public NumericCollectionSpinnerControl()
        {
            this.InitializeComponent();
            this.currentIndex = 0;
            this.ValueCollection = new List<double>();
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

        public List<double> ValueCollection
        {
            get { return this.valueCollection; }
            set
            {
                this.valueCollection = value;
                if (this.valueCollection.Count > 0)
                {
                    this.SpinnerValue = this.valueCollection[0];
                    this.Notify(this.valueCollection[0]);
                }
            }
        }

        public double SpinnerValue
        {
            get { return this.spinnerValue; }
            set
            {
                this.spinnerValue = value;
                this.ValueLabel.Content = string.Format("{0:0.#}", value);
            }
        }

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
        }

        private void PlusButtonHoverHandler(object sender, EventArgs e)
        {
            this.plusTimer.Stop();
            this.plusPressed = true;
            this.incrementTimer.Interval = TimeSpan.FromSeconds(0.3);
            this.incrementTimer.Start();
        }

        private void IncrementHandler(object sender, EventArgs e)
        {
            if (this.plusPressed && this.currentIndex < this.ValueCollection.Count - 1)
            {
                this.currentIndex++;
                this.SpinnerValue = this.ValueCollection[this.currentIndex];
                this.Notify(this.spinnerValue);
            }
            else if (!this.plusPressed && this.currentIndex > 0)
            {
                this.currentIndex--;
                this.SpinnerValue = this.ValueCollection[this.currentIndex];
                this.Notify(this.spinnerValue);
            }
        }

        private void Notify(double value)
        {
            if (this.SpinnerChangedEvent != null)
            {
                Delegate[] invokeList = this.SpinnerChangedEvent.GetInvocationList();
                foreach (SpinnerChangedHandler handler in invokeList)
                {
                    try
                    {
                        handler(value);
                    }
                    catch (Exception)
                    {
                        this.SpinnerChangedEvent -= handler;
                    }
                }
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