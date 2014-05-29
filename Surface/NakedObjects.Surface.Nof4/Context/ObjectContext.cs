// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Redirect;

namespace NakedObjects.Surface.Nof4.Context {
    public class ObjectContext : Context {
        public ObjectContext(INakedObject target) {
            Target = target;
        }

        public bool Mutated { get; set; }

        public Tuple<string, string> Redirected {
            get {
                var rdo = Target.Object as IRedirectedObject;
                if (rdo != null) {
                    return new Tuple<string, string>(rdo.ServerName, rdo.Oid);
                }

                return null;
            }
        }

        public override string Id {
            get { throw new NotImplementedException(); }
        }

        public override INakedObjectSpecification Specification {
            get { return Target.Specification; }
        }

        public PropertyContext[] VisibleProperties { get; set; }

        public ActionContext[] VisibleActions { get; set; }

        public ObjectContextSurface ToObjectContextSurface(INakedObjectsSurface surface) {
            var oc = new ObjectContextSurface {
                VisibleProperties = VisibleProperties == null ? null : VisibleProperties.Select(p => p.ToPropertyContextSurface(surface)).ToArray(),
                VisibleActions = VisibleActions == null ? null : VisibleActions.Select(p => p.ToActionContextSurface(surface)).ToArray(),
                Mutated = Mutated, 
                Redirected = Redirected
            };
            return ToContextSurface(oc, surface);
        }
    }
}