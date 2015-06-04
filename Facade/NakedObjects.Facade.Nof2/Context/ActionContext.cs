// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Facade.Contexts;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2.Context {
    public class ActionContext : Context {
        private ParameterContext[] parameters;
        public ActionWrapper Action { get; set; }

        public override string Id {
            get { return Action.getId(); }
        }

        public override NakedObjectSpecification Specification {
            get { return Action.getReturnType(); }
        }

        public ParameterContext[] VisibleParameters {
            get { return parameters ?? new ParameterContext[] {}; }
            set { parameters = value; }
        }

        public ActionContextFacade ToActionContextSurface(IFrameworkFacade surface) {
            var ac = new ActionContextFacade {
                Action = new ActionFacade(Action, Target, surface),
                VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextSurface(surface)).ToArray()
            };

            return ToContextSurface(ac, surface);
        }
    }
}