// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace NakedObjects.TestSystem {
    public class TestPojo {
        private static int nextId;
        private readonly int id = nextId++;
        private readonly string state;

        public TestPojo() {
            state = "pojo" + id;
        }

        public override string ToString() {
            return "Pojo#" + id;
        }

        public override bool Equals(object obj) {
            if (obj == this) return true;
            if (obj is TestPojo) {
                var other = (TestPojo) obj;
                return other.state.Equals(state);
            }
            return false;
        }

        public override int GetHashCode() {
            return state.GetHashCode();
        }
    }
}