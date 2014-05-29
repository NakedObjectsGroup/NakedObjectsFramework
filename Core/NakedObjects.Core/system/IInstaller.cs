// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Core.NakedObjectsSystem {
    /// <summary>
    ///     This interface marks a class as able to install a component for the NOF during its boot strapping process.
    /// </summary>
    public interface IInstaller {
        string Name { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}