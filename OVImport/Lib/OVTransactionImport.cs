using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using OVImport.Models;
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

                if (HasValidNumberOfFields(csvFields))
                {
                    Line CSVLine = new Line(csvFields);

                    try
                    {
                        Retry.Repeat(3)
                         .WithPolling(TimeSpan.FromSeconds(1))
                         .WithTimeout(TimeSpan.FromSeconds(10))
                         .Until(() => PostCSVLine(CSVLine));
                    }
                    catch (Exception)
                    {
                        log.DebugFormat("Max retries reached, skipping line: {0}", CSVLine);
                    }  
                }      
            }
        }

        private bool HasValidNumberOfFields(IList<string> csvFields)
        {
            return csvFields.Count == 6;
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
                log.ErrorFormat("API responsed with an error {0}: {1}", response.error, line);
            }

            return response.success;
        }
    }
}
