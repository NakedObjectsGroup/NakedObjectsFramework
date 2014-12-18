// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcTestApp.Tests.Util;

namespace MvcTestApp.Tests.Routing {
    [TestClass]
    public class RoutingTest {
        [TestMethod]
        public void TestAction() {
            ContextMocks.TestRoute("~/anything/Action/anAction", new {controller = "Generic", action = "Action"});
            ContextMocks.TestRoute("~/anythingElse/Action/anAction", new {controller = "Generic", action = "Action"});
        }

        [TestMethod]
        public void TestActionOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "Action"});
            Assert.AreEqual("/Generic/Action", result.VirtualPath);
        }

        [TestMethod]
        public void TestDetails() {
            ContextMocks.TestRoute("~/anything/Details", new {controller = "Generic", action = "Details"});
            ContextMocks.TestRoute("~/anythingElse/Details", new {controller = "Generic", action = "Details"});
        }

        [TestMethod]
        public void TestDetailsOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "Details"});
            Assert.AreEqual("/Generic/Details", result.VirtualPath);
        }

        [TestMethod]
        public void TestDialog() {
            ContextMocks.TestRoute("~/anything/Dialog", new {controller = "Generic", action = "Dialog"});
            ContextMocks.TestRoute("~/anythingElse/Dialog", new {controller = "Generic", action = "Dialog"});
        }

        [TestMethod]
        public void TestDialogOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "Dialog"});
            Assert.AreEqual("/Generic/Dialog", result.VirtualPath);
        }

        [TestMethod]
        public void TestEdit() {
            ContextMocks.TestRoute("~/anything/Edit", new {controller = "Generic", action = "Edit"});
            ContextMocks.TestRoute("~/anythingElse/Edit", new {controller = "Generic", action = "Edit"});
        }

        [TestMethod]
        public void TestEditObject() {
            ContextMocks.TestRoute("~/anything/EditObject", new {controller = "Generic", action = "EditObject"});
            ContextMocks.TestRoute("~/anythingElse/EditObject", new {controller = "Generic", action = "EditObject"});
        }

        [TestMethod]
        public void TestEditObjectOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "EditObject"});
            Assert.AreEqual("/Generic/EditObject", result.VirtualPath);
        }

        [TestMethod]
        public void TestEditOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "Edit"});
            Assert.AreEqual("/Generic/Edit", result.VirtualPath);
        }

        [TestMethod]
        public void TestGetFile() {
            ContextMocks.TestRoute("~/anything/GetFile/file.ext", new {controller = "Generic", action = "GetFile"});
            ContextMocks.TestRoute("~/anythingElse/getFile/file.ext", new {controller = "Generic", action = "GetFile"});
        }

        [TestMethod]
        public void TestGetImageOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "GetImage"});
            Assert.AreEqual("/Generic/GetImage", result.VirtualPath);
        }

        [TestMethod]
        public void TestHome() {
            ContextMocks.TestRoute("~/", new {controller = "Home", action = "Index"});
        }

        [TestMethod]
        public void TestRemoveOutgoing() {
            VirtualPathData result = ContextMocks.GenerateUrlViaMocks(new {controller = "Generic", action = "Remove"});
            Assert.AreEqual("/Generic/Remove", result.VirtualPath);
        }
    }
}