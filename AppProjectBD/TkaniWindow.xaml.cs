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
    /// Lógica interna para TkaniWindow.xaml
    /// </summary>
    public partial class TkaniWindow : Window
    {
        OracleConnection con = null;
        public TkaniWindow()
        {
            this.setConnection();
            InitializeComponent();
        }
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ, НАИМЕНОВАНИЕ, ЦВЕТ, СОСТАВ, ШИРИНА, ДЛИНА, ЦЕНА, РИСУНОК FROM ТКАНЬ ORDER BY АРТИКУЛ DESC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            TkanidataGrade.ItemsSource = dt.DefaultView;
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

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO ТКАНЬ(АРТИКУЛ, НАИМЕНОВАНИЕ, ЦВЕТ, СОСТАВ, ШИРИНА, ДЛИНА, ЦЕНА, РИСУНОК)" +
               "VALUES(:АРТИКУЛ, :НАИМЕНОВАНИЕ, :ЦВЕТ, :СОСТАВ, :ШИРИНА, :ДЛИНА, :ЦЕНА, :РИСУНОК)";
            this.AUD(sql, 0);

            btAdd.IsEnabled = false;
            btUpdate.IsEnabled = true;
            btDelete.IsEnabled = true;
        }
        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE ТКАНЬ SET НАИМЕНОВАНИЕ=:НАИМЕНОВАНИЕ, ЦВЕТ=:ЦВЕТ, СОСТАВ=:СОСТАВ,ШИРИНА=:ШИРИНА, ДЛИНА=:ДЛИНА, ЦЕНА=:ЦЕНА, РИСУНОК=:РИСУНОК " +
                 "WHERE АРТИКУЛ=:АРТИКУЛ";
            this.AUD(sql, 1);
        }
        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            String sql = "DELETE FROM ТКАНЬ " +
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
            tbCostav.Text = "";
            tbDlina.Text = "";
            tbNaimenovania.Text = "";
            tbRisunak.Text = "";
            tbTsena.Text = "";
            tbTsvet.Text = "";
            
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
                    msg = "Успешно добавлен ткань!";
                    cmd.Parameters.Add("АРТИКУЛ", OracleDbType.Varchar2, 25).Value = tbArtikul.Text;
                    cmd.Parameters.Add("НАИМЕНОВАНИЕ", OracleDbType.Varchar2, 25).Value = tbNaimenovania.Text;
                    cmd.Parameters.Add("ЦВЕТ", OracleDbType.Varchar2, 25).Value = tbTsvet.Text;
                    cmd.Parameters.Add("СОСТАВ", OracleDbType.Varchar2, 10).Value = tbCostav.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("ЦЕНА", OracleDbType.Double, 30).Value = Double.Parse(tbTsena.Text);
                    cmd.Parameters.Add("РИСУНОК", OracleDbType.Varchar2, 25).Value = tbRisunak.Text;
                    break;

                case 1:
                    msg = "Успешно обновлен ткань";
                    cmd.Parameters.Add("НАИМЕНОВАНИЕ", OracleDbType.Varchar2, 25).Value = tbNaimenovania.Text;
                    cmd.Parameters.Add("ЦВЕТ", OracleDbType.Varchar2, 25).Value = tbTsvet.Text;
                    cmd.Parameters.Add("СОСТАВ", OracleDbType.Varchar2, 25).Value = tbCostav.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("ЦЕНА", OracleDbType.Double, 30).Value = Double.Parse(tbTsena.Text);
                    cmd.Parameters.Add("РИСУНОК", OracleDbType.Varchar2, 25).Value = tbRisunak.Text;

                    cmd.Parameters.Add("АРТИКУЛ", OracleDbType.Varchar2, 25).Value = tbArtikul.Text;
                    break;

                case 2:
                    msg = "Успешно удален ткань!";
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

        private void dataGradeTkani_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                tbArtikul.Text =dr["АРТИКУЛ"].ToString();
                tbChirina.Text = dr["ШИРИНА"].ToString();
                tbCostav.Text = dr["СОСТАВ"].ToString();
                tbDlina.Text = dr["ДЛИНА"].ToString();
                tbNaimenovania.Text = dr["НАИМЕНОВАНИЕ"].ToString();
                tbRisunak.Text = dr["РИСУНОК"].ToString();
                tbTsena.Text = dr["ЦЕНА"].ToString();
                tbTsvet.Text = dr["ЦВЕТ"].ToString();      

                btAdd.IsEnabled = false;
                btUpdate.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
        }     
    }
}
