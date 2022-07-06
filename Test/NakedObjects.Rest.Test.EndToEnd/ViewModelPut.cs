// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ViewModelPut : ObjectAbstract {
    [TestMethod]
    public void MostSimple() {
        var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 123))));
        Object(Urls.VmMostSimple + Key1, "MostSimpleViewModel", body.ToString(), Methods.Put, Codes.Forbidden);
    }

    [TestMethod]
    public void WithReference() {
        var simpleJ = new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1))));

        var body = new JObject(
            new JProperty("AReference", simpleJ),
            new JProperty("ANullReference", simpleJ),
            new JProperty("AChoicesReference", simpleJ),
            new JProperty("AnEagerReference", simpleJ),
            new JProperty("Id", new JObject(new JProperty("value", Key1)))
        );
        Object($"{Urls.VmWithReference}1--2--2--1", "WithReference", body.ToString(), Methods.Put, Codes.Forbidden);
    }

    #region Helpers

    protected override string FilePrefix => "ViewModel-Put-";

    private const string Key1 = "31459"; //An arbitrarily large Id

    #endregion
}