// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class PropertyPost : PropertyAbstract {
        [TestMethod]
        public void AttemptPropertyPost() {
            WithValue("AValue", null, JsonRep.Empty(), Methods.Post, Codes.MethodNotValid);
        }
    }
}