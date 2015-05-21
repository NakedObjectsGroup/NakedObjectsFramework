// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Surface.Nof2.Utility;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Wrapper {
    public class NakedObjectActionWrapper : ScalarPropertyHolder, INakedObjectActionSurface {
        private readonly ActionWrapper action;
        private readonly Naked target;

        public NakedObjectActionWrapper(ActionWrapper action, Naked target, INakedObjectsSurface surface) {
            this.action = action;
            this.target = target;
            Surface = surface;
        }

        public INakedObjectSpecificationSurface Specification {
            get {
                NakedObjectSpecification rt = action.getReturnType();
                return rt == null ? (INakedObjectSpecificationSurface) new VoidNakedObjectSpecificationWrapper() : new NakedObjectSpecificationWrapper(action.getReturnType(), target, Surface);
            }
        }

        public bool IsContributed {
            get { return false; }
        }

        public string Name {
            get { return action.getName(); }
        }

        public string Description {
            get { return action.getDescription(); }
        }

        public bool IsQueryOnly {
            get { return false; }
        }

        public bool IsIdempotent {
            get { return false; }
        }

        protected int MemberOrder {
            get { return 0; }
        }

        #region INakedObjectActionSurface Members

        public string Id {
            get { return action.getId(); }
        }

        public INakedObjectSpecificationSurface ReturnType {
            get { return Specification; }
        }

        public INakedObjectSpecificationSurface ElementType {
            get {
                throw new NotImplementedException();
            }
        }

        public int ParameterCount {
            get { return action.getParameterCount(); }
        }

        public INakedObjectActionParameterSurface[] Parameters {
            get { return action.GetParameters((NakedReference) target).Select(p => new NakedObjectActionParameterWrapper(p, target, Surface)).Cast<INakedObjectActionParameterSurface>().ToArray(); }
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return action.isVisible((NakedReference) (((NakedObjectWrapper) nakedObject).NakedObject)).isAllowed();
        }

        public IConsentSurface IsUsable(INakedObjectSurface nakedObject) {
            return new ConsentWrapper(action.isAvailable((NakedReference) (((NakedObjectWrapper) nakedObject).NakedObject)));
        }

        public INakedObjectSpecificationSurface OnType {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectActionWrapper = obj as NakedObjectActionWrapper;
            if (nakedObjectActionWrapper != null) {
                return Equals(nakedObjectActionWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectActionWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.action, action);
        }

        public override int GetHashCode() {
            return (action != null ? action.GetHashCode() : 0);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.Name):
                    return Name;
                case (ScalarProperty.Description):
                    return Description;
                case (ScalarProperty.IsQueryOnly):
                    return IsQueryOnly;
                case (ScalarProperty.IsIdempotent):
                    return IsIdempotent;
                case (ScalarProperty.MemberOrder):
                    return MemberOrder;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public INakedObjectsSurface Surface { get; set; }
    }
}