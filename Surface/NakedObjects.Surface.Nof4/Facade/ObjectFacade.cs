// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
using NakedObjects.Facade.Contexts;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Value;

namespace NakedObjects.Facade.Nof4 {
    public class ObjectFacade : IObjectFacade {
        private readonly INakedObjectsFramework framework;

        protected ObjectFacade(INakedObjectAdapter nakedObject, IFrameworkFacade surface, INakedObjectsFramework framework) {
            SurfaceUtils.AssertNotNull(nakedObject, "NakedObject is null");
            SurfaceUtils.AssertNotNull(surface, "Surface is null");
            SurfaceUtils.AssertNotNull(framework, "framework is null");

            WrappedNakedObject = nakedObject;
            this.framework = framework;
            Surface = surface;
        }

        public INakedObjectAdapter WrappedNakedObject { get; private set; }

        #region IObjectFacade Members

        public bool IsTransient {
            get { return WrappedNakedObject.ResolveState.IsTransient(); }
        }

        public IDictionary<string, object> ExtensionData {
            get {
                var extData = new Dictionary<string, object>();
                ITypeSpec spec = WrappedNakedObject.Spec;

                if (spec.ContainsFacet<IViewModelFacet>() && spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject)) {
                    extData[IdConstants.RenderInEditMode] = true;
                }

                if (spec.ContainsFacet<IPresentationHintFacet>()) {
                    extData[IdConstants.PresentationHint] = spec.GetFacet<IPresentationHintFacet>().Value;
                }

                return extData.Any() ? extData : null;
            }
        }

        public void Resolve() {
            if (WrappedNakedObject.ResolveState.IsResolvable()) {
                WrappedNakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                WrappedNakedObject.ResolveState.Handle(Events.EndResolvingEvent);
            }
        }

        public void SetIsNotQueryableState(bool state) {
            var memento = WrappedNakedObject.Oid as ICollectionMemento;

            if (memento != null) {
                memento.IsNotQueryable = state;
            }
        }

        public ITypeFacade Specification {
            get { return new TypeFacade(WrappedNakedObject.Spec, Surface, framework); }
        }

        public ITypeFacade ElementSpecification {
            get {
                ITypeOfFacet typeOfFacet = WrappedNakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.GetValueSpec(WrappedNakedObject, framework.MetamodelManager.Metamodel);
                var spec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new TypeFacade(spec, Surface, framework);
            }
        }

        public object Object {
            get { return WrappedNakedObject.Object; }
        }

