// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class ServiceActionInvokeDelete : AbstractActionInvokeDelete {
        protected override string BaseUrl {
            get { return Urls.Services + Urls.WithActionService + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Service-Action-Invoke-Delete-"; }
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