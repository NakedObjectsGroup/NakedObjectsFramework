﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;
using NakedFramework.Value;

namespace NakedFramework.Facade.Impl.Impl;

public class ObjectFacade : IObjectFacade {
    private readonly INakedFramework framework;

    private ObjectFacade(INakedObjectAdapter nakedObject, IFrameworkFacade frameworkFacade, INakedFramework framework) {
        WrappedNakedObject = nakedObject ?? throw new NullReferenceException($"{nameof(nakedObject)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
    }

    public INakedObjectAdapter WrappedNakedObject { get; }

    public static ObjectFacade Wrap(INakedObjectAdapter nakedObject, IFrameworkFacade facade, INakedFramework framework) => nakedObject is null ? null : new ObjectFacade(nakedObject, facade, framework);

    private static bool IsNotQueryable(INakedObjectAdapter objectRepresentingCollection) => objectRepresentingCollection.Oid is ICollectionMemento { IsNotQueryable: true };

    private INakedObjectAdapter Page(INakedObjectAdapter objectRepresentingCollection, int page, int size, bool forceEnumerable) {
        var toEnumerable = IsNotQueryable(objectRepresentingCollection) || forceEnumerable;

        var newNakedObject = objectRepresentingCollection.GetCollectionFacetFromSpec().Page(page, size, objectRepresentingCollection, framework.NakedObjectManager, toEnumerable);

        var objects = newNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => no.Object).ToArray();

        var currentMemento = (ICollectionMemento)WrappedNakedObject.Oid;
        var newMemento = currentMemento.NewSelectionMemento(objects, true);
        newNakedObject.SetATransientOid(newMemento);
        return newNakedObject;
    }

    private INakedObjectAdapter Select(INakedObjectAdapter objectRepresentingCollection, object[] selected, bool forceEnumerable) {
        var result = CollectionUtils.CloneCollectionAndPopulate(objectRepresentingCollection.Object, selected);
        var adapter = framework.NakedObjectManager.CreateAdapter(objectRepresentingCollection.Spec.IsQueryable && !forceEnumerable ? result.AsQueryable() : result, null, null);
        var currentMemento = (ICollectionMemento)objectRepresentingCollection.Oid;
        var newMemento = currentMemento.NewSelectionMemento(selected, false);
        adapter.SetATransientOid(newMemento);
        return adapter;
    }

    public override bool Equals(object obj) => obj is ObjectFacade of && Equals(of);

    private bool Equals(ObjectFacade other) {
        if (other is null) {
            return false;
        }

        return ReferenceEquals(this, other) || Equals(other.WrappedNakedObject, WrappedNakedObject);
    }

    public override int GetHashCode() => WrappedNakedObject.GetHashCode();

    #region IObjectFacade Members

    public bool IsTransient => WrappedNakedObject.ResolveState.IsTransient();

    public void SetIsNotQueryableState(bool state) {
        if (WrappedNakedObject.Oid is ICollectionMemento memento) {
            memento.IsNotQueryable = state;
        }
    }

    private ITypeFacade cachedSpecification;
    private ITypeFacade cachedElementSpecification;
    private string cachedPresentationHint;
    private NullCache<(string, string)?> cachedRestExtension;
    private IEnumerable<IObjectFacade> cachedToEnumerable;
    private AttachmentContextFacade cachedAttachmentContextFacade;
    private PropertyInfo[] cachedKeys;
    private bool? cachedIsViewModel;

    public ITypeFacade Specification => cachedSpecification ??= new TypeFacade(WrappedNakedObject.Spec, FrameworkFacade, framework);

    private ITypeFacade GetElementSpec() {
        var typeOfFacet = WrappedNakedObject.GetTypeOfFacetFromSpec();
        var introspectableSpecification = typeOfFacet.GetValueSpec(WrappedNakedObject, framework.MetamodelManager.Metamodel);
        var spec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
        return new TypeFacade(spec, FrameworkFacade, framework);
    }

    public ITypeFacade ElementSpecification => cachedElementSpecification ??= GetElementSpec();

    public string PresentationHint => cachedPresentationHint ??= WrappedNakedObject.Spec.GetFacet<IPresentationHintFacet>()?.Value ?? "";

    public (string, string)? RestExtension => (cachedRestExtension ??= FacadeUtils.NullCache(WrappedNakedObject.Spec.GetRestExtension())).Value;

    public object Object => WrappedNakedObject.Object;

    public IEnumerable<IObjectFacade> ToEnumerable() => cachedToEnumerable ??= WrappedNakedObject.GetAsEnumerable(framework.NakedObjectManager).Select(no => new ObjectFacade(no, FrameworkFacade, framework));

    public IObjectFacade Page(int page, int size, bool forceEnumerable) => new ObjectFacade(Page(WrappedNakedObject, page, size, forceEnumerable), FrameworkFacade, framework);

    public IObjectFacade Select(object[] selection, bool forceEnumerable) => new ObjectFacade(Select(WrappedNakedObject, selection, forceEnumerable), FrameworkFacade, framework);

    public int Count() => WrappedNakedObject.GetAsQueryable().Count();

    public AttachmentContextFacade GetAttachment() {
        AttachmentContextFacade GetAttachmentContext() {
            var context = new AttachmentContextFacade();
            if (WrappedNakedObject.Object is FileAttachment fa) {
                context.Content = fa.GetResourceAsStream();
                context.MimeType = fa.MimeType;
                context.ContentDisposition = fa.DispositionType;
                context.FileName = fa.Name;
            }

            return context;
        }

        return cachedAttachmentContextFacade ??= GetAttachmentContext();
    }

    public PropertyInfo[] GetKeys() =>
        cachedKeys ??= WrappedNakedObject.Spec is IServiceSpec
            ? Array.Empty<PropertyInfo>()
            : framework.Persistor.GetKeys(WrappedNakedObject.Object.GetType());

    public IVersionFacade Version => new VersionFacade(WrappedNakedObject.Version);

    public IOidFacade Oid => WrappedNakedObject.Oid is null ? null : new OidFacade(WrappedNakedObject.Oid);

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool IsViewModelEditView => IsViewModel && WrappedNakedObject.Spec.GetFacet<IViewModelFacet>().IsEditView(WrappedNakedObject, framework);

    public bool IsViewModel => cachedIsViewModel ??= WrappedNakedObject.Spec.ContainsFacet<IViewModelFacet>();

    public bool IsDestroyed => WrappedNakedObject.ResolveState.IsDestroyed();

    public bool IsUserPersistable => WrappedNakedObject.Spec.Persistable is PersistableType.UserPersistable;

    public bool IsNotPersistent => WrappedNakedObject.IsNotPersistent();

    public string TitleString {
        get {
            var title = WrappedNakedObject.TitleString();
            return string.IsNullOrWhiteSpace(title) && !WrappedNakedObject.Spec.IsParseable ? WrappedNakedObject.Spec.UntitledName : title;
        }
    }

    public object ToValue(string format = null) => WrappedNakedObject.Spec.GetFacet<IValueFacet>().Value(WrappedNakedObject, format);

    #endregion
}