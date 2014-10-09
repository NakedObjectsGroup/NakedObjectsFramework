// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Surface.Interface;
using NakedObjects.Value;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectWrapper : ScalarPropertyHolder, INakedObjectSurface {
        private readonly INakedObjectsFramework framework;
        private readonly INakedObject nakedObject;

        protected NakedObjectWrapper(INakedObject nakedObject, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            this.nakedObject = nakedObject;
            this.framework = framework;
            Surface = surface;
        }

        public INakedObject WrappedNakedObject {
            get { return nakedObject; }
        }

        public bool IsTransient {
            get { return nakedObject.ResolveState.IsTransient(); }
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();
                INakedObjectSpecification spec = WrappedNakedObject.Specification;

                if (spec.IsService) {
                    ServiceTypes st = framework.LifecycleManager.GetServiceType(spec);
                    extData[ServiceType] = st.ToString();
                }

                if (spec.ContainsFacet<IViewModelFacet>() && spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject)) {
                    extData[RenderInEditMode] = true;
                }

                if (spec.ContainsFacet<IPresentationHintFacet>()) {
                    extData[PresentationHint] = spec.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        #region INakedObjectSurface Members

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(WrappedNakedObject.Specification, Surface, framework); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get {
                ITypeOfFacet typeOfFacet = nakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.ValueSpec;
                var spec = framework.Metamodel.GetSpecification(introspectableSpecification);
                return new NakedObjectSpecificationWrapper(spec, Surface, framework);
            }
        }

        public object Object {
            get { return nakedObject.Object; }
        }

        public IEnumerable<INakedObjectSurface> ToEnumerable() {
            return WrappedNakedObject.GetAsEnumerable(framework.LifecycleManager).Select(no => new NakedObjectWrapper(no, Surface, framework));
        }

        // todo move into adapterutils

        public INakedObjectSurface Page(int page, int size) {
            return new NakedObjectWrapper(Page(nakedObject, page, size), Surface, framework);
        }

        public int Count() {
            return WrappedNakedObject.GetAsQueryable().Count();
        }

        public AttachmentContext GetAttachment() {
            var fa = WrappedNakedObject.Object as FileAttachment;
            var context = new AttachmentContext();

            if (fa != null) {
                context.Content = fa.GetResourceAsStream();
                context.MimeType = fa.MimeType;
                context.ContentDisposition = fa.DispositionType;
                context.FileName = fa.Name;
            }
            return context;
        }

        public PropertyInfo[] GetKeys() {
            if (nakedObject.Specification.IsService) {
                // services don't have keys
                return new PropertyInfo[] {};
            }
            return framework.LifecycleManager.GetKeys(nakedObject.Object.GetType());
        }

        public IVersionSurface Version {
            get { return new VersionWrapper(nakedObject.Version); }
        }

        public IOidSurface Oid {
            get { return new OidWrapper(nakedObject.Oid); }
        }

        public INakedObjectsSurface Surface { get; set; }

        #endregion

        public static NakedObjectWrapper Wrap(INakedObject nakedObject, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            return nakedObject == null ? null : new NakedObjectWrapper(nakedObject, surface, framework);
        }

        private INakedObject Page(INakedObject objectRepresentingCollection, int page, int size) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.LifecycleManager, true);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.IsTransient):
                    return IsTransient;
                case (ScalarProperty.TitleString):
                    return TitleString();
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        public string TitleString() {
            return nakedObject.TitleString();
        }

        public override bool Equals(object obj) {
            var nakedObjectWrapper = obj as NakedObjectWrapper;
            if (nakedObjectWrapper != null) {
                return Equals(nakedObjectWrapper);
            }
            return false;
        }

        public bool Equals(NakedObjectWrapper other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.nakedObject, nakedObject);
        }

        public override int GetHashCode() {
            return (nakedObject != null ? nakedObject.GetHashCode() : 0);
        }
    }
}