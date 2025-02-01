using System;
using System.IO;
using System.Windows;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Pdf2Bmp_cui
{
    internal class Pdf2Bmp
    {
        private static Pdf2Bmp instance = new Pdf2Bmp();

        public static Pdf2Bmp Instance { get { return instance; } }

        private Pdf2Bmp() { }


        /// <summary>
        /// Converts each page of the specified PDF file into a BMP file and returns an array of file names.
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <param name="tgtWidth"></param>
        /// <param name="tgtHeight"></param>
        /// <returns></returns>
        public string[] ToBmp(string pdfFilePath, double tgtWidth = 1920.0, double tgtHeight = 1080.0)
        {
            var pdfName = Path.GetFileNameWithoutExtension(pdfFilePath);

            var pdfDocument = LoadPdfDocument(pdfFilePath);
            if (pdfDocument == null)
            {
                Console.WriteLine($"Failed to load :{pdfFilePath}.");
                MessageBox.Show($"Failed to load :{pdfFilePath}.", "PDF to BMP", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var bmpFolder = System.IO.Path.GetTempPath();
            var bmpFiles = new string[pdfDocument.PageCount];


            for (uint i = 0; i < pdfDocument.PageCount; i++)
            {
                var bmpFilename = $"{pdfName}-{i}.bmp";

                var bmpFilePath = Path.Combine(bmpFolder, bmpFilename);
                bmpFiles[i] = bmpFilePath;

                using (var pdfPage = pdfDocument.GetPage(i))
                {
                    var widthRatio = tgtWidth / pdfPage.Size.Width;
                    var heigthRatio = tgtHeight / pdfPage.Size.Height;
                    var ratio = widthRatio > heigthRatio ? heigthRatio : widthRatio;
                    //Console.WriteLine($"Page:{i} {pdfPage.Size.Width} {pdfPage.Size.Height} Ratio:{ratio}");

                    var options = new PdfPageRenderOptions();
                    options.DestinationWidth = (uint)(pdfPage.Size.Width * ratio);
                    options.DestinationHeight = (uint)(pdfPage.Size.Height * ratio);
                    options.BitmapEncoderId = BitmapEncoder.BmpEncoderId;
                    var stream = new InMemoryRandomAccessStream();
                    pdfPage.RenderToStreamAsync(stream, options).AsTask().Wait();
                    SaveToFile(stream, bmpFilePath);
                }
            }
            return bmpFiles;
        }

        /// <summary>
        /// Load a PDF document
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        private PdfDocument LoadPdfDocument(string pdfFilePath)
        {
            using (var stream = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                var randomAccessStream = new InMemoryRandomAccessStream();
                var inputStream = stream.AsInputStream();
                RandomAccessStream.CopyAsync(inputStream, randomAccessStream).AsTask().Wait();
                return PdfDocument.LoadFromStreamAsync(randomAccessStream).AsTask().Result;
            }
        }

        /// <summary>
        /// Save the stream to a file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bmpFilePath"></param>
        private void SaveToFile(IRandomAccessStream stream, string bmpFilePath)
        {
            using (var outputFileStream = new FileStream(bmpFilePath, FileMode.Create, FileAccess.Write))
            {
                stream.AsStreamForRead().CopyTo(outputFileStream);
            }
        }
    }
}
