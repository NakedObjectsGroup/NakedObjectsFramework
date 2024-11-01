﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
using NakedFramework.Architecture.Persist;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Architecture.Component;

/// <summary>
///     The non-store specific parts of the Object persistence mechanism. Implemented as a composite rather than with
///     inheritance.
/// </summary>
public interface IObjectPersistor {
    IQueryable<T> Instances<T>() where T : class;
    IQueryable Instances(Type type);
    IQueryable Instances(IObjectSpec spec);
    INakedObjectAdapter LoadObject(IOid oid, IObjectSpec spec);
    void AddPersistedObject(INakedObjectAdapter nakedObjectAdapter);
    void Reload(INakedObjectAdapter nakedObjectAdapter);
    void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field);
    void LoadField(INakedObjectAdapter nakedObjectAdapter, string field);
    int CountField(INakedObjectAdapter nakedObjectAdapter, string field);
    PropertyInfo[] GetKeys(Type type);
    INakedObjectAdapter FindByKeys(Type type, object[] keys);
    void Refresh(INakedObjectAdapter nakedObjectAdapter);
    object Resolve(object domainObject);
    void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter);
    void DestroyObject(INakedObjectAdapter nakedObjectAdapter);
    object CreateObject(ITypeSpec spec);
    IEnumerable GetBoundedSet(IObjectSpec spec);
    void LoadComplexTypes(INakedObjectAdapter adapter, bool isGhost);
    void ObjectChanged(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager, IMetamodelManager metamodel);
    IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects);
    bool HasChanges();
    T ValidateProxy<T>(T toCheck) where T : class;
}