// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Util {
    public static class TypeNameUtils {
        public static string DecodeTypeName(string typeName, string separator = "-") {
            if (typeName.Contains("-")) {
                string rootType = typeName.Substring(0, typeName.IndexOf('`') + 2);
                string[] args = typeName.Substring(typeName.IndexOf('`') + 3).Split('-');

                Type genericType = TypeUtils.GetType(rootType);
                Type[] argTypes = args.Select(TypeUtils.GetType).ToArray();

                Type newType = genericType.MakeGenericType(argTypes);

                return newType.FullName;
            }

            return typeName;
        }

        public static string EncodeTypeName(this INakedObjectSpecification spec) {
            return EncodeTypeName(spec.FullName);
        }

        public static string EncodeTypeName(string typeName, string separator = "-") {
            Type type = TypeUtils.GetType(typeName);

            if (type.IsGenericType) {
                return EncodeGenericTypeName(type, separator);
            }

            return type.FullName;
        }

        public static string EncodeGenericTypeName(Type type, string separator = "-") {
            string rootType = type.GetGenericTypeDefinition().FullName;
            return type.GetGenericArguments().Aggregate(rootType, (s, t) => s + separator + t.FullName);
        }

        public static string GetShortName(string name) {
            name = name.Substring(name.LastIndexOf('.') + 1);
            if (name.LastIndexOf('`') > 0) {
                name = name.Substring(0, name.LastIndexOf('`'));
            }
            return name;
        }
    }
}