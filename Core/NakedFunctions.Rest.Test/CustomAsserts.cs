// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {
    public static class CustomAsserts {
        public static void AssertMember(this JToken member, string memberType, string id) {
            Assert.AreEqual(memberType, member["memberType"].ToString());
            Assert.AreEqual(id, member["id"].ToString());
        }

        public static void AssertAction(this JToken action, string id, string parameters) {
            action.AssertMember("action", id);
            Assert.AreEqual(parameters, action["parameters"].ToString());
        }

        public static void AssertProperty(this JToken property, string id, string value, bool hasChoices) {
            property.AssertMember("property", id);
            Assert.AreEqual(value, property["value"].ToString());
            Assert.AreEqual(hasChoices, property["hasChoices"].Value<bool>());
        }

        public static void AssertLink(this JToken link, string method, string rel, string type, string href) {
            Assert.AreEqual(method, link["method"].ToString());
            Assert.AreEqual(rel, link["rel"].ToString());
            Assert.AreEqual(type, link["type"].ToString());
            Assert.AreEqual(href, link["href"].ToString());
        }

        public static void AssertMenuLink(this JToken link, string title, string method, string id) {
            Assert.AreEqual(5, link.Count());
            Assert.AreEqual(title, link["title"].ToString());
            link.AssertLink(method,
                            $"urn:org.restfulobjects:rels/menu;menuId=\"{id}\"",
                            "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                            $"http://localhost/menus/{id}");
        }

        public static void AssertInvokeLink(this JToken link, string arguments, string method, string id, string href) {
            Assert.AreEqual(5, link.Count());
            Assert.AreEqual(arguments, link["arguments"].ToString());
            link.AssertLink(method,
                            $"urn:org.restfulobjects:rels/invoke;action=\"{id}\"",
                            "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result\"; charset=utf-8",
                            href);
        }


        public static void AssertServiceInvokeLink(this JToken link, string arguments, string method, string service, string id) {
            Assert.AreEqual(5, link.Count());
            link.AssertInvokeLink(arguments,
                                  method,
                                  id,
                                  $"http://localhost/services/{service}/actions/{id}/invoke");
        }

        public static void AssertObjectInvokeLink(this JToken link, string arguments, string method, string type, string instance, string id) {
            Assert.AreEqual(5, link.Count());
            link.AssertInvokeLink(arguments,
                                  method,
                                  id,
                                  $"http://localhost/objects/{type}/{instance}/actions/{id}/invoke");
        }

     
        public static void AssertObjectElementLink(this JToken link, string title, string method, string type, string instance)
        {
            Assert.AreEqual(5, link.Count());
            Assert.AreEqual(title, link["title"].ToString());
            link.AssertLink(method,
                            "urn:org.restfulobjects:rels/element",
                            $"application/json; profile=\"urn:org.restfulobjects:repr-types/object\"; charset=utf-8; x-ro-domain-type=\"{type}\"",
                            $"http://localhost/objects/{type}/{instance}");
        }


        public static void AssertExtensions(this JToken extensions, int count) {
            Assert.AreEqual(count, extensions.Count());
        }
    }
}