// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ObjectPostAndDelete {
        #region Helpers

        private const string objectsUrl = @"http://nakedobjectsrotest.azurewebsites.net/objects/";

        #endregion

        [TestMethod, Ignore]
        public void AttemptPost() {
            Helpers.TestResponse(objectsUrl + @"RestfulObjects.Test.Data.MostSimple/1", null, JsonRep.Empty(), Methods.Post, Codes.MethodNotValid);
        }

        [TestMethod, Ignore]
        public void AttemptDelete() {
            Helpers.TestResponse(objectsUrl + @"RestfulObjects.Test.Data.MostSimple/1", null, null, Methods.Delete, Codes.MethodNotValid);
        }
    }
}