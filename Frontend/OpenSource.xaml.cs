using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// OpenSource.xaml 的交互逻辑
    /// </summary>
    public partial class OpenSource : Window
    {
        public OpenSource()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/hanning-wang/AutoRollcallWpf");
        }

        private void Lanzou_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
