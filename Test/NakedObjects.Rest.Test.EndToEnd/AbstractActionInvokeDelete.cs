// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

public class AbstractActionInvokeDelete : AbstractAction {
    protected void DoAttemptInvokePostActionWithDelete() {
        TestActionInvoke("AnAction", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
    }

    protected void DoAttemptInvokePutActionWithDelete() {
        TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
    }

    protected void DoAttemptInvokeGetActionWithDelete() {
        TestActionInvoke("AnActionAnnotatedQueryOnly", JsonRep.Empty(), Methods.Delete, Codes.MethodNotValid);
    }
}