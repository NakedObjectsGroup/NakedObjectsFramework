// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.Util {
    public class InvokeUtils {
        /// <summary>
        ///     Invoke the specified method with all its parameter, if any, as null.
        /// </summary>
        public static object InvokeStatic(MethodInfo method) {
            var parameters = new object[method.GetParameters().Length];
            return Invoke(method, null, parameters);
        }

        public static object InvokeStatic(MethodInfo method, INakedObject[] parameters) {
            return Invoke(method, null, parameters);
        }

        public static object InvokeStatic(MethodInfo method, object[] parameters) {
            return Invoke(method, null, parameters);
        }

        public static object Invoke(MethodInfo method, INakedObject nakedObject) {
            var parameters = new object[method.GetParameters().Length];
            return Invoke(method, nakedObject.GetDomainObject(), parameters);
        }

        public static object Invoke(MethodInfo method, INakedObject nakedObject, INakedObject[] parameters) {
            object[] parameterPocos = parameters == null ? new object[] {} : parameters.Select(p => p == null ? null : p.Object).ToArray();
            return Invoke(method, nakedObject.Object, parameterPocos);
        }

        /// <summary>
        ///     Invoke the specified method with the user name (from the specified session) as the one and only parameter.
        /// </summary>
        public static object InvokeForSession(MethodInfo method, ISession session) {
            int len = method.GetParameters().Length;
            var parameters = new object[len];
            parameters[0] = session.Principal;
            return Invoke(method, null, parameters);
        }

        public static object Invoke(MethodInfo method, object obj, object[] parameters) {
            try {
                return method.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e) {
                InvocationException("Exception executing " + method, e);
                return null;
            }
        }

        public static void InvocationException(string error, Exception e) {
            Exception innerException = e.InnerException;
            if (innerException is DomainException) {
                // a domain  exception from the domain code is re-thrown as an NO exception with same
                // semantics
                throw new NakedObjectDomainException(innerException.Message, innerException);
            }
            if (e is TargetInvocationException) {
                throw new InvokeException(innerException.Message, innerException);
            }
            throw new ReflectionException(e.Message, e);
        }
    }
}