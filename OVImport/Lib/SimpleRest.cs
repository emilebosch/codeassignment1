﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace OVImport
{
    /// <summary>
    /// Super minimal layer for rest requests 
    /// </summary>
    class SimpleRestApi
    {    
        string rootUrl;
        NetworkCredential credentials = null;

        public SimpleRestApi(string rootUrl, NetworkCredential credentials = null)
        {
            this.rootUrl = rootUrl;
            this.credentials = credentials;
        }

        /// <summary>
        /// Send a POST request to a rest endpoint
        /// </summary>
        public Response Post<Response>(string endpoint, object req, Response r)
        {
            var request = Request(endpoint, "POST");
            var json = JsonConvert.SerializeObject(req);
            //Console.WriteLine(json);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }

            var resp = request.GetResponse();
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                string data = reader.ReadToEnd();
                return JsonConvert.DeserializeAnonymousType<Response>(data, r);
            }
        }

        /// <summary>
        /// Send a PUT request to a rest endpoint
        /// </summary>
        public Response Put<Response>(string endpoint, object req, Response r)
        {
            var request = Request(endpoint, "PUT");
            var json = JsonConvert.SerializeObject(req);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }

            var resp = request.GetResponse();
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                string data = reader.ReadToEnd();
                return JsonConvert.DeserializeAnonymousType<Response>(data, r);
            }
        }

        /// <summary>
        /// Send a GET request to a rest endpoint
        /// </summary>
        public Response Get<Response>(string url, Response r)
        {
            var request = Request(url, "GET");
            var resp = request.GetResponse();
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                string data = reader.ReadToEnd();
                return JsonConvert.DeserializeAnonymousType<Response>(data, r);
            }
        }

        /// <summary>
        /// Create a web request with given method.
        /// </summary>
        private WebRequest Request(string url, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(rootUrl + url));
            request.Method = method;
            request.ContentType = "application/json";
            request.KeepAlive = false;                          //Keep alive kills the connection pool :( 
            request.ProtocolVersion = HttpVersion.Version10;
            request.ServicePoint.ConnectionLimit = 1;           
  
            if (credentials == null) 
                return request;
 
            if (String.IsNullOrEmpty(credentials.Password))
            {
                var authDigest = Convert.ToBase64String(Encoding.Default.GetBytes(credentials.UserName));
                request.Headers["Authorization"] = "Basic " + authDigest;
            }
            else
            {
                request.Credentials = credentials;
            }

            return request;
        }
    }
}
