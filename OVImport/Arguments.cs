using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Args;
using System.ComponentModel;

namespace OVImport
{
	[Description(@"OVImport
Usage: OVImport (import|help) [switches]

examples: OVimport import -csv ""datafile.csv""
          OVimport import -csv ""datafile.csv"" -skipUntil 1207")]
	public class Arguments
	{
		[ArgsMemberSwitch(0)]
		public ActionType Action { get; set; }

		[ArgsMemberSwitch("csv"), DefaultValue("data/sample.csv"), Description("The file containing the transactions to upload.\n\t\t")]
		public string Csv { get; set; }
		[ArgsMemberSwitch("api"), DefaultValue("http://localhost:5000/")]
		public string ApiRoot { get; set; }
		[ArgsMemberSwitch("su"), Description("All IDs appearing in the CSV file before this ID will be \n\t\tskipped. Used to resume a failed process.")]
		public string SkipUntil { get; set; }
		[ArgsMemberSwitch("p"), DefaultValue(1), Description("Values >1 will cause multiple transactions to be posted \n\t\tin parallel.")]
		public int ParallelPosts { get; set; }
		[ArgsMemberSwitch("r"), DefaultValue(0), Description("When an unexpected error is returned from the service, a \n\t\tretry is performed after a short wait. This can help against \n\t\tfailing batches by short failures and network hiccups.")]
		public int PostRetries { get; set; }

		public static Arguments Current { get; set; }


	}
	public enum ActionType
	{
		Import,
		Help
	}
}
