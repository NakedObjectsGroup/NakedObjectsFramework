// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class DomainTypeCollectionTests {
    private static readonly string dtc = $@"{Urls.DomainTypes}{Urls.NameSpace}WithCollection/collections/AnEmptyCollection";

    [TestMethod]
    public void DomainTypeCollection() {
        Helpers.Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptPut() {
        Helpers.Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptPost() {
        Helpers.Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptDelete() {
        Helpers.Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
    }
}