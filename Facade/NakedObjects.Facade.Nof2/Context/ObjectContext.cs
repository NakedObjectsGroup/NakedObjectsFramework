// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Facade.Contexts;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2.Context {
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