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
using Microsoft.Win32;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace AppProjectBD
{
    /// <summary>
    /// Lógica interna para ZakazMWindow1.xaml
    /// </summary>
    public partial class ZakazMWindow1 : Excel.Window
    {
        OracleConnection con = null;
        public ZakazMWindow1()
        {
            setConnection();
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
        private void UpdateComboboxZ()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ЛОГИН FROM ПОЛЬЗОВАТЕЛЬ WHERE РОЛЬ ='Заказчик' ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbZakachiq.Items.Add(functiusers);
            }
        }
        private void UpdateComboboxM()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ЛОГИН FROM ПОЛЬЗОВАТЕЛЬ WHERE РОЛЬ ='Менеджер' ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbManager.Items.Add(functiusers);
            }
        }
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT НОМЕР, ДАТА, ЭТП_ВЫПОЛНЕНИЯ, ЗАКАЧИК, МЕНЕДЖЕР,СТОИМОСТЬ FROM ЗАКАЗ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            dataGradeZakaz.ItemsSource = dt.DefaultView;
            dr.Close();

            lbNumberZ.Content = "Сейчас есть " + dt.Rows.Count + " заказов";
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }
        private void AUD(String sql_stmt, int state)
        {
            
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            string msg = "";

            switch (state)
            {
                case 0:
                    msg = "Успешно добавлен!";
                    cmd.Parameters.Add("ЗАКАЧИК", OracleDbType.Varchar2, 25).Value = cbZakachiq.SelectedItem.ToString() ;
                    cmd.Parameters.Add("ДАТА", OracleDbType.Date, 7).Value = dataZakaz.SelectedDate;
                    cmd.Parameters.Add("СТАТУС", OracleDbType.Varchar2, 25).Value = cbStatus.SelectedItem.ToString();
                    cmd.Parameters.Add("МАНАЖЕР", OracleDbType.Varchar2, 25).Value =cbManager.SelectedItem.ToString() ;
                    cmd.Parameters.Add("СТОИМОСТЬ", OracleDbType.Double, 30).Value = Double.Parse(tbStoimost.Text);
                    break;

                case 1:
                    msg = "Успешно обновлен изделий";
                    cmd.Parameters.Add("ЗАКАЧИК", OracleDbType.Varchar2, 25).Value = cbZakachiq.SelectedItem.ToString();
                    cmd.Parameters.Add("ДАТА", OracleDbType.Date, 7).Value = dataZakaz.SelectedDate;
                    cmd.Parameters.Add("СТАТУС", OracleDbType.Varchar2, 25).Value = cbStatus.SelectedItem.ToString();
                    cmd.Parameters.Add("МАНАЖЕР", OracleDbType.Varchar2, 25).Value = cbManager.SelectedItem.ToString();
                    cmd.Parameters.Add("СТОИМОСТЬ", OracleDbType.Double, 30).Value = Double.Parse(tbStoimost.Text);
                    cmd.Parameters.Add("НОМЕР", OracleDbType.Double, 30).Value = Double.Parse(tbNumber.Text);

                    break;

                case 2:
                    msg = "Успешно удален изделий!";
                    cmd.Parameters.Add("НОМЕР", OracleDbType.Double, 30).Value = Double.Parse(tbNumber.Text);
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
        private void dataGradeZakaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                tbNumber.Text = dr["НОМЕР"].ToString();
                cbZakachiq.Text = dr["ЗАКАЧИК"].ToString();
                dataZakaz.Text = dr["ДАТА"].ToString();
                cbStatus.Text = dr["ЭТП_ВЫПОЛНЕНИЯ"].ToString();
                cbManager.Text = dr["МЕНЕДЖЕР"].ToString();
                tbStoimost.Text = dr["СТОИМОСТЬ"].ToString();

                btAdd.IsEnabled = false;
               btUpdade.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
        }
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO ЗАКАЗ VALUES(DEFAULT,:ДАТА,:СТАТУС,:ЗАКАЧИК,:МАНАЖЕР,:СТОИМОСТЬ)";

            this.AUD(sql, 0);
            updateDateGrid();
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
            btAdd.IsEnabled = true;
        }
        public void resetAll()
        {
            cbZakachiq.Text = "";
            dataZakaz.Text = "";
            cbStatus.Text = "";
            cbManager.Text = "";
            tbStoimost.Text = "";
            tbNumber.Text = "";
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            String sql = "DELETE FROM ЗАКАЗ " +
                " WHERE НОМЕР=:НОМЕР";
            this.AUD(sql, 2);
            this.resetAll();
            updateDateGrid();
        }

        private void btConstrutorIzdelie_Click(object sender, RoutedEventArgs e)
        {
            BuildOrder o =new BuildOrder();
            o.Show();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btUpdade_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE ЗАКАЗ SET ДАТА=:ДАТА, ЭТП_ВЫПОЛНЕНИЯ=:СТАТУС, ЗАКАЧИК=:ЗАКАЧИК,МЕНЕДЖЕР=:МАНАЖЕР,СТОИМОСТЬ=:СТОИМОСТЬ " +
                "WHERE НОМЕР=:НОМЕР";
            this.AUD(sql, 1);
            this.resetAll();
            updateDateGrid();

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            UpdateComboboxM();
            UpdateComboboxZ();
            updateDateGrid();

            cbStatus.Items.Add("Новый");
            cbStatus.Items.Add("Ожидает");
            cbStatus.Items.Add("Обработка");
            cbStatus.Items.Add("Отклонен");
            cbStatus.Items.Add("К оплате");
            cbStatus.Items.Add("Оплачен");
            cbStatus.Items.Add("Раскрой");
            cbStatus.Items.Add("Готов");
        }

        private void btPrint_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dataGradeZakaz.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = dataGradeZakaz.Columns[j].Header;
            }
            for (int i = 0; i < dataGradeZakaz.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < dataGradeZakaz.Items.Count; j++)
                {
                    TextBlock b = dataGradeZakaz.Columns[i].GetCellContent(dataGradeZakaz.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        dynamic Excel.Window.Activate()
        {
            throw new NotImplementedException();
        }

        public dynamic ActivateNext()
        {
            throw new NotImplementedException();
        }

        public dynamic ActivatePrevious()
        {
            throw new NotImplementedException();
        }

        public bool Close(object SaveChanges, object Filename, object RouteWorkbook)
        {
            throw new NotImplementedException();
        }

        public dynamic LargeScroll(object Down, object Up, object ToRight, object ToLeft)
        {
            throw new NotImplementedException();
        }

        public Excel.Window NewWindow()
        {
            throw new NotImplementedException();
        }

        public dynamic _PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName)
        {
            throw new NotImplementedException();
        }

        public dynamic PrintPreview(object EnableChanges)
        {
            throw new NotImplementedException();
        }

        public dynamic ScrollWorkbookTabs(object Sheets, object Position)
        {
            throw new NotImplementedException();
        }

        public dynamic SmallScroll(object Down, object Up, object ToRight, object ToLeft)
        {
            throw new NotImplementedException();
        }

        public int PointsToScreenPixelsX(int Points)
        {
            throw new NotImplementedException();
        }

        public int PointsToScreenPixelsY(int Points)
        {
            throw new NotImplementedException();
        }

        public dynamic RangeFromPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void ScrollIntoView(int Left, int Top, int Width, int Height, object Start)
        {
            throw new NotImplementedException();
        }

        public dynamic PrintOut(object From, object To, object Copies, object Preview, object ActivePrinter, object PrintToFile, object Collate, object PrToFileName)
        {
            throw new NotImplementedException();
        }

        public Excel.Application Application => throw new NotImplementedException();

        public XlCreator Creator => throw new NotImplementedException();

        dynamic Excel.Window.Parent => throw new NotImplementedException();

        public Range ActiveCell => throw new NotImplementedException();

        public Chart ActiveChart => throw new NotImplementedException();

        public Pane ActivePane => throw new NotImplementedException();

        public dynamic ActiveSheet => throw new NotImplementedException();

        public dynamic Caption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayFormulas { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayGridlines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayHeadings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayHorizontalScrollBar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayOutline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool _DisplayRightToLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayVerticalScrollBar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayWorkbookTabs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayZeros { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool EnableResize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool FreezePanes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int GridlineColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public XlColorIndex GridlineColorIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Index => throw new NotImplementedException();

        public string OnWindow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Panes Panes => throw new NotImplementedException();

        public Range RangeSelection => throw new NotImplementedException();

        public int ScrollColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ScrollRow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Sheets SelectedSheets => throw new NotImplementedException();

        public dynamic Selection => throw new NotImplementedException();

        public bool Split { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SplitColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double SplitHorizontal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SplitRow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double SplitVertical { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double TabRatio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public XlWindowType Type => throw new NotImplementedException();

        public double UsableHeight => throw new NotImplementedException();

        public double UsableWidth => throw new NotImplementedException();

        public bool Visible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Range VisibleRange => throw new NotImplementedException();

        public int WindowNumber => throw new NotImplementedException();

        XlWindowState Excel.Window.WindowState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public dynamic Zoom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public XlWindowView View { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayRightToLeft { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SheetViews SheetViews => throw new NotImplementedException();

        public dynamic ActiveSheetView => throw new NotImplementedException();

        public bool DisplayRuler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoFilterDateGrouping { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DisplayWhitespace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Hwnd => throw new NotImplementedException();
    }
}
