// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Persist;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Util;

namespace NakedFramework.Persistor.EFCore.Component;

public class EFCoreCreateObjectCommand : ICreateObjectCommand {
    private readonly EFCoreLocalContext context;
    private readonly DbContext dbContext;
    private readonly INakedObjectAdapter nakedObjectAdapter;
    private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();
    private readonly EFCoreObjectStore parent;

    public EFCoreCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter, EFCoreLocalContext context, EFCoreObjectStore parent) {
        this.context = context;
        dbContext = context.WrappedDbContext;
        this.parent = parent;
        this.nakedObjectAdapter = nakedObjectAdapter;
    }

    private void SetKeyAsNecessary(object objectToProxy, object proxy) {
        if (!dbContext.IdMembersAreIdentity(objectToProxy.GetType())) {
            var idMembers = dbContext.SafeGetKeys(objectToProxy.GetType());
            idMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
        }
    }

    private INakedObjectAdapter CreateAdapter(object obj) => parent.CreateAdapter(null, obj);

    private object ProxyObjectIfAppropriate(object originalObject) {
        if (originalObject is null) {
            return null;
        }

        if (FasterTypeUtils.IsEFCoreProxy(originalObject.GetType().FullName)) {
            // object already proxied assume previous save failed - add object to context again 
            var entry = dbContext.Entry(originalObject);

            // if EF knows object check if detached 
            if (entry is null || entry.State == EntityState.Detached) {
                dbContext.Add(originalObject);
            }

            return originalObject;
        }

        if (objectToProxyScratchPad.ContainsKey(originalObject)) {
            return objectToProxyScratchPad[originalObject];
        }

        var adapterForOriginalObjectAdapter = CreateAdapter(originalObject);

        return adapterForOriginalObjectAdapter.ResolveState.IsPersistent()
            ? originalObject
            : ProxyObject(originalObject, adapterForOriginalObjectAdapter);
    }

    private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObjectAdapter) {
        var objectToAdd = context.WrappedDbContext.CreateProxy(originalObject.GetEFCoreProxiedType());
        var entry = dbContext.Entry(originalObject);
        var persisting = entry.State == EntityState.Detached;
        if (persisting) {
            SetKeyAsNecessary(originalObject, objectToAdd);
            dbContext.Add(objectToAdd);
        }
        else {
            objectToAdd = originalObject;
        }

        objectToProxyScratchPad[originalObject] = objectToAdd;
        adapterForOriginalObjectAdapter.Persisting();

        // create transient adapter here so that LoadObjectIntoNakedObjectsFramework knows proxy domainObject is transient
        // if not proxied this should just be the same as adapterForOriginalObjectAdapter
        var proxyAdapter = CreateAdapter(objectToAdd);

        if (persisting) {
            ProxyReferencesAndCopyValuesToProxy(originalObject, objectToAdd);
            context.PersistedNakedObjects.Add(proxyAdapter);
            // remove temporary adapter for proxy (tidy and also means we will not get problem 
            // with already known object in identity map when replacing the poco
            parent.RemoveAdapter(proxyAdapter);
            parent.ReplacePoco(adapterForOriginalObjectAdapter, objectToAdd);
            //dbContext.Add(objectToAdd);
        }
        else {
            SetKeyAsNecessary(originalObject, objectToAdd);
            dbContext.Add(objectToAdd);
            ProxyReferences(originalObject);
            context.PersistedNakedObjects.Add(proxyAdapter);
        }

        CallPersistingPersistedForComplexObjects(proxyAdapter);

        return objectToAdd;
    }

    private void CallPersistingPersistedForComplexObjects(INakedObjectAdapter parentAdapter) {
        var complexMembers = dbContext.GetComplexMembers(parentAdapter.Object.GetEFCoreProxiedType());
        foreach (var pi in complexMembers) {
            var complexObject = pi.GetValue(parentAdapter.Object, null);
            var childAdapter = parent.CreateAggregatedAdapter(nakedObjectAdapter, pi, complexObject);
            childAdapter.Persisting();
            context.PersistedNakedObjects.Add(childAdapter);
        }
    }

    private void ProxyReferences(object objectToProxy) {
        // this is to ensure persisting/persisted gets call for all referenced transient objects - what it wont handle 
        // is if a referenced object is proxied - as it doesn't update the reference - not sure if that will be a requirement. 
        var refMembers = dbContext.GetReferenceMembers(objectToProxy.GetType());
        refMembers.ForEach(pi => ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)));

        var collectionMembers = dbContext.GetCollectionMembers(objectToProxy.GetType());
        foreach (var pi in collectionMembers) {
            foreach (var item in pi.GetValue(objectToProxy, null).AsEnumerable()) {
                ProxyObjectIfAppropriate(item);
            }
        }
    }

    private void ProxyReferencesAndCopyValuesToProxy(object objectToProxy, object proxy) {
        var nonIdMembers = dbContext.GetNonIdMembers(objectToProxy.GetType());
        nonIdMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));

        var refMembers = dbContext.GetReferenceMembers(objectToProxy.GetType());
        refMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)), null));

        var collectionMembers = dbContext.GetCollectionMembers(objectToProxy.GetType());
        foreach (var pi in collectionMembers) {
            var toCol = proxy.GetProperty(pi.Name).GetValue(proxy, null);
            var fromCol = pi.GetValue(objectToProxy, null).AsEnumerable();
            foreach (var item in fromCol) {
                toCol.Invoke("Add", ProxyObjectIfAppropriate(item));
            }
        }

        var notPersistedMembers = objectToProxy.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && parent.IsNotPersisted(objectToProxy, p)).ToArray();
        notPersistedMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
    }

    public override string ToString() => $"EFCoreCreateObjectCommand [object={nakedObjectAdapter}]";

    #region ICreateObjectCommand Members

    public void Execute() {
        try {
            context.CurrentSaveRootObjectAdapter = nakedObjectAdapter;
            objectToProxyScratchPad.Clear();
            ProxyObjectIfAppropriate(nakedObjectAdapter.Object);
        }
        catch (InvalidOperationException e) {
            var due = new DataUpdateException(e.Message, e);
            parent.InvokeErrorFacet(due);
        }
        catch (Exception e) {
            parent.Logger.LogWarning($"Error in EntityCreateObjectCommand.Execute: {e.Message}");
            throw;
        }
    }

    public INakedObjectAdapter OnObject() => nakedObjectAdapter;

    #endregion
}