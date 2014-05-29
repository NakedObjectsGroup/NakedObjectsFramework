// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    /// <summary>
    ///     Broadly speaking, keeps track of the oid/adapter/domain object tuple
    /// </summary>
    public interface INakedObjectManager : IOidGenerator {
        INakedObject CreateInstance(INakedObjectSpecification specification);

        INakedObject CreateViewModel(INakedObjectSpecification specification);

        INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification);

        /// <summary>
        ///     Forces a reload of this object from the persistent object store
        /// </summary>
        void Reload(INakedObject nakedObject);

        void RemoveAdapter(INakedObject objectToDispose);

        INakedObject GetAdapterFor(object obj);

        INakedObject GetAdapterFor(IOid oid);

        INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version);

        void ReplacePoco(INakedObject nakedObject, object newDomainObject);
        object CreateObject(INakedObjectSpecification specification);

        INakedObject GetViewModel(IOid oid);
    }


    // Copyright (c) Naked Objects Group Ltd.
}