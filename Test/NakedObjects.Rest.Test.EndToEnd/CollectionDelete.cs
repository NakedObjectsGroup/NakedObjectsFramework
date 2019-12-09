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
        }

        [TestMethod, Ignore]
        public void DeleteItemFromList() {
            Helpers.TestResponse(simpleList, FilePrefix + "DeleteItemFromList", simple1AsArgument.ToString(), Methods.Delete, Codes.Forbidden);
        }
    }
}