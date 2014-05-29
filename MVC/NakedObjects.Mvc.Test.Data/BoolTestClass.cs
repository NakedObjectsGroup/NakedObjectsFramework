// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [Bounded]
    public class BoolTestClass {

        [Title]
        public string Name { get; set; }

        private bool testBool1 = true;
        public bool TestBool1 {
            get { return testBool1; }
            set { testBool1 = value; }
        }

        private bool testBool2 = false;
        public bool TestBool2 {
            get { return testBool2; }
            set { testBool2 = value; }
        }

        private bool? testNullableBool1 = true;
        public bool? TestNullableBool1 {
            get { return testNullableBool1; }
            set { testNullableBool1 = value; }
        }

        private bool? testNullableBool2 = false;
        public bool? TestNullableBool2 {
            get { return testNullableBool2; }
            set { testNullableBool2 = value; }
        }
        private bool? testNullableBool3 = null;
        public bool? TestNullableBool3 {
            get { return testNullableBool3; }
            set { testNullableBool3 = value; }
        }


        public void TestBoolAction(bool parm) {}

        public void TestNullableBoolAction(bool? parm) { }

        public void DuplicateAction() { }
        public void DuplicateAction(string parm) { }
    }
}
