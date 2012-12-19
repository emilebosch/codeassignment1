using System;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace OVTransactionWebservice.Controllers
{
    /// <summary>
    /// THIS HAS BEEN MADE BY CAPGEMINI. CAP IS THE ONLY EVER OWNER OF THIS CODE. 
    /// YOU CAN NOT EVER CHANGE THIS BECAUSE WE HAVE AN ARMY OF LAWYERS!
    /// 
    /// Made by: Minakshi Avani damini
    /// </summary>
    [HandleError]
    public class OVTransactionImportController : Controller
    {
        /// <summary>
        /// Public transactionrecord.
        /// </summary>
        public class TransactionRecord 
        {
            public string station { get; set; }
            public int cardid { get; set; }
            public int userid { get; set; }
            public string eventcode { get; set; }
            public string action { get; set; }
            public DateTime time { get; set; }
        }

        /// <summary>
        /// Process a transaction
        /// </summary>
        [HttpPost]
        public ActionResult Process() 
        {
            var record = this.GetJson <TransactionRecord>();
			if (record.userid == 1019)
			{
				return Json(new { sucess = false, error = "USERNOTFOUND" });
			}

			if (record.cardid == 2782828)
			{
				return Json(new { sucess = false, error = "CARDNOTFOUND" });
			}

            return Json(new { success = true});    
        }
    }

    public static class ControllerExtensions
    {
        public static T GetJson<T>(this Controller t)
        {
            string json = null;
            using (var stream = t.Request.InputStream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                    json = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
