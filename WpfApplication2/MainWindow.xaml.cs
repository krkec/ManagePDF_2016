using System;
//using System.Collections.Generic;
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
using System.Windows.Navigation;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int i;
        static int iM;
        static int iS;
        string putanjaIDE;
        string putbaze;
        string putanjadod;
        MySqlConnection con;
        System.Data.DataTable dt;
        System.Data.DataTable dtM;
        System.Data.DataTable dtS;
        System.Data.DataTable dtO;
        public static string Ime;
        public static string Company;
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;
        System.Collections.Generic.List<dok> doks;
        public void ocisti(ref System.Data.DataTable dataTable, ref DataGrid dataGrid)
        {
            if (dataTable!=null&&dataGrid!=null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    dataTable.Rows.Clear();
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            
        }
        public void reset()
        {
            ocisti(ref dt,ref dg);
            ocisti(ref dtM, ref dgM);
            ocisti(ref dtS, ref dgS);
            ocisti(ref dtO, ref dgO);
            putanjadod = "";
            putanjaIDE = "";
            lableIde.Content = "Datoteka nije odabrana";
        }

        public MainWindow()
        {
            InitializeComponent();
            //string e = Encrypt("198.38.82.92");
            string server = "";
            putbaze = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.db3";
            if (System.IO.File.Exists(putbaze))
            {
                System.Data.DataTable dtsetings = citajIzBaze(putbaze, "select* from 'setings' where ID=1");
                if (dtsetings.Rows.Count!=0)
                {
                    tabc.SelectedIndex = Convert.ToInt32(dtsetings.Rows[0].ItemArray[1]);
                    dad.IsChecked = Convert.ToBoolean(dtsetings.Rows[0].ItemArray[2]);
                    server = Decrypt(Convert.ToString(dtsetings.Rows[0].ItemArray[3]));
                }
            }
            string strconn = "Server="+server+"; Database =donatene_lic; Uid = donatene_padmin; Password =2311Luka2602Valentina;connection Timeout=60;";
            try
            {
                con = new MySqlConnection(strconn);
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                {
                    string kode = "2311xLuka.2602xValentina" + getUniqueId();
                    DateTime vrijeme = GetNetworkTime();
                    string q = "SELECT * FROM `Licence` WHERE `UID` = '" + Encrypt(kode) + "'LIMIT 0 , 1";
                    MySqlCommand com = new MySqlCommand(q, con);
                    MySqlDataReader sqr = com.ExecuteReader();
                    System.Data.DataTable dtr = new System.Data.DataTable();
                    dtr.Load(sqr);
                    if (dtr.Rows.Count == 0)
                    {
                        Info inf = new Info();
                        inf.ShowDialog();
                        //upisujem podatke po prvi put
                        DateTime v1 = vrijeme.AddDays(1.00);
                        string u = "INSERT INTO `Licence` (`Ime`, `UID`,`Do`, `Tvrtka`) VALUES ('" + Ime + "','" + Encrypt(kode) + "','" + v1.ToString("yyyy-MM-dd") + "','" + Company + "')";
                        MySqlCommand com1 = new MySqlCommand();
                        com1.CommandText = u;
                        com1.Connection = con;
                        com1.ExecuteNonQuery();
                    }
                    else
                    {
                        //string sss = Convert.ToString(dtr.Rows[0].ItemArray[3]);
                        //DateTime v = convertDateMC(sss);
                        DateTime v = Convert.ToDateTime(dtr.Rows[0].ItemArray[3]);
                        if (vrijeme > v)
                        {
                            MessageBox.Show("Licenca istekla kontanktirajte vlasnika");
                            Application.Current.Shutdown();
                        }
                    }
                    con.Close();
                }
                else
                {
                    //nema pristupa bazi
                    // offile
                    MessageBox.Show("Pristup bazi nije moguć molimo provjerite internet konekciju ili kontaktirajte programera");
                    Application.Current.Shutdown();
                }
            }
            catch (Exception)
            {

                throw;
            }
            dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Ime");
            dt.Columns.Add("Putanja");
            dt.PrimaryKey = new System.Data.DataColumn[] { dt.Columns[0] };
            dtM = new System.Data.DataTable();
            dtM.Columns.Add("Id");
            dtM.Columns.Add("Ime");
            dtM.Columns.Add("Putanja");
            //dtM.PrimaryKey = new System.Data.DataColumn[] { dtM.Columns[0] };
            dtS = new System.Data.DataTable();
            dtS.Columns.Add("Id");
            dtS.Columns.Add("Ime");
            dtS.Columns.Add("Format");
            dtS.Columns.Add("Putanja");
            dtS.PrimaryKey = new System.Data.DataColumn[] { dtS.Columns[0] };
            dtO = new System.Data.DataTable();
            
            i = 0;
            iS = 0;
            iM = 0;
            doks = new System.Collections.Generic.List<dok>();
            putanjaIDE = "";
            putanjadod = "";
        }
        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
            Point point = position((IInputElement)theTarget);
            return rect.Contains(point);
        }
        public DateTime convertDateMC(string datum)
        {
            if (datum =="")
            {
                return default(DateTime);
            }
            else
            {
                string[] s = datum.Split('-');
                return new DateTime(Convert.ToInt32(datum[0]), Convert.ToInt32(datum[1]), Convert.ToInt32(datum[2]));
            }
            
 

        }
        public static void up(string ime, string company)
        {
            Ime = ime;
            Company = company;
        }
        private DataGridRow GetRowItem(int index)
        {
            if (dgM.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return dgM.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }
        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dgM.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }
        private Rectangle getOutputPageSize(PdfReader reader, int page)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            MarginFinder finder = parser.ProcessContent<MarginFinder>(page, new MarginFinder());
            //MessageBox.Show("Text/bitmap boundary: " + finder.GetLlx() + "_" + finder.GetLly() + "_" + finder.GetUrx() + "_" + finder.GetUry());
            double lijevo = finder.Llx;
            double desno = finder.Urx;
            double gore = finder.Ury;
            double dole = finder.Lly;
            return new Rectangle(finder.Llx, finder.Lly, finder.Urx, finder.Ury);
        }
        public void test()
        {
            string putanja = "C:\\Users\\VIK\\Desktop\\pdfcrop\\1.pdf";
            if (System.IO.File.Exists(putanja))
            {
                PdfReader pdfReader1 = new PdfReader(System.IO.File.ReadAllBytes(putanja));
            }
        }
        public void okreniPDF(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                string putanja = System.IO.Path.GetDirectoryName(fileName);
                PdfReader pdfReader1 = new PdfReader(System.IO.File.ReadAllBytes(fileName));
                PdfStamper stamper1 = new PdfStamper(pdfReader1, new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write));
                PdfDictionary page1 = pdfReader1.GetPageN(1);
                Rectangle pagesize = pdfReader1.GetPageSizeWithRotation(page1);
                Rectangle ps = getOutputPageSize(pdfReader1, 1);
                if (arotiraj.IsChecked == true)
                {
                    if (pagesize.Left - pagesize.Right > 300)
                    {
                        if (pagesize.Left - pagesize.Right < pagesize.Top - pagesize.Bottom)
                        {
                            page1.Put(PdfName.ROTATE, new PdfNumber(90));
                        }
                    }
                }
                stamper1.Close();
                pdfReader1.Close();
            }
        }
        public void obreziPDF(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                string putanja = System.IO.Path.GetDirectoryName(fileName);
                PdfReader pdfReader = new PdfReader(System.IO.File.ReadAllBytes(fileName));
                PdfStamper stamper = new PdfStamper(pdfReader, new System.IO.FileStream(fileName, FileMode.Create, FileAccess.Write));
                PdfDictionary page = pdfReader.GetPageN(1);
                Rectangle rect = getOutputPageSize(pdfReader, 1);
                float[] flo = { rect.Left, rect.Bottom, rect.Right, rect.Top };
                PdfArray pdfa = new PdfArray(flo);
                Rectangle r2 = getOutputPageSize(pdfReader, 1);
                float f= new float();
                Rectangle mediabox = pdfReader.GetPageSize(1);
                float f2 =  pdfReader.GetPageSize(1).GetRight(f) - pdfReader.GetPageSize(1).GetLeft(f);
                page.Put(PdfName.MEDIABOX, pdfa);
                stamper.MarkUsed(page);
                stamper.Close();
                pdfReader.Close();
            }
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            prozor.AllowDrop = true;
            prozor.DragEnter += new DragEventHandler(dg_DragEnter1);
            prozor.Drop += new DragEventHandler(dg_Drop1);
        }
        private void dg_DragEnter1(object sender, DragEventArgs e)
        {
            e.Effects=DragDropEffects.Move;
        }
        private void dg_Drop1(object sender, DragEventArgs e)
        {
            //obrezuje pdf
                if (tabc.SelectedIndex == 0)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                        foreach (string file in files)
                        {
                            if (System.IO.Path.GetExtension(file) == ".pdf")
                            {
                                if (dad.IsChecked==true)
                                {
                                    obreziPDF(file);
                                }
                                else
                                {
                                    i++;
                                    dt.Rows.Add(i, System.IO.Path.GetFileName(file), file);
                                    dg.ItemsSource = dt.DefaultView;
                                }
                            }
                        }
                    }
                }
                //spaja pdf
                else if (tabc.SelectedIndex == 1)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                        foreach (string file in files)
                        {
                            if (System.IO.Path.GetExtension(file) == ".pdf")
                            {
                                iM++;
                                dtM.Rows.Add(iM, System.IO.Path.GetFileName(file), file);
                                dgM.ItemsSource = dtM.DefaultView;
                            }
                        }
                    }
                    else
                    {
                        if (rowIndex < 0)
                            return;
                        int index = this.GetCurrentRowIndex(e.GetPosition);
                        if (index < 0)
                            return;
                        if (index == rowIndex)
                            return;
                        if (index == dgM.Items.Count - 1)
                        {
                            MessageBox.Show("This row-index cannot be drop");
                            return;
                        }
                        /*
                        System.Data.DataRowView drv = dgM.Items.GetItemAt(rowIndex) as System.Data.DataRowView;
                        dgM.Items.RemoveAt(rowIndex);
                        dgM.Items.Insert(index, drv);
                        */
                        System.Data.DataRow drClone = dtM.NewRow();
                        drClone.ItemArray = dtM.Rows[rowIndex].ItemArray;
                        dtM.Rows.RemoveAt(rowIndex);
                        dtM.Rows.InsertAt(drClone, index);

                    }


                }
                //razvrstava pdf
                else if (tabc.SelectedIndex == 2)
                {
                    if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        return;
                    }

                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    foreach (string file in files)
                    {
                        if (System.IO.Path.GetExtension(file) == ".pdf")
                        {
                            PdfReader pdfReader = new PdfReader(file);
                            if (pdfReader.NumberOfPages > 1)
                            {
                                MessageBox.Show("Datoteka " + file + " neće biti dodana jer ima više stranica.");
                            }
                            else
                            {
                                PdfDictionary page = pdfReader.GetPageN(1);
                                Rectangle mediabox = pdfReader.GetPageSize(page);
                                iS++;
                                dtS.Rows.Add(iS, System.IO.Path.GetFileName(file), Math.Ceiling(mediabox.Width).ToString() + "×" + Math.Ceiling(mediabox.Height).ToString(), file);
                                dgS.ItemsSource = dtS.DefaultView;
                            }

                        }
                    }
                }
                /*
                else if (tabc.SelectedIndex == 3)
                {
                    if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        return;
                    }
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    string f = files[0];
                    if (System.IO.Path.GetExtension(f) == ".pdf")
                    {
                        if (files.Length > 1)
                        {
                            MessageBox.Show("Odabrano je " + files.Length + "datoteka obrađivati će se samo prva.");
                        }
                        else
                        {
                            putanjaIDE = f;
                        }
                        lableIde.Content = "Obrađuje se datoteka " + f;
                    }
                }
                 * */
            }
        public velicina odrediV(string s)
        {
            string[] ss = s.Split('×');
            //int a=
            return new velicina(Convert.ToInt32(ss[0]), Convert.ToInt32(ss[1]));
        }
        public void razvrstaj()
        {

            if (dgS.Items.Count > 0)
            {

                String folder = System.IO.Path.GetDirectoryName(dtS.Rows[0].ItemArray[3].ToString());
                foreach (System.Data.DataRow dr in dtS.Rows)
                {
                    velicina ve = odrediV(dr.ItemArray[2].ToString());
                    //Point3d tocka = tocke.Where(w => w.Y == tocke.Max(p => p.Y)).FirstOrDefault<Point3d>();
                    dok dkuk = doks.Where(d => d.v.usporedi(ve)).FirstOrDefault<dok>();
                    if (dkuk == null)
                    {
                        dok dokument = new dok(ve);
                        dokument.dodaj(dr.ItemArray[3].ToString());
                        doks.Add(dokument);
                    }
                    else
                    {
                        dkuk.dodaj(dr.ItemArray[3].ToString());
                    }
                }
                foreach (dok dokum in doks)
                {
                    string s = folder + "\\" + dokum.v.sirina + "x" + dokum.v.visina + ".pdf";
                    SpojiPDF(dokum.datoteke, s);
                }
            }

        }
        private void dodajC_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF dokumenti (.pdf)|*.pdf";
            dlg.Multiselect = true;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                if (tabc.SelectedIndex == 0)
                {
                    foreach (string file in dlg.FileNames)
                    {
                        i++;
                        dt.Rows.Add(i, System.IO.Path.GetFileName(file), file);
                        dg.ItemsSource = dt.DefaultView;
                    }
                }
                else if (tabc.SelectedIndex == 1)
                {
                    foreach (string file in dlg.FileNames)
                    {
                        iM++;
                        dtM.Rows.Add(iM, System.IO.Path.GetFileName(file), file);
                        dgM.ItemsSource = dtM.DefaultView;
                    }
                }
                else if (tabc.SelectedIndex == 2)
                {
                    foreach (string file in dlg.FileNames)
                    {
                        PdfReader pdfReader = new PdfReader(file);
                        if (pdfReader.NumberOfPages > 1)
                        {
                            MessageBox.Show("Datoteka " + file + " neće biti dodana jer ima više stranica.");
                        }
                        else
                        {
                            PdfDictionary page = pdfReader.GetPageN(1);
                            Rectangle mediabox = pdfReader.GetPageSize(page);
                            iS++;
                            dtS.Rows.Add(iS, System.IO.Path.GetFileName(file), Math.Ceiling(mediabox.Width).ToString() + "×" + Math.Ceiling(mediabox.Height).ToString(), file);
                            dgS.ItemsSource = dtS.DefaultView;
                        }
                    }
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItems.Count != 0)
            {
                foreach (System.Data.DataRowView dr in dg.SelectedItems)
                {
                    //DataRow foundRow = dataSet1.Tables["AnyTable"].Rows.Find(s);
                    dt.Rows.Find(dr.Row.ItemArray[0]).Delete();
                    dg.ItemsSource = dt.DefaultView;
                }
            }
        }
        private void KreniC_Click(object sender, RoutedEventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    obreziPDF(dr.ItemArray[2].ToString());
                    okreniPDF(dr.ItemArray[2].ToString());
                }
            }
            reset();

        }
        private void SpojiPDF(System.Collections.Generic.List<string> pdf, String ofilename)
        {
            //string ofilename = "e:\\output.pfg";
            Document document = new Document();
            PdfCopy copy = new PdfCopy(document, new System.IO.FileStream(ofilename, FileMode.Create, FileAccess.Write));
            document.Open();
            PdfImportedPage page;
            foreach (string p in pdf)
            {
                //PdfReader reader = new PdfReader(new RandomAccessFileOrArray(p), null);
                PdfReader reader = new PdfReader(p, null);
                int pages = reader.NumberOfPages;
                // loop over document pages
                for (int i = 0; i < pages; )
                {
                    page = copy.GetImportedPage(reader, ++i);
                    copy.AddPage(page);
                }
            }
            document.Close();
        }
        private void kreniM_Click(object sender, RoutedEventArgs e)
        {


        }
        private void dg_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

        }
        private void Razvrstaj_Click(object sender, RoutedEventArgs e)
        {
            razvrstaj();
            reset();
        }
        private void SpojiM_Click(object sender, RoutedEventArgs e)
        {
            if (dtM.Rows.Count > 0)
            {
                System.Collections.Generic.List<string> puts = new System.Collections.Generic.List<string>();
                foreach (System.Data.DataRow dr in dtM.Rows)
                {
                    puts.Add(dr.ItemArray[2].ToString());
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(puts[1]);
                saveFileDialog.FileName = "Dokument";
                saveFileDialog.DefaultExt = ".pdf";
                saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
                // Show save file dialog box
                Nullable<bool> result = saveFileDialog.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = saveFileDialog.FileName;
                    SpojiPDF(puts, filename);
                }
                reset();

            }
        }
        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if (dad.IsChecked == true)
            {
                KreniC.IsEnabled = false;
            }
            else
            {
                KreniC.IsEnabled = true;
            }
            if (dt.Rows.Count > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    obreziPDF(dr.ItemArray[2].ToString());
                }
            }
        }
        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reset();
        }
        private void ofile2(object sender, RoutedEventArgs e)
        {
            
        }
        private void ofileide_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF dokumenti (.pdf)|*.pdf";
            dlg.Multiselect = false;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                if (tabc.SelectedIndex == 3)
                {

                    string f = dlg.FileNames[0];
                    if (System.IO.Path.GetExtension(f) == ".pdf")
                    {
                        if (dlg.FileNames.Length > 1)
                        {
                            MessageBox.Show("Odabrano je " + dlg.FileNames.Length + "datoteka obrađivati će se samo prva.");
                        }
                        else
                        {
                            putanjaIDE = f;
                        }
                        lableIde.Content = "Obrađuje se datoteka " + f;
                        gb.IsEnabled = true;
                        dgOFill(ref dtO, ref dgO, putanjaIDE);
                    }
                }
            }
        }
        private void pbbbbb_Click(object sender, RoutedEventArgs e)
        {
            string putanja = "e://proba.pdf";
            PdfReader pdfReader = new PdfReader(putanja);
            int k = pdfReader.NumberOfPages;
            int c = 5;
            string[] po = new string[c];
            dtO.Columns.Clear();
            for (int i = 0; i < c; i++)
            {
                dtO.Columns.Add();
            }
            int ik = 0;
            for (int i = 0; i < k; i++)
            {

                int a = i + 1;
                if (ik == c - 1)
                {
                    po[ik] = a.ToString("0");
                    dtO.Rows.Add(po);
                    Array.Clear(po, 0, po.Length);
                    ik = 0;
                }
                else if (i == k - 1)
                {
                    po[ik] = a.ToString("0");
                    dtO.Rows.Add(po);
                }
                else
                {
                    po[ik] = a.ToString("0");
                    ik++;
                }
            }
            dgO.ItemsSource = dtO.DefaultView;
        }
        public void dgOFill(ref System.Data.DataTable dt, ref DataGrid dg, string putanja)
        {
                PdfReader pdfReader = new PdfReader(putanja);
                int k = pdfReader.NumberOfPages;
                int c = 5;
                string[] po = new string[c];
                dtO.Columns.Clear();
                for (int i2 = 0; i2 < c; i2++)
                {
                    dtO.Columns.Add();
                }
                int ik = 0;
                for (int i2 = 0; i2 < k; i2++)
                {

                    int a = i2 + 1;
                    if (ik == c - 1)
                    {
                        po[ik] = a.ToString("0");
                        dtO.Rows.Add(po);
                        Array.Clear(po, 0, po.Length);
                        ik = 0;
                    }
                    else if (i2 == k - 1)
                    {
                        po[ik] = a.ToString("0");
                        dtO.Rows.Add(po);
                    }
                    else
                    {
                        po[ik] = a.ToString("0");
                        ik++;
                    }
                }
                dg.ItemsSource = dt.DefaultView;
        }
        private void dgO_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            string a = "";
            //var drw = (System.Data.DataRowView)dgO.SelectedItems;
            //List rows = dg.SelectedCells;
            for (int i = 0; i < dgO.SelectedCells.Count; i++)
            {
                System.Data.DataRowView drv = dgO.SelectedCells[i].Item as System.Data.DataRowView;
                string s = (dgO.SelectedCells[i].Column.GetCellContent(dgO.SelectedCells[i].Item) as TextBlock).Text;
                a = a + s+"; ";
            }
            //testl.Content = a;
        }
        private void dgO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DataGridCellInfo item in dgO.SelectedCells)
            {
                //item
            }
        }
        private void odfilefod_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".pdf";
            dlg.Filter = "PDF dokumenti (.pdf)|*.pdf";
            dlg.Multiselect = false;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                if (tabc.SelectedIndex == 3)
                {

                    string f = dlg.FileNames[0];
                    if (System.IO.Path.GetExtension(f) == ".pdf")
                    {
                        if (dlg.FileNames.Length > 1)
                        {
                            MessageBox.Show("Odabrano je " + dlg.FileNames.Length + "datoteka obrađivati će se samo prva.");
                        }
                        else
                        {
                            putanjadod = f;
                        }
                        PdfReader pdfReader = new PdfReader(putanjadod);
                        //l11.Content = "/ "+ pdfReader.NumberOfPages;
                    }
                }
            }
        }
        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (rb3.IsChecked==true)
            {
                dgO.IsEnabled = false;
            }
            else
            {
                dgO.IsEnabled = true;
            }
        }
        public void DeletePages(ref List<int> pagesToDelete, string SourcePdfPath, string OutputPdfPath, string Password = "")
        {
             // check for non-consecutive ranges
            Document doc = new Document();
            PdfReader reader = new PdfReader(SourcePdfPath, new System.Text.ASCIIEncoding().GetBytes(Password));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();
                doc.AddDocListener(writer);
                for (int p = 1; p <= reader.NumberOfPages; p++)
                {
                    if (pagesToDelete.FindIndex(s => s == p) != -1)
                    {
                        continue;
                    }
                    doc.SetPageSize(reader.GetPageSize(p));
                    doc.NewPage();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage pageImport = writer.GetImportedPage(reader, p);
                    int rot = reader.GetPageRotation(p);
                    if (rot == 90 || rot == 270)
                        cb.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(p).Height);
                    else
                        cb.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
                }
                doc.Close();
                reader.Close();
               
                File.WriteAllBytes(OutputPdfPath, memoryStream.ToArray());
            }
        }
        public void ExtractPages2(ref List<int> pagesToDelete, string SourcePdfPath, string OutputPdfPath, string Password = "")
        {
            // check for non-consecutive ranges
            Document doc = new Document();
            PdfReader reader = new PdfReader(SourcePdfPath, new System.Text.ASCIIEncoding().GetBytes(Password));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();
                doc.AddDocListener(writer);
                for (int p = 1; p <= reader.NumberOfPages; p++)
                {
                    if (pagesToDelete.FindIndex(s => s == p) == -1)
                    {
                        continue;
                    }
                    doc.SetPageSize(reader.GetPageSize(p));
                    doc.NewPage();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage pageImport = writer.GetImportedPage(reader, p);
                    int rot = reader.GetPageRotation(p);
                    if (rot == 90 || rot == 270)
                        cb.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(p).Height);
                    else
                        cb.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
                }
                doc.Close();
                reader.Close();
                
                File.WriteAllBytes(OutputPdfPath, memoryStream.ToArray());
            }
        }
        //mislim da nije dobro
        private void ExtractPages(ref List<int> pagesToDelete, string sourcePDFpath, string outputPDFpath, int startpage, int endpage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            reader = new PdfReader(sourcePDFpath);
            sourceDocument = new Document(reader.GetPageSizeWithRotation(startpage));
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPDFpath, System.IO.FileMode.Create));

            sourceDocument.Open();

            for (int i = startpage; i <= endpage; i++)
            {
                importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                pdfCopyProvider.AddPage(importedPage);
            }
            sourceDocument.Close();
            reader.Close();
        }
        private void ideKreni_Click(object sender, RoutedEventArgs e)
        {
            if (rb1.IsChecked==true)
            {
                if (dgO.SelectedCells.Count>0)
                {
                    List<int> lista = new List<int>();

                    for (int i = 0; i < dgO.SelectedCells.Count; i++)
                    {
                        System.Data.DataRowView drv = dgO.SelectedCells[i].Item as System.Data.DataRowView;
                        lista.Add(Convert.ToInt32((dgO.SelectedCells[i].Column.GetCellContent(dgO.SelectedCells[i].Item) as TextBlock).Text));
                        
                    }
                    string pp = putanjaIDE.Replace(".pdf", "_1.pdf");
                    DeletePages(ref lista, putanjaIDE, putanjaIDE);
                }
            }
            else if (rb2.IsChecked == true)
            {
                if (dgO.SelectedCells.Count > 0)
                {
                    List<int> lista = new List<int>();

                    for (int i = 0; i < dgO.SelectedCells.Count; i++)
                    {
                        System.Data.DataRowView drv = dgO.SelectedCells[i].Item as System.Data.DataRowView;
                        lista.Add(Convert.ToInt32((dgO.SelectedCells[i].Column.GetCellContent(dgO.SelectedCells[i].Item) as TextBlock).Text));

                    }
                    string pp = putanjaIDE.Replace(".pdf", "_e.pdf");
                    ExtractPages2(ref lista, putanjaIDE, pp);
                }
            }
            else if (rb3.IsChecked == true)
            {
                if (putanjaIDE!=""&&System.IO.File.Exists(putanjaIDE))
	            {
                    dodajstr froma = new dodajstr(putanjaIDE);
                    froma.Show();
	            }
                
            }
            else if (rb4.IsChecked == true)
            {
                //zamjeni();
                if (putanjaIDE != "" && System.IO.File.Exists(putanjaIDE))
                {
                    zamjenistr froma = new zamjenistr(putanjaIDE);
                    froma.Show();
                }
            }
            reset();
        }
        private void rb4_Click(object sender, RoutedEventArgs e)
        {
            if (rb3.IsChecked == true)
            {
                dgO.IsEnabled = false;
            }
            else
            {
                dgO.IsEnabled = true;
            }
        }
        private void rb3_Unchecked(object sender, RoutedEventArgs e)
        {
            if (rb3.IsChecked == true)
            {
                dgO.IsEnabled = true;
            }
            else
            {
                dgO.IsEnabled = false;
            }
        }
        public string getUniqueId()
        {
            try
            {
                string cpuInfo = string.Empty;
                /*
                System.Management.ManagementClass mc = new ManagementClass("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
                 */
                string drive = "C";
                ManagementObject dsk = new ManagementObject(
                    @"win32_logicaldisk.deviceid=""" + drive + @":""");
                dsk.Get();
                string volumeSerial = dsk["VolumeSerialNumber"].ToString();
                string uniqueId = volumeSerial;
                return uniqueId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException + "/uniqueid");
                //Application.Exit();
                return "";
            }
        }
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new System.IO.MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
        public static DateTime GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "zg1.ntp.carnet.hr";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }
        public static bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string strconn = "Server=198.38.82.92; Database =donatene_lic; Uid = donatene_padmin; Password =2311Luka2602Valentina;connection Timeout=60;";
            try
            {
                con = new MySqlConnection(strconn);
                con.Open();
                MessageBox.Show("Uspjelo!!!!!!");
                con.Close();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        private Rectangle getOutputPageSize1(PdfReader reader, int page) 
        { 
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            TextMarginFinder finder = parser.ProcessContent<TextMarginFinder>(1, new TextMarginFinder());
            Rectangle result = new Rectangle(finder.GetLlx(), finder.GetLly(), finder.GetUrx(), finder.GetUry());
            return result;
        }
        private void test(object sender, RoutedEventArgs e)
        {
            string putanja = "C:\\Users\\VIK\\Desktop\\pdfcrop\\1.pdf";
            if (System.IO.File.Exists(putanja))
            {
                PdfReader reader = new PdfReader(System.IO.File.ReadAllBytes(putanja));
                //Rectangle okvir = getOutputPageSize1(reader,1);
                Rectangle cropbox = reader.GetCropBox(1);
                Rectangle mediabox = reader.GetPageSize(1);
            }
            


        }
        public static void UpisiUbazu(String putanja, String upitUpisa)
        {

            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand())
                    {
                        com.CommandText = upitUpisa;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
            }
            finally
            {
            }
        }
        public static int UpisiUBazuR(String putanja, String upitUpisa)
        {
            int id = default(int);
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    SQLiteCommand com = null;
                    com = new SQLiteCommand(upitUpisa, con);
                    //com.Transaction = tr;
                    com.ExecuteNonQuery();
                    com = new SQLiteCommand("SELECT last_insert_rowid()", con);
                    object o = com.ExecuteScalar();
                    id = Convert.ToInt32(o);
                    con.Close();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
            }
            finally
            {

            }
            return id;
        }
        public static System.Data.DataTable citajIzBaze(String putanja, String upit)
        {
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand())
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        SQLiteDataAdapter sda = new SQLiteDataAdapter();
                        sda.SelectCommand = new SQLiteCommand(upit, con);
                        sda.Fill(dt);
                        con.Close();
                        con.Dispose();
                        return dt;
                    }

                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
                con.Dispose();
                return default(System.Data.DataTable);
            }
            finally
            {
                //con.Close();
                //con.Dispose();
            }
        }
        public static void obrisiIzbaze(String putanja, String upit)
        {
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand())
                    {
                        com.CommandText = upit;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                    }
                    con.Close();
                    con.Dispose();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
                con.Dispose();
            }
            finally
            {
                /* con.Close();
                 con.Dispose();*/
            }
        }
        public static void obrisiIzbaze(String putanja, String tablica, int ID)
        {
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand())
                    {
                        com.CommandText = "delete from " + tablica + " where ID = " + ID;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                    }
                    con.Close();
                    con.Dispose();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
                con.Dispose();
            }
            finally
            {
                /*con.Close();
                con.Dispose();*/
            }
        }
        public static void updateBaze(String putanja, String upit)
        {
            SQLiteConnection con = new SQLiteConnection();
            try
            {
                String constring = "Data Source=" + putanja;
                using (con = new SQLiteConnection(constring))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand())
                    {
                        com.CommandText = upit;
                        com.Connection = con;
                        com.ExecuteNonQuery();
                    }
                    con.Close();
                    con.Dispose();
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Pojavila se greška: " + ex.Message + "/" + ex.InnerException);
                con.Close();
                con.Dispose();
            }
            finally
            {
            }
        }
        public void show(Rectangle r)
        {
           MessageBox.Show("llx: " + r.GetLeft(0) + ", lly: "+r.GetBottom(0)+", urx: "+r.GetRight(0)+", ury: "+r.GetTop(0));
        }
        private void test2(object sender, RoutedEventArgs e)
        {
            string putanja = "C:\\Users\\VIK\\Desktop\\pdfcrop\\1.pdf";

            if (System.IO.File.Exists(putanja))
            {
                ////croping part
                PdfReader pdfReader = new PdfReader(System.IO.File.ReadAllBytes(putanja));
                PdfStamper stamper = new PdfStamper(pdfReader, new System.IO.FileStream(putanja, FileMode.Create, FileAccess.Write));
                PdfDictionary page = pdfReader.GetPageN(1);
                Rectangle rect = getOutputPageSize(pdfReader, 1);
                show(rect);
                float[] flo = { rect.Left, rect.Bottom, rect.Right, rect.Top };
                PdfArray pdfa = new PdfArray(flo);
                page.Put(PdfName.MEDIABOX, pdfa);
               // stamper.MarkUsed(page);
                stamper.Close();
                pdfReader.Close();
                ////looking for size part
                    PdfReader reader = new PdfReader(System.IO.File.ReadAllBytes(putanja));
                    PdfDictionary pageDict = reader.GetPageN(1);
                    PdfArray cropbox = pageDict.GetAsArray(PdfName.CROPBOX);
                    PdfArray trimbox = pageDict.GetAsArray(PdfName.TRIMBOX);
                    PdfArray mediabox = pageDict.GetAsArray(PdfName.MEDIABOX);
                    Rectangle rect1 = getOutputPageSize(reader, 1);
                    show(rect1);
            }
        }
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (System.IO.File.Exists(putbaze))
            {
                string u = "update 'setings' set tabcontrol="+tabc.SelectedIndex+", autokreni="+Convert.ToInt16(dad.IsChecked)+" where id=1";
                updateBaze(putbaze, u);
            }
        }

        

        
    }
    public class velicina
    {
        public int sirina;
        public int visina;
        public velicina(int _sirina, int _visina)
        {
            sirina = _sirina;
            visina = _visina;
        }
        private velicina()
        {

        }
        public bool usporedi(velicina vu)
        {
            if (sirina==vu.sirina & visina==vu.visina)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class dok
    {
        public velicina v;
        public System.Collections.Generic.List<String> datoteke;
        public dok(velicina _V)
        { 
            datoteke= new System.Collections.Generic.List<string>();
            v=_V;
        }
        public void dodaj(string Putanja)
        {
            datoteke.Add(Putanja);
        }
    }
    public class dok2
    {
        public int id;
        public string ime;
        public string putanja;

        public dok2(int _id, string _ime, string _putanja)
        {
            id = _id;
            ime = _ime;
            putanja = _putanja;
        }
    }
}
