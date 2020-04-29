using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools.ViewModel;

namespace Tools.pages
{
    /// <summary>
    /// page1.xaml 的交互逻辑
    /// </summary>
    public partial class Index : Page
    {
        public Index()
        {
            InitializeComponent();
            DataContext = new IndexViewModel();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (e.Delta > 0)
                scrollViewer.LineLeft();
            else
                scrollViewer.LineRight();

            e.Handled = true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                var view = border.Tag;
                var window = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
                window.PageUrl = "pack://application:,,," + view;
            }
        }

        private void Window_Closing(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }


}
