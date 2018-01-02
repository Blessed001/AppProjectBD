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
    /// Lógica interna para ZakazWindow.xaml
    /// </summary>
    public partial class ZakazWindow : Window
    {
        public ZakazWindow()
        {
            InitializeComponent();
        }
        public ZakazWindow(string login)
        {
            InitializeComponent();
            lbLogined.Content = login;
        }  
        private void menuFile1_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow l = new LoginWindow();
            l.Show();
            this.Close();
        }

        private void menuFile10_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow o = new OrderWindow(lbLogined.Content.ToString());
            o.Show();
        }


        private void menuFile_Click(object sender, RoutedEventArgs e)
        {
            OrderListWindow o = new OrderListWindow();
            o.Show();
        }

        private void menuFile0_Click_1(object sender, RoutedEventArgs e)
        {
            curtWindow c = new curtWindow();
            c.Show();
        }
    }
}
