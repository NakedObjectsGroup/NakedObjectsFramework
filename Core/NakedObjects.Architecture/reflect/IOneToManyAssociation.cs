// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Reflect {
    public interface IOneToManyAssociation : INakedObjectAssociation, IOneToManyFeature {

        /// <summary>
        ///     Return the count of elements in this collection field on the specified object
        /// </summary>
        int Count(INakedObject nakedObject, INakedObjectPersistor persistor);
    }

    // Copyright (c) Naked Objects Group Ltd.
}