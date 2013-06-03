using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for GraphicControl.xaml
    /// </summary>
    public partial class GraphicControl : UserControl
    {
        public GraphicControl(List<List<KeyValuePair<string, double>>> data)
        {
            InitializeComponent();
            showColumnChart(data);
        }

        private static Style GetNewDataPointStyle()
        {
            Color gray = Color.FromRgb((byte)36, (byte)36, (byte)37);

            Style style = new Style(typeof(DataPoint));
            Setter st1 = new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(gray));
            Setter st2 = new Setter(DataPoint.BorderBrushProperty, new SolidColorBrush(Colors.Black));

            //Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));
            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);

            //style.Setters.Add(st3);
            style.Setters.Add(st4);
            return style;
        }

        private void showColumnChart(List<List<KeyValuePair<string, double>>> data)
        {
            if (data.Count > 0)
            {
                lineChart.DataContext = data[0];

                for (int i = 1; i < data.Count; i++)
                {
                    LineSeries lineSeries1 = new LineSeries();
                    lineSeries1.DependentValuePath = "Value";
                    lineSeries1.IndependentValuePath = "Key";
                    lineSeries1.ItemsSource = data[i];
                    lineChart.Series.Add(lineSeries1);

                    Style dataPointStyle = GetNewDataPointStyle();
                    lineSeries1.DataPointStyle = dataPointStyle;
                }
            }
        }
    }
}