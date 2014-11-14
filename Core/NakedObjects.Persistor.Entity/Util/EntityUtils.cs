// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Persistor.Entity.Util {
    public static class EntityUtils {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityUtils));

        public static void UpdateVersion(this INakedObject nakedObject, ISession session, INakedObjectManager manager) {
            object versionObject = nakedObject == null ? null : nakedObject.GetVersion(manager);
            if (versionObject != null) {
                nakedObject.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
                Log.DebugFormat("GetObject: Updating Version {0} on {1}", nakedObject.Version, nakedObject);
            }
        }

        public static bool IsEntityProxy(Type type) {
            return IsEntityProxy(type.FullName ?? "");
        }

        public static bool IsEntityProxy(string typeName) {
            return typeName.StartsWith("System.Data.Entity.DynamicProxies.");
        }

        public static string GetEntityProxiedTypeName(object domainObject) {
            return domainObject.GetEntityProxiedType().FullName;
        }

        public static Type GetEntityProxiedType(this object domainObject) {
            return IsEntityProxy(domainObject.GetType()) ? domainObject.GetType().BaseType : domainObject.GetType();
        }

    }
}