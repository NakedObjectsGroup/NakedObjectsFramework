// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ObjectActionInvokePut : AbstractActionInvokePut {
        protected override string BaseUrl {
            get { return Urls.Objects + Urls.WithActionObject1 + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Object-Action-Invoke-Put-"; }
        }


        [TestMethod, Ignore]
        public void AnActionAnnotatedIdempotent() {
            DoAnActionAnnotatedIdempotent();
        }

        [TestMethod, Ignore]
        public void AnActionAnnotatedIdempotentReturnsNull() {
            DoAnActionAnnotatedIdempotentReturnsNull();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsObjectWithParametersAnnotatedIdempotent() {
            DoAnActionReturnsObjectWithParametersAnnotatedIdempotent();
        }


        [TestMethod]
        public void AttemptInvokePutActionWithGet() {
            DoAttemptInvokePutActionWithGet();
        }
    }
}