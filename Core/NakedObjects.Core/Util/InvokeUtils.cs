// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Util {
    public class InvokeUtils {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InvokeUtils));

        public static object InvokeStatic(MethodInfo method, object[] parameters) => Invoke(method, null, parameters);

        public static object Invoke(MethodInfo method, INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters) {
            var parameterPocos = parameters == null ? new object[] { } : parameters.Select(p => p == null ? null : p.Object).ToArray();
            return Invoke(method, nakedObjectAdapter.Object, parameterPocos);
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
            var innerException = e.InnerException;
            if (innerException is DomainException) {
                // a domain  exception from the domain code is re-thrown as an NO exception with same semantics
                throw new NakedObjectDomainException(Log.LogAndReturn(innerException.Message), innerException);
            }

            if (e is TargetInvocationException) {
                throw new InvokeException(Log.LogAndReturn(innerException.Message), innerException);
            }

            throw new ReflectionException(Log.LogAndReturn(e.Message), e);
        }
    }
}