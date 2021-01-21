// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace NakedFunctions.Reflector.Utils {
    public static class FactoryUtils {
        public static bool Matches(this MethodInfo methodInfo, string name, Type declaringType, Type returnType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == declaringType &&
            methodInfo.ReturnType == returnType;

        public static MethodInfo FindComplementaryMethod(Type declaringType, string name, Func<MethodInfo, bool> matcher, ILogger logger) {
            var complementaryMethods = declaringType.GetMethods().Where(matcher).ToArray();

            if (complementaryMethods.Length > 1) {
                logger.LogWarning($"Multiple methods found: {name} with matching signature - ignoring");
                return null;
            }

            var complementaryMethod = complementaryMethods.SingleOrDefault();
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != complementaryMethod);

            foreach (var methodInfo in nameMatches) {
                logger.LogWarning($"Method found: {methodInfo.Name} not matching expected signature");
            }

            return complementaryMethod;
        }
    }
}