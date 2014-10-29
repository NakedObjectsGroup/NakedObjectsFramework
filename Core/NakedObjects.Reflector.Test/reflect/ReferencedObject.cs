// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflect.DotNet.Reflect {
    public class ReferencedObject {
        private static int next;
        private readonly int id = next++;

        public override string ToString() {
            return "ReferencedObject#" + id;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}