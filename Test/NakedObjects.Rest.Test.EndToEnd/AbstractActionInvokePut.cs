// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

public class AbstractActionInvokePut : AbstractAction {
    protected void DoAnActionAnnotatedIdempotent() {
        TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Put);
    }

    protected void DoAnActionAnnotatedIdempotentReturnsNull() {
        TestActionInvoke("AnActionAnnotatedIdempotentReturnsNull", JsonRep.Empty(), Methods.Put);
    }

    protected void DoAnActionReturnsObjectWithParametersAnnotatedIdempotent() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionInvoke("AnActionReturnsObjectWithParametersAnnotatedIdempotent", parms.ToString(), Methods.Put);
    }

    protected void DoAttemptInvokePutActionWithGet() {
        TestActionInvoke("AnActionAnnotatedIdempotent", JsonRep.Empty(), Methods.Get, Codes.MethodNotValid);
    }
}