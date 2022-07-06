// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class PropertyPost : PropertyAbstract {
    [TestMethod]
    public void AttemptPropertyPost() {
        WithValue("AValue", null, JsonRep.Empty(), Methods.Post, Codes.MethodNotValid);
    }
}