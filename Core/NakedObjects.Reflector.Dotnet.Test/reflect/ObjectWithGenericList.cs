// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class ObjectWithGenericList {
        private const bool available = false;
        private const bool valid = false;
        private const bool visible = false;
        private readonly IList<ReferencedObject> collection = new List<ReferencedObject>();
        private ReferencedObject added;
        private ReferencedObject removed;

        public static string NameMethod() {
            return "my about";
        }

        public void AddToMethod(ReferencedObject person) {
            added = person;
        }

        public string AvailableMethod(ReferencedObject person) {
            return available ? null : "not available";
        }

        public IList<ReferencedObject> Method() {
            return collection;
        }

        public void RemoveFromMethod(ReferencedObject person) {
            removed = person;
        }

        public string ValidMethod(ReferencedObject person) {
            return valid ? null : "not valid";
        }

        public bool HideMethod(ReferencedObject person) {
            return !visible;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}