        public IEnumerable<IObjectFacade> ToEnumerable() {
            return WrappedNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => new ObjectFacade(no, Surface, framework));
        }

        // todo move into adapterutils

        public IObjectFacade Page(int page, int size) {
            return new ObjectFacade(Page(WrappedNakedObject, page, size), Surface, framework);
        }

        public IObjectFacade Select(object[] selection, bool forceEnumerable) {
            return new ObjectFacade(Select(WrappedNakedObject, selection, forceEnumerable), Surface, framework);
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
            var memento = WrappedNakedObject.Oid as ICollectionMemento;
            if (memento != null) {
                return memento.SelectedObjects.ToArray();
            }

            return new object[] {};
        }

        public PropertyInfo[] GetKeys() {
            if (WrappedNakedObject.Spec is IServiceSpec) {
                // services don't have keys
                return new PropertyInfo[] {};
            }
            return framework.Persistor.GetKeys(WrappedNakedObject.Object.GetType());
        }

        public IVersionFacade Version {
            get { return new VersionFacade(WrappedNakedObject.Version); }
        }

        public IOidFacade Oid {
            get { return WrappedNakedObject.Oid == null ? null : new OidFacade(WrappedNakedObject.Oid); }
        }

        public IFrameworkFacade Surface { get; set; }

        public bool IsPaged {
            get {
                ICollectionMemento collectionMemento = WrappedNakedObject.Oid as ICollectionMemento;
                return collectionMemento != null && collectionMemento.IsPaged;
            }
        }

        public bool IsViewModelEditView {
            get { return IsViewModel && WrappedNakedObject.Spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject); }
        }

        public bool IsViewModel {
            get { return WrappedNakedObject.Spec.ContainsFacet<IViewModelFacet>(); }
        }

        public bool IsDestroyed {
            get { return WrappedNakedObject.ResolveState.IsDestroyed(); }
        }

        public IActionFacade MementoAction {
            get {
                var mementoOid = WrappedNakedObject.Oid as ICollectionMemento;
                return mementoOid == null ? null : new ActionFacade(mementoOid.Action, Surface, framework, "");
            }
        }

        public string EnumIntegralValue {
            get {
                var enumFacet = WrappedNakedObject.Spec.GetFacet<IEnumValueFacet>();

                if (enumFacet != null) {
                    return enumFacet.IntegralValue(WrappedNakedObject);
                }

                var value = WrappedNakedObject.Object == null ? "" : WrappedNakedObject.Object.ToString();

                long result;
                return long.TryParse(value, out result) ? result.ToString() : null;
            }
        }

        public string InvariantString {
            get { return WrappedNakedObject.InvariantString(); }
        }

        public bool IsCollectionMemento {
            get { return WrappedNakedObject.Oid is ICollectionMemento; }
        }

        public bool IsUserPersistable {
            get { return WrappedNakedObject.Spec.Persistable == PersistableType.UserPersistable; }
        }

        public bool IsNotPersistent {
            get { return WrappedNakedObject.IsNotPersistent(); }
        }

        public string TitleString {
            get {
                var title = WrappedNakedObject.TitleString();
                return string.IsNullOrWhiteSpace(title) ? WrappedNakedObject.Spec.UntitledName : title;
            }
        }

        #endregion

        public static ObjectFacade Wrap(INakedObjectAdapter nakedObject, IFrameworkFacade surface, INakedObjectsFramework framework) {
            return nakedObject == null ? null : new ObjectFacade(nakedObject, surface, framework);
        }

        private static bool IsNotQueryable(INakedObjectAdapter objectRepresentingCollection) {
            ICollectionMemento collectionMemento = objectRepresentingCollection.Oid as ICollectionMemento;
            return collectionMemento != null && collectionMemento.IsNotQueryable;
        }

        private INakedObjectAdapter Page(INakedObjectAdapter objectRepresentingCollection, int page, int size) {
            var forceEnumerable = IsNotQueryable(objectRepresentingCollection);

            var newNakedObject = objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.NakedObjectManager, forceEnumerable);

            object[] objects = newNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => no.Object).ToArray();

            var currentMemento = (ICollectionMemento) WrappedNakedObject.Oid;
            ICollectionMemento newMemento = currentMemento.NewSelectionMemento(objects, true);
            newNakedObject.SetATransientOid(newMemento);
            return newNakedObject;
        }

        private INakedObjectAdapter Select(INakedObjectAdapter objectRepresentingCollection, object[] selected, bool forceEnumerable) {
            IList result = CollectionUtils.CloneCollectionAndPopulate(objectRepresentingCollection.Object, selected);
            INakedObjectAdapter adapter = framework.NakedObjectManager.CreateAdapter(objectRepresentingCollection.Spec.IsQueryable && !forceEnumerable ? (IEnumerable) result.AsQueryable() : result, null, null);
            var currentMemento = (ICollectionMemento) objectRepresentingCollection.Oid;
            var newMemento = currentMemento.NewSelectionMemento(selected, false);
            adapter.SetATransientOid(newMemento);
            return adapter;
        }

        public override bool Equals(object obj) {
            var nakedObjectWrapper = obj as ObjectFacade;
            if (nakedObjectWrapper != null) {
                return Equals(nakedObjectWrapper);
            }
            return false;
        }

        public bool Equals(ObjectFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.WrappedNakedObject, WrappedNakedObject);
        }

        public override int GetHashCode() {
            return (WrappedNakedObject != null ? WrappedNakedObject.GetHashCode() : 0);
        }
    }
}