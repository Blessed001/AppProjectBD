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
        public KladovchikWindow(string login)
        {
            InitializeComponent();
            lbLogined.Content = "Здравствуйте, " + login;
        }
        private void mTikani_Click(object sender, RoutedEventArgs e)
        {
            TkaniWindow t = new TkaniWindow();
            t.Show();
        }

        private void mfurniture_Click(object sender, RoutedEventArgs e)
        {
            FurnituraWindow f = new FurnituraWindow();
            f.Show();
        }

        private void mDocument_Click(object sender, RoutedEventArgs e)
        {
            skladWindow s = new skladWindow();
            s.Show();
        }

        private void menuFile12_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow l = new LoginWindow();
            l.Show();
            this.Close();
        }
    }
}
