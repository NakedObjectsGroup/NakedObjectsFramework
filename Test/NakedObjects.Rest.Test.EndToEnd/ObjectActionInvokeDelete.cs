// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class ObjectActionInvokeDelete : AbstractActionInvokeDelete {
    protected override string BaseUrl => Urls.Objects + Urls.WithActionObject1 + Urls.Actions;

    protected override string FilePrefix => "Object-Action-Invoke-Delete-";

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