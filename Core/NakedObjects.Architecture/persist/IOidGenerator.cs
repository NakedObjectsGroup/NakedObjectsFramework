// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Persist {
    // TODO this should be done by OS - move OIDGen to OS
    public interface IOidGenerator : IRequiresSetup {
        /// <summary>
        ///     Removes the specified object from the system.
        /// </summary>
        /// <para>
        ///     The specified object's data should be removed from the persistence mechanism.
        /// </para>
        void ConvertPersistentToTransientOid(IOid oid);

        void ConvertTransientToPersistentOid(IOid oid);

        IOid CreateTransientOid(object obj);

        IOid RestoreOid(string[] encodedData);
    }

    // Copyright (c) Naked Objects Group Ltd.
}