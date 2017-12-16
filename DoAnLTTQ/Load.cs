using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnLTTQ
{
    public class Load
    {
        public static string[] ThuatToan = { "Interchange sort", "Selection sort", "Bubble sort", "BinaryInsertion sort", "Insertion sort", "Shell sort", "Shaker sort", "Heap sort", "Quick sort", "Merge sort" };

        public static void LoadThuatToan(ComboBox cbb, RichTextBox rtxb)
        {
            StreamReader sr;
            switch (cbb.SelectedItem.ToString())
            {
                case "Selection sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Selection sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Interchange sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Interchange sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Bubble sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Bubble sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Insertion sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Insertion sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Shaker sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Shaker sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "BinaryInsertion sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\BinaryInsertion sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Quick sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Quick sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Shell sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Shell sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Heap sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Heap sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                case "Merge sort":
                    rtxb.Text = "";
                    sr = new StreamReader(@"File\Merge sort.txt");
                    while (!sr.EndOfStream)
                    {
                        rtxb.Text += sr.ReadLine() + Environment.NewLine;
                    }
                    sr.Dispose();
                    break;
                default:
                    break;
            }
        }
    }
}
