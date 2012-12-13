using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;

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

        /// <summary>
        /// Imports the given csv file
        /// </summary>
        public void StartTransactionImport(string csvfile)
        {
            api = new SimpleRestApi(ConfigurationManager.AppSettings["ovservice"]);

            parser = new TextFieldParser(csvfile);
            parser.Delimiters = new [] {","};
            parser.ReadFields();

            while (!parser.EndOfData)
            {
                var csvFields = parser.ReadFields();
                var apiresponse = 
                    api.Post("ovtransactionimport/process", 
                    new
                    {
                        id = Convert.ToInt64(csvFields[0]),
                        date = csvFields[1],
                        station = csvFields[2],
                        action = csvFields[3],
                        cardid = Convert.ToInt64(csvFields[4]),
                        userid = Convert.ToInt64(csvFields[5])
                    },
                    new
                    {
                        success = default(Boolean),
                        error = default(String)
                    });

                if (apiresponse.success)
                {
                    Console.WriteLine("Succesfully uploaded transaction!");
                }
                else
                {
                    throw new Exception("OMG ERROER!!!!");

                }
            }
        }
    }
}
