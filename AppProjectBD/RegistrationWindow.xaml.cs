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
    /// Lógica interna para RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        OracleConnection con = null;
        public RegistrationWindow()
        {
            this.setConnection();
            InitializeComponent();
            btLogin.IsEnabled = false;
        }
        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Password == tbPassword_again.Password)
            {
                String sql = "INSERT INTO ПОЛЬЗОВАТЕЛЬ(ЛОГИН, ПАРОЛЬ, РОЛЬ)" +
                "VALUES(:ЛОГИН,:ПАРОЛЬ,:РОЛЬ)";
                this.AUD(sql, 0);
                
            }
            else
            {
                MessageBox.Show("Пароли не совпадают");
            }  

        }

        void resetAll()
        {
            tbLogin.Text = "";
            tbPassword.Password = "";
            tbPassword_again.Password = "";
            
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow log = new LoginWindow();
            log.Show();
            this.Close();

        }
        private void AUD(String sql_stmt, int state)
        {

            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;

            switch (state)
            {
                case 0:
                    msg = "Успешно зарегистрирован пользователь!";
                    cmd.Parameters.Add("ЛОГИН", OracleDbType.Varchar2, 150).Value = tbLogin.Text;
                    cmd.Parameters.Add("ПАРОЛЬ", OracleDbType.Varchar2, 150).Value = tbPassword.Password;
                    cmd.Parameters.Add("РОЛЬ", OracleDbType.Varchar2, 150).Value = tbFunction.Text;
                    break;
            }

            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    btLogin.IsEnabled = true;
                    resetAll();
                }
            }
            catch (Exception)
            {
                if (tbLogin.Text != "" && tbPassword.Password == "" || tbLogin.Text == "")
                {
                    MessageBox.Show("Пожалуйста запольняйте все поли!");
                }
                else
                {
                    MessageBox.Show(" Логин знаят, пожалуйста придумайте другой!");
                }
                
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow log = new LoginWindow();
            log.Show();
            this.Close();
        }
    }
}
