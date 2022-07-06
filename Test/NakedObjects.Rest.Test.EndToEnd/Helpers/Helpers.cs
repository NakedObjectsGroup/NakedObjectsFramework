// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net).
// All Rights Reserved. This code released under the terms of the
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html)

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd.Helpers;

/// <summary>
/// </summary>
public static class Helpers {
    private const string SourceFiles = @"..\..\Json reference files";
    private const string Txt = ".txt";
    private static readonly HttpClient Client = new();

    public static bool UrlExistsForGet(string url) {
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        var content = JsonContent.Create("", new MediaTypeHeaderValue("application/json"));
        httpRequest.Content = content;
        try {
            Client.Send(httpRequest);
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public static void TestResponse(string url, string fileName, string body = null, string method = Methods.Get, Codes expectedCode = Codes.Succeeded, string acceptHeader = null, string ifMatchHeader = null) {
        if (body != null && method is Methods.Get or Methods.Delete) {
            //Add the arguments as a url-encoded query string
            var args = HttpUtility.UrlEncode(body);
            url += $"?{args}";
        }

        var httpRequest = new HttpRequestMessage(new HttpMethod(method), url);

        if (acceptHeader is not null) {
            httpRequest.Headers.Add("Accept", acceptHeader);
        }

        if (ifMatchHeader != null) {
            httpRequest.Headers.Add("If-Match", "\"1234\"");
        }

        if (body != null && method != Methods.Get) {
            var stream = new MemoryStream(body.Length);
            var sw = new StreamWriter(stream);
            var jr = new JsonTextWriter(sw);
            jr.WriteRaw(body);
            jr.Flush();
            stream.Position = 0;
            var content = new StreamContent(stream);

            content.Headers.ContentType = new MediaTypeHeaderValue(@"application/json");
            httpRequest.Content = content;
        }

        switch (expectedCode) {
            case Codes.Succeeded or Codes.SucceededNewRepresentation:
                ErrorNotExpected(httpRequest, fileName);
                break;
            case Codes.SucceededValidation:
                ErrorNotExpectedNoContent(httpRequest);
                break;
            default:
                ErrorExpected(httpRequest, (int)expectedCode);
                break;
        }
    }

    private static void ErrorNotExpectedNoContent(HttpRequestMessage httpRequest) {
        try {
            using var response = Client.Send(httpRequest);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    private static void ErrorExpected(HttpRequestMessage httpRequest, int expectedErrorCode) {
        try {
            using var response = Client.Send(httpRequest);
            Assert.IsTrue((int)response.StatusCode == expectedErrorCode, $"Expected: {expectedErrorCode}, Actual:{response.StatusCode}");
            //Assert.Fail("Request did not generate an error as expected");
        }
        catch (Exception e) {
            Assert.IsTrue(e.Message.Contains($"({expectedErrorCode})"), $"Expected: {expectedErrorCode}, Actual:{e.Message}");
        }
    }

    private static void ErrorNotExpected(HttpRequestMessage httpRequest, string fileName) {
        try {
            using var response = Client.Send(httpRequest);
            using var sr = new StreamReader(response.Content.ReadAsStream());
            using var jr = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            var result = serializer.Deserialize<JObject>(jr);

            var resultAsString = result == null ? "" : result.ToString();

            if (fileName != null) {
                CheckResults(fileName, resultAsString);
            }
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    private static string GetTestData(string fileName) =>
        //string file = Path.Combine(sourceFiles, name) + txt;
        File.ReadAllText(PathTo(fileName));

    // for testcreation

    private static void WriteTestData(string fileName, string data) {
        File.WriteAllText(PathTo(fileName), data);
    }

    private static void CheckResults(string fileName, string result) {
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

        if (UseLocalUrl) {
            fileVersion = fileVersion.Replace(Urls.BaseUrlRemote, Urls.BaseUrlLocal);
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
                    var len = fileVersion.Length >= result.Length ? result.Length : fileVersion.Length;
                    var firstChange = 0;

                    for (var i = 0; i < len; i++) {
                        if (fileVersion[i] != result[i]) {
                            firstChange = i;
                            break;
                        }
                    }

                    Console.WriteLine($"Strings differ from offset {firstChange}");

                    var s = firstChange - 200;
                    s = s < 0 ? 0 : s;
                    var e = firstChange + 200;
                    var e1 = e > fileVersion.Length - 1 ? fileVersion.Length - 1 : e;
                    var e2 = e > result.Length - 1 ? result.Length - 1 : e;

                    var expected = fileVersion.Substring(s, e1 - s);
                    var actual = result.Substring(s, e2 - s);

                    Console.WriteLine($"Expected :{expected}");
                    Console.WriteLine($"Actual :{actual}");

                    Assert.Fail("Results did not match file.");
                }
                // Assert.AreEqual(fileVersion, result, "Results did not match file.");

                // Assert.Fail("Results did not match file.");
            }
        }
        //WriteTestData(resultsFile, s);
    }

    private static string PathTo(string fileName) {
        var machineName = Environment.MachineName;
        var suffix = $@"Test\NakedObjects.Rest.Test.EndToEnd\Json reference files\{fileName}{Txt}";

        return machineName switch {
            "YOGA" => $@"C:\NakedObjectsFramework\{suffix}",
            "DESKTOP-988MJTA" => $@"C:\GitHub\NakedObjectsFramework\{suffix}",
            _ => $@"C:\projects\nakedobjectsframework-4mlx1\{suffix}" // Azure
        };
    }

    // ReSharper disable ConvertToConstant.Local
    private static readonly bool WriteFileIfNoneExists = false;
    private static readonly bool WriteFileIfResponseDiffersFromExisting = false;

    public static bool UseLocalUrl = false;
    // ReSharper restore ConvertToConstant.Local
}