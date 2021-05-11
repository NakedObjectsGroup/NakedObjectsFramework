// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Authentication;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Configuration;
using Template.RestTest.TestCase;

namespace Template.RestTest.Helpers {
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade ff, ILogger<RestfulObjectsControllerBase> l, ILoggerFactory lf, IRestfulObjectsConfiguration c) : base(ff, l, lf, c) { }
    }

    public class TestSession : WindowsSession {
        public TestSession(IPrincipal principal) : base(principal) { }
    }

    public static class TestHelpers {
        public static IPrincipal TestPrincipal => CreatePrincipal("Test", Array.Empty<string>());
        private static IPrincipal CreatePrincipal(string name, string[] roles) => new GenericPrincipal(new GenericIdentity(name), roles);

        private static DefaultHttpContext CreateTestHttpContext(IServiceProvider sp) {
            var httpContext = new DefaultHttpContext {RequestServices = sp};
            httpContext.Response.Body = new MemoryStream();

            var uri = new Uri(@"http://localhost/");
            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString(uri.Host);
            httpContext.Request.Path = new PathString(uri.PathAndQuery);
            httpContext.Request.Method = "GET";

            return httpContext;
        }

        private static RestfulObjectsControllerBase SetMockContext(RestfulObjectsControllerBase api, IServiceProvider sp) {
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

        public static RestfulObjectsControllerBase AsMethod(this RestfulObjectsControllerBase api, Methods method) {
            api.HttpContext.Request.Method = method.ToString().ToUpper();
            return api;
        }

        public static RestfulObjectsControllerBase AsPost(this RestfulObjectsControllerBase api) => api.AsMethod(Methods.Post);

        public static RestfulObjectsControllerBase AsPut(this RestfulObjectsControllerBase api) => api.AsMethod(Methods.Put);

        public static RestfulObjectsControllerBase AsGet(this RestfulObjectsControllerBase api) => api.AsMethod(Methods.Get);

        public static string FullName<T>() => typeof(T).FullName;

        public static RestfulObjectsControllerBase GetController(AbstractRestTest tc) {
            var sp = tc.GetScopeServices();
            var api = sp.GetService<RestfulObjectsController>();
            return SetMockContext(api, sp);
        }
    }
}