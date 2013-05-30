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
    /// Interaction logic for GraphicColumnControl.xaml
    /// </summary>
    public partial class GraphicColumnControl : UserControl
    {
        public GraphicColumnControl(List<KeyValuePair<string, int>> data)
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

        private void showColumnChart(List<KeyValuePair<string, int>> data)
        {
            columnChart.DataContext = data;
        }
    }
}