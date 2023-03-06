// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Configuration;
using ROSI.Interfaces;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

namespace NakedFramework.RATL.Helpers;

public class RestfulObjectsController : RestfulObjectsControllerBase {
    public RestfulObjectsController(IFrameworkFacade ff, ILogger<RestfulObjectsControllerBase> l, ILoggerFactory lf, IRestfulObjectsConfiguration c) : base(ff, l, lf, c) { }
}

public record TestInvokeOptions : IInvokeOptions {
    private readonly Func<RestfulObjectsControllerBase> factory;

    public TestInvokeOptions(Func<RestfulObjectsControllerBase> factory) {
        this.factory = factory;
    }


    public string Token { get; init; }
    public EntityTagHeaderValue Tag { get; init; }
    public virtual HttpClient HttpClient => new HttpClient(new StubHttpMessageHandler(factory()));

    public IDictionary<string, object>? ReservedArguments { get; set; }
}

public static class TestHelpers {
    private static DefaultHttpContext CreateTestHttpContext(IServiceProvider sp) {
        var uri = new Uri(@"http://localhost/");
        return new DefaultHttpContext {
            RequestServices = sp,
            Response = {
                Body = new MemoryStream()
            },
            Request = {
                Scheme = "http",
                Method = "GET",
                Host = new HostString(uri.Host),
                Path = new PathString(uri.PathAndQuery)
            }
        };
    }

    public static RestfulObjectsControllerBase SetMockContext(RestfulObjectsControllerBase api, IServiceProvider sp) {
        var mockContext = new ControllerContext();
        var httpContext = CreateTestHttpContext(sp);
        mockContext.HttpContext = httpContext;
        api.ControllerContext = mockContext;
        return api;
    }

    public static async Task<(string, int, ResponseHeaders)> ReadActionResult(ActionResult ar, HttpContext hc) {
        var testContext = new ActionContext { HttpContext = hc };
        await ar.ExecuteResultAsync(testContext);
        await using var s = testContext.HttpContext.Response.Body;
        s.Position = 0L;
        using var sr = new StreamReader(s);
        var json = await sr.ReadToEndAsync();
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

    public static (string, string) GetReturnTypeAndFormat(object toFind) {
        return toFind switch {
            string s => ("string", "string"),
            int i => ("number", "int"),
            _ => throw new NotImplementedException()
        };
    }


}