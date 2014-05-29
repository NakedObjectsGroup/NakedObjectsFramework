using System;

namespace NakedObjects.Xat.Generic {
    public interface ITestAction<TOwner, TReturn> : ITestAction{
      
        new ITestParameter[] Parameters { get; }
        new ITestObject<TReturn> InvokeReturnObject(params object[] parameters);
        new ITestCollection<TOwner, TReturn> InvokeReturnCollection(params object[] parameters);
        new ITestAction<TOwner, TReturn> AssertIsDisabled();
        new ITestAction<TOwner, TReturn> AssertIsEnabled();
        new ITestAction<TOwner, TReturn> AssertIsInvalidWithParms(params object[] parameters);
        new ITestAction<TOwner, TReturn> AssertIsValidWithParms(params object[] parameters);
        new ITestAction<TOwner, TReturn> AssertIsVisible();
        new ITestAction<TOwner, TReturn> AssertIsInvisible();
        new ITestAction<TOwner, TReturn> AssertIsDescribedAs(string expected);
        new ITestAction<TOwner, TReturn> AssertLastMessageIs(string message);
        new ITestAction<TOwner, TReturn> AssertLastMessageContains(string message);
    }
}