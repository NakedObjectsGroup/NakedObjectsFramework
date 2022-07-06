// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;
/*
 * These tests clear properties.  To avoid clashes with other tests, they create all the objects that they modify.
 */

[TestClass]
public class PropertyDelete : PropertyAbstract {
    [TestInitialize]
    public void CheckVerySimple1Exists() {
        try {
            Helpers.Helpers.TestResponse(vs1, null);
        }
        catch (AssertFailedException) {
            var body = new JObject();
            var name = new JObject(new JProperty("value", "fred"));
            var members = new JObject(new JProperty("Name", name));
            body.Add(JsonRep.Members, members);
            body.Add(JsonRep.DomainType, $"{Urls.NameSpace}VerySimple");
            //Need either to suppress creation of file, or create a new method that just does the work rather than testing.
            Helpers.Helpers.TestResponse(Urls.Objects, null, body.ToString(), Methods.Post, Codes.SucceededNewRepresentation);
        }

        var fred = new JObject(new JProperty("value", "fred")).ToString();
        Helpers.Helpers.TestResponse(valueProp, null, fred, Methods.Put);

        var simple = Urls.Objects + Urls.MostSimple1;
        var newRef = new JObject(new JProperty("value", new JObject(new JProperty("href", simple)))).ToString();
        Helpers.Helpers.TestResponse(refProp, $"{FilePrefix}Before", newRef, Methods.Put);
    }

    [TestMethod]
    public void DeleteValueProperty() {
        Helpers.Helpers.TestResponse(valueProp, $"{FilePrefix}After-ValueProperty", null, Methods.Delete);
    }

    [TestMethod]
    public void DeleteReferenceProperty() {
        Helpers.Helpers.TestResponse(refProp, $"{FilePrefix}After-ReferenceProperty", null, Methods.Delete);
    }

    [TestMethod]
    public void AttemptDeleteNonExistentProperty() {
        Helpers.Helpers.TestResponse($"{vs1}{Urls.Properties}NonExistentProp", null, null, Methods.Delete, Codes.NotFound);
    }

    #region Helpers

    private static readonly string vs1 = $@"{Urls.Objects}{Urls.NameSpace}VerySimple/1";
    private static readonly string valueProp = $"{vs1}{Urls.Properties}Name";
    private static readonly string refProp = $"{vs1}{Urls.Properties}MostSimple";

    protected override string FilePrefix => "Property-Delete-";

    #endregion
}