// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Facade;

namespace NakedObjects.Facade.Impl.Contexts {
    public class ActionResultContext : Context {
        private bool hasResult;
        private ObjectContext result;

        public ObjectContext Result {
            get => result;
            set {
                result = value;
                hasResult = true;
            }
        }

        public bool HasResult => !IsVoid() && hasResult;

        public ActionContext ActionContext { get; set; }

        public override string Id => ActionContext.Action.Id;

        public override ITypeSpec Specification => ActionContext.Action.ReturnSpec;

        public string TransientSecurityHash { get; set; }

        private bool IsVoid() => hasResult && Specification.FullName == "System.Void";

        public ActionResultContextFacade ToActionResultContextFacade(IFrameworkFacade facade, INakedObjectsFramework framework) {
            var ac = new ActionResultContextFacade {
                Result = Result?.ToObjectContextFacade(facade, framework),
                ActionContext = ActionContext.ToActionContextFacade(facade, framework),
                HasResult = HasResult,
                TransientSecurityHash = TransientSecurityHash
            };

            if (Reason == null) {
                Reason = ActionContext.Reason;
                ErrorCause = ActionContext.ErrorCause;
            }

            return ToContextFacade(ac, facade, framework);
        }
    }
}