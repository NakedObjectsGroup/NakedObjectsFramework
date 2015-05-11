// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;
using NakedObjects.Surface.Interface;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Value;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class NakedObjectWrapper : ScalarPropertyHolder, INakedObjectSurface {
        private readonly INakedObjectsFramework framework;
        private  INakedObjectAdapter nakedObject;

        protected NakedObjectWrapper(INakedObjectAdapter nakedObject, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            SurfaceUtils.AssertNotNull(nakedObject, "NakedObject is null");
            SurfaceUtils.AssertNotNull(surface, "Surface is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");


            this.nakedObject = nakedObject;
            this.framework = framework;
            Surface = surface;
        }

        public INakedObjectAdapter WrappedNakedObject {
            get { return nakedObject; }
            set { nakedObject = value; } // hack remove
        }

        public bool IsTransient {
            get { return nakedObject.ResolveState.IsTransient(); }
        }

        protected IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();
                ITypeSpec spec = WrappedNakedObject.Spec;

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

        public void Resolve() {
            if (nakedObject.ResolveState.IsResolvable()) {
                nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
            }
        }

        public INakedObjectSpecificationSurface Specification {
            get { return new NakedObjectSpecificationWrapper(WrappedNakedObject.Spec, Surface, framework); }
        }

        public INakedObjectSpecificationSurface ElementSpecification {
            get {
                ITypeOfFacet typeOfFacet = nakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.GetValueSpec(nakedObject, framework.MetamodelManager.Metamodel);
                var spec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new NakedObjectSpecificationWrapper(spec, Surface, framework);
            }
        }

        public object Object {
            get { return nakedObject.Object; }
        }

        public IEnumerable<INakedObjectSurface> ToEnumerable() {
            return WrappedNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => new NakedObjectWrapper(no, Surface, framework));
        }

        // todo move into adapterutils

        public INakedObjectSurface Page(int page, int size) {
            return new NakedObjectWrapper(Page(nakedObject, page, size), Surface, framework);
        }

        public INakedObjectSurface Select(object[] selection, bool forceEnumerable) {
            return new NakedObjectWrapper(Select(nakedObject, selection, forceEnumerable), Surface, framework);
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

        public object[] GetSelected() {
            var memento = nakedObject.Oid as ICollectionMemento;
            if (memento != null) {
                return memento.SelectedObjects.ToArray();
            }

            return new object[] {};
        }

        public PropertyInfo[] GetKeys() {
            if (nakedObject.Spec is IServiceSpec) {
                // services don't have keys
                return new PropertyInfo[] {};
            }
            return framework.Persistor.GetKeys(nakedObject.Object.GetType());
        }

        public IVersionSurface Version {
            get { return new VersionWrapper(nakedObject.Version); }
        }

        public IOidSurface Oid {
            get { return nakedObject.Oid == null ? null : new OidWrapper(nakedObject.Oid); }
        }

        public INakedObjectsSurface Surface { get; set; }

        #endregion

        public static NakedObjectWrapper Wrap(INakedObjectAdapter nakedObject, INakedObjectsSurface surface, INakedObjectsFramework framework) {
            return nakedObject == null ? null : new NakedObjectWrapper(nakedObject, surface, framework);
        }

        private static bool IsNotQueryable(INakedObjectAdapter objectRepresentingCollection) {
            ICollectionMemento collectionMemento = objectRepresentingCollection.Oid as ICollectionMemento;
            return collectionMemento != null && collectionMemento.IsNotQueryable;
        }

        private INakedObjectAdapter Page(INakedObjectAdapter objectRepresentingCollection, int page, int size) {
            var forceEnumerable = IsNotQueryable(objectRepresentingCollection);

            var newNakedObject = objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.NakedObjectManager, forceEnumerable);

            object[] objects = newNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => no.Object).ToArray();

            var currentMemento = (ICollectionMemento)nakedObject.Oid;
            ICollectionMemento newMemento = currentMemento.NewSelectionMemento(objects, true);
            newNakedObject.SetATransientOid(newMemento);
            return newNakedObject;
        }

        private INakedObjectAdapter Select(INakedObjectAdapter objectRepresentingCollection, object[] selected, bool forceEnumerable) {
            IList result = CollectionUtils.CloneCollectionAndPopulate(objectRepresentingCollection.Object, selected);
            INakedObjectAdapter adapter = framework.NakedObjectManager.CreateAdapter(objectRepresentingCollection.Spec.IsQueryable && !forceEnumerable ? (IEnumerable)result.AsQueryable() : result, null, null);
            var currentMemento = (ICollectionMemento)objectRepresentingCollection.Oid;
            var newMemento = currentMemento.NewSelectionMemento(selected, false);
            adapter.SetATransientOid(newMemento);
            return adapter;
        }

        private  bool IsPaged() {
            ICollectionMemento collectionMemento = nakedObject.Oid as ICollectionMemento;
            return collectionMemento != null && collectionMemento.IsPaged;
        }

        private  bool IsViewModelEditView() {
            return nakedObject.Spec.ContainsFacet<IViewModelFacet>() && nakedObject.Spec.GetFacet<IViewModelFacet>().IsEditView(nakedObject);
        }

        public override object GetScalarProperty(ScalarProperty name) {
            switch (name) {
                case (ScalarProperty.IsTransient):
                    return IsTransient;
                case (ScalarProperty.TitleString):
                    return TitleString();
                case (ScalarProperty.InvariantString):
                    return InvariantString();
                case (ScalarProperty.IsNotPersistent):
                    return IsNotPersistent();
                case (ScalarProperty.IsUserPersistable):
                    return IsUserPersistable();
                case (ScalarProperty.IsCollectionMemento):
                    return IsCollectionMemento();
                case (ScalarProperty.IsPaged):
                    return IsPaged();
                case (ScalarProperty.IsViewModelEditView):
                    return IsViewModelEditView();
                case (ScalarProperty.EnumIntegralValue):
                    return EnumIntegralValue();
                case (ScalarProperty.ExtensionData):
                    return ExtensionData;
                default:
                    throw new NotImplementedException(string.Format("{0} doesn't support {1}", GetType(), name));
            }
        }

        private object EnumIntegralValue() {
            var enumFacet = nakedObject.Spec.GetFacet<IEnumValueFacet>();
            return enumFacet != null ? enumFacet.IntegralValue(nakedObject) : null;
        }


        private object InvariantString() {
            return nakedObject.InvariantString();
        }

        private bool IsCollectionMemento() {
            return nakedObject.Oid is ICollectionMemento;
        }

        private object IsUserPersistable() {
            return nakedObject.Spec.Persistable == PersistableType.UserPersistable;
        }

        private object IsNotPersistent() {
            return nakedObject.IsNotPersistent();
        }

        public string TitleString() {
            var title =  nakedObject.TitleString();
            return string.IsNullOrWhiteSpace(title) ? nakedObject.Spec.UntitledName : title;
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