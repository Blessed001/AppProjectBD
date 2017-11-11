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
    /// Lógica interna para FurnituraWindow.xaml
    /// </summary>
    public partial class FurnituraWindow : Window
    {
        OracleConnection con = null;
        public FurnituraWindow()
        {
            this.setConnection();
            InitializeComponent();
        }
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ, НАИМЕНОВАНИЕ, ТИП, ШИРИНА, ДЛИНА, ВЕС, ЦЕНА FROM ФУРНИТУРА ORDER BY АРТИКУЛ DESC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            FurnituradataGrade.ItemsSource = dt.DefaultView;
            dr.Close();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateDateGrid();
        }

        private void FurnituradataGrade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                tbArtikul.Text = dr["АРТИКУЛ"].ToString();
                tbChirina.Text = dr["ШИРИНА"].ToString();
                tbTip.Text = dr["ТИП"].ToString();
                tbDlina.Text = dr["ДЛИНА"].ToString();
                tbNaimenovania.Text = dr["НАИМЕНОВАНИЕ"].ToString();
                tbTsena.Text = dr["ЦЕНА"].ToString();
                tbVes.Text = dr["ВЕС"].ToString();

                btAdd.IsEnabled = false;
                btUpdate.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO ФУРНИТУРА(АРТИКУЛ, НАИМЕНОВАНИЕ, ТИП, ШИРИНА, ДЛИНА, ВЕС, ЦЕНА)" +
               "VALUES(:АРТИКУЛ, :НАИМЕНОВАНИЕ, :ТИП, :ШИРИНА, :ДЛИНА, :ВЕС, :ЦЕНА)";
            this.AUD(sql, 0);
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE ФУРНИТУРА SET НАИМЕНОВАНИЕ=:НАИМЕНОВАНИЕ, ТИП=:ТИП, ШИРИНА=:ШИРИНА, ДЛИНА=:ДЛИНА, ВЕС=:ВЕС, ЦЕНА=:ЦЕНА " +
                "WHERE АРТИКУЛ=:АРТИКУЛ";
            this.AUD(sql, 1);
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            String sql = "DELETE FROM ФУРНИТУРА " +
                " WHERE АРТИКУЛ=:АРТИКУЛ";
            this.AUD(sql, 2);
            this.resetAll();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void resetAll()
        {
            tbArtikul.Text = "";
            tbChirina.Text = "";
            tbTip.Text = "";
            tbDlina.Text = "";
            tbNaimenovania.Text = "";
            tbVes.Text = "";
            tbTsena.Text = "";

            btAdd.IsEnabled = true;
            btUpdate.IsEnabled = false;
            btDelete.IsEnabled = false;

        }

        private void AUD(String sql_stmt, int state)
        {
            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            switch (state)
            {
                case 0:
                    msg = "Успешно добавлен фурнитур!";
                    cmd.Parameters.Add("АРТИКУЛ", OracleDbType.Varchar2, 25).Value = tbArtikul.Text;
                    cmd.Parameters.Add("НАИМЕНОВАНИЕ", OracleDbType.Varchar2, 25).Value = tbNaimenovania.Text;
                    cmd.Parameters.Add("ТИП", OracleDbType.Varchar2, 10).Value = tbTip.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("ЦЕНА", OracleDbType.Double, 30).Value = Double.Parse(tbTsena.Text);
                    cmd.Parameters.Add("ВЕС", OracleDbType.Double, 30).Value = Double.Parse(tbVes.Text);
                    break;

                case 1:
                    msg = "Успешно обновлен фурнитур";
                    cmd.Parameters.Add("НАИМЕНОВАНИЕ", OracleDbType.Varchar2, 25).Value = tbNaimenovania.Text;
                    cmd.Parameters.Add("ТИП", OracleDbType.Varchar2, 10).Value = tbTip.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("ЦЕНА", OracleDbType.Double, 30).Value = Double.Parse(tbTsena.Text);
                    cmd.Parameters.Add("ВЕС", OracleDbType.Double, 30).Value = Double.Parse(tbVes.Text);

                    cmd.Parameters.Add("АРТИКУЛ", OracleDbType.Varchar2, 25).Value = tbArtikul.Text;
                    break;

                case 2:
                    msg = "Успешно удален фурнитур!";
                    cmd.Parameters.Add("АРТИКУЛ", OracleDbType.Varchar2, 25).Value = tbArtikul.Text;
                    break;

            }

            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    this.updateDateGrid();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Пажалуйста проверяете все поли");
            }
        }

    }
}
