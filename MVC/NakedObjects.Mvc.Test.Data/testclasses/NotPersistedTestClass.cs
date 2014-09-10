// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [NotPersisted]
    public class NotPersistedTestClass {
        private DateTime testDateTime = new DateTime(2014, 7, 9, 17, 6, 0, DateTimeKind.Utc);
        public string TestString { get; set; }
        public void SimpleAction() {}

        [Mask("dd/mmm/yyyy hh:mm:ss")]
        public DateTime TestDateTime {
            get { return testDateTime; }
            set { testDateTime = value; }
        }
    }
}
