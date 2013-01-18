using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Args.Help;
using log4net;

namespace OVImport
{
    class Program
    {
		static ILog _logger = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
			log4net.Config.XmlConfigurator.Configure();
			InitArgs(args);

			if (Arguments.Current.Action == ActionType.Import)
			{
				var process = new OVTransactionImport();
				try
				{
					process.RunTransactionImport(Arguments.Current.Csv);
					_logger.Info("Completed run.");
				}
				catch (Exception e)
				{
					// log exception
					_logger.ErrorFormat("Stopped because of a failure: {0}", e);
					_logger.WarnFormat("The batch can be resumed using the SkipUntil switch like this: /su {0}", process.LastSuccessfullyProcessed);
				}
				_logger.InfoFormat("Read {0} records. Imported {1} transactions.",
					process.ApiCaller.ItemsProcessed,
					process.SuccessLogger.ItemsProcessed);
				if (process.FunctionalWarningsLogged)
				{
					_logger.WarnFormat("There were some issues with some of the records. Send the functional log {0} file to internal affairs.", 
						"funclog.txt");
				}

			}
			if (Arguments.Current.Action == ActionType.Help)
			{
				Usage();
			}
        }

		private static void InitArgs(string[] args)
		{
			if (args.Length == 0) args = new string[1] { "help" };
			Arguments.Current = Args.Configuration.Configure<Arguments>().CreateAndBind(args);
		}

		private static void Usage()
		{
			var h = new HelpProvider();
			var m = h.GenerateModelHelp(Args.Configuration.Configure<Arguments>());
			Console.WriteLine(m.HelpText);
			Console.WriteLine();
			foreach (var mem in m.Members)
			{
				if (mem.Switches.Count() > 0)
				{
					string defaultText = String.IsNullOrEmpty(mem.DefaultValue) ? "" : String.Format("(default: {0})", mem.DefaultValue);
					Console.WriteLine(" /{3} {0} - {1} {2}", mem.Name, mem.HelpText, defaultText, mem.Switches.First());
				}
			}
			
		}
	}
}
