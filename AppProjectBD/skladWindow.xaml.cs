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
    /// Lógica interna para skladWindow.xaml
    /// </summary>
    public partial class skladWindow : Excel.Window
    {
        double price,pricef, pUnit;

        OracleConnection con = null;
        public skladWindow()
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
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM СКД_ТКАНИ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            dataGradTkani.ItemsSource = dt.DefaultView;
            dr.Close();

            lbCount1.Content = "На склад есть " + dt.Rows.Count + " ткани";

        }
        private void UpdateCombobox()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ FROM ТКАНЬ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbArtikulTkani.Items.Add(functiusers);
            }
        }
        private void GetPrice()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM ТКАНЬ WHERE АРТИКУЛ ="+"'"+cbArtikulTkani.SelectedItem+"'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                
                double dlina = double.Parse((dr["ДЛИНА"].ToString()));
                double chirina = double.Parse((dr["ШИРИНА"].ToString()));
                price = double.Parse((dr["ЦЕНА"].ToString()));
                pUnit = price / dlina;
                lbPriceUnit.Content = "Ценна за Ширина "+chirina.ToString() +"(mm/см/м)" + " X Длина 1(mm / см / м) = " + pUnit.ToString() +" руб.";
                
            }
        }

        private void cbArtikulTkani_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Tkani/" + cbArtikulTkani.SelectedItem.ToString() + ".jpg");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageTkani.Background = _ib;
            }
            catch (Exception)
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Tkani/notfoundimage.png");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageTkani.Background = _ib;
            }
            GetPrice();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCombobox();
            UpdateCombobox1();
            updateDateGrid();
            updateDateGrid1();
            cbDli.Items.Add("мм");
            cbDli.Items.Add("см");
            cbDli.Items.Add("м");
            cbKol.Items.Add("штук");
            cbKol.Items.Add("кг");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }
        private void resetAll()
        {
            cbArtikulTkani.Text = "";
            tbRulon.Text = "";
            tbDlina.Text = "";
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
                    cmd.Parameters.Add("АРТ_ТКАНИ", OracleDbType.Varchar2, 25).Value = cbArtikulTkani.SelectedItem.ToString();
                    cmd.Parameters.Add("РУЛОН", OracleDbType.Double, 10).Value = Double.Parse(tbRulon.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);

                    break;

            }

            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Пажалуйста проверяете все поли");
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO СКД_ТКАНИ(РУЛОН, АРТ_ТКАНИ, ДЛИНА)" +
               "VALUES(:РУЛОН, :АРТ_ТКАНИ, :ДЛИНА)";
            this.AUD(sql, 0);
            updateDateGrid();
        }
        private void tbDlina_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (tbDlina.Text == "")
            {
                lbPrice.Content =  "0 руб.";
            }
            else
            {
                lbPrice.Content = pUnit * double.Parse(tbDlina.Text) + " руб.";
            }

        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //================================================================================================================================
        private void updateDateGrid1()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM СКД_ФУРНИТУРЫ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            DataGradFurniture.ItemsSource = dt.DefaultView;
            dr.Close();
            lbCount2.Content = "На склад есть " + dt.Rows.Count + " фурнитуры";
        }
        private void UpdateCombobox1()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ FROM ФУРНИТУРА";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbArtikulFurniture.Items.Add(functiusers);
            }
        }
        private void GetPriceF()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM ФУРНИТУРА WHERE АРТИКУЛ =" + "'" + cbArtikulFurniture.SelectedItem + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {

                pricef = double.Parse((dr["ЦЕНА"].ToString()));
                lbPriceUnitF.Content = "Ценна за 1(кг/штук) = " + pricef.ToString() + " руб.";

            }
        }
        private void AUD1(String sql_stmt, int state)
        {
            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;

            switch (state)
            {
                case 0:
                    msg = "Успешно добавлен фурнитуры!";
                    cmd.Parameters.Add("АРТ_ФУРНИТУРЫ", OracleDbType.Varchar2, 25).Value = cbArtikulFurniture.SelectedItem.ToString();
                    cmd.Parameters.Add("ПАРТИЯ", OracleDbType.Varchar2, 25).Value = tbPartia.Text;
                    cmd.Parameters.Add("КОЛИЧЕСТВО", OracleDbType.Double, 30).Value = Double.Parse(tbCalitchestva.Text);

                    break;

            }

            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Пажалуйста проверяете все поли");
            }
        }

        private void cbArtikulFurniture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Furniture/" + cbArtikulFurniture.SelectedItem.ToString() + ".jpg");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageFurniture.Background = _ib;
            }
            catch (Exception)
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Furniture/notfoundimage.png");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageFurniture.Background = _ib;
            }

            GetPriceF();
        }

        private void btAddF_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO СКД_ФУРНИТУРЫ(ПАРТИЯ, АРТ_ФУРНИТУРЫ, КОЛИЧЕСТВО)" +
               "VALUES(:ПАРТИЯ, :АРТ_ФУРНИТУРЫ, :КОЛИЧЕСТВО)";
            this.AUD1(sql, 0);
            updateDateGrid1();
        }

        private void btprint1_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dataGradTkani.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = dataGradTkani.Columns[j].Header;
            }
            for (int i = 0; i < dataGradTkani.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < dataGradTkani.Items.Count; j++)
                {
                    TextBlock b = dataGradTkani.Columns[i].GetCellContent(dataGradTkani.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        private void btprint2_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < DataGradFurniture.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = DataGradFurniture.Columns[j].Header;
            }
            for (int i = 0; i < DataGradFurniture.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < DataGradFurniture.Items.Count; j++)
                {
                    TextBlock b = DataGradFurniture.Columns[i].GetCellContent(DataGradFurniture.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        private void tbCalitchestva_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbCalitchestva.Text == "")
            {
                lbPriceF.Content = "0 руб.";
            }
            else
            {
                lbPriceF.Content = pricef * double.Parse(tbCalitchestva.Text) + " руб.";
            }
        }

        private void btResetF_Click(object sender, RoutedEventArgs e)
        {
            cbArtikulFurniture.Text = "";
            tbCalitchestva.Text = "";
            tbPartia.Text = "";
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
