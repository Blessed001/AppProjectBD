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

namespace AppProjectBD
{
    /// <summary>
    /// Lógica interna para KladovchikWindow.xaml
    /// </summary>
    public partial class KladovchikWindow : Window
    {
        public KladovchikWindow()
        {
            InitializeComponent();
        }

        private void menuFile1_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow l = new LoginWindow();
            l.Show();
            this.Close();
        }
    }
}
