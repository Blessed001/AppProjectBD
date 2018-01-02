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
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace AppProjectBD
{
    /// <summary>
    /// Lógica interna para curtWindow.xaml
    /// </summary>
    public partial class curtWindow : Window
    {
        OracleConnection con = null;
        public curtWindow()
        {
            this.setConnection();
            InitializeComponent();
        }
        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Соединение к баз данных не может быть установлено");
            }
        }
        private void UpdateComboboxI()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТ_ИЗДЕЛЯ FROM ЗАК_ИЗДЕЛЯ ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbIzdelie.Items.Add(functiusers);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateComboboxI();
        }

        private void cbIzdelie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Izdeliya/" + cbIzdelie.SelectedItem.ToString() + ".jpg");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkIzdelia.Background = _ib;
            }
            catch (Exception)
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Izdeliya/notfoundimage.png");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkIzdelia.Background = _ib;
            }
        }

        private void btAdd1_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ, НАИМЕНОВАНИЕ, ШИРИНА, ДЛИНА, КОМНТАРИЙ FROM ИЗДЕЛИЕ WHERE АРТИКУЛ ='" + cbIzdelie.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                String functiusers = dr.GetString(1);
                tbZaka1.Text = functiusers;
            }
            OracleCommand cmd1 = con.CreateCommand();
            cmd1.CommandText = "SELECT * FROM ЗАК_ИЗДЕЛЯ WHERE АРТ_ИЗДЕЛЯ='" + cbIzdelie.SelectedItem.ToString() + "'";
            cmd1.CommandType = CommandType.Text;
            OracleDataReader dr1 = cmd1.ExecuteReader();
            if (dr1.Read())
            {
                Int32 functiusers = dr1.GetInt32(2);
                tbkolichestvo1.Text = functiusers.ToString();
            }
        }

        private void btAdd2_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM ИЗДЕЛИЕ WHERE АРТИКУЛ ='" + cbIzdelie.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                String functiusers = dr.GetString(1);
                tbZakaz2.Text = functiusers;
            }
            OracleCommand cmd1 = con.CreateCommand();
            cmd1.CommandText = "SELECT * FROM ЗАК_ИЗДЕЛЯ WHERE АРТ_ИЗДЕЛЯ='" + cbIzdelie.SelectedItem.ToString() + "'";
            cmd1.CommandType = CommandType.Text;
            OracleDataReader dr1 = cmd1.ExecuteReader();
            if (dr1.Read())
            {
                Int32 functiusers = dr1.GetInt32(2);
                tbKolichestvo2.Text = functiusers.ToString();
            }
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Corte2.png");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkZakaz.Background = _ib;


                Image _image1 = new Image();
                BitmapImage _bi1 = new BitmapImage();
                _bi1.BeginInit();
                _bi1.UriSource = new System.Uri("pack://Application:,,,/Images/Corte1.png");
                _bi1.EndInit();

                _image1.Source = _bi1;

                ImageBrush _ib1 = new ImageBrush();
                _ib1.ImageSource = _bi1;

                stkPaket.Background = _ib1;
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }

        private void btReset_Copy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Успешно упокован ткани");
        }
    }
    
}
