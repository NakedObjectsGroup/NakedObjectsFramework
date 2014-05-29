// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;

namespace NakedObjects.Xat {
    public interface ITestAction {
        string Name { get; }
        string SubMenu { get; }
        string LastMessage { get; }
        ITestParameter[] Parameters { get; }
        bool MatchParameters(Type[] typestoMatch);
        ITestObject InvokeReturnObject(params object[] parameters);
        ITestCollection InvokeReturnCollection(params object[] parameters);
        void Invoke(params object[] parameters);

        ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters);

        ITestAction AssertIsDisabled();
        ITestAction AssertIsEnabled();
        ITestAction AssertIsInvalidWithParms(params object[] parameters);
        ITestAction AssertIsValidWithParms(params object[] parameters);
        ITestAction AssertIsVisible();
        ITestAction AssertIsInvisible();
        ITestAction AssertIsDescribedAs(string expected);
        ITestAction AssertLastMessageIs(string message);
        ITestAction AssertLastMessageContains(string message);
    }
}