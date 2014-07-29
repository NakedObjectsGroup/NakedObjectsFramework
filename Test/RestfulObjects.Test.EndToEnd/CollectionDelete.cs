// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class CollectionDelete : CollectionAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Collection-Delete-"; }
        }

        #endregion

        [TestInitialize]
        public void AddItemsToCollections() {
            base.InitializeVerySimple1();
            Helpers.TestResponse(simpleSet, null, simple1AsArgument.ToString(), Methods.Post);
            Helpers.TestResponse(simpleSet, null, simple2AsArgument.ToString(), Methods.Post);
            Helpers.TestResponse(simpleList, null, simple1AsArgument.ToString(), Methods.Post);
            Helpers.TestResponse(simpleSet, null, simple2AsArgument.ToString(), Methods.Post);
        }

        [TestMethod]
        public void DeleteItemFromList() {
            Helpers.TestResponse(simpleList, FilePrefix + "DeleteItemFromList", simple1AsArgument.ToString(), Methods.Delete, Codes.Forbidden);
        }
    }
}