// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Surface.Nof4.Context {
    public class ActionResultContext : Context {
        private bool hasResult;
        private ObjectContext result;


        public ObjectContext Result {
            get { return result; }
            set {
                result = value;
                hasResult = true;
            }
        }

        public bool HasResult {
            get {
                if (hasResult && Specification.FullName == "System.Void") {
                    return false;
                }
                return hasResult;
            }
        }

        public ActionContext ActionContext { get; set; }

        public override string Id {
            get { return ActionContext.Action.Id; }
        }

        public override ITypeSpec Specification {
            get { return ActionContext.Action.ReturnSpec; }
        }

        public ActionResultContextSurface ToActionResultContextSurface(INakedObjectsSurface surface, INakedObjectsFramework framework) {
            var ac = new ActionResultContextSurface {
                Result = Result == null ? null : Result.ToObjectContextSurface(surface, framework),
                ActionContext = ActionContext.ToActionContextSurface(surface, framework),
                HasResult = HasResult
            };


            if (Reason == null) {
                Reason = ActionContext.Reason;
                ErrorCause = ActionContext.ErrorCause;
            }


            return ToContextSurface(ac, surface, framework);
        }
    }
}