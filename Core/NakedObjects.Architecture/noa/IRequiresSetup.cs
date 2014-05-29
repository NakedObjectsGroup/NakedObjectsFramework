// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture {
    /// <summary>
    ///     Indicate that the implementing class might have dependencies that need to be set up (and hence also shut down)
    /// </summary>
    public interface IRequiresSetup {
        /// <summary>
        ///     Indicates to the component that it is to initialise itself as it will soon be receiving requests
        /// </summary>
        void Init();

        /// <summary>
        ///     Indicates to the component that no more requests will be made of it and it can safely release any
        ///     services it has hold of.
        /// </summary>
        void Shutdown();
    }

    // Copyright (c) Naked Objects Group Ltd.
}