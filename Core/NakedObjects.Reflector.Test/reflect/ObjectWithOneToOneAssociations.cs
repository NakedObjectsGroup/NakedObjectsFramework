// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class ObjectWithOneToOneAssociations {
        private bool available = false;
        private bool valid = false;
        private bool visible = false;

        public ReferencedObject ReferencedObject { get; set; }

        public string AvailableReferencedObject(ReferencedObject referencedObject) {
            return available ? null : "not available";
        }

        public string ValidReferencedObject(ReferencedObject referencedObject) {
            return valid ? null : "not valid";
        }

        public bool HideReferencedObject(ReferencedObject referencedObject) {
            return !visible;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}