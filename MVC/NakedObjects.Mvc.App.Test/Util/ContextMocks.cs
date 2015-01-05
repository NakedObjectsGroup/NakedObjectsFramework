// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Moq;
using NakedObjects.Mvc.App;
using NUnit.Framework;

namespace MvcTestApp.Tests.Util {
    public static class Constants {
#if AV
        public static string Server = @"(local)\SQL2012SP1";
#else
        public static string Server = @".\SQLEXPRESS";
#endif
    }


    /// <summary>
    /// Helper code lifted from Pro ASP.NET MVC Framework - Sanderson 
    /// </summary>
    public class ContextMocks {
        public ContextMocks(ControllerBase onController) {
            HttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            ViewContext = new Mock<ViewContext>();
            View = new Mock<IView>();
            ViewDataContainer = new Mock<IViewDataContainer>();


            onController.ValueProvider = new DictionaryValueProvider<string>(new Dictionary<string, string>(), null);


            HttpContext.Setup(x => x.Request).Returns(Request.Object);
            HttpContext.Setup(x => x.Response).Returns(Response.Object);
            HttpContext.Setup(x => x.Session).Returns(new FakeSessionState());
            HttpContext.Setup(x => x.User).Returns(new WindowsPrincipal(WindowsIdentity.GetCurrent()));
            Request.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Response.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            Request.Setup(x => x.Form).Returns(new NameValueCollection());
            Request.Setup(x => x.ApplicationPath).Returns("/");
            Request.Setup(x => x.Files).Returns(new Mock<HttpFileCollectionBase>().Object);

            var rc = new RequestContext(HttpContext.Object, new RouteData());
            onController.ControllerContext = new ControllerContext(rc, onController);

            ViewContext.Setup(x => x.Controller).Returns(onController);
            ViewContext.Setup(x => x.HttpContext).Returns(HttpContext.Object);
            ViewContext.Setup(x => x.RouteData).Returns(RouteData);
            ViewContext.Setup(x => x.TempData).Returns(new TempDataDictionary());
            ViewContext.Setup(x => x.View).Returns(View.Object);
            ViewContext.Setup(x => x.ViewData).Returns(new ViewDataDictionary());
            ViewDataContainer.Setup(x => x.ViewData).Returns(ViewContext.Object.ViewData);

            HtmlHelper = new HtmlHelper(ViewContext.Object, ViewDataContainer.Object);
        }

        public Mock<ViewContext> ViewContext { get; private set; }
        public Mock<HttpContextBase> HttpContext { get; private set; }
        public Mock<HttpRequestBase> Request { get; private set; }
        public Mock<HttpResponseBase> Response { get; private set; }
        public Mock<IView> View { get; private set; }
        public RouteData RouteData { get; private set; }
        public Mock<IViewDataContainer> ViewDataContainer { get; private set; }
        public HtmlHelper HtmlHelper { get; private set; }

        public HtmlHelper<T> GetHtmlHelper<T>() {
            return new HtmlHelper<T>(ViewContext.Object, ViewDataContainer.Object);
        }

        public static RouteData TestRoute(string url, object expectedValues) {
            var routeConfig = new RouteCollection();
            RouteConfig.RegisterRoutes(routeConfig);
            Mock<HttpContextBase> mockHttpContext = MakeMockHttpContext(url);

            RouteData routeData = routeConfig.GetRouteData(mockHttpContext.Object);

            Assert.IsNotNull(routeData.Route, "No route was matched");

            var expectedDict = new RouteValueDictionary(expectedValues);

            foreach (var expectedVal in expectedDict) {
                if (expectedVal.Value == null) {
                    Assert.IsNull(routeData.Values[expectedVal.Key]);
                }
                else {
                    Assert.AreEqual(expectedVal.Value.ToString(),
                        routeData.Values[expectedVal.Key].ToString());
                }
            }
            return routeData;
        }

        public static VirtualPathData GenerateUrlViaMocks(object values) {
            var routeConfig = new RouteCollection();
            RouteConfig.RegisterRoutes(routeConfig);
            Mock<HttpContextBase> mockContext = MakeMockHttpContext(null);
            var context = new RequestContext(mockContext.Object, new RouteData());
            return routeConfig.GetVirtualPath(context, new RouteValueDictionary(values));
        }

        private static Mock<HttpContextBase> MakeMockHttpContext(string url) {
            var mockHttpContext = new Mock<HttpContextBase>();

            var mockRequest = new Mock<HttpRequestBase>();
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockRequest.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns(url);

            var mockResponse = new Mock<HttpResponseBase>();
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);
            mockResponse.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(x => x);

            return mockHttpContext;
        }

        #region Nested type: FakeSessionState

        private class FakeSessionState : HttpSessionStateBase {
            private readonly Dictionary<string, object> items = new Dictionary<string, object>();

            public override object this[string name] {
                get { return items.ContainsKey(name) ? items[name] : null; }
                set { items[name] = value; }
            }

            public override void Add(string name, object value) {
                items[name] = value;
            }

            public override void Remove(string name) {
                items.Remove(name);
            }
        }

        #endregion
    }
}