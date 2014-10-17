// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.NakedObjectsSystem {
    /// <summary>
    ///     Installs a <see cref="INakedObjectReflector" /> during system start up
    /// </summary>
    public interface IReflectorInstaller : IInstaller {
        INakedObjectReflector CreateReflector();
        void AddEnhancement(IReflectorEnhancementInstaller enhancementInstaller);
    }

    // Copyright (c) Naked Objects Group Ltd.
}