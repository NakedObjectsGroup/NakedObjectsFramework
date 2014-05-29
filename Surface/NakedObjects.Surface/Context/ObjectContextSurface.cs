// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class ObjectContextSurface : ContextSurface {
        public bool Mutated { get; set; }

        public Tuple<string, string> Redirected { get; set; }

        public override string Id {
            get { throw new NotImplementedException(); }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return Target.Specification; }
        }

        public PropertyContextSurface[] VisibleProperties { get; set; }

        public ActionContextSurface[] VisibleActions { get; set; }
    }
}