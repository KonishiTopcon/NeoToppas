using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bpac;

namespace NeoToppas.Infrastructure.BrotherPrinter
{
    public static class BrotherPrint
    {
        public static void Print_Brother(DocumentClass doc)
        {
            doc.StartPrint("", PrintOptionConstants.bpoDefault);
            doc.PrintOut(1, PrintOptionConstants.bpoDefault);
            doc.EndPrint();
        }
    }
}
