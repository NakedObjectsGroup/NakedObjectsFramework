// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class CollectionPost : CollectionAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Collection-Post-"; }
        }

        #endregion

        [TestMethod, Ignore]
        public void PostItemIntoList() {
            Helpers.TestResponse(simpleList, FilePrefix + "PostItemIntoList", simple1AsArgument.ToString(), Methods.Post, Codes.Forbidden);
        }
    }
}