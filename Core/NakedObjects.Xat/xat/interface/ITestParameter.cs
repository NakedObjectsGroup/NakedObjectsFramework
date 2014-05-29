// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Xat {
    public interface ITestParameter : ITestNaked {
        string Name { get; }
        ITestNaked[] GetChoices();
        ITestNaked[] GetCompletions(string autoCompleteParm);
        ITestNaked GetDefault();
        ITestParameter AssertIsOptional();
        ITestParameter AssertIsMandatory();
        ITestParameter AssertIsDescribedAs(string description);
        ITestParameter AssertIsNamed(string name);
    }
}