// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Core.Reflect {
    public interface IContainerInjector {
        /// <summary>
        ///     Among other things, will inject all services into the object
        /// </summary>
        void InitDomainObject(object obj);

        void InitInlineObject(object root, object inlineObject);
        INakedObjectsFramework Framework { set; }
        Type[] ServiceTypes { set; }
    }


    // Copyright (c) Naked Objects Group Ltd.
}