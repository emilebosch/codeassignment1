using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
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
        private static ILog _log = LogManager.GetLogger(typeof(OVTransactionImport));
        TextFieldParser parser;
        SimpleRestApi api = new SimpleRestApi(ConfigurationManager.AppSettings["ovservice"]);
        private const int RETRY_COUNT = 10;

        /// <summary>
        /// Imports the given csv file
        /// </summary>
        public void StartTransactionImport(string csvfile)
        {
            int failedCount = 0;
            int attempts = 0;
            var objectsToPost = GetObjectsToPost(csvfile);
            var failedPosts = PostObjects(objectsToPost);

            while (attempts <= RETRY_COUNT)
            {
                if (failedPosts.Count == 0)
                {
                    break;
                }
            
                Thread.Sleep(10000);
                failedPosts = PostObjects(failedPosts);
                attempts++;
            }

            _log.InfoFormat("Processed {0} objects. Number of failed results: {1}", objectsToPost.Count, failedCount);

        }

        private IList<PostObject> PostObjects(IEnumerable<PostObject> objectsToPost)
        {
            IList<PostObject> failedPosts = new List<PostObject>();
            foreach (var postObject in objectsToPost)
            {
                try
                {
                    var response = api.Post("ovtransactionimport/process",
                             postObject,
                             new
                             {
                                 success = default(Boolean),
                                 error = default(String)
                             });

                    if (!response.success)
                    {
                        var message = string.Format("Failed to post object with ID {0}. ServerResponse {1}", postObject.id, response.error);
                        failedPosts.Add(postObject);
                        _log.Error(message);
                    }
                }
                catch (Exception exception)
                {
                    _log.Error("Exception while posting.", exception);
                    failedPosts.Add(postObject);
                }
            }

            return failedPosts;
        }

        private IList<PostObject> GetObjectsToPost(string csvfile)
        {
            IList<PostObject> returnObject = new List<PostObject>();
            parser = new TextFieldParser(csvfile);
            parser.Delimiters = new[] { "," };
            parser.ReadFields();

            while (!parser.EndOfData)
            {
                string[] csvFields = parser.ReadFields();
                PostObject postobject = GetPostobject(csvFields);
                if (postobject != null)
                {
                    returnObject.Add(postobject);
                }
            }

            return returnObject;
        }

        private PostObject GetPostobject(string[] csvFields)
        {
            try
            {
                return new PostObject()
                {
                    id = csvFields[0],
                    date = csvFields[1],
                    station = csvFields[2],
                    action = csvFields[3],
                    cardid = Convert.ToInt64(csvFields[4]),
                    userid = Convert.ToInt64(csvFields[5])
                };
            }
            catch (Exception exception)
            {
                string csvString = string.Empty;

                foreach (string csvField in csvFields)
                {
                    csvString += csvField + ",";
                }

                _log.Error(string.Format("Failed to Parse csv-data. Record data: {0}", csvString), exception);
            }

            return null;
        }

        private class PostObject
        {
            public string id { get; set; }
            public string date { get; set; }
            public string station { get; set; }
            public string action { get; set; }
            public long cardid { get; set; }
            public long userid { get; set; }
        }
    }
}
