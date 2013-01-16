using System;
using System.IO;
using log4net;

namespace OVImport
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            var process = new OVTransactionImport();
            log4net.Config.XmlConfigurator.Configure();
            const string csvPath = "data/sample.csv";

            log.DebugFormat("OVImport started on {0}", DateTime.Now);

            try
            {
                if (!FileExists(csvPath))
                {
                    throw new FileNotFoundException(string.Format("CSV file {0} does not exist!", csvPath));
                }

                process.StartTransactionImport(csvPath);
            }
            catch (Exception e)
            {
                log.Fatal(e.Message);
            }

            log.DebugFormat("OVImport finished on {0}", DateTime.Now);
        }

        private static bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
