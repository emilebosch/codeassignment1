using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OVImport
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = new OVTransactionImport();
            process.StartTransactionImport("data/sample.csv");
        }
    }
}
