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
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Value;

namespace NakedObjects.Facade.Impl {
    public class ObjectFacade : IObjectFacade {
        private readonly INakedObjectsFramework framework;

        protected ObjectFacade(INakedObjectAdapter nakedObject, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework) {
            FacadeUtils.AssertNotNull(nakedObject, "NakedObject is null");
            FacadeUtils.AssertNotNull(frameworkFacade, "FrameworkFacade is null");
            FacadeUtils.AssertNotNull(framework, "framework is null");

            WrappedNakedObject = nakedObject;
            this.framework = framework;
            FrameworkFacade = frameworkFacade;
        }

        public INakedObjectAdapter WrappedNakedObject { get; }

        #region IObjectFacade Members

        public bool IsTransient => WrappedNakedObject.ResolveState.IsTransient();

      

        public void SetIsNotQueryableState(bool state) {
            var memento = WrappedNakedObject.Oid as ICollectionMemento;

            if (memento != null) {
                memento.IsNotQueryable = state;
            }
        }

        public ITypeFacade Specification => new TypeFacade(WrappedNakedObject.Spec, FrameworkFacade, framework);

        public ITypeFacade ElementSpecification {
            get {
                ITypeOfFacet typeOfFacet = WrappedNakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.GetValueSpec(WrappedNakedObject, framework.MetamodelManager.Metamodel);
                var spec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new TypeFacade(spec, FrameworkFacade, framework);
            }
        }

        public string PresentationHint {
            get {
                ITypeSpec spec = WrappedNakedObject.Spec;
                var hintFacet = spec.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? "" : hintFacet.Value;
            }
        }

        public object Object => WrappedNakedObject.Object;

        public IEnumerable<IObjectFacade> ToEnumerable() {
            return WrappedNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => new ObjectFacade(no, FrameworkFacade, framework));
        }

        public IObjectFacade Page(int page, int size, bool forceEnumerable) {
            return new ObjectFacade(Page(WrappedNakedObject, page, size, forceEnumerable), FrameworkFacade, framework);
        }

        public IObjectFacade Select(object[] selection, bool forceEnumerable) {
            return new ObjectFacade(Select(WrappedNakedObject, selection, forceEnumerable), FrameworkFacade, framework);
        }

        public int Count() {
            return WrappedNakedObject.GetAsQueryable().Count();
        }

        public AttachmentContextFacade GetAttachment() {
            var fa = WrappedNakedObject.Object as FileAttachment;
            var context = new AttachmentContextFacade();

            if (fa != null) {
                context.Content = fa.GetResourceAsStream();
                context.MimeType = fa.MimeType;
                context.ContentDisposition = fa.DispositionType;
                context.FileName = fa.Name;
            }
            return context;
        }

       

        public PropertyInfo[] GetKeys() {
            if (WrappedNakedObject.Spec is IServiceSpec) {
                // services don't have keys
                return new PropertyInfo[] {};
            }
            return framework.Persistor.GetKeys(WrappedNakedObject.Object.GetType());
        }

        public IVersionFacade Version => new VersionFacade(WrappedNakedObject.Version);

        public IOidFacade Oid => WrappedNakedObject.Oid == null ? null : new OidFacade(WrappedNakedObject.Oid);

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool IsViewModelEditView => IsViewModel && WrappedNakedObject.Spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject);

        public bool IsViewModel => WrappedNakedObject.Spec.ContainsFacet<IViewModelFacet>();

        public bool IsDestroyed => WrappedNakedObject.ResolveState.IsDestroyed();

        public string InvariantString => WrappedNakedObject.InvariantString();

        public bool IsUserPersistable => WrappedNakedObject.Spec.Persistable == PersistableType.UserPersistable;

        public bool IsNotPersistent => WrappedNakedObject.IsNotPersistent();

        public string TitleString {
            get {
                var title = WrappedNakedObject.TitleString();
                return string.IsNullOrWhiteSpace(title) && !WrappedNakedObject.Spec.IsParseable ? WrappedNakedObject.Spec.UntitledName : title;
            }
        }

        #endregion

        public static ObjectFacade Wrap(INakedObjectAdapter nakedObject, IFrameworkFacade facade, INakedObjectsFramework framework) {
            return nakedObject == null ? null : new ObjectFacade(nakedObject, facade, framework);
        }

        private static bool IsNotQueryable(INakedObjectAdapter objectRepresentingCollection) {
            ICollectionMemento collectionMemento = objectRepresentingCollection.Oid as ICollectionMemento;
            return collectionMemento != null && collectionMemento.IsNotQueryable;
        }

        private INakedObjectAdapter Page(INakedObjectAdapter objectRepresentingCollection, int page, int size, bool forceEnumerable) {
            var toEnumerable = IsNotQueryable(objectRepresentingCollection) || forceEnumerable;

            var newNakedObject = objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.NakedObjectManager, toEnumerable);

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
            return WrappedNakedObject != null ? WrappedNakedObject.GetHashCode() : 0;
        }
    }
}