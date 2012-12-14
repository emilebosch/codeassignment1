using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using log4net;

namespace OVImport
{
    /// <summary>
    /// This class is responsible for reading the transaction log and submitting 
    /// it to the OV servers
    /// </summary>
    public class OVTransactionImport
    {
        TextFieldParser parser;
        SimpleRestApi api;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Imports the given csv file
        /// </summary>
        public void StartTransactionImport(string csvfile)
        {
            api = new SimpleRestApi(ConfigurationManager.AppSettings["ovservice"]);
            log4net.Config.XmlConfigurator.Configure();

            parser = new TextFieldParser(csvfile) {Delimiters = new[] {","}};
            parser.ReadFields();

            while (!parser.EndOfData)
            {                
                IList<string> csvFields = parser.ReadFields();
                Line CSVLine = new Line(csvFields);

                Retry.Repeat(3)
                     .WithPolling(TimeSpan.FromSeconds(1))
                     .WithTimeout(TimeSpan.FromSeconds(10))
                     .Until(() => PostCSVLine(CSVLine));
            }
        }

        private bool PostCSVLine(Line line)
        {
            APIResponse response = api.Post("ovtransactionimport/process", line, new APIResponse());

            if (response.success)
            {
                log.Info("Succesfully uploaded transaction!");
            }
            else
            {
                log.ErrorFormat("API responsed with an error {0}", response.error);
            }

            return response.success;
        }

        public class Line
        {
            public Line(IList<string> csvFields)
            {
                id = Convert.ToInt64(csvFields[0]);
                date = csvFields[1];
                station = csvFields[2];
                action = csvFields[3];
                cardid = Convert.ToInt64(csvFields[4]);
                userid = Convert.ToInt64(csvFields[5]);
            }

            public long id;
            public string date;
            public string station;
            public string action;
            public long cardid;
            public long userid;
        }

        public class APIResponse
        {
            public bool success;
            public string error;
        }
    }
}
