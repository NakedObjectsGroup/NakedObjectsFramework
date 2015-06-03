// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Redirect;

namespace NakedObjects.Surface.Nof4.Context {
    public class ObjectContext : Context {
        public ObjectContext(INakedObjectAdapter target) {
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

        public override ITypeSpec Specification {
            get { return Target.Spec; }
        }

        public PropertyContext[] VisibleProperties { get; set; }

        public ActionContext[] VisibleActions { get; set; }

        public ObjectContextSurface ToObjectContextSurface(IFrameworkFacade surface, INakedObjectsFramework framework) {
            var oc = new ObjectContextSurface {
                VisibleProperties = VisibleProperties == null ? null : VisibleProperties.Select(p => p.ToPropertyContextSurface(surface, framework)).ToArray(),
                VisibleActions = VisibleActions == null ? null : VisibleActions.Select(p => p.ToActionContextSurface(surface, framework)).ToArray(),
                Mutated = Mutated,
                Redirected = Redirected
            };
            return ToContextSurface(oc, surface, framework);
        }
    }
}