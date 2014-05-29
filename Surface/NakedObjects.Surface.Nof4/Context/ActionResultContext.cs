// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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

        public override INakedObjectSpecification Specification {
            get { return ActionContext.Action.ReturnType; }
        }

        public ActionResultContextSurface ToActionResultContextSurface(INakedObjectsSurface surface) {
            var ac = new ActionResultContextSurface {
                Result = Result == null ? null : Result.ToObjectContextSurface(surface),
                ActionContext = ActionContext.ToActionContextSurface(surface),
                HasResult = HasResult
            };


            if (Reason == null) {
                Reason = ActionContext.Reason;
                ErrorCause = ActionContext.ErrorCause;
            }


            return ToContextSurface(ac, surface);
        }
    }
}