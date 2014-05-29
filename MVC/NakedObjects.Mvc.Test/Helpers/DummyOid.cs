// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace MvcTestApp.Tests.Helpers {
    public class DummyOid : IOid, IEncodedToStrings {
        public IOid Previous {
            get { return  null; }
        }

        public bool IsTransient {
            get { return true; }
        }

        public bool HasPrevious {
            get { return false; }
        }

        public void CopyFrom(IOid oid) {
                
        }

        public INakedObjectSpecification Specification {
            get { return null; }
        }

        public string[] ToEncodedStrings() {
            return new[] {""};
        }

        public string[] ToShortEncodedStrings() {
            return ToEncodedStrings();
        }
    }
}