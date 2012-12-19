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

		[ArgsMemberSwitch("csv"), DefaultValue("data/sample.csv")]
		public string Csv { get; set; }
		[ArgsMemberSwitch("api"), DefaultValue("http://localhost:5000/")]
		public string ApiRoot { get; set; }
		[ArgsMemberSwitch("su") ]
		public string SkipUntil { get; set; }
		[ArgsMemberSwitch("p"), DefaultValue(1)]
		public int ParallelPosts { get; set; }
		[ArgsMemberSwitch("r"), DefaultValue(1), Description("test")]
		public int PostAttempts { get; set; }

		public static Arguments Current { get; set; }


	}
	public enum ActionType
	{
		Import,
		Help
	}
}
