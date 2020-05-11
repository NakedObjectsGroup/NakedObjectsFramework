// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Facade.Impl.Contexts {
    public class ActionContext : Context {
        private ParameterContext[] parameters;

        private PropertyContext[] properties;

        public IActionSpec Action { get; set; }

        public override string Id => Action.Id;

        public override ITypeSpec Specification => Action.ReturnSpec;

        public ParameterContext[] VisibleParameters {
            get => parameters ?? new ParameterContext[] { };
            set => parameters = value;
        }

        public PropertyContext[] VisibleProperties {
            get => properties ?? new PropertyContext[] { };
            set => properties = value;
        }

        public string MenuPath { get; set; }

        public string OverloadedUniqueId { get; set; }

        public ActionContextFacade ToActionContextFacade(IFrameworkFacade facade, INakedObjectsFramework framework) {
            var ac = new ActionContextFacade {
                MenuPath = MenuPath,
                Action = new ActionFacade(Action, facade, framework, OverloadedUniqueId ?? ""),
                VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextFacade(facade, framework)).ToArray(),
                VisibleProperties = VisibleProperties.Select(p => p.ToPropertyContextFacade(facade, framework)).ToArray()
            };
            return ToContextFacade(ac, facade, framework);
        }
    }
}