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
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace AppProjectBD
{
    /// <summary>
    /// Lógica interna para WindowPatupMaterial.xaml
    /// </summary>
    public partial class WindowPatupMaterial : Excel.Window
    {
        OracleConnection con = null;
        public WindowPatupMaterial()
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
        private void tbTsena_TextChanged(object sender, TextChangedEventArgs e)
        {
            double valor1;
            int valor2;
            if (tbTsena.Text == "")
            {
                tbSuma.Text = "0 руб.";
                valor1 = 0;             
            }
            else if (tbKalitchestvo.Text == "")
            {
                tbSuma.Text = "0 руб.";
                valor2 = 0;
            }
            else
            {
                valor1 = double.Parse(tbTsena.Text);
                valor2 = int.Parse(tbKalitchestvo.Text);
                tbSuma.Text = (valor1 * valor2).ToString()+ " руб.";
            }
        }
        private void tbKalitchestvo_TextChanged(object sender, TextChangedEventArgs e)
        {
            double valor1;
            int valor2;
            if (tbTsena.Text=="")
            {
                valor1 = 0;
                tbSuma.Text = "0 руб.";
            }
            else if (tbKalitchestvo.Text == "")
            {
                valor2 = 0;
                tbSuma.Text = "0 руб.";
            }
            else
            {
                valor1 = double.Parse(tbTsena.Text);
                valor2 = int.Parse(tbKalitchestvo.Text);
                tbSuma.Text = (valor1 * valor2).ToString() + " руб.";
            }
            
        }
       
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ID_МАТЕРИАЛ, НАЗВАНИЕ_МАТЕР, КОЛИЧЕСТВА, ЦЕНА, СУММА, ДАТА_ДОБАВ FROM ПС_МАТЕРИАЛ ORDER BY ДАТА_ДОБАВ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            MaterialdataGrade.ItemsSource = dt.DefaultView;
            dr.Close();

            lbCount.Content = "Были найдены " + dt.Rows.Count + " материалы ";
        }
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO ПС_МАТЕРИАЛ(ID_МАТЕРИАЛ, НАЗВАНИЕ_МАТЕР, КОЛИЧЕСТВА, ЦЕНА, СУММА, ДАТА_ДОБАВ)" +
               "VALUES(:ID_МАТЕРИАЛ, :НАЗВАНИЕ_МАТЕР,:КОЛИЧЕСТВА, :ЦЕНА, :СУММА, :ДАТА_ДОБАВ)";
            this.AUD(sql, 0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void UpdateCombobox()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT ДАТА_ДОБАВ FROM ПС_МАТЕРИАЛ ORDER BY ДАТА_ДОБАВ DESC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                DateTime functiusers = dr.GetDateTime(0);
                cbPoisk_date.Items.Add(functiusers);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateDateGrid();
            UpdateCombobox();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            KladovchikWindow k = new KladovchikWindow();
            k.Show();
            this.Close();
        }

        private void btReset_Copy_Click(object sender, RoutedEventArgs e)
        {
            tbKalitchestvo.Text = "";
            tbSuma.Text = "";
            tbTsena.Text = "";
            tbNazvaniaMaterial.Text = "";
            tbId.Text = "";

        }
        private void AUD(String sql_stmt, int state)
        {
            double valor1 = double.Parse(tbTsena.Text);
            int valor2 = int.Parse(tbKalitchestvo.Text);
            double sum = (valor1 * valor2);

            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            switch (state)
            {
                case 0:
                    msg = "Успешно добавлен материал!";
                    cmd.Parameters.Add("НАЗВАНИЕ_МАТЕР", OracleDbType.Varchar2, 25).Value = tbNazvaniaMaterial.Text;
                    cmd.Parameters.Add("КОЛИЧЕСТВА", OracleDbType.Int32, 30).Value = Int32.Parse(tbKalitchestvo.Text);
                    cmd.Parameters.Add("ЦЕНА", OracleDbType.Double, 30).Value = Double.Parse(tbTsena.Text);
                    cmd.Parameters.Add("СУММА", OracleDbType.Double, 30).Value = sum;
                    cmd.Parameters.Add("ID_МАТЕРИАЛ", OracleDbType.Varchar2, 25).Value = tbId.Text;
                    cmd.Parameters.Add("ДАТА_ДОБАВ", OracleDbType.Date,7).Value = tbDate.SelectedDate; 
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];
 
            for (int j = 0; j < MaterialdataGrade.Columns.Count; j++) 
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true; 
                sheet1.Columns[j + 1].ColumnWidth = 15; 
                myRange.Value2 = MaterialdataGrade.Columns[j].Header;
            }
            for (int i = 0; i < MaterialdataGrade.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < MaterialdataGrade.Items.Count; j++)
                {
                    TextBlock b = MaterialdataGrade.Columns[i].GetCellContent(MaterialdataGrade.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        private void cbPoisk_date_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.BindByName = true;
            cmd.CommandText = "SELECT ID_МАТЕРИАЛ, НАЗВАНИЕ_МАТЕР, КОЛИЧЕСТВА, ЦЕНА, СУММА, ДАТА_ДОБАВ FROM ПС_МАТЕРИАЛ WHERE ДАТА_ДОБАВ=:ДАТА_ДОБАВ";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("ДАТА_ДОБАВ", OracleDbType.Date, 7).Value = cbPoisk_date.SelectedItem;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            MaterialdataGrade.ItemsSource = dt.DefaultView;
            dr.Close();

            lbCount.Content = "Были найдены " + dt.Rows.Count + " материалы ";
        }
        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            cbPoisk_date.Items.Clear();
            UpdateCombobox();
            updateDateGrid();
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
