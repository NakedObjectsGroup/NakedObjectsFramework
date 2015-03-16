// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    ///     Broadly speaking, keeps track of the oid/adapter/domain object tuple
    /// </summary>
    public interface INakedObjectManager {
        void RemoveAdapter(INakedObjectAdapter objectAdapterToDispose);
        INakedObjectAdapter GetAdapterFor(object obj);
        INakedObjectAdapter GetAdapterFor(IOid oid);
        INakedObjectAdapter CreateAdapter(object domainObject, IOid oid, IVersion version);
        void ReplacePoco(INakedObjectAdapter nakedObjectAdapter, object newDomainObject);
        INakedObjectAdapter CreateAggregatedAdapter(INakedObjectAdapter parent, string fieldId, object obj);
        INakedObjectAdapter NewAdapterForKnownObject(object domainObject, IOid transientOid);
        void MadePersistent(INakedObjectAdapter nakedObjectAdapter);
        void UpdateViewModel(INakedObjectAdapter adapter, string[] keys);
        List<INakedObjectAdapter> GetCollectionOfAdaptedObjects(IEnumerable domainObjects);
        INakedObjectAdapter GetServiceAdapter(object service);
        INakedObjectAdapter GetKnownAdapter(IOid oid);
        INakedObjectAdapter CreateViewModelAdapter(IObjectSpec spec, object viewModel);
        INakedObjectAdapter CreateInstanceAdapter(object obj);
        INakedObjectAdapter AdapterForExistingObject(object domainObject, IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}