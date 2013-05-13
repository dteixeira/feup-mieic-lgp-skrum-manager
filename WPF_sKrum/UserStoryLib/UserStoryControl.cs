using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UserStoryLib
{
    public class UserStoryControl : Button
    {
        //Point startpoint;

        #region DependencyPropertys...

        public static readonly DependencyProperty USIDProperty =
            DependencyProperty.Register("USID", typeof(int), typeof(UserStoryControl));

        public static readonly DependencyProperty NomeProperty =
            DependencyProperty.Register("Nome", typeof(string), typeof(UserStoryControl));

        public static readonly DependencyProperty DescricaoProperty =
            DependencyProperty.Register("Descricao", typeof(string), typeof(UserStoryControl));

        public static readonly DependencyProperty EquipaProperty =
           DependencyProperty.Register("Equipa", typeof(string), typeof(UserStoryControl));

        public static readonly DependencyProperty USPrioridadeProperty =
            DependencyProperty.Register("USPrioridade", typeof(Brush), typeof(UserStoryControl));

        public static readonly DependencyProperty EstimativaProperty =
            DependencyProperty.Register("Estimativa", typeof(int), typeof(UserStoryControl));

        #endregion DependencyPropertys...

        #region Properties...

        public int USID
        {
            get { return (int)GetValue(USIDProperty); }
            set { SetValue(USIDProperty, value); }
        }

        public string Nome
        {
            get { return (string)GetValue(NomeProperty); }
            set { SetValue(NomeProperty, value); }
        }

        public string Descricao
        {
            get { return (string)GetValue(DescricaoProperty); }
            set { SetValue(DescricaoProperty, value); }
        }

        public string Equipa
        {
            get { return (string)GetValue(EquipaProperty); }
            set { SetValue(EquipaProperty, value); }
        }

        public Brush USPrioridade
        {
            get { return (Brush)GetValue(USPrioridadeProperty); }
            set { SetValue(USPrioridadeProperty, value); }
        }

        public int Estimativa
        {
            get { return (int)GetValue(EstimativaProperty); }
            set { SetValue(EstimativaProperty, value); }
        }

        #endregion Properties...

        /*
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            startpoint = e.GetPosition(null);
        }

        //TODO esta a ser chamado 2 vezes, fazer com syncrhonized
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startpoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                //this is to prevent another event to enter before DoDragDrop is called
                startpoint = mousePos;
                DataObject data = new DataObject("UserStoryControl", this);
                Console.WriteLine("tmp");
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                e.Handled = true;
            }
        }
        */

        static UserStoryControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UserStoryControl), new FrameworkPropertyMetadata(typeof(UserStoryControl)));
        }
    }
}