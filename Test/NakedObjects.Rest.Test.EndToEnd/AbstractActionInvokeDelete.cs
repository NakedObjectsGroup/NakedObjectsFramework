// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Test.EndToEnd {
    public class AbstractActionInvokeDelete : AbstractAction {
        public void DoAttemptInvokePostActionWithDelete() {
            TestActionInvoke("AnAction", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
        }

        public void DoAttemptInvokePutActionWithDelete() {
            TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
        }

        public void DoAttemptInvokeGetActionWithDelete() {
            TestActionInvoke("AnActionAnnotatedQueryOnly", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
        }
    }
}