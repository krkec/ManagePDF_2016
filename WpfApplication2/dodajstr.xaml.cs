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
    /// Interaction logic for dodajstr.xaml
    /// </summary>
    public partial class dodajstr : Window
    {
        public string put1;
        public string put2;
        int brojstr1 = 0;
        int brojstr2 = 0;
        public dodajstr(string put)
        {
            InitializeComponent();
            put1 = "";
            put2 = put;
            oddat(ref put1, ref brojstr1);
            if (put1!=""&&put2!="")
            {
                insertdatlabel.Content = put1;
                kroz1.Content = "/ " + brojstr1;
                //PdfReader pdfReader = new PdfReader(put2);
                
                PdfReader pdfReader1 = new PdfReader(put1);
                brojstr1 = pdfReader1.NumberOfPages;
                izaispred.SelectedIndex = 0;
                prvazadnja.SelectedIndex = 0;
                iodstr.Text = "1";
                idostr.Text = brojstr1.ToString("0");
                pdfReader1.Close();
            }
            else
            {
                this.Close();
            }
        }

        private dodajstr()
        {
        }
        public void napraviListu(ref List<int> brojevi, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                brojevi.Add(i);
            }
        }
        public void dodajstranicu(int bs, string put1, string put2, int zod, int zdo)
        {
            if (put2 != "" && System.IO.File.Exists(put2) && put1 != "" && System.IO.File.Exists(put1)&&zod<=zdo)
            {
                Document document = new Document();
                
                string tempf = put2.Replace(".pdf", ".bak");
                System.IO.File.Copy(put2, tempf, true);
                //file u koji se ubacuje
                PdfReader reader = new PdfReader(tempf);
                brojstr2 = reader.NumberOfPages;
                //create PdfCopy object
                PdfCopy copy = new PdfCopy(document, new FileStream(put2, FileMode.Create));
                //open the document
                document.Open();
                //zamjenski file
                PdfReader readerz = new PdfReader(put1);
                int ostalo = 0;
                if (ostalostr.Text!="")
                {
                    ostalo = Convert.ToInt16(ostalostr.Text);
                }
                
                // uvijek ispred
                //ako je izabrano da se umece ispred prve stranice
                if (izaispred.SelectedIndex == 0 && prvazadnja.SelectedIndex == 0)
                {
                        for (int j = zod; j <= zdo; j++)
                        {
                            copy.AddPage(copy.GetImportedPage(readerz, j));
                        }
                        for (int i = 1; i <= brojstr2; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                }
                //ako je izabrano da se umece ispred zadnje stranice
                else if (izaispred.SelectedIndex == 0&&prvazadnja.SelectedIndex == 1)
                {
                        for (int i = 1; i < brojstr2; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                        for (int j = zod; j <= zdo; j++)
                        {
                            copy.AddPage(copy.GetImportedPage(readerz, j));
                        }
                        copy.AddPage(copy.GetImportedPage(reader, brojstr2));
                }
                //ako je izabrano da se umece iza prve stranice
                else if (izaispred.SelectedIndex == 1&&prvazadnja.SelectedIndex == 0)
                {
                    copy.AddPage(copy.GetImportedPage(reader, 1));
                        for (int j = zod; j <= zdo; j++)
                        {
                            copy.AddPage(copy.GetImportedPage(readerz, j));
                        }
                        for (int i = 2; i < brojstr2; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                }
                //ako je izabrano da se umece iza zadnje stranice
                else if (izaispred.SelectedIndex == 1&&prvazadnja.SelectedIndex == 1)
                {
                        for (int i = 1; i <= brojstr2; i++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                        for (int j = zod; j <= zdo; j++)
                        {
                            copy.AddPage(copy.GetImportedPage(readerz, j));
                        }
                }
                //ako je izabrano da se umece ispred ostalo
                else if (izaispred.SelectedIndex == 0&&prvazadnja.SelectedIndex == 2&&ostalo!=0)
                {
                        for (int i = 1; i <= brojstr2; i++)
                        {
                            if (i==ostalo-1)
                            {
                                for (int j = zod; j <= zdo; j++)
                                {
                                    copy.AddPage(copy.GetImportedPage(readerz, j));
                                }
                            }
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                }
                //ako je izabrano da se umece iza ostalo
                else if (izaispred.SelectedIndex == 1&&prvazadnja.SelectedIndex == 2 && ostalo != 0)
                {
                        for (int i = 1; i <= brojstr2; i++)
                        {
                            if (i == ostalo)
                            {
                                for (int j = zod; j <= zdo; j++)
                                {
                                    copy.AddPage(copy.GetImportedPage(readerz, j));
                                }
                            }
                            copy.AddPage(copy.GetImportedPage(reader, i));
                        }
                }
                reader.Close();
                readerz.Close();
                document.Close();
            }
        }

        private void prvazadnja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prvazadnja.SelectedIndex ==2)
            {
                ostalostr.IsEnabled = true;
            }
            else
            {
                ostalostr.IsEnabled = false;
            }
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

        private void dodcancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dodaj(object sender, RoutedEventArgs e)
        {
            int zod = Convert.ToInt32(iodstr.Text);
            int zdo = Convert.ToInt32(idostr.Text);


            if (izaispred.SelectedIndex == 0)//ispred
            {
                if (prvazadnja.SelectedIndex == 0) //prva
                {
                    dodajstranicu(0, put1, put2, zod, zdo);
                }
                else if (prvazadnja.SelectedIndex == 1) //zadnja
                {
                    dodajstranicu(brojstr2, put1, put2, zod, zdo);
                }
                else if (prvazadnja.SelectedIndex == 2) //ostalo
                {
                    int b = Convert.ToInt32(ostalostr.Text);
                    dodajstranicu(b, put1, put2, zod, zdo);
                }
            }
            else if (izaispred.SelectedIndex == 1) // iza
            {
                if (prvazadnja.SelectedIndex == 0) //prva
                {
                    dodajstranicu(1, put1, put2, zod, zdo);
                }
                else if (prvazadnja.SelectedIndex == 1) //zadnja
                {
                    dodajstranicu(-1, put1, put2, zod, zdo);
                }
                else if (prvazadnja.SelectedIndex == 2) //ostalo
                {
                    int b = Convert.ToInt32(ostalostr.Text);
                    if (b < brojstr2)
                    {
                        dodajstranicu(b + 1, put1, put2, zod, zdo);
                    }
                    else
                    {
                        dodajstranicu(-1, put1, put2, zod, zdo);
                    }

                }
            }
            this.Close();
        }

    }
}
