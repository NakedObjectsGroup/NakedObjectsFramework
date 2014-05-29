// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets.Actions.Potency;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectActionWrapper : ScalarPropertyHolder, INakedObjectActionSurface {
        private readonly INakedObjectAction action;
        private readonly string overloadedUniqueId;

        public NakedObjectActionWrapper(INakedObjectAction action, INakedObjectsSurface surface, string overloadedUniqueId) {
            this.action = action;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(action.Specification, Surface); }
        }

        public bool IsContributed {
            get { return action.IsContributedMethod; }
        }

        public string Name {
            get { return action.Name; }
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (action.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = action.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }


        public string Description {
            get { return action.Description; }
        }

        public bool IsQueryOnly {
            get {
                if (action.ReturnType.IsQueryable) {
                    return true;
                }
                return action.ContainsFacet<IQueryOnlyFacet>();
            }
        }

        public bool IsIdempotent {
            get { return action.ContainsFacet<IIdempotentFacet>(); }
        }

        protected int MemberOrder {
            get {
                var facet = action.GetFacet<IMemberOrderFacet>();

                int result;
                if (facet != null && Int32.TryParse(facet.Sequence, out result)) {
                    return result;
                }

                return 0;
            }
        }

        #region INakedObjectActionSurface Members

        public string Id {
            get { return action.Id + overloadedUniqueId; }
        }

        public INakedObjectSpecificationSurface ReturnType {
            get { return new NakedObjectSpecificationWrapper(action.ReturnType, Surface); }
        }

        public int ParameterCount {
            get { return action.ParameterCount; }
        }

        public INakedObjectActionParameterSurface[] Parameters {
            get { return action.Parameters.Select(p => new NakedObjectActionParameterWrapper(p, Surface, overloadedUniqueId)).Cast<INakedObjectActionParameterSurface>().ToArray(); }
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return action.IsVisible(NakedObjectsContext.Session, ((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public IConsentSurface IsUsable(INakedObjectSurface nakedObject) {
            return new ConsentWrapper(action.IsUsable(NakedObjectsContext.Session, ((NakedObjectWrapper) nakedObject).WrappedNakedObject));
        }

        public INakedObjectSpecificationSurface OnType {
            get { return new NakedObjectSpecificationWrapper(action.OnType, Surface); }
        }

        #endregion

        public INakedObjectsSurface Surface { get; set; }

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
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }
    }
}