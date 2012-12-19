using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;
using BatchFlow;
using OVImport.Lib;
using System.IO;
using log4net;

namespace OVImport
{
    /// <summary>
    /// This class is responsible for reading the transaction log and submitting 
    /// it to the OV servers
    /// </summary>
    public class OVTransactionImport
    {
		static ILog _logger = LogManager.GetLogger(typeof(OVTransactionImport));
		static ILog _functionalLogger = LogManager.GetLogger("Functional");

		public bool FunctionalWarningsLogged { get; set; }
		void LogFunctionalProblem(string error, string[] line)
		{
			_functionalLogger.Warn(String.Format("# {0}:\n{1}", error, String.Join(",", line)));
			this.FunctionalWarningsLogged = true;
		}

		Flow importFlow;
		public OVTransactionImport()
		{
			importFlow = Flow.FromAsciiArt(@"r-->a-->s",
					  new Dictionary<char, TaskNode>()
					  {
						  {'r', GetCsvReader()},
						  {'a', GetApiCaller()},
						  {'s', GetSuccessLogger()}
					  }
					  );

		}
		/// <summary>
		/// Imports the given csv file
		/// </summary>
		public void RunTransactionImport(string csvfile)
		{
			this.FunctionalWarningsLogged = false;
			_logger.Info("Setting up flow");
			importFlow.Start();
			_logger.Info("Started flow");
			importFlow.RunToCompletion();
		}

		const string CSV_READER_NODE = "CsvReader";
		const string API_CALLER_NODE = "ApiCaller";
		const string SUCCESS_LOGGER_NODE = "SuccessLogger";


		private TaskNode GetCsvReader()
		{
			StartPoint<string[]> csvReaderNode = new StartPoint<string[]>(
				(outQueue) =>
				{
					TextFieldParser parser = new TextFieldParser(Arguments.Current.Csv);
					parser.Delimiters = new[] { "," };
					parser.ReadFields(); // skip first line

					string waitingFor = Arguments.Current.SkipUntil;
					while (!parser.EndOfData)
					{
						var fields = parser.ReadFields();
						if (waitingFor != null)
						{
							if (fields[0] == waitingFor)
							{
								waitingFor = null;
							}
							continue;
						}
						outQueue.Send(fields);
					}
				}
				);
			csvReaderNode.Name = CSV_READER_NODE;
			return csvReaderNode;
		}

		private TaskNode GetApiCaller()
		{
			SimpleRestApi api = new SimpleRestApi(Arguments.Current.ApiRoot);
			TaskNode<string[], string> apiCaller = new TaskNode<string[], string>(
				(csvFields, successOut) =>
				{
					long cardId;
					long userId;
					try
					{
						cardId = long.Parse(csvFields[4]);
						userId = long.Parse(csvFields[5]);
					}
					catch (Exception)
					{
						LogFunctionalProblem("Error occurred while reading userId and cardId", csvFields);
						return;
					}
					var request =
						new
						{
							id = csvFields[0],
							date = csvFields[1],
							station = csvFields[2],
							action = csvFields[3],
							cardid = cardId,
							userid = userId
						};
					var apiresponse =
						api.Post("ovtransactionimport/process",
						request,
						new
						{
							success = default(Boolean),
							error = default(String)
						});

					if (apiresponse.success)
					{
						successOut.Send(csvFields[0]);
					}
					else
					{
						LogFunctionalProblem(apiresponse.error, csvFields);
						return;
					}
					_logger.DebugFormat("Processed id {0}", csvFields[0]);
				}
				);
			apiCaller.Name = API_CALLER_NODE;
			apiCaller.ThreadNumber = Arguments.Current.ParallelPosts;
			apiCaller.Retries = Arguments.Current.PostRetries;
			apiCaller.KeepOrder = true;
			return apiCaller;
		}

		private TaskNode GetSuccessLogger()
		{
			EndPoint<string> successLogger = new EndPoint<string>(
				(id) => {
					this.LastSuccessfullyProcessed = id;
				}
			);
			successLogger.Name = SUCCESS_LOGGER_NODE;
			return successLogger;
		}
		public string LastSuccessfullyProcessed { get; set; }
		public TaskNode SuccessLogger { get { return importFlow.GetTask(SUCCESS_LOGGER_NODE); } }
		public TaskNode ApiCaller { get { return importFlow.GetTask(API_CALLER_NODE); } }
		public TaskNode CsvReader { get { return importFlow.GetTask(CSV_READER_NODE); } }

    }
}
