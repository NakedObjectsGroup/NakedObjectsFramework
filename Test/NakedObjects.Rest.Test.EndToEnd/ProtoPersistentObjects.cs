﻿// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ProtoPersistentObjects {
    [TestMethod]
    public void CreateTransientMostSimple() {
        var url = $@"{Urls.RestDataRepository}{Urls.Actions}CreateTransientMostSimple{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, "CreateTransientMostSimple", null, Methods.Post, Codes.Succeeded, null, "1234");
    }

    [TestMethod]
    public void CreateTransientWithValue() {
        var url = $@"{Urls.RestDataRepository}{Urls.Actions}CreateTransientWithValue{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, "CreateTransientWithValue", null, Methods.Post, Codes.Succeeded, null, "1234");
    }

    [TestMethod]
    public void CreateTransientWithReference() {
        var url = $@"{Urls.RestDataRepository}{Urls.Actions}CreateTransientWithReference{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, "CreateTransientWithReference", null, Methods.Post, Codes.Succeeded, null, "1234");
    }

    [TestMethod]
    public void CreateTransientWithCollection() {
        var url = $@"{Urls.RestDataRepository}{Urls.Actions}CreateTransientWithCollection{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, "CreateTransientWithCollection", null, Methods.Post, Codes.Succeeded, null, "1234");
    }

    [TestMethod]
    public void PersistMostSimple() {
        var body = ProtoPersistentMostSimple(10001).ToString();
        Helpers.Helpers.TestResponse($"{Urls.Objects}{Urls.NameSpace}MostSimplePersist", "PersistMostSimple", body, Methods.Post, Codes.SucceededNewRepresentation, null, "1234");
    }

    [TestMethod]
    public void PersistVerySimple() {
        var body = ProtoPersistentVerySimple(10002).ToString();
        Helpers.Helpers.TestResponse($"{Urls.Objects}{Urls.NameSpace}VerySimplePersist", "PersistVerySimple", body, Methods.Post, Codes.SucceededNewRepresentation, null, "1234");
    }

    [TestMethod]
    public void AttemptPersistWithPut() {
        var body = ProtoPersistentMostSimple(10003).ToString();
        Helpers.Helpers.TestResponse($"{Urls.Objects}{Urls.NameSpace}MostSimplePersist", null, body, Methods.Put, Codes.MethodNotValid, null, "1234");
    }

    [TestMethod]
    public void AttemptPersistWithMalformedBody() {
        var body = ProtoPersistentMalformed(10004).ToString();
        Helpers.Helpers.TestResponse($"{Urls.Objects}{Urls.NameSpace}MostSimplePersist", null, body, Methods.Post, Codes.SyntacticallyInvalid, null, "1234");
    }

    [TestMethod]
    public void AttemptPersistWithUnknownType() {
        var body = ProtoPersistentMostSimple(10005).ToString();
        Helpers.Helpers.TestResponse($"{Urls.Objects}{Urls.NameSpace}NotAType", null, body, Methods.Post, Codes.NotFound, null, "1234");
    }

    #region Helpers

    private static JObject ProtoPersistentMostSimple(int val) {
        var body = new JObject();
        var id = new JObject(new JProperty("value", val));
        var members = new JObject(new JProperty("Id", id));
        body.Add(JsonRep.Members, members);
        return body;
    }

    private static JObject ProtoPersistentVerySimple(int val) {
        var body = new JObject();
        var id = new JProperty("Id", new JObject(new JProperty("value", val)));
        var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1)))));
        var name = new JProperty("Name", new JObject(new JProperty("value", "fred")));
        var members = new JObject(ms1, name, id);
        body.Add(JsonRep.Members, members);
        return body;
    }

    private static JObject ProtoPersistentMalformed(int val) {
        var body = new JObject();
        var id = new JObject(new JProperty("value", val));
        var members = new JObject(new JProperty("id", id)); //Because 'Id' should be capitalised
        body.Add(JsonRep.Members, members);
        return body;
    }

    #endregion
}