// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class ObjectWithList {
        private ReferencedObject added;
        private readonly ArrayList collection = new ArrayList();
        private ReferencedObject removed;

        public IList Method {
            get { return collection; }
        }

        public void AddToMethod(ReferencedObject person) {
            added = person;
        }

        public void RemoveFromMethod(ReferencedObject person) {
            removed = person;
        }

        public static string NameMethod() {
            return "my name";
        }

        /*
            public void aboutMethod( FieldAbout about,  JavaReferencedObject obj,  bool isAdd) {
                if (about.mode().getValue().Equals(AboutType.VISIBLE.getValue())) {
                    if (!visible) {
                        about.invisible();
                    }
                } else if (about.mode().getValue().Equals(AboutType.AVAILABLE.getValue())) {
                    if (!modifiable) {
                        about.unmodifiable("NO");
                    }
                } else if (about.mode().getValue().Equals(AboutType.VALID.getValue())) {
                    if (!valid) {
                        about.invalid();
                    }
                }
            }*/
    }

    // Copyright (c) Naked Objects Group Ltd.
}