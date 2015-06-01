// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Facade;
using NakedObjects.Surface;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public class ActionResultContext : Context {
        private bool hasResult;

        private ObjectContext result;

        public bool HasResult {
            get {
                if (hasResult && Specification == null) {
                    return false;
                }
                return hasResult;
            }
        }

        public ObjectContext Result {
            get { return result; }
            set {
                result = value;
                hasResult = true;
            }
        }

        public ActionContext ActionContext { get; set; }

        public override string Id {
            get { return ActionContext.Action.getId(); }
        }

        public override NakedObjectSpecification Specification {
            get { return Result == null ? ActionContext.Action.getReturnType() : Result.Specification; }
        }

        public ActionResultContextSurface ToActionResultContextSurface(IFrameworkFacade surface) {
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