// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [NotPersisted]
    public class NotPersistedTestClass {
        public string TestString { get; set; }
        public void SimpleAction() {}
        public DateTime TestDateTime { get; set; }
    }
}
