// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Configuration;

namespace NakedFramework.Core.Util;

public static class CollectionUtils {
    #region public

    public const int IncompleteCollection = -1;

    public static bool IsDictionary(Type type) => IsGenericEnumerable(type) && IsGenericOfKeyValuePair(type);

    /// <summary>IEnumerable of T where T is not a value type</summary>
    public static bool IsGenericEnumerableOfRefType(Type type) => IsGenericEnumerable(type) && IsGenericOfRefType(type);

    public static bool IsGenericEnumerable(Type type) => IsGenericType(type, typeof(IEnumerable<>));

    public static bool IsOrImplementsGenericEnumerable(Type type) => IsGenericType(type, typeof(IEnumerable<>)) || type.GetInterfaces().Any(IsOrImplementsGenericEnumerable);

    public static bool IsGenericCollection(Type type) => IsGenericType(type, typeof(ICollection<>));

    /// <summary>IEnumerable of T where T is not a value type</summary>
    public static bool IsGenericQueryable(Type type) => IsGenericType(type, typeof(IQueryable<>));

    /// <summary>IQueryable or IQueryable of T</summary>
    public static bool IsQueryable(Type type) => typeof(IQueryable).IsAssignableFrom(type);

    /// <summary> ICollection but not Array or IEnumerable of T where T is not a value type </summary>
    public static bool IsCollectionButNotArray(Type type) => IsCollection(type) && !type.IsArray;

    /// <summary> ICollection or IEnumerable of T or Array of T where T is not a value type </summary>
    public static bool IsCollection(Type type) => IsNonGenericCollection(type) || IsGenericCollection(type);

    public static Type ElementType(Type type) => IsGenericEnumerable(type) ? GetGenericEnumerableType(type).GetGenericArguments().Single() : typeof(object);

    public static bool IsBlobOrClob(Type type) => type.IsArray && (type.GetElementType() == typeof(byte) || type.GetElementType() == typeof(sbyte) || type.GetElementType() == typeof(char));

    public static bool IsGenericOfEnum(Type type) => type.GetGenericArguments().Length == 1 && type.GetGenericArguments().All(t => t.IsEnum);

    public static void ForEach<T>(this T[] toIterate, Action<T> action) => Array.ForEach(toIterate, action);

    public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T> action) {
        foreach (var item in toIterate) {
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T, int> action) {
        var i = 0;
        foreach (var item in toIterate) {
            action(item, i++);
        }
    }

    public static IList CloneCollection(object toClone) {
        var collectionType = MakeCollectionType(toClone, typeof(List<>));
        return Activator.CreateInstance(collectionType) as IList;
    }

    public static IList CloneCollectionAndPopulate(object toClone, IEnumerable<object> contents) {
        var newCollection = CloneCollection(toClone);
        contents.ForEach(o => newCollection.Add(o));
        return newCollection;
    }

    public static bool IsSet(Type type) => IsGenericType(type, typeof(ISet<>));

    public static IList ToTypedIList(IEnumerable<object> toWrap, Type instanceType) {
        var typedListType = typeof(List<>).MakeGenericType(instanceType);
        var typedList = (IList)Activator.CreateInstance(typedListType);
        toWrap.ForEach(o => typedList.Add(o));
        return typedList;
    }

    public static string CollectionTitleString(IObjectSpec elementSpec, int size) {
        if (elementSpec is null || elementSpec.FullName.Equals(typeof(object).FullName)) {
            return CollectionTitleStringUnknownType(size);
        }

        return CollectionTitleStringKnownType(elementSpec, size);
    }

    public static ParallelQuery<TSource> AsCustomParallel<TSource>(this IEnumerable<TSource> source) {
        var pSource = source.AsParallel();
        return ReflectorDefaults.ParallelDegree > 0 ? pSource.WithDegreeOfParallelism(ReflectorDefaults.ParallelDegree) : pSource;
    }

    public static bool IsGenericIEnumerableOrISet(Type type) =>
        CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
        CollectionUtils.IsGenericType(type, typeof(ISet<>));

    #endregion

    #region private

    private static bool IsGenericOfKeyValuePair(Type type) => type.GetGenericArguments().Any(t => IsGenericType(t, typeof(KeyValuePair<,>)));

    private static bool IsGenericOfRefType(Type type) => type.GetGenericArguments().Length == 1 && type.GetGenericArguments().All(t => !t.IsValueType);

    private static bool IsNonGenericCollection(Type type) => typeof(ICollection).IsAssignableFrom(type);

    private static Type MakeCollectionType(object toClone, Type genericCollectionType) {
        var itemType = toClone.GetType().IsGenericType ? toClone.GetType().GetGenericArguments().Single() : typeof(object);
        return genericCollectionType.MakeGenericType(itemType);
    }

    public static bool IsGenericType(Type type, Type toMatch) =>
        type.IsGenericType
        && type.GetGenericArguments().Length == 1
        && (type.GetGenericTypeDefinition() == toMatch || type.GetInterfaces().Any(interfaceType => IsGenericType(interfaceType, toMatch)));

    private static Type GetGenericEnumerableType(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? type : type.GetInterfaces().FirstOrDefault(IsGenericEnumerable) : null;

    public static Type GetGenericType(Type type) => type.IsGenericType ? type : type.GetInterfaces().FirstOrDefault(i => i.IsGenericType);

    private static string CollectionTitleStringKnownType(IObjectSpec elementSpec, int size) =>
        size switch {
            IncompleteCollection => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleUnloaded, elementSpec.PluralName),
            0 => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleEmpty, elementSpec.PluralName),
            1 => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleOne, elementSpec.SingularName),
            _ => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleMany, size, elementSpec.PluralName)
        };

    private static string CollectionTitleStringUnknownType(int size) =>
        size switch {
            IncompleteCollection => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleUnloaded, ""),
            0 => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleEmpty, NakedObjects.Resources.NakedObjects.Objects),
            1 => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleOne, NakedObjects.Resources.NakedObjects.Object),
            _ => string.Format(NakedObjects.Resources.NakedObjects.CollectionTitleMany, size, NakedObjects.Resources.NakedObjects.Objects)
        };

    #endregion
}