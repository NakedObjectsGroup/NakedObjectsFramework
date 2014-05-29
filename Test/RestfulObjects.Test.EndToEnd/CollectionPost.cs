// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class CollectionPost : CollectionAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Collection-Post-"; }
        }

        #endregion

        [TestMethod]
        public void PostItemIntoList() {
            Helpers.TestResponse(simpleList, FilePrefix + "PostItemIntoList", simple1AsArgument.ToString(), Methods.Post);
        }

        [TestMethod]
        public void SyntacticallyMalformedArgument1() {
            string arg = new JObject(new JProperty("vaalue", new JObject(JsonRep.MostSimple1AsRef()))).ToString(); //mis-spelled name
            Helpers.TestResponse(simpleList, null, arg, Methods.Post, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void SyntacticallyMalformedArgument2() {
            string arg = new JObject(new JProperty("value", new JObject(JsonRep.MostSimple1AsRef())), new JProperty("value2", 100)).ToString(); //additional value
            Helpers.TestResponse(simpleList, null, arg, Methods.Post, Codes.SyntacticallyInvalid);
        }

        //Should be allowed
        [TestMethod]
        public void PostItemIntoSet() {
            Helpers.TestResponse(simpleSet, FilePrefix + "PostItemIntoSet", simple1AsArgument.ToString(), Methods.Post);
        }

        [TestMethod]
        public void PostItemTwiceIntoSet() {
            Helpers.TestResponse(simpleSet, FilePrefix + "PostItemIntoSet", simple1AsArgument.ToString(), Methods.Post);
            Helpers.TestResponse(simpleSet, FilePrefix + "PostItemIntoSet", simple1AsArgument.ToString(), Methods.Post);
        }

        [TestMethod]
        public void AttemptPostItemIntoNonExistentCollection() {
            Helpers.TestResponse(simpleList + "ThatIsntThere", null, simple1AsArgument.ToString(), Methods.Post, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptPostItemOfWrongTypeIntoCollection() {
            Helpers.TestResponse(simpleList, null, withValueAsArgument.ToString(), Methods.Post, Codes.ValidationFailed);
        }
    }
}