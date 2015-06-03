// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Linq;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Surface;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public class ObjectContext : Context {
        public ObjectContext(NakedObject target) {
            Target = target;
        }

        public ObjectContext(Naked naked) {
            NakedTarget = naked;
        }

        public override string Id {
            get { throw new NotImplementedException(); }
        }

        public override NakedObjectSpecification Specification {
            get { return NakedTarget == null ? Target.getSpecification() : NakedTarget.getSpecification(); }
        }

        public PropertyContext[] VisibleProperties { get; set; }

        public ActionContext[] VisibleActions { get; set; }

        public bool Mutated { get; set; }

        public ObjectContextFacade ToObjectContextSurface(IFrameworkFacade surface) {
            var oc = new ObjectContextFacade {
                                                  VisibleProperties = VisibleProperties == null ? null : VisibleProperties.Select(p => p.ToPropertyContextSurface(surface)).ToArray(),
                                                  VisibleActions = VisibleActions == null ? null : VisibleActions.Select(p => p.ToActionContextSurface(surface)).ToArray(),
                                                  Mutated = Mutated
                                              };
            return ToContextSurface(oc, surface);
        }
    }
}