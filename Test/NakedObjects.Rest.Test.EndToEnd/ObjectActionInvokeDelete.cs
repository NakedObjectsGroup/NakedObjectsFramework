// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class ObjectActionInvokeDelete : AbstractActionInvokeDelete {
        protected override string BaseUrl {
            get { return Urls.Objects + Urls.WithActionObject1 + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Object-Action-Invoke-Delete-"; }
        }

        [TestMethod]
        public void AttemptInvokePostActionWithDelete() {
            DoAttemptInvokePostActionWithDelete();
        }

        [TestMethod]
        public void AttemptInvokePutActionWithDelete() {
            DoAttemptInvokePutActionWithDelete();
        }

        [TestMethod]
        public void AttemptInvokeGetActionWithDelete() {
            DoAttemptInvokeGetActionWithDelete();
        }
    }
}