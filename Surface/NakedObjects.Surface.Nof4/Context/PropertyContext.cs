// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Context {
    public class PropertyContext : Context {
        public INakedObjectAssociation Property { get; set; }

        public bool Mutated { get; set; }

        public override string Id {
            get { return Property.Id; }
        }

        public override INakedObjectSpecification Specification {
            get { return Property.Specification; }
        }

        public PropertyContextSurface ToPropertyContextSurface(INakedObjectsSurface surface) {
            var pc = new PropertyContextSurface {
                Property = new NakedObjectAssociationWrapper(Property, surface),
                Mutated = Mutated,
            };

            return ToContextSurface(pc, surface);
        }
    }
}