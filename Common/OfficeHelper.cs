using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class OfficeHelper
    {
        /// <summary>
        /// 是否为真的PDF文件
        /// </summary>
        /// <returns></returns>
        public static bool IsTruePdf(string pdfFilename)
        {
            Vintasoft.Imaging.Pdf.Processing.PdfA.PdfA1bVerifier verifier = new();
            System.Console.WriteLine("Verification...");
            if (verifier.Verify(pdfFilename))
                return true;
            else
                return false;
        }
    }
}
