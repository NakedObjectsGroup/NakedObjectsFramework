// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Reflector.DotNet.Reflect.Strategy {
    /// <summary>
    ///     Strategy used to determine facts about classes, such as whether an an obj of a particular class can be
    ///     used as a field. Used for the <see cref="DotNetIntrospector" />
    /// </summary>
    public interface IClassStrategy {
        void Init();

        /// <summary>
        ///     Return the actual Type for the supplied Type.  For example, if the application uses
        ///     proxies or other means to wrap a Type, then this method should return the
        ///     underlying Type which should be exposed by the introspector.
        /// </summary>
        Type GetType(Type type);

        /// <summary>
        ///     Return true if the class is used by the system, and should therefore not be exposed to the user as a
        ///     field
        /// </summary>
        bool IsSystemClass(Type type);

        bool IsTypeUnsupportedByReflector(Type type);
    }

    // Copyright (c) Naked Objects Group Ltd.
}