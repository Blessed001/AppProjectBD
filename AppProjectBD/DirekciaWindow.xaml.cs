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
    /// Lógica interna para DirekciaWindow.xaml
    /// </summary>
    public partial class DirekciaWindow : Window
    {
        public DirekciaWindow()
        {
            InitializeComponent();
        }
        public DirekciaWindow(string login)
        {
            InitializeComponent();
            lbLogined.Content = "Здравствуйте, " + login;
        }
        private void menuFile1_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow l = new LoginWindow();
            l.Show();
            this.Close();
        }

        private void mIzdelie_Click(object sender, RoutedEventArgs e)
        {
            IzdeleieWindow i = new IzdeleieWindow();
            i.Show();
        }

        private void menuFile10_Click(object sender, RoutedEventArgs e)
        {
            skladWindow s =new skladWindow();
            s.Show();
        }
    }
}
