// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Component {
    //TODO: Review the value added by this component -  given the huge overlap with IObjectStore.
    //Consider merging all the methods down.
    public interface IObjectPersistor {
        IQueryable<T> Instances<T>() where T : class;

        IQueryable Instances(Type type);

        IQueryable Instances(IObjectSpec spec);

        INakedObject LoadObject(IOid oid, IObjectSpec spec);

        void AddPersistedObject(INakedObject nakedObject);

        void Reload(INakedObject nakedObject);
        void ResolveField(INakedObject nakedObject, IAssociationSpec field);
        void LoadField(INakedObject nakedObject, string field);
        int CountField(INakedObject nakedObject, string field);
        PropertyInfo[] GetKeys(Type type);
        INakedObject FindByKeys(Type type, object[] keys);
        void Refresh(INakedObject nakedObject);
        void ResolveImmediately(INakedObject nakedObject);
        void ObjectChanged(INakedObject nakedObject);
        void DestroyObject(INakedObject nakedObject);
        object CreateObject(IObjectSpec spec);
        IEnumerable GetBoundedSet(IObjectSpec spec);
        void LoadComplexTypes(INakedObject pocoAdapter, bool isGhost);
    }
}