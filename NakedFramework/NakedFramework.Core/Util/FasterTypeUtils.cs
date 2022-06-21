// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Core.Util;

public static class FasterTypeUtils {
    private const string EntityProxyPrefix = "System.Data.Entity.DynamicProxies.";
    private const string CastleProxyPrefix = "Castle.Proxies.";

    private static bool IsCastleProxy(string typeName) => typeName.StartsWith(CastleProxyPrefix, StringComparison.Ordinal);

    private static bool IsAnyProxy(string typeName) => IsEFCoreProxy(typeName) || IsEF6Proxy(typeName);

    private static bool IsEF6Proxy(string typeName) => typeName.StartsWith(EntityProxyPrefix, StringComparison.Ordinal);

    public static bool IsEF6OrCoreProxy(Type type) => IsEFCoreProxy(type) || IsEF6Proxy(type);

    public static bool IsAnyProxy(Type type) => IsAnyProxy(type?.FullName ?? "");

    public static bool IsEFCoreProxy(Type type) => IsCastleProxy(type?.FullName ?? "");

    public static bool IsEFCoreProxy(string typeName) => IsCastleProxy(typeName ?? "");

    public static bool IsEF6Proxy(Type type) => IsEF6Proxy(type?.FullName ?? "");

    public static bool IsObjectArray(Type type) => type.IsArray && !(type.GetElementType()?.IsValueType == true || type.GetElementType() == typeof(string));

    public static string GetProxiedTypeFullName(Type type) => !IsAnyProxy(type) ? type?.FullName : type.BaseType?.FullName;

    public static Type GetProxiedType(Type type) => !IsAnyProxy(type) ? type : type.BaseType;
}