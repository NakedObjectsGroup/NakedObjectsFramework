// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ServiceActionInvokePut : AbstractActionInvokePut {
        protected override string BaseUrl {
            get { return Urls.Services + Urls.WithActionService + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Service-Action-Invoke-Put-"; }
        }

        [TestMethod]
        public void AnActionAnnotatedIdempotent() {
            DoAnActionAnnotatedIdempotent();
        }

        [TestMethod]
        public void AnActionAnnotatedIdempotentReturnsNull() {
            DoAnActionAnnotatedIdempotentReturnsNull();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParametersAnnotatedIdempotent() {
            DoAnActionReturnsObjectWithParametersAnnotatedIdempotent();
        }


        [TestMethod]
        public void AttemptInvokePutActionWithGet() {
            DoAttemptInvokePutActionWithGet();
        }
    }
}