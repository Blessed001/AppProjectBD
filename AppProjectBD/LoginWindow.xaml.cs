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
    /// Lógica interna para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        OracleConnection con = null;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(1) FROM ПОЛЬЗОВАТЕЛЬ WHERE ЛОГИН=:ЛОГИН AND ПАРОЛЬ=:ПАРОЛЬ AND РОЛЬ=:РОЛЬ";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("ЛОГИН", OracleDbType.Varchar2, 150).Value = txtUsername.Text;
                cmd.Parameters.Add("ПАРОЛЬ", OracleDbType.Varchar2, 150).Value = txtPassword.Password;
                cmd.Parameters.Add("РОЛЬ", OracleDbType.Varchar2, 150).Value = FunctionCBox.SelectedItem.ToString();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 1)
                {
                    MainWindow dashboard = new MainWindow();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username or password is incorrect.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
           
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT РОЛЬ FROM ПОЛЬЗОВАТЕЛЬ";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string functiusers = dr.GetString(0);
                    FunctionCBox.Items.Add(functiusers);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
