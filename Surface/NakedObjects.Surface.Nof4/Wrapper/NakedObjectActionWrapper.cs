// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Utility;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectActionWrapper : ScalarPropertyHolder, INakedObjectActionSurface {
        private readonly IActionSpec action;
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;


        public NakedObjectActionWrapper(IActionSpec action, INakedObjectsSurface surface, INakedObjectsFramework framework, string overloadedUniqueId) {
            SurfaceUtils.AssertNotNull(action, "Action is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");
            SurfaceUtils.AssertNotNull(overloadedUniqueId, "overloadedUniqueId is null");
            SurfaceUtils.AssertNotNull(surface, "surface is null");


            this.action = action;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        public IActionSpec WrappedSpec { get { return action; } }


        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(action.ReturnSpec, Surface, framework); }
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
                if (action.ReturnSpec.IsQueryable) {
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
            get { return new NakedObjectSpecificationWrapper(action.ReturnSpec, Surface, framework); }
        }

        public INakedObjectSpecificationSurface ElementType {
            get {
                var elementSpec = action.ElementSpec;
                return elementSpec == null ? null : new NakedObjectSpecificationWrapper(elementSpec, Surface, framework);
            }
        }

        public int ParameterCount {
            get { return action.ParameterCount; }
        }

        public INakedObjectActionParameterSurface[] Parameters {
            get { return action.Parameters.Select(p => new NakedObjectActionParameterWrapper(p, Surface, framework, overloadedUniqueId)).Cast<INakedObjectActionParameterSurface>().ToArray(); }
        }

        public bool IsVisible(INakedObjectSurface nakedObject) {
            return action.IsVisible(((NakedObjectWrapper) nakedObject).WrappedNakedObject);
        }

        public IConsentSurface IsUsable(INakedObjectSurface nakedObject) {
            return new ConsentWrapper(action.IsUsable(((NakedObjectWrapper) nakedObject).WrappedNakedObject));
        }

        public INakedObjectSpecificationSurface OnType {
            get { return new NakedObjectSpecificationWrapper(action.OnSpec, Surface, framework); }
        }

        public INakedObjectsSurface Surface { get; set; }

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
                case (ScalarProperty.IsContributed):
                    return IsContributed;
                case (ScalarProperty.MemberOrder):
                    return MemberOrder;
                case (ScalarProperty.PageSize):
                    return PageSize;
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                case (ScalarProperty.TableViewData):
                    return TableViewData;
                case (ScalarProperty.RenderEagerly):
                    return RenderEagerly;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public bool RenderEagerly {
            get {
                IEagerlyFacet eagerlyFacet = action.GetFacet<IEagerlyFacet>();
                return eagerlyFacet != null && eagerlyFacet.What == EagerlyAttribute.Do.Rendering;
            }
        }

        public Tuple<bool, string[]> TableViewData {
            get {
                var facet = action.GetFacet<ITableViewFacet>();
                return facet == null ? null : new Tuple<bool, string[]>(facet.Title, facet.Columns);
            }
        }

        private int PageSize {
            get {
                return action.GetFacet<IPageSizeFacet>().Value;
            }
        }
    }
}