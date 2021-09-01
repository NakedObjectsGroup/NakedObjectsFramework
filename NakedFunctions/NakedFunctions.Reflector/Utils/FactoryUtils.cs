// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFunctions.Reflector.Utils {
    public static class FactoryUtils {
        public static bool Matches(this MethodInfo methodInfo, string name, Type declaringType, Type returnType, Type targetType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == declaringType &&
            methodInfo.ReturnType == returnType &&
            methodInfo.ContributedToType() == targetType;

        // if no need to log don't pass in logger
        public static MethodInfo FindComplementaryMethod(Type declaringType, string name, Func<MethodInfo, bool> matcher, ILogger logger = null) {
            var complementaryMethods = declaringType.GetMethods().Where(matcher).ToArray();

            if (complementaryMethods.Length > 1) {
                logger?.LogWarning($"Multiple methods found: {name} with matching signature - ignoring");
                return null;
            }

            var complementaryMethod = complementaryMethods.SingleOrDefault();
            var nameMatches = declaringType.GetMethods().Where(mi => mi.Name == name && mi != complementaryMethod);

            foreach (var methodInfo in nameMatches) {
                logger?.LogWarning($"Method found: {methodInfo.DeclaringType}.{methodInfo.Name} not matching expected signature");
            }

            return complementaryMethod;
        }

        public static T Invoke<T>(this Func<object, object[], object> methodDelegate, MethodInfo method, object[] parms) {
            try {
                return methodDelegate is not null ? (T) methodDelegate(null, parms) : (T) method.Invoke(null, parms);
            }
            catch (InvalidCastException) {
                throw new NakedObjectDomainException($"Must return {typeof(T)} from  method: {method.DeclaringType}.{method.Name}");
            }
        }

        public static bool IsVoid(Type type) => type == typeof(void);

        public static bool IsSealed(Type type) => type.IsSealed;

        public static bool IsInterface(Type type) => type.IsInterface;

        public static bool IsAbstract(Type type) => type.IsAbstract;

        public static bool IsStatic(Type type) => IsAbstract(type) && IsSealed(type);

        private static bool Matches(ParameterInfo pri, PropertyInfo ppi) =>
            string.Equals(pri.Name, ppi.Name, StringComparison.CurrentCultureIgnoreCase) &&
            pri.ParameterType == ppi.PropertyType;

        public static IDictionary<ParameterInfo, PropertyInfo> MatchParmsAndProperties(MethodInfo method, ILogger logger) {
            var allParms = method.GetParameters();
            var toMatchParms = allParms.Where(p => !p.IsInjectedParameter() && !p.IsTargetParameter()).ToArray();

            if (toMatchParms.Any()) {
                var firstParm = allParms.First();

                if (firstParm.IsTargetParameter()) {
                    var allProperties = firstParm.ParameterType.GetProperties();
                    return MatchUp(method, logger, allProperties, toMatchParms);
                }
            }

            return new Dictionary<ParameterInfo, PropertyInfo>();
        }

        private static IDictionary<ParameterInfo, PropertyInfo> MatchUp(MethodInfo method, ILogger logger, PropertyInfo[] allProperties, ParameterInfo[] toMatchParms) {
            var matchedProperties = allProperties.Where(p => toMatchParms.Any(tmp => Matches(tmp, p))).ToArray();
            var matchedParameters = toMatchParms.Where(tmp => allProperties.Any(p => Matches(tmp, p))).ToArray();

            // all parameters must be matched 
            if (toMatchParms.Length == matchedParameters.Length) {
                return matchedParameters.ToDictionary(p => p, p => matchedProperties.Single(mp => Matches(p, mp)));
            }

            logger.LogWarning($"Not all parameters on {method.DeclaringType}.{method.Name} matched properties");
            return new Dictionary<ParameterInfo, PropertyInfo>();
        }

        public static IDictionary<ParameterInfo, PropertyInfo> MatchParmsAndProperties(MethodInfo method, Type toCreateType, ILogger logger) {
            var allParms = method.GetParameters();
            var toMatchParms = allParms.Where(p => !p.IsInjectedParameter() && !p.IsTargetParameter()).ToArray();

            if (toMatchParms.Any()) {
                var allProperties = toCreateType.GetProperties();
                return MatchUp(method, logger, allProperties, toMatchParms);
            }

            return new Dictionary<ParameterInfo, PropertyInfo>();
        }

        private static Func<object, object, string, IContext, bool> TypeAuthorizerHelper<TTarget, TAuth>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TAuth, string, IContext, bool>)Delegate.CreateDelegate(typeof(Func<TTarget, TAuth, string, IContext, bool>), method);
            return (target, auth, name, context) => func((TTarget)target, (TAuth)auth, name, context);
        }

        public static Func<object, object, string, IContext, bool> CreateFunctionalTypeAuthorizerDelegate(MethodInfo method) {
            var genericHelper = typeof(FactoryUtils).GetMethod("TypeAuthorizerHelper", BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            var typeArgs = new List<Type> { method.DeclaringType, Enumerable.First(DelegateUtils.GetTypeAuthorizerType(method.DeclaringType).GenericTypeArguments) };
            var delegateHelper = genericHelper.MakeGenericMethod(typeArgs.ToArray());

            // Now call it. The null argument is because it’s a static method.
            var ret = delegateHelper.Invoke(null, new object[] { method });

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object, string, IContext, bool>)ret;
        }
    }
}