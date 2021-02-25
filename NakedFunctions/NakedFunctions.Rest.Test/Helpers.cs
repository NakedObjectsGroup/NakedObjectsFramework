// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NakedFramework.Facade.Facade;
using NakedFramework.Rest.Configuration;
using NakedFramework.Facade;
using NakedObjects.Rest;

namespace NakedFunctions.Rest.Test {
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade ff, ILogger<RestfulObjectsControllerBase> l, ILoggerFactory lf, IRestfulObjectsConfiguration c) : base(ff, l, lf, c) { }
    }

    public static class Helpers {
        public static DefaultHttpContext CreateTestHttpContext(IServiceProvider sp) {
            var httpContext = new DefaultHttpContext {RequestServices = sp};
            httpContext.Response.Body = new MemoryStream();

            var uri = new Uri(@"http://localhost/");
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString(uri.Host);
            httpContext.Request.Path = new PathString(uri.PathAndQuery);
            httpContext.Request.Method = "GET";

            return httpContext;
        }

        public static RestfulObjectsControllerBase SetMockContext(RestfulObjectsControllerBase api, IServiceProvider sp) {
            var mockContext = new ControllerContext();
            var httpContext = CreateTestHttpContext(sp);
            mockContext.HttpContext = httpContext;
            api.ControllerContext = mockContext;
            return api;
        }

        public static (string, int, ResponseHeaders) ReadActionResult(ActionResult ar, HttpContext hc) {
            var testContext = new ActionContext {HttpContext = hc};
            ar.ExecuteResultAsync(testContext).Wait();
            using var s = testContext.HttpContext.Response.Body;
            s.Position = 0L;
            using var sr = new StreamReader(s);
            var json = sr.ReadToEnd();
            var statusCode = testContext.HttpContext.Response.StatusCode;
            var headers = testContext.HttpContext.Response.GetTypedHeaders();
            return (json, statusCode, headers);
        }

        public static RestfulObjectsControllerBase AsPost(this RestfulObjectsControllerBase api) {
            api.HttpContext.Request.Method = "POST";
            return api;
        }

        public static RestfulObjectsControllerBase AsPut(this RestfulObjectsControllerBase api) {
            api.HttpContext.Request.Method = "PUT";
            return api;
        }

        public static RestfulObjectsControllerBase AsGet(this RestfulObjectsControllerBase api) {
            api.HttpContext.Request.Method = "GET";
            return api;
        }

        public static string FullName<T>() => typeof(T).FullName;
    }
}