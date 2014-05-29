// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Model {
    public class ListValue : IValue {
        private readonly IValue[] internalValue;

        public ListValue(IValue[]  value) {
            internalValue = value;
        }

        #region IValue Members

      
        // this is cloned from TypeUtils 
        private const string NakedObjectsProxyPrefix = "NakedObjects.Proxy.";
        private const string EntityProxyPrefix = "System.Data.Entity.DynamicProxies.";
        private const string CastleProxyPrefix = "Castle.Proxies.";

        private static bool IsNakedObjectsProxy(string typeName) {
            return typeName.StartsWith(NakedObjectsProxyPrefix);
        }

        private static bool IsCastleProxy(string typeName) {
            return typeName.StartsWith(CastleProxyPrefix);
        }

        private static bool IsEntityProxy(string typeName) {
            return typeName.StartsWith(EntityProxyPrefix);
        }

        private static bool IsProxy(Type type) {
            return IsProxy(type.FullName ?? "");
        }

        private static bool IsProxy(string typeName) {
            return IsEntityProxy(typeName) || IsNakedObjectsProxy(typeName) || IsCastleProxy(typeName);
        }      

        public static Type GetProxiedType( Type type) {
            return IsProxy(type) ? type.BaseType : type;
        }
        // end clone 


        private static Type GetCommonBaseType(Type[] types, Type baseType) {
            return types.Any(type => !type.IsAssignableFrom(baseType)) ? GetCommonBaseType(types, baseType.BaseType) : baseType;
        }

        public object GetValue(INakedObjectsSurface surface, UriMtHelper helper) {
            object[] items = internalValue.Select(iv => iv.GetValue(surface, helper)).ToArray();

            if (items.Any()) {
                Type[] types = items.Select(i => GetProxiedType(i.GetType())).ToArray();
                Type type = GetCommonBaseType(types, types.First());

                Type collType = typeof (List<>).MakeGenericType(type);
                var coll = (IList) Activator.CreateInstance(collType);

                Array.ForEach(items, i => coll.Add(i));
                return coll;
            }
            return null;
        }

        #endregion
    }
}