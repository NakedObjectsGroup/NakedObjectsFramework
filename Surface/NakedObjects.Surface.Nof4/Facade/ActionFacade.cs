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
using NakedObjects.Facade.Utility;
using NakedObjects.Surface.Nof4.Utility;

namespace NakedObjects.Facade.Nof4 {
    public class ActionFacade : IActionFacade {
        private readonly IActionSpec action;
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;

        public ActionFacade(IActionSpec action, IFrameworkFacade surface, INakedObjectsFramework framework, string overloadedUniqueId) {
            SurfaceUtils.AssertNotNull(action, "Action is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");
            SurfaceUtils.AssertNotNull(overloadedUniqueId, "overloadedUniqueId is null");
            SurfaceUtils.AssertNotNull(surface, "surface is null");

            this.action = action;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            Surface = surface;
        }

        public IActionSpec WrappedSpec {
            get { return action; }
        }

        public ITypeFacade Specification {
            get { return new TypeFacade(action.ReturnSpec, Surface, framework); }
        }

        #region IActionFacade Members

        public bool IsContributed {
            get { return action.IsContributedMethod; }
        }

        public string Name {
            get { return action.Name; }
        }

        public IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();

                if (action.ContainsFacet<IPresentationHintFacet>()) {
                    extData[IdConstants.PresentationHint] = action.GetFacet<IPresentationHintFacet>().Value;
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

        public int MemberOrder {
            get {
                var facet = action.GetFacet<IMemberOrderFacet>();

                int result;
                if (facet != null && Int32.TryParse(facet.Sequence, out result)) {
                    return result;
                }

                return 0;
            }
        }

        public string Id {
            get { return action.Id + overloadedUniqueId; }
        }

        public ITypeFacade ReturnType {
            get { return new TypeFacade(action.ReturnSpec, Surface, framework); }
        }

        public ITypeFacade ElementType {
            get {
                var elementSpec = action.ElementSpec;
                return elementSpec == null ? null : new TypeFacade(elementSpec, Surface, framework);
            }
        }

        public int ParameterCount {
            get { return action.ParameterCount; }
        }

        public IActionParameterFacade[] Parameters {
            get { return action.Parameters.Select(p => new ActionParameterFacade(p, Surface, framework, overloadedUniqueId)).Cast<IActionParameterFacade>().ToArray(); }
        }

        public bool IsVisible(IObjectFacade nakedObject) {
            return action.IsVisible(((ObjectFacade) nakedObject).WrappedNakedObject);
        }

        public IConsentFacade IsUsable(IObjectFacade nakedObject) {
            return new ConsentFacade(action.IsUsable(((ObjectFacade) nakedObject).WrappedNakedObject));
        }

        public ITypeFacade OnType {
            get { return new TypeFacade(action.OnSpec, Surface, framework); }
        }

        public IFrameworkFacade Surface { get; set; }

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

        public int PageSize {
            get { return action.GetFacet<IPageSizeFacet>().Value; }
        }

        public string PresentationHint {
            get {
                var hintFacet = action.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? null : hintFacet.Value;
            }
        }

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectActionWrapper = obj as ActionFacade;
            if (nakedObjectActionWrapper != null) {
                return Equals(nakedObjectActionWrapper);
            }
            return false;
        }

        public bool Equals(ActionFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.action, action);
        }

        public override int GetHashCode() {
            return (action != null ? action.GetHashCode() : 0);
        }
    }
}