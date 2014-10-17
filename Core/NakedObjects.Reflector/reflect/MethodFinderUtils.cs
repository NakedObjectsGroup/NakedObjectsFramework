// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public static class MethodFinderUtils {
        /// <summary>
        ///     Searches the supplied array of methods for specific method and returns it, also removing it
        ///     from supplied array if found (by setting to <c>null</c>)
        /// </summary>
        /// <para>
        ///     Any methods that do not meet the search criteria are left in the array of methods.
        /// </para>
        public static MethodInfo RemoveMethod(MethodInfo[] methods,
                                              MethodType methodType,
                                              string name,
                                              Type returnType,
                                              Type[] paramTypes) {
            for (int i = 0; i < methods.Length; i++) {
                if (methods[i] == null) {
                    continue;
                }

                MethodInfo method = methods[i];

                // check for public modifier
                if (!method.IsPublic) {
                    continue;
                }

                // check for static modifier
                if (method.IsStatic && methodType == MethodType.Object) {
                    continue;
                }

                // check for name
                if (!method.Name.Equals(name)) {
                    continue;
                }

                // check for return Type
                if (returnType != null && returnType != method.ReturnType) {
                    continue;
                }

                // check parms (if required)
                if (paramTypes != null) {
                    ParameterInfo[] parameters = method.GetParameters();
                    var parameterTypes = new Type[parameters.Length];
                    int j = 0;
                    foreach (ParameterInfo parameter in parameters) {
                        parameterTypes[j++] = parameter.ParameterType;
                    }

                    if (paramTypes.Length != parameterTypes.Length) {
                        continue;
                    }

                    bool possible = true;
                    for (int c = 0; c < paramTypes.Length; c++) {
                        if ((paramTypes[c] != null) && (paramTypes[c] != parameterTypes[c])) {
                            possible = false;
                            break;
                        }
                    }
                    if (!possible) continue;
                }
                methods[i] = null;

                return method;
            }

            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}