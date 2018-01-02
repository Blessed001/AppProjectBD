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
    /// Lógica interna para BuildOrder.xaml
    /// </summary>
    public partial class BuildOrder : Excel.Window
    {
        OracleConnection con = null;
        public BuildOrder()
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
        private void Window_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateComboboxT();
            UpdateComboboxF();
            UpdateComboboxI();
            updateDateGrid();
            cbChe.Items.Add("мм");
            cbChe.Items.Add("см");
            cbChe.Items.Add("м");
            cbDli.Items.Add("мм");
            cbDli.Items.Add("см");
            cbDli.Items.Add("м");
            cbVis.Items.Add("мм");
            cbVis.Items.Add("см");
            cbVis.Items.Add("м");
            cbKol.Items.Add("штук");
            cbKol.Items.Add("кг");

        }
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            String sql = "INSERT INTO ФУРНИ_ИЗДЛЯ(АРТ_ФУРНИТУРЫ, АРТ_ИЗДЕЛЯ, РАЗМЕЩЕНИЕ, ШИРИНА, ДЛИНА, ПОВТОР, КОЛИЧЕСТВО, АРТ_ТКАНЬ, ВЫСОТА)" +
               "VALUES(:АРТ_ФУРНИТУРЫ, :АРТ_ИЗДЕЛЯ, :РАЗМЕЩЕНИЕ, :ШИРИНА, :ДЛИНА, :ПОВТОР, :КОЛИЧЕСТВО, :АРТ_ТКАНЬ, :ВЫСОТА)";
            this.AUD(sql, 0);

            btAdd.IsEnabled = false;
            btUpdate.IsEnabled = true;
            btDelete.IsEnabled = true;
        }
        private void resetAll()
        {
            tbVisot.Text = "";
            tbChirina.Text = "";
            tbPovtor.Text = "";
            tbDlina.Text = "";
            tbKolichestva.Text = "";
            tbRasmechenia.Text = "";
            cbArtikulFurniture.Text = "";
            cbArtikulIzdelie.Text = "";
            cbArtikulTkani.Text = "";

            btAdd.IsEnabled = true;
            btUpdate.IsEnabled = false;
            btDelete.IsEnabled = false;

        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE ФУРНИ_ИЗДЛЯ SET АРТ_ФУРНИТУРЫ=:АРТ_ФУРНИТУРЫ, АРТ_ИЗДЕЛЯ=:АРТ_ИЗДЕЛЯ, РАЗМЕЩЕНИЕ=:РАЗМЕЩЕНИЕ, ШИРИНА=:ШИРИНА, ДЛИНА=:ДЛИНА, ПОВТОР=:ПОВТОР, КОЛИЧЕСТВО=:КОЛИЧЕСТВО, АРТ_ТКАНЬ=:АРТ_ТКАНЬ, ВЫСОТА=:ВЫСОТА " +
                 "WHERE АРТ_ФУРНИТУРЫ=:АРТ_ФУРНИТУРЫ";
            this.AUD(sql, 1);
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            String sql = "DELETE FROM ФУРНИ_ИЗДЛЯ " +
                " WHERE АРТ_ФУРНИТУРЫ=:АРТ_ФУРНИТУРЫ";
            this.AUD(sql, 2);
            this.resetAll();
        }
        private void updateDateGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТ_ФУРНИТУРЫ, АРТ_ИЗДЕЛЯ, РАЗМЕЩЕНИЕ, ШИРИНА, ДЛИНА, ПОВТОР, КОЛИЧЕСТВО, АРТ_ТКАНЬ, ВЫСОТА FROM ФУРНИ_ИЗДЛЯ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            dataGradeBuided.ItemsSource = dt.DefaultView;
            dr.Close();

            lbCount.Content = "Сейчас у вас есть " + dt.Rows.Count + " конструрование изделие";
        }

        private void dataGradeBuided_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                cbArtikulFurniture.Text = dr["АРТ_ФУРНИТУРЫ"].ToString();
                tbChirina.Text = dr["ШИРИНА"].ToString();
                cbArtikulIzdelie.Text = dr["АРТ_ИЗДЕЛЯ"].ToString();
                tbDlina.Text = dr["ДЛИНА"].ToString();
                tbRasmechenia.Text = dr["РАЗМЕЩЕНИЕ"].ToString();
                tbPovtor.Text = dr["ПОВТОР"].ToString();
                tbKolichestva.Text = dr["КОЛИЧЕСТВО"].ToString();
                cbArtikulTkani.Text = dr["АРТ_ТКАНЬ"].ToString();
                tbVisot.Text = dr["ВЫСОТА"].ToString();

                btAdd.IsEnabled = false;
                btUpdate.IsEnabled = true;
                btDelete.IsEnabled = true;
            }
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
                    msg = "Успешно добавлен!";
                    cmd.Parameters.Add("АРТ_ФУРНИТУРЫ", OracleDbType.Varchar2, 25).Value = cbArtikulFurniture.SelectedItem.ToString();
                    cmd.Parameters.Add("РАЗМЕЩЕНИЕ", OracleDbType.Varchar2, 25).Value = tbRasmechenia.Text;
                    cmd.Parameters.Add("АРТ_ИЗДЕЛЯ", OracleDbType.Varchar2, 25).Value = cbArtikulIzdelie.SelectedItem.ToString();
                    cmd.Parameters.Add("ПОВТОР", OracleDbType.Varchar2, 10).Value = tbPovtor.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("КОЛИЧЕСТВО", OracleDbType.Double, 30).Value = Double.Parse(tbKolichestva.Text);
                    cmd.Parameters.Add("АРТ_ТКАНЬ", OracleDbType.Varchar2, 25).Value = cbArtikulTkani.SelectedItem.ToString();
                    cmd.Parameters.Add("ВЫСОТА", OracleDbType.Double, 30).Value = Double.Parse(tbVisot.Text);
                    break;

                case 1:
                    msg = "Успешно обновлен";
                    cmd.Parameters.Add("АРТ_ФУРНИТУРЫ", OracleDbType.Varchar2, 25).Value = cbArtikulFurniture.SelectedItem.ToString();
                    cmd.Parameters.Add("РАЗМЕЩЕНИЕ", OracleDbType.Varchar2, 25).Value = tbRasmechenia.Text;
                    cmd.Parameters.Add("АРТ_ИЗДЕЛЯ", OracleDbType.Varchar2, 25).Value = cbArtikulIzdelie.SelectedItem.ToString();
                    cmd.Parameters.Add("ПОВТОР", OracleDbType.Varchar2, 10).Value = tbPovtor.Text;
                    cmd.Parameters.Add("ШИРИНА", OracleDbType.Double, 30).Value = Double.Parse(tbChirina.Text);
                    cmd.Parameters.Add("ДЛИНА", OracleDbType.Double, 30).Value = Double.Parse(tbDlina.Text);
                    cmd.Parameters.Add("КОЛИЧЕСТВО", OracleDbType.Double, 30).Value = Double.Parse(tbKolichestva.Text);
                    cmd.Parameters.Add("АРТ_ТКАНЬ", OracleDbType.Varchar2, 25).Value = cbArtikulTkani.SelectedItem.ToString();
                    cmd.Parameters.Add("ВЫСОТА", OracleDbType.Double, 30).Value = Double.Parse(tbVisot.Text);
                    break;

                case 2:
                    msg = "Успешно удален!";
                    cmd.Parameters.Add("АРТ_ФУРНИТУРЫ", OracleDbType.Varchar2, 25).Value = cbArtikulFurniture.SelectedItem.ToString();
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
        //========================================================================================================================
        private void UpdateComboboxT()
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
        }
        private void btAddImageTkani_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                string ImagePath = ofd.FileName;

                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri(ImagePath);
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageTkani.Background = _ib;
            }
            catch(Exception)
            {
                MessageBox.Show("Отменён");
            }
        }
        //===========================================================================================================================================

        private void UpdateComboboxF()
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
        }

        private void btAddImageFurniture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                string ImagePath = ofd.FileName;

                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri(ImagePath);
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageFurniture.Background = _ib;
            }
            catch (Exception)
            {
                MessageBox.Show("Отменён");
            }
        }

        //===========================================================================================================================================
        private void UpdateComboboxI()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT АРТИКУЛ FROM ИЗДЕЛИЕ ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                String functiusers = dr.GetString(0);
                cbArtikulIzdelie.Items.Add(functiusers);
            }
        }
        private void cbArtikulIzdelie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri("pack://Application:,,,/Images/Izdeliya/" + cbArtikulIzdelie.SelectedItem.ToString() + ".jpg");
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageIzdelie.Background = _ib;
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

                stkImageIzdelie.Background = _ib;
            }
        }
        private void btAddImageIzdelie_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                string ImagePath = ofd.FileName;

                Image _image = new Image();
                BitmapImage _bi = new BitmapImage();
                _bi.BeginInit();
                _bi.UriSource = new System.Uri(ImagePath);
                _bi.EndInit();

                _image.Source = _bi;

                ImageBrush _ib = new ImageBrush();
                _ib.ImageSource = _bi;

                stkImageIzdelie.Background = _ib;
             }
            catch(Exception)
            {
                MessageBox.Show("Отменён");
            }
}

        private void btprint_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dataGradeBuided.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = dataGradeBuided.Columns[j].Header;
            }
            for (int i = 0; i < dataGradeBuided.Columns.Count; i++)
            { //www.ahmetcansever.com
                for (int j = 0; j < dataGradeBuided.Items.Count; j++)
                {
                    TextBlock b = dataGradeBuided.Columns[i].GetCellContent(dataGradeBuided.Items[j]) as TextBlock;
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
