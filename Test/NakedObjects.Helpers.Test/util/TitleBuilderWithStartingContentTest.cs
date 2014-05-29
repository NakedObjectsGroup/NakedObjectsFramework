// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {
    internal class ObjectWithToString {
        public override string ToString() {
            return "from ToString";
        }
    }

    internal class ObjectWithTitleMethod {
        public string Title() {
            return "from Title method";
        }
    }

    internal class ObjectWithTitleAttribute {
        [Title]
        public string property {
            get { return "from property"; }
        }
    }

    internal class ObjectWithNullTitleAttribute {
        [Title]
        public string property {
            get { return null; }
        }
    }

    internal class ObjectWithTitleAttributeThatIsAReference {
        [Title]
        public object property {
            get { return new ObjectWithTitleMethod(); }
        }
    }
}