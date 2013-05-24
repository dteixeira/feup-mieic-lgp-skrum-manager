using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for NumericSpinnerControl.xaml
    /// </summary>
    public partial class NumericSpinnerControl : UserControl
    {
        private double min;
        private double max;
        private double increment;
        private double spinnerValue;
        private DispatcherTimer minusTimer;
        private DispatcherTimer plusTimer;
        private DispatcherTimer incrementTimer;
        private bool plusPressed;
        private int incrementedCount;

        public delegate void SpinnerChangedHandler(double value);

        public event SpinnerChangedHandler SpinnerChangedEvent;

        public NumericSpinnerControl()
        {
            this.InitializeComponent();
            this.min = 1;
            this.max = 999999;
            this.increment = 1;
            this.SpinnerValue = 1;
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

        public double SpinnerValue
        {
            get { return this.spinnerValue; }
            set
            {
                this.spinnerValue = value;
                this.ValueLabel.Content = string.Format("{0:0.#}", value);
            }
        }

        public double Min
        {
            get { return this.min; }
            set
            {
                this.min = value;
                this.SpinnerValue = min;
            }
        }

        public double Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        public double Increment
        {
            get { return this.increment; }
            set { this.increment = value; }
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
                this.Notify(this.spinnerValue);
            }
            else if (!this.plusPressed && this.spinnerValue - this.increment >= this.min)
            {
                this.SpinnerValue -= this.Increment;
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
    }
}