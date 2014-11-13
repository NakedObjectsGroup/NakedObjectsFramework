// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;

namespace NakedObjects.Core.Util {
    public static class TypeFactory {
        public static Type GetNullableTypeFromLoadedAssembly(string className) {
            Type type = GetTypeFromLoadedAssembly(className);
            return typeof (Nullable<>).MakeGenericType(new[] {type});
        }

        public static Type GetTypeFromLoadedAssembly(string className) {
            if (className.EndsWith("?")) {
                return GetNullableTypeFromLoadedAssembly(className.Remove(className.Length - 1));
            }

            try {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    Type type = assembly.GetType(className);
                    if (type != null) {
                        return type;
                    }
                }
            }
            catch (Exception e) {
                throw new InitialisationException(string.Format(Resources.NakedObjects.ErrorFindingClass, className), e);
            }

            throw new InitialisationException(string.Format(Resources.NakedObjects.ErrorFindingClass, className));
        }
    }
}