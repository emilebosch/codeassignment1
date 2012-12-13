using System;
using System.Web.Mvc;

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
            public string station;
            public int cardid;
            public int userid;
            public string eventcode;
            public string action;
            public DateTime time;
        }

        /// <summary>
        /// Process a transaction
        /// </summary>
        [HttpPost]
        public ActionResult Process(TransactionRecord record) 
        {
            return Json(new { success = true});    
        }
    }
}
