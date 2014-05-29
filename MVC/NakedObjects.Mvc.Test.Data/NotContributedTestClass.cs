// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [Bounded]
    public class NotContributedTestClass1 {
        [Title]
        public string Name { get; set; }

        public void TestAction() {}
    }

    [Bounded]
    public class NotContributedTestClass2 {
        [Title]
        public string Name { get; set; }

        public void TestAction() {}
    }
}