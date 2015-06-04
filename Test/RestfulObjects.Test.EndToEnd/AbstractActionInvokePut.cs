// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    public class AbstractActionInvokePut : AbstractAction {
        public void DoAnActionAnnotatedIdempotent() {
            TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Put);
        }

        public void DoAnActionAnnotatedIdempotentReturnsNull() {
            TestActionInvoke("AnActionAnnotatedIdempotentReturnsNull", JsonRep.Empty(), Methods.Put);
        }

        public void DoAnActionReturnsObjectWithParametersAnnotatedIdempotent() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionInvoke("AnActionReturnsObjectWithParametersAnnotatedIdempotent", parms.ToString(), Methods.Put);
        }

        public void DoAttemptInvokePutActionWithGet() {
            TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Get, Codes.MethodNotValid);
        }
    }
}