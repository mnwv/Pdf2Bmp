using Microsoft.Win32;
using System;

namespace Pdf2Bmp_cui
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var pdfPath = SelectPdf();
                Console.WriteLine(pdfPath);
                var result = Pdf2Bmp.Instance.ToBmp(pdfPath);
                foreach (var item in result)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string SelectPdf()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "PDF file(*.pdf)|*.pdf|all files(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "Select your PDF file";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return string.Empty;
        }
    }
}
