// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NakedObjects;

namespace MvcTestApp.Tests.Controllers {
    [NotPersisted]
    public class NotPersistedObject {
        public string Name { get; set; }
        public void SimpleAction() {}
        public NotPersistedObject SimpleActionWithReturn() {
            return this;
        }
    }
}