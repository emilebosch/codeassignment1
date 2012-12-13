using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace OVImport
{
    class Program
    {
        private static ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                var process = new OVTransactionImport();
                process.StartTransactionImport("data/sample.csv");
            }
            catch (Exception exception)
            {
                _log.Fatal("Something went terribly wrong", exception);
            }
        }
    }
}
