// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.Performance {
    [TestClass]
    public class ObjectTest {
        [TestMethod]
        public void SimpleObjectRetrieval() {
            var url = "http://localhost:57840/objects/MostSimple/1";
            var count = 100;

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {

                    var httpRequest = (HttpWebRequest) WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "GET";

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse) httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);
                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

        [TestMethod]
        public void WithScalarsObjectRetrieval() {
            var url = "http://localhost:57840/objects/WithScalars/1";
            var count = 100;

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "GET";

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);
                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

        [TestMethod]
        public void MixedObjectRetrieval() {
            var url1 = "http://localhost:57840/objects/WithScalars/1";
            var url2 = "http://localhost:57840/objects/WithActionObject/1";
            var url3 = "http://localhost:57840/objects/WithCollection/1";
            var url4 = "http://localhost:57840/objects/WithValue/1";
            var url5 = "http://localhost:57840/objects/WithCollection/1";
            var url6 = "http://localhost:57840/objects/WithAttachments/1";
            var url7 = "http://localhost:57840/objects/WithReference/1";

            var count = 1000;
            var urls = new[] {url1, url2, url3, url4, url5, url6, url7};

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {
                    var url = urls[i % urls.Length ];

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "GET";

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);

                  
                    Thread.Sleep(100);
                  
                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

        [TestMethod]
        public void SimpleObjectAction() {
            var url1 = "http://localhost:57840/objects/WithActionObject/1/actions/AnAction/invoke";
          

            var count = 100;
            var urls = new[] { url1 };

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {
                    var url = urls[i % urls.Length];

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "POST";
                    httpRequest.ContentLength = 0;

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);


                    Thread.Sleep(100);

                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

        public static JObject MostSimple1AsRef() {
            return ObjectAsRef(@"MostSimple/1");
        }

       
        private static JObject ObjectAsRef(string oid) {
            return new JObject(new JProperty("href", "http://localhost:57840/objects/" + oid));
        }

   
        [TestMethod]
        public void ParmsObjectAction() {
            var url1 = "http://localhost:57840/objects/WithActionObject/1/actions/AnActionReturnsObjectWithParameters/invoke";
            var url2 = "http://localhost:57840/objects/WithActionObject/1/actions/AnActionReturnsCollectionWithParameters/invoke";

            var count = 1000;
            var urls = new[] { url1, url2 };

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {
                    var url = urls[i % urls.Length];

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "POST";
                    

                    httpRequest.ContentType = @"application/json";

                    var parm1 = new JObject(new JProperty("value", 101));
                    var parm2 = new JObject(new JProperty("value", MostSimple1AsRef()));
                    var parms = new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2)); 

                    var body = parms.ToString();

                    httpRequest.ContentLength = body.Length;
                    using (var sw = new StreamWriter(httpRequest.GetRequestStream())) {
                        using (var jr = new JsonTextWriter(sw)) {
                            jr.WriteRaw(body);
                        }
                    }

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);


                    Thread.Sleep(100);

                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

        [TestMethod]
        public void CollectionPropertyAsTable() {
            var url = "http://localhost:57840/objects/WithCollection/1/collections/ACollection?x-ro-inline-collection-items=true";
            var count = 100;

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "GET";

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);
                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }
        [TestMethod]
        public void Prompts() {
            var url = "http://localhost:57840/objects/WithReference/1/properties/AnAutoCompleteReference/prompt";
            var count = 100;

            var value = new JObject(new JProperty("value", "aaa"));
            var search = new JObject(new JProperty("x-ro-searchTerm", value));
            var body =    search.ToString();
            string args = HttpUtility.UrlEncode(body);
            url += "?" + args;

            for (int i = 0; i < count; i++) {

                //Console.WriteLine("Starting iteration#" + i);

                try {

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.KeepAlive = false;
                    httpRequest.Method = "GET";

                    httpRequest.ContentType = @"application/json";

                    using (var response = (HttpWebResponse)httpRequest.GetResponse()) {
                        // Code here 

                        var rc = response.StatusCode;
                    }
                    //Console.WriteLine("Complete iteration#" + i);
                }
                catch (Exception e) {
                    //Console.WriteLine("Failed iteration#" + i  + " " + e.Message);
                }
            }
        }

    }
}