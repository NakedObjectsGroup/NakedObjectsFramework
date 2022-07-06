// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public abstract class CollectionAbstract {
    private static readonly string Collections = Urls.Objects + Urls.WithCollection1 + Urls.Collections;
    private static readonly string Vs1 = $@"{Urls.Objects}{Urls.NameSpace}VerySimple/1";
    protected static readonly string SimpleSet = $"{Vs1}{Urls.Collections}SimpleSet";
    protected static readonly string SimpleList = $"{Vs1}{Urls.Collections}SimpleList";

    protected readonly JObject Simple1AsArgument = new(new JProperty("value", new JObject(JsonRep.MostSimple1AsRef())));
    protected JObject Simple2AsArgument = new(new JProperty("value", new JObject(JsonRep.MostSimple2AsRef())));
    protected JObject Simple3AsArgument = new(new JProperty("value", new JObject(JsonRep.MostSimple3AsRef())));
    protected JObject WithValueAsArgument = new(new JProperty("value", new JObject(JsonRep.WithValue1AsRef())));

    protected virtual string FilePrefix => "NO PREFIX SPECIFIED";

    protected void Collection(string collectionName, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded, string acceptHeader = null) {
        Helpers.Helpers.TestResponse(Collections + collectionName, FilePrefix + fileName, body, method, code, acceptHeader);
    }

    [TestInitialize]
    public void InitializeVerySimple1() {
        if (!Helpers.Helpers.UrlExistsForGet(Vs1)) {
            var body = new JObject();
            var name = new JObject(new JProperty("value", "fred"));
            var members = new JObject(new JProperty("Name", name));
            body.Add(JsonRep.Members, members);
            body.Add(JsonRep.DomainType, $"{Urls.NameSpace}VerySimple");
            //Need either to suppress creation of file, or create a new method that just does the work rather than testing.
            Helpers.Helpers.TestResponse(Urls.Objects, null, body.ToString(), Methods.Post, Codes.SucceededNewRepresentation);
        }

        //Clear both the collections first
        var url = $"{Vs1}{Urls.Actions}EmptyTheSet{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Post);
        url = $"{Vs1}{Urls.Actions}EmptyTheList{Urls.Invoke}";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Post);
    }
}