// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq.Expressions;
using System.Reflection;
using Core = NakedCore.Util.TypeUtils;

namespace NakedObjects.Util
{
    /// <summary>
    /// Utility methods for safely obtaining and using types defined within
    /// a domain model.  The framework makes extensive
    /// use of these utils, but they are provided within the ProgrammingModel assemblies to permit optional use within domain code.
    /// </summary>
    public static class TypeUtils
    {

        public static object NewInstance(Type type) => Core.NewInstance(type);

        public static Type ImplementingTypeOrNull(Type typeCandidate, Type requiredType) => Core.ImplementingTypeOrNull(typeCandidate, requiredType);

        public static Type GetType(string typeName) => Core.GetType(typeName);

        public static Type ImplementingTypeOrNull(string classCandidateName, Type requiredType) => Core.ImplementingTypeOrNull(classCandidateName, requiredType);

        public static string[] GetInterfaces(Type type) => Core.GetInterfaces(type);

        public static string GetBaseType(Type type) => Core.GetBaseType(type);

        public static bool IsString(Type type) => Core.IsString(type);

        public static bool IsPublic(Type type) => Core.IsPublic(type);

        public static bool IsSystem(Type type) => Core.IsSystem(type);

        public static bool IsMicrosoft(Type type) => Core.IsMicrosoft(type);

        public static bool IsSystem(string typeName) => Core.IsSystem(typeName);

        public static bool IsMicrosoft(string typeName) => Core.IsMicrosoft(typeName);

        public static bool IsNakedObjectsProxy(Type type) => Core.IsNakedObjectsProxy(type);

        public static bool IsNakedObjectsProxy(string typeName) => Core.IsNakedObjectsProxy(typeName);

        public static bool IsCastleProxy(Type type) => Core.IsCastleProxy(type);

        public static bool IsCastleProxy(string typeName) => Core.IsCastleProxy(typeName);

        public static bool IsEntityProxy(Type type) => Core.IsEntityProxy(type);

        public static bool IsEntityProxy(string typeName) => Core.IsEntityProxy(typeName);

        public static bool IsProxy(Type type) => Core.IsProxy(type);

        public static bool IsProxy(string typeName) => Core.IsProxy(typeName);

        public static bool IsNakedObjects(Type type) => Core.IsNakedObjects(type);

        public static bool IsNakedObjects(string typeName) => Core.IsNakedObjects(typeName);

        public static bool IsEntityDomainObject(Type type) => Core.IsEntityDomainObject(type);

        public static bool IsEntityDomainObject(string typeName) => Core.IsEntityDomainObject(typeName);

        public static string GetProxiedTypeFullName(this Type type) => Core.GetProxiedTypeFullName(type);

        public static Type GetProxiedType(this Type type) => Core.GetProxiedType(type);

        public static T CreateGenericInstance<T>(Type genericTypeDefinition,
                                                 Type[] genericTypeParms,
                                                 object[] constructorParms) => Core.CreateGenericInstance<T>(genericTypeDefinition, genericTypeParms, constructorParms);

        public static bool IsNullableType(Type type) => Core.IsNullableType(type);

        public static Type GetNulledType(Type type) => Core.GetNulledType(type);

        public static bool IsEnum(Type type) => Core.IsEnum(type);

        public static bool IsIntegralValueForEnum(object obj) => Core.IsIntegralValueForEnum(obj);

        public static string PropertyName<TTarget, TProperty>(this TTarget target, Expression<Func<TTarget, TProperty>> expr) =>
            Core.PropertyName(target, expr);

        public static bool IsPropertyMatch<TTarget, TProperty>(this TTarget target, string memberName, Expression<Func<TTarget, TProperty>> expr) =>
            Core.IsPropertyMatch(target, memberName, expr);

        public static bool IsPropertyMatch<TTarget, TProperty>(this object target, string memberName, Expression<Func<TTarget, TProperty>> expr) =>
            Core.IsPropertyMatch(target, memberName, expr);

        // ** Lifted from MSDN **  
        //    The identity most users would expect MemberInfos (other than Type) to have is not what Reflection provides.
        //    So for example most folks would expect the following program to print true instead of false:
        //
        //    public class B { public void M() { } }
        //    public class D : B { }
        //    public class Program
        //    {
        //        public static void Main
        //        {
        //            Console.WriteLine(typeof(B).GetMethod("M") == typeof(D).GetMethod("M"));
        //        }
        //    }
        //
        // The MethodInfos are not equal because ReflectedType is included in the identity of MethodInfos.
        // The identity Reflection uses for MethdInfo can be expressed as:
        // DeclaringType + MethodName + Signature + DeclaringType instantiation + Method Instantiation + ReflectedType
        // And what most users would expect it to be can be expressed as:
        // DeclaringType + MethodName + Signature + DeclaringType Instantiation + Method Instantiation
        // We can’t make them match because that would be a breaking change. However here is a little code snippet
        // that will do the comparison using the latter definition of identity. This will work for all subclasses of
        // MemberInfo (Type, MethodBase, MethodInfo, ConstructorInfo, FieldInfo, PropertyInfo and EventInfo):

        public static bool MemberInfoEquals(this MemberInfo lhs, MemberInfo rhs) => Core.MemberInfoEquals(lhs, rhs);
    }
}