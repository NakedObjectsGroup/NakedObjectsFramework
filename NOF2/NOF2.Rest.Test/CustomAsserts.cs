// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NakedFunctions.Rest.Test;

public static class CustomAsserts {
    public static void AssertMember(this JToken member, string memberType, string id) {
        ClassicAssert.AreEqual(memberType, member["memberType"].ToString());
        ClassicAssert.AreEqual(id, member["id"].ToString());
    }

    public static void AssertAction(this JToken action, string id, string parameters) {
        action.AssertMember("action", id);
        ClassicAssert.AreEqual(parameters, action["parameters"].ToString());
    }

    public static void AssertAction(this JToken action, string id) {
        action.AssertMember("action", id);
    }

    public static void AssertProperty(this JToken property, string id, string value, bool hasChoices) {
        property.AssertMember("property", id);
        ClassicAssert.AreEqual(value, property["value"].ToString());
        ClassicAssert.AreEqual(hasChoices, property["hasChoices"].Value<bool>());
    }

    public static void AssertLink(this JToken link, string method, string rel, string type, string href) {
        ClassicAssert.AreEqual(method, link["method"].ToString());
        ClassicAssert.AreEqual(rel, link["rel"].ToString());
        ClassicAssert.AreEqual(type, link["type"].ToString());
        ClassicAssert.AreEqual(href, link["href"].ToString());
    }

    public static void AssertMenuLink(this JToken link, string title, string method, string id) {
        ClassicAssert.AreEqual(5, link.Count());
        ClassicAssert.AreEqual(title, link["title"].ToString());
        link.AssertLink(method,
                        $"urn:org.restfulobjects:rels/menu;menuId=\"{id}\"",
                        "application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8",
                        $"http://localhost/menus/{id}");
    }

    public static void AssertInvokeLink(this JToken link, string arguments, string method, string id, string href) {
        ClassicAssert.AreEqual(5, link.Count());
        ClassicAssert.AreEqual(arguments, link["arguments"].ToString());
        link.AssertLink(method,
                        $"urn:org.restfulobjects:rels/invoke;action=\"{id}\"",
                        "application/json; profile=\"urn:org.restfulobjects:repr-types/action-result\"; charset=utf-8",
                        href);
    }

    public static void AssertServiceInvokeLink(this JToken link, string arguments, string method, string service, string id) {
        ClassicAssert.AreEqual(5, link.Count());
        link.AssertInvokeLink(arguments,
                              method,
                              id,
                              $"http://localhost/services/{service}/actions/{id}/invoke");
    }

    public static void AssertMenuInvokeLink(this JToken link, string arguments, string method, string menu, string id) {
        ClassicAssert.AreEqual(5, link.Count());
        link.AssertInvokeLink(arguments,
                              method,
                              id,
                              $"http://localhost/menus/{menu}/actions/{id}/invoke");
    }

    public static void AssertObjectInvokeLink(this JToken link, string arguments, string method, string type, string instance, string id) {
        ClassicAssert.AreEqual(5, link.Count());
        link.AssertInvokeLink(arguments,
                              method,
                              id,
                              $"http://localhost/objects/{type}/{instance}/actions/{id}/invoke");
    }

    public static void AssertObjectElementLink(this JToken link, string title, string method, string type, string instance) {
        ClassicAssert.AreEqual(5, link.Count());
        ClassicAssert.AreEqual(title, link["title"].ToString());
        link.AssertLink(method,
                        "urn:org.restfulobjects:rels/element",
                        $"application/json; profile=\"urn:org.restfulobjects:repr-types/object\"; charset=utf-8; x-ro-domain-type=\"{type}\"",
                        $"http://localhost/objects/{type}/{instance}");
    }

    public static void AssertExtensions(this JToken extensions, int count) {
        ClassicAssert.AreEqual(count, extensions.Count());
    }

    public static void AssertObject(this JToken obj, string title, string type, string instance) {
        ClassicAssert.AreEqual(instance, obj["instanceId"].ToString());
        ClassicAssert.AreEqual(type, obj["domainType"].ToString());
        ClassicAssert.AreEqual(title, obj["title"].ToString());
    }
}