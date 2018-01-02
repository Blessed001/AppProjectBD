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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace AppProjectBD
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(string login)
        {
            InitializeComponent();
            lbLogined.Content = login;
        }


        private void menuFile12_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow l = new LoginWindow();
            l.Show();
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            IzdeleieWindow i = new IzdeleieWindow();
            i.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            ZakazMWindow1 z = new ZakazMWindow1();
            z.Show();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            skladWindow s = new skladWindow();
            s.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OrderWindowM o = new OrderWindowM(lbLogined.Content.ToString());
            o.Show();
        }
    }
}
