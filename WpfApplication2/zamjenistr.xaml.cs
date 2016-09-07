using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for zamjenistr.xaml
    /// </summary>
    public partial class zamjenistr : Window
    {
        public string put1;
        public string put2;
        int brojstr1 = 0;
        int brojstr2 = 0;
        public zamjenistr(string put)
        {
            InitializeComponent();
            put1 = "";
            put2 = put;
            PdfReader pdfReader = new PdfReader(put2);
            brojstr2 = pdfReader.NumberOfPages;
            dostr2.Text = brojstr2.ToString("0");
            l8.Content = "/ " + brojstr2;
            pdfReader.Close();
        }

        private zamjenistr()
        {
        }
        public void oddat(ref string putnaja, ref int brojstr)
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
                string f = dlg.FileNames[0];
                if (System.IO.Path.GetExtension(f) == ".pdf")
                {
                    putnaja = f;
                    PdfReader pdfReader = new PdfReader(putnaja);
                    brojstr = pdfReader.NumberOfPages;
                }
            }
        }
        private void odfilefod_Click(object sender, RoutedEventArgs e)
        {

        }
        public void zamjeni()
        {
            int o = Convert.ToInt32(odstr2.Text);
            int d = Convert.ToInt32(dostr2.Text);
            int oz = Convert.ToInt32(odstr3.Text);
            int dz = Convert.ToInt32(dostr3.Text);
            Document document = new Document();
            string tempf = put2.Replace(".pdf", ".bak");
            System.IO.File.Copy(put2, tempf, true);
            //create PdfCopy object
            PdfCopy copy = new PdfCopy(document, new FileStream(put2, FileMode.Create));
            //open the document
            document.Open();
            //PdfReader variable  putanjadod
            PdfReader reader = new PdfReader(tempf);
            PdfReader readerz = new PdfReader(put1);



            List<int> ptd = new List<int>();
            for (int i = o; i < d + 1; i++)
            {
                ptd.Add(i);
            }
            
            List<int> ptc = new List<int>();
            for (int i = oz; i < dz + 1; i++)
            {
                ptc.Add(i);
            }
            //DeletePages(ref ptd, putanjaIDE, putanjaIDE);
            ;
            for (int i = 1; i < d; i++)
            {
                copy.AddPage(copy.GetImportedPage(reader, i));
            }
            for (int i = oz; i < dz + 1; i++)
            {
                copy.AddPage(copy.GetImportedPage(readerz, i));
            }
            for (int i = d - 1; i <= reader.NumberOfPages; i++)
            {
                copy.AddPage(copy.GetImportedPage(reader, i));
            }
            reader.Close();
            readerz.Close();
            document.Close();
        }

        private void odfilefod2_Click(object sender, RoutedEventArgs e)
        {
            oddat(ref put1, ref brojstr1);
            if (put1 != "" && put2 != "")
            {
                datname.IsEnabled = true;
                datname.Content = put1;
                l9.IsEnabled = true;
                odstr3.IsEnabled = true;
                dostr3.IsEnabled = true;
                l10.IsEnabled = true;
                l11.IsEnabled = true;
                l11.Content = "/ " + brojstr1;
                odstr3.Text = "1";
                dostr3.Text = brojstr1.ToString("0");
            }
            else
            {
                //this.Close();
            }



            
        }

        private void zamcencel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void zamcencel_KeyDown(object sender, KeyEventArgs e)
        {

        }
        /// <summary>
        ///  sto je kada se mjenja jedna stranica
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int o = Convert.ToInt32(odstr2.Text);
            int d = Convert.ToInt32(dostr2.Text);
            List<int> ptd = new List<int>();
            for (int i = o; i < d + 1; i++)
            {
                ptd.Add(i);
            }
            int oz = Convert.ToInt32(odstr3.Text);
            int dz = Convert.ToInt32(dostr3.Text);
            List<int> ptc = new List<int>();
            for (int i = oz; i < dz + 1; i++)
            {
                ptc.Add(i);
            }
            //DeletePages(ref ptd, putanjaIDE, putanjaIDE);
            Document document = new Document();
            string tempf = put2.Replace(".pdf", ".bak");
            System.IO.File.Copy(put2, tempf, true);
            //create PdfCopy object
            PdfCopy copy = new PdfCopy(document, new FileStream(put2, FileMode.Create));
            //open the document
            document.Open();
            //PdfReader variable  putanjadod
            PdfReader reader = new PdfReader(tempf);
            PdfReader readerz = new PdfReader(put1);

            for (int i = 1; i < reader.NumberOfPages; i++)
            {
                if (i== o)
                {
                    for (int k = oz; k <= dz; k++)
                    {
                        copy.AddPage(copy.GetImportedPage(readerz, k));
                    }
                    i = i +1+ (d - o);
                }
                copy.AddPage(copy.GetImportedPage(reader, i));
                
            }

            /*
            for (int i = 1; i < d; i++)
            {
                copy.AddPage(copy.GetImportedPage(reader, i));
            }
            for (int i = oz; i < dz + 1; i++)
            {
                copy.AddPage(copy.GetImportedPage(readerz, i));
            }
            for (int i = d - 1; i <= reader.NumberOfPages; i++)
            {
                copy.AddPage(copy.GetImportedPage(reader, i));
            }*/
            reader.Close();
            readerz.Close();
            document.Close();
            this.Close();
        }
        
    }
}
