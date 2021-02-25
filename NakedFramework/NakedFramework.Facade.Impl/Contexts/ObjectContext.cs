// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedObjects.Facade.Contexts;
using NakedObjects.Redirect;

namespace NakedObjects.Facade.Impl.Contexts {
    public class ObjectContext : Context {
        public ObjectContext(INakedObjectAdapter target) => Target = target;

        public bool Mutated { get; set; }

        public (string serverName, string oid)? Redirected => Target.Object is IRedirectedObject rdo ? (rdo.ServerName, rdo.Oid) : ((string ServerName, string Oid)?) null;

        public override string Id => Target.Oid.ToString();

        public override ITypeSpec Specification => Target.Spec;

        public PropertyContext[] VisibleProperties { get; set; }
        public ActionContext[] VisibleActions { get; set; }

        public ObjectContextFacade ToObjectContextFacade(IFrameworkFacade facade, INakedObjectsFramework framework) {
            var oc = new ObjectContextFacade {
                VisibleProperties = VisibleProperties?.Select(p => p.ToPropertyContextFacade(facade, framework)).ToArray(),
                VisibleActions = VisibleActions?.Select(p => p.ToActionContextFacade(facade, framework)).ToArray(),
                Mutated = Mutated,
                Redirected = Redirected
            };
            return ToContextFacade(oc, facade, framework);
        }
    }
}