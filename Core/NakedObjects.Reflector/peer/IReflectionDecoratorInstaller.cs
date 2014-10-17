// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Peer {
    public interface IReflectionDecoratorInstaller : IReflectorEnhancementInstaller {
        IFacetDecorator[] CreateDecorators(IReflector reflector);
    }

    // Copyright (c) Naked Objects Group Ltd.
}