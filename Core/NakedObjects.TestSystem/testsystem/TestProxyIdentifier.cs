// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.TestSystem {
    public class TestProxyIdentifier : IIdentifier {
        private readonly string name;

        public TestProxyIdentifier(string name) {
            this.name = name;
        }

        #region IIdentifier Members

        public string ClassName {
            get { return GetType().FullName; }
        }

        public string MemberName {
            get { return name; }
        }

        public string[] MemberParameterTypeNames {
            get { return null; }
        }

        public string[] MemberParameterNames {
            get { return null; }
        }

        public INakedObjectSpecification[] MemberParameterSpecifications {
            get { return null; }
        }

        public bool IsField {
            get { return false; }
        }

        public string ToIdentityString(IdentifierDepth depth) {
            return null;
        }

        public string ToIdentityStringWithCheckType(IdentifierDepth depth, CheckType checkType) {
            return ToIdentityString(depth);
        }

        public int CompareTo(object o) {
            return 0;
        }

        #endregion
    }
}