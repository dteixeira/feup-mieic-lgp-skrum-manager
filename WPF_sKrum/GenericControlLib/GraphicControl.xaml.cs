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
using System.Windows.Controls.DataVisualization.Charting;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for GraphicControl.xaml
    /// </summary>
    public partial class GraphicControl : UserControl
    {
        public GraphicControl()
        {
            InitializeComponent();
            showColumnChart();
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

        private void showColumnChart()
        {

            //Adicionar valores a introduzir no gráfico
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("1", 60));
            valueList.Add(new KeyValuePair<string, int>("2", 55));
            valueList.Add(new KeyValuePair<string, int>("3", 20));
            valueList.Add(new KeyValuePair<string, int>("4", 20));
            valueList.Add(new KeyValuePair<string, int>("5", 10));

            //Novos valores para a linha adicional
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
            lineChart.Series.Add(lineSeries1);

            /*lineSeries2.Title = "Real";
            lineSeries2.DependentValuePath = "Value";
            lineSeries2.IndependentValuePath = "Key";
            lineSeries2.ItemsSource = valueList;
            //lineChart.Series.Add(lineSeries2);*/

            Style dataPointStyle = GetNewDataPointStyle();
            lineSeries1.DataPointStyle = dataPointStyle;

            this.Default_line_serie.Title = "Real";

            //Setting data for line chart
            lineChart.DataContext = valueList;
        }
    }
}