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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Value;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Impl.Utility;


namespace NakedObjects.Facade.Impl {
    public class ObjectFacade : IObjectFacade {
        private readonly INakedObjectsFramework framework;

        protected ObjectFacade(INakedObjectAdapter nakedObject, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework) {
            WrappedNakedObject = nakedObject ?? throw new NullReferenceException($"{nameof(nakedObject)} is null");
            this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
            FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
        }

        public INakedObjectAdapter WrappedNakedObject { get; }

        #region IObjectFacade Members

        public bool IsTransient => WrappedNakedObject.ResolveState.IsTransient();

        public void SetIsNotQueryableState(bool state) {
            if (WrappedNakedObject.Oid is ICollectionMemento memento) {
                memento.IsNotQueryable = state;
            }
        }

        public ITypeFacade Specification => new TypeFacade(WrappedNakedObject.Spec, FrameworkFacade, framework);

        public ITypeFacade ElementSpecification {
            get {
                var typeOfFacet = WrappedNakedObject.GetTypeOfFacetFromSpec();
                var introspectableSpecification = typeOfFacet.GetValueSpec(WrappedNakedObject, framework.MetamodelManager.Metamodel);
                var spec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
                return new TypeFacade(spec, FrameworkFacade, framework);
            }
        }

        public string PresentationHint {
            get {
                var spec = WrappedNakedObject.Spec;
                var hintFacet = spec.GetFacet<IPresentationHintFacet>();
                return hintFacet == null ? "" : hintFacet.Value;
            }
        }

        public object Object => WrappedNakedObject.Object;

        public IEnumerable<IObjectFacade> ToEnumerable() => WrappedNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => new ObjectFacade(no, FrameworkFacade, framework));

        public IObjectFacade Page(int page, int size, bool forceEnumerable) => new ObjectFacade(Page(WrappedNakedObject, page, size, forceEnumerable), FrameworkFacade, framework);

        public IObjectFacade Select(object[] selection, bool forceEnumerable) => new ObjectFacade(Select(WrappedNakedObject, selection, forceEnumerable), FrameworkFacade, framework);

        public int Count() => WrappedNakedObject.GetAsQueryable().Count();

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

        public PropertyInfo[] GetKeys() =>
            WrappedNakedObject.Spec is IServiceSpec
                ? new PropertyInfo[] { }
                : framework.Persistor.GetKeys(WrappedNakedObject.Object.GetType());

        public IVersionFacade Version => new VersionFacade(WrappedNakedObject.Version);

        public IOidFacade Oid => WrappedNakedObject.Oid == null ? null : new OidFacade(WrappedNakedObject.Oid);

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool IsViewModelEditView => IsViewModel && WrappedNakedObject.Spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject, framework);

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

        public static ObjectFacade Wrap(INakedObjectAdapter nakedObject, IFrameworkFacade facade, INakedObjectsFramework framework) => nakedObject == null ? null : new ObjectFacade(nakedObject, facade, framework);

        private static bool IsNotQueryable(INakedObjectAdapter objectRepresentingCollection) => objectRepresentingCollection.Oid is ICollectionMemento cm && cm.IsNotQueryable;

        private INakedObjectAdapter Page(INakedObjectAdapter objectRepresentingCollection, int page, int size, bool forceEnumerable) {
            var toEnumerable = IsNotQueryable(objectRepresentingCollection) || forceEnumerable;

            var newNakedObject = objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.NakedObjectManager, toEnumerable);

            var objects = newNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => no.Object).ToArray();

            var currentMemento = (ICollectionMemento) WrappedNakedObject.Oid;
            var newMemento = currentMemento.NewSelectionMemento(objects, true);
            newNakedObject.SetATransientOid(newMemento);
            return newNakedObject;
        }

        private INakedObjectAdapter Select(INakedObjectAdapter objectRepresentingCollection, object[] selected, bool forceEnumerable) {
            var result = CollectionUtils.CloneCollectionAndPopulate(objectRepresentingCollection.Object, selected);
            var adapter = framework.NakedObjectManager.CreateAdapter(objectRepresentingCollection.Spec.IsQueryable && !forceEnumerable ? (IEnumerable) result.AsQueryable() : result, null, null);
            var currentMemento = (ICollectionMemento) objectRepresentingCollection.Oid;
            var newMemento = currentMemento.NewSelectionMemento(selected, false);
            adapter.SetATransientOid(newMemento);
            return adapter;
        }

        public override bool Equals(object obj) => obj is ObjectFacade of && Equals(of);

        public bool Equals(ObjectFacade other) {
            if (ReferenceEquals(null, other)) { return false; }

            return ReferenceEquals(this, other) || Equals(other.WrappedNakedObject, WrappedNakedObject);
        }

        public override int GetHashCode() => WrappedNakedObject != null ? WrappedNakedObject.GetHashCode() : 0;
    }
}