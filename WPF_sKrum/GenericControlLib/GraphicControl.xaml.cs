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
        public GraphicControl(List<List<KeyValuePair<string, int>>> data)
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

        private void showColumnChart(List<List<KeyValuePair<string, int>>> data)
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

            /*//Novos valores para a linha adicional
            List<KeyValuePair<string, int>> valueList2 = new List<KeyValuePair<string, int>>();
            valueList2.Add(new KeyValuePair<string, int>("1", 60));
            valueList2.Add(new KeyValuePair<string, int>("2", 48));
            valueList2.Add(new KeyValuePair<string, int>("3", 36));
            valueList2.Add(new KeyValuePair<string, int>("4", 24));
            valueList2.Add(new KeyValuePair<string, int>("5", 12));
            valueList2.Add(new KeyValuePair<string, int>("6", 0));

            //Teste-Adicionar mais uma linha ao ultimo grafico de linhas
            LineSeries lineSeries1 = new LineSeries();

            //LineSeries lineSeries2 = new LineSeries();

            lineSeries1.Title = "Estimativa";
            lineSeries1.DependentValuePath = "Value";
            lineSeries1.IndependentValuePath = "Key";
            lineSeries1.ItemsSource = valueList2;
            lineChart.Series.Add(lineSeries1);*/

            /*lineSeries2.Title = "Real";
            lineSeries2.DependentValuePath = "Value";
            lineSeries2.IndependentValuePath = "Key";
            lineSeries2.ItemsSource = valueList;

            //lineChart.Series.Add(lineSeries2);*/

            /*Style dataPointStyle = GetNewDataPointStyle();
            lineSeries1.DataPointStyle = dataPointStyle;*/

            //this.Default_line_serie.Title = "Real";

            //Setting data for line chart
            //lineChart.DataContext = valueList;
        }
    }
}