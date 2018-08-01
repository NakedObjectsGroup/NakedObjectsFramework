// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    /// <summary>
    /// 
    /// </summary>
    public static class Helpers {
        private const string SourceFiles = @"..\..\Json reference files";
        private const string Txt = ".txt";
        private static bool WriteFileIfNoneExists = false;
        private static bool WriteFileIfResponseDiffersFromExisting = false;

        public static bool UrlExistsForGet(string url) {
            WebRequest httpRequest = WebRequest.Create(url);
            httpRequest.Method = Methods.Get;
            httpRequest.ContentLength = 0;
            httpRequest.ContentType = @"application/json";
            try {
                using (httpRequest.GetResponse()) {}
                return true;
            }
            catch (WebException) {
                return false;
            }
        }

        public static void TestResponse(string url, string fileName, string body = null, string method = Methods.Get, Codes expectedCode = Codes.Succeeded, string acceptHeader = null) {
            if (body != null && (method == Methods.Get || method == Methods.Delete)) {
                //Add the arguments as a url-encoded query string
                string args = HttpUtility.UrlEncode(body);
                url += "?" + args;
            }
            var httpRequest = (HttpWebRequest) WebRequest.Create(url);

            httpRequest.KeepAlive = false;
            httpRequest.Method = method;

            httpRequest.ContentType = @"application/json";
            if (acceptHeader != null) {
                httpRequest.Accept = acceptHeader;
            }
            if (body != null && method != Methods.Get) {
                httpRequest.ContentLength = body.Length;
                using (var sw = new StreamWriter(httpRequest.GetRequestStream())) {
                    using (var jr = new JsonTextWriter(sw)) {
                        jr.WriteRaw(body);
                    }
                }
            }
            else {
                httpRequest.ContentLength = 0;
            }

            if (expectedCode == Codes.Succeeded || expectedCode == Codes.SucceededNewRepresentation) {
                ErrorNotExpected(httpRequest, fileName);
            }
            else if (expectedCode == Codes.SucceededValidation) {
                ErrorNotExpectedNoContent(httpRequest);
            }
            else {
                ErrorExpected(httpRequest, (int) expectedCode);
            }
        }

        private static void ErrorNotExpectedNoContent(WebRequest httpRequest) {
            try {
                using (var response = (HttpWebResponse) httpRequest.GetResponse()) {
                    Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
                }
            }
            catch (WebException e) {
                Assert.Fail(e.Message);
            }
        }


        private static void ErrorNotExpected(WebRequest httpRequest, string fileName) {
            try {
                using (WebResponse response = httpRequest.GetResponse()) {
                    using (var sr = new StreamReader(response.GetResponseStream())) {
                        using (var jr = new JsonTextReader(sr)) {
                            var serializer = new JsonSerializer();

                            var result = serializer.Deserialize<JObject>(jr);

                            string resultAsString = result == null ? "" : result.ToString();

                            if (fileName != null) {
                                CheckResults(fileName, resultAsString);
                            }
                        }
                    }
                }
            }
            catch (WebException e) {
                var content = "";

                try {
                    using (var sr = new StreamReader(e.Response.GetResponseStream())) {
                        content = sr.ReadToEnd();
                    }
                }
                catch {
                    // suppress failures 
                }

                Assert.Fail(e.Message + " " + content);
            }
        }

        private static void ErrorExpected(WebRequest httpRequest, int expectedErrorCode) {
            try {
                using (httpRequest.GetResponse()) {
                    Assert.Fail("Request did not generate an error as expected");
                }
            }
            catch (WebException e) {
                Assert.IsTrue(e.Message.Contains("(" + expectedErrorCode + ")"), "Expected: " + expectedErrorCode + ", Actual:" + e.Message);
            }
        }

        public static string GetTestData(string fileName) {
            //string file = Path.Combine(sourceFiles, name) + txt;
            return File.ReadAllText(PathTo(fileName));
        }

        // for testcreation 

        public static void WriteTestData(string fileName, string data) {
            File.WriteAllText(PathTo(fileName), data);
        }

        public static void CheckResults(string fileName, string result) {
            string fileVersion = null;
            try {
                fileVersion = GetTestData(fileName);
            }
            catch (FileNotFoundException) {
                if (WriteFileIfNoneExists) {
                    WriteTestData(fileName, result);
                    Assert.Fail("No file found; results recorded as new file.");
                }
                else {
                    Assert.Fail("No file found.");
                }
            }

            if (fileVersion.Contains("<nextId>")) {
                fileVersion = fileVersion.Replace("<nextId>", JsonRep.CurrentId.ToString(CultureInfo.InvariantCulture));
            }

            // remove line endings so  it works on AV 
            fileVersion = fileVersion.Replace("\r", "").Replace("\n", "");
            result = result.Replace("\r", "").Replace("\n", "");

            if (fileVersion != result) {
                if (WriteFileIfResponseDiffersFromExisting) {
                    WriteTestData(fileName, result);
                    Assert.Fail("Results did not match file; file over-written with new version ");
                }
                else {
                    

                    if (fileVersion != result) {
                        
                        var len = fileVersion.Length >= result.Length ?  result.Length : fileVersion.Length;
                        var firstChange = 0; 

                        for (int i = 0; i < len; i++) {

                            if (fileVersion[i] != result[i]) {
                                firstChange = i;
                                break; 
                            }
                        }

                        Console.WriteLine("Strings differ from offset " + firstChange);

                        int s = firstChange - 200;
                        s = s < 0 ? 0 : s;
                        int e = firstChange + 200;
                        int e1 = e > fileVersion.Length - 1 ? fileVersion.Length - 1 : e;
                        int e2 = e > result.Length - 1 ? result.Length - 1 : e;

                        var expected = fileVersion.Substring(s, e1 - s);
                        var actual = result.Substring(s, e2 - s);

                        Console.WriteLine("Expected :" + expected);
                        Console.WriteLine("Actual :" + actual);


                    }
                    Assert.AreEqual(fileVersion, result, "Results did not match file.");

                    // Assert.Fail("Results did not match file.");
                }
            }
            //WriteTestData(resultsFile, s);
        }


        private static string PathTo(string fileName) {
            string machineName = Environment.MachineName;
            string suffix = @"Test\NakedObjects.Rest.Test.EndToEnd\Json reference files\" + fileName + Txt;

            if (machineName == "YOGA") {  //Richard's
                return @"C:\NakedObjectsFramework\" + suffix;
            }

            if (machineName == "STEF-PRECISION") {
                return @"E:\Users\scasc_000\Documents\GitHub\NakedObjectsFramework\" + suffix;
            }

            if (machineName == "STEF-LAPTOP") {
                return @"C:\Users\Stefano\Documents\GitHub\NakedObjectsFramework\" + suffix;
            }
     
            // Azure
            return @"C:\projects\nakedobjectsframework-2dqwa\" + suffix;
        }
    }
}