// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Common.Logging;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect.Strategy {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    public class DefaultClassStrategy : IClassStrategy {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DefaultClassStrategy));

        #region IClassStrategy Members

        public virtual void Init() {}

        public virtual Type GetType(Type type) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                // use type inside nullable wrapper
                Log.DebugFormat("Using wrapped type instead of {0}", type);
                return type.GetGenericArguments()[0];
            }

            if (TypeUtils.IsProxy(type)) {
                Log.DebugFormat("Using proxied type instead of {0}", type);
                return type.BaseType;
            }

            return type;
        }

        public virtual bool IsSystemClass(Type type) {
            return TypeUtils.IsSystem(type);
        }

        public virtual bool IsTypeUnsupportedByReflector(Type type) {
            return type.IsPointer ||
                   type.IsByRef ||
                   CollectionUtils.IsDictionary(type) ||
                   (type.IsGenericType && !(TypeUtils.IsNullableType(type) || CollectionUtils.IsGenericEnumerable(type)));
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